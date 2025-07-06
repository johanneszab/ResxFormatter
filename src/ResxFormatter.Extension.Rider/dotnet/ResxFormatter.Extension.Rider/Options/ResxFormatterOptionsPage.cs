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
            
            AddHeader(Properties.Resources.General);
            AddBoolOption((ResxFormatterSettings x) => x.FormatOnSave, Properties.Resources.SortResxOnSave, Properties.Resources.SortResxOnSaveToolTip);
            AddComboEnum((ResxFormatterSettings x) => x.SortOrder, Properties.Resources.KeyOrder);
            
            AddHeader(Properties.Resources.Configuration);
            AddText(Properties.Resources.ExternalConfigurationFile);
            var configPath = new Property<FileSystemPath>("ResxFormatterOptionsPage::configPath");
            OptionsSettingsSmartContext.SetBinding(lifetime, (ResxFormatterSettings k) => k.ConfigPath, configPath);
            AddFileChooserOption(configPath, Properties.Resources.ExternalConfigurationFileToolTip, FileSystemPath.Empty, null, commonFileDialogs, null, false, "", null, null, null, null);
            AddBoolOption((ResxFormatterSettings x) => x.SearchToDriveRoot, Properties.Resources.SearchToDriveRoot, Properties.Resources.SearchToDriveRootToolTip);
        }
    }
}