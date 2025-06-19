using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Controls.FileSystem;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionPages;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.Util;

namespace ResxFormatter.Extension.Rider.Options
{
    [OptionsPage(PID, PageTitle, typeof(FeaturesEnvironmentOptionsThemedIcons.FormattingStyle), ParentId = ToolsPage.PID)]
    public class ResxFormatterOptionsPage : BeSimpleOptionsPage
    {
        private const string PID = nameof(ResxFormatterOptionsPage);
        private const string PageTitle = "Resx Formatter";

        private readonly IconHostBase _iconHost;
        
        public ResxFormatterOptionsPage(
            Lifetime lifetime,
            OptionsPageContext optionsPageContext,
            OptionsSettingsSmartContext optionsSettingsSmartContext,
            [NotNull] IconHostBase iconHost,
            [NotNull] ICommonFileDialogs commonFileDialogs)
            : base(lifetime, optionsPageContext, optionsSettingsSmartContext)
        {
            _iconHost = iconHost;
            
            AddHeader("General");
            AddBoolOption((ResxFormatterSettings x) => x.FormatOnSave, "Sort resx on save");
            AddComboEnum((ResxFormatterSettings x) => x.SortOrder, "Key order:");
            
            AddHeader("Configuration");
            AddText("External configuration file:");
            var configPath = new Property<FileSystemPath>("ResxFormatterOptionsPage::configPath");
            OptionsSettingsSmartContext.SetBinding(lifetime, (ResxFormatterSettings k) => k.ConfigPath, configPath);
            AddFileChooserOption(configPath, "External configuration file", FileSystemPath.Empty, null, commonFileDialogs, null, false, "", null, null, null, null);
            AddBoolOption((ResxFormatterSettings x) => x.SearchToDriveRoot, "Search to drive root");
        }
    }
}