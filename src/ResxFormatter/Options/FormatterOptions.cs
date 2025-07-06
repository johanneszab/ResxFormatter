using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ResxFormatter.Options
{
    public class FormatterOptions : IFormatterOptions
    {
        private const string DefaultOptionsPath = "ResxFormatter.Options.DefaultSettings.json";

        public FormatterOptions()
        {
            InitializeProperties();
        }

        /// <summary>
        /// Constructor that accepts an external configuration path before initializing settings.
        /// </summary>
        /// <param name="config">Path to external configuration file.</param>
        public FormatterOptions(string config = "")
        {
            if (!string.IsNullOrWhiteSpace(config) && File.Exists(config))
            {
                ConfigPath = config;
            }

            InitializeProperties();
        }

        /// <summary>
        /// JSON Constructor required to prevent an infinite loop during deserialization.
        /// </summary>
        /// <param name="isJsonConstructor">Dummy parameter to differentiate from default constructor.</param>
        [JsonConstructor]
        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Required for deserialization", MessageId = "isJsonConstructor")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for deserialization")]
        public FormatterOptions(bool isJsonConstructor = true) { }
        
        // Configuration
        [Category("General")]
        [DisplayName("Sort resx on save")]
        [JsonProperty(nameof(FormatOnSave), DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Defines whether to automatically format the active resx file while saving.\r\n\r\nDefault Value: true")]
        [DefaultValue(true)]
        public bool FormatOnSave { get; set; }
        
        [Category("General")]
        [DisplayName("When saving a resx file, sort the XML data nodes by the key using the following sort order")]
        [JsonProperty(nameof(SortOrder), DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [Description("Define the sort order of the XML data nodes when saving a resx file.\r\n\r\nDefault Value: OrdinalIgnoreCase")]
        [DefaultValue(StringComparison.OrdinalIgnoreCase)]
        public StringComparison SortOrder { get; set; }
        
        [Category("Configuration")]
        [RefreshProperties(RefreshProperties.All)]
        [DisplayName("Search to drives root")]
        [Description("When set to true, Resx Formatter will look for an external Resx Formatter configuration file not only up through your solution directory, but up through the drives root of the current solution so you can share one configuration file through multiple solutions.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        [JsonIgnore]
        public bool SearchToDriveRoot { get; set; }

        private string configPath = string.Empty;
        [Category("Configuration")]
        [RefreshProperties(RefreshProperties.All)]
        [DisplayName("External configuration file")]
        [Description("Defines location of external Resx Formatter configuration file. Specifying an external configuration file allows you to easily point multiple instances to a shared configuration. The configuration path can be local or network-based. Invalid configurations will be ignored.\r\n\r\nDefault Value: N/A")]
        [DefaultValue("")]
        [JsonIgnore]
        public string ConfigPath
        {
            get => configPath;
            set
            {
                configPath = value;
                if (!String.IsNullOrEmpty(value))
                {
                    TryLoadExternalConfiguration();
                }
            }
        }
        
        private bool resetToDefault;
        [Category("Configuration")]
        [RefreshProperties(RefreshProperties.All)]
        [DisplayName("Reset to default")]
        [Description("When set to true, all Resx Formatter settings will be reset to their defaults.\r\n\r\nDefault Value: false")]
        [DefaultValue(false)]
        [JsonIgnore]
        public bool ResetToDefault
        {
            get => resetToDefault;
            set
            {
                resetToDefault = value;

                if (resetToDefault)
                {
                    ConfigPath = String.Empty;
                    InitializeProperties();
                }
            }
        }
        
        /// <summary>
        /// Creates a clone from the current instance.
        /// </summary>
        /// <returns>A clone from the current instance.</returns>
        public IFormatterOptions Clone()
        {
            string jsonFormatterOptions = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<FormatterOptions>(jsonFormatterOptions);
        }

        private void InitializeProperties()
        {
            if (!TryLoadExternalConfiguration())
            {
                var assembly = Assembly.GetExecutingAssembly();
                using Stream stream = assembly.GetManifestResourceStream(DefaultOptionsPath);
                using var reader = new StreamReader(stream);
                LoadConfiguration(reader.ReadToEnd());
            }
        }

        private bool TryLoadExternalConfiguration()
        {
            return !string.IsNullOrWhiteSpace(ConfigPath) &&
                   File.Exists(ConfigPath) &&
                   LoadConfiguration(File.ReadAllText(ConfigPath));
        }

        private bool LoadConfiguration(string config)
        {
            try
            {
                FormatterOptions configOptions = JsonConvert.DeserializeObject<FormatterOptions>(config);

                if (configOptions == null)
                {
                    LoadFallbackConfiguration();
                }
                else
                {
                    foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(this))
                    {
                        // Cannot set Config Path from External Configuration.
                        if (propertyDescriptor.Name.Equals(nameof(ConfigPath), StringComparison.Ordinal))
                        {
                            continue;
                        }

                        propertyDescriptor.SetValue(this, propertyDescriptor.GetValue(configOptions));
                    }
                }
            }
            catch (Exception)
            {
                LoadFallbackConfiguration();
                return false;
            }

            return true;
        }

        private void LoadFallbackConfiguration()
        {
            // Initialize all properties with "DefaultValueAttribute" to their default value
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(this))
            {
                // Set default value if DefaultValueAttribute is present
                if (propertyDescriptor.Attributes[typeof(DefaultValueAttribute)] is DefaultValueAttribute attribute)
                {
                    propertyDescriptor.SetValue(this, attribute.Value);
                }
            }
        }
    }
}