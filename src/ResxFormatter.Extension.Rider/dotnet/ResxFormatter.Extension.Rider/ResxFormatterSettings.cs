using System;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Resources.Settings;
using JetBrains.Util;

namespace ResxFormatter.Extension.Rider
{
    [SettingsKey(typeof(CodeStyleSettings), "Settings for ResX Formatter")]
    public class ResxFormatterSettings
    {
        [SettingsEntry(DefaultValue: true, Description: "Read ResxFormatter settings from .editorconfig, if available.\r\nOverwrites all other settings.")]
        public bool ImportResxFormatterEditorConfig { get; set; }
        
        [SettingsEntry(DefaultValue: true, Description: "Defines whether to automatically format the active resx document while saving.")]
        public bool FormatOnSave { get; set; }
        
        [SettingsEntry(DefaultValue: StringComparison.OrdinalIgnoreCase, Description: "Define the sort order of the XML data nodes when saving a resx file.")]
        public StringComparison SortOrder { get; set; }
        
        [SettingsEntry(DefaultValue: false, Description: "Remove the XSD schema from the resx file.")]
        public bool RemoveXsdSchema { get; set; }
        
        [SettingsEntry(DefaultValue: false, Description: "Remove the documentation comment from the resx file.")]
        public bool RemoveDocumentationComment { get; set; }

        [SettingsEntry(DefaultValue: "", Description: "Defines location of external Resx Formatter configuration file. Specifying an external configuration file allows you to easily point multiple instances to a shared configuration. The configuration path can be local or network-based. Invalid configurations will be ignored.")]
        public FileSystemPath ConfigPath { get; set; }

        [SettingsEntry(DefaultValue: false, Description: "When set to true, Resx Formatter will look for an external Resx Formatter configuration file not only up through your solution directory, but up through the drives root of the current solution so you can share one configuration file through multiple solutions.")]
        public bool SearchToDriveRoot { get; set; }
    }
}