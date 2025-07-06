using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Intentions.Xml.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Resx;
using JetBrains.TextControl;
using JetBrains.Util;
using ResxFormatter.Extension.Rider.Resources;

namespace ResxFormatter.Extension.Rider
{
    [ContextAction(
        Name = "ResxFormatter.Reformat",
        Description = "Sorts the document(s) using Resx Formatter.",
        GroupType = typeof(XmlContextActions),
        Disabled = false,
        Priority = -10)]
    public class ResxFormatterReformatContextAction : ContextActionBase
    {
        [NotNull] private readonly ResxContextActionDataProvider _dataProvider;
        [NotNull] private readonly string _text;
        private readonly ActionAppliesTo _actionAppliesTo;

        public ResxFormatterReformatContextAction([NotNull] ResxContextActionDataProvider dataProvider)
            : this(dataProvider, Properties.Resources.SortWithResxFormatter, ActionAppliesTo.File)
        {
        }
        
        private ResxFormatterReformatContextAction(
            [NotNull] ResxContextActionDataProvider dataProvider, 
            [NotNull] string text,
            ActionAppliesTo actionAppliesTo)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _actionAppliesTo = actionAppliesTo;
        }
 
        public override string Text => _text;
        
        public override bool IsAvailable(IUserDataHolder cache) => _dataProvider.Document != null;

        public override IEnumerable<IntentionAction> CreateBulbItems()
        {
            var mainAnchor = new SubmenuAnchor(IntentionsAnchors.ContextActionsAnchor, 
                new SubmenuBehavior(text: Properties.Resources.SortWithResxFormatter, icon: null, executable: true, removeFirst: true));
            var subAnchor2 = new InvisibleAnchor(mainAnchor);
            var subAnchor3 = subAnchor2.CreateNext(separate: true);

            IntentionAction Create(string text, ActionAppliesTo appliesTo, IAnchor anchor)
            {
                return new ResxFormatterReformatContextAction(_dataProvider, text, appliesTo)
                    .ToContextActionIntention(anchor, ResxFormatterThemedIcons.Logo.Id);
            }
            
            return
            [
                Create(_text, _actionAppliesTo, subAnchor3),
                Create(Properties.Resources.SortResxFile, ActionAppliesTo.File, subAnchor3),
                Create(Properties.Resources.SortResxFilesInProject, ActionAppliesTo.Project, subAnchor3),
                Create(Properties.Resources.SortResxFilesInSolution, ActionAppliesTo.Solution, subAnchor3)
            ];
        } 

        protected override Action<ITextControl> ExecutePsiTransaction(
            [NotNull] ISolution solution, 
            [NotNull] IProgressIndicator progress)
        {
            // Fetch settings
            var lifetime = solution.GetSolutionLifetimes().MaximumLifetime;
            var settings = solution.GetSettingsStore().SettingsStore.BindToContextLive(lifetime, ContextRange.Smart(solution.ToDataContext()));
            var formatterOptions = FormatterOptionsFactory.FromSettings(
                settings,
                solution, 
                _dataProvider.Project,
                _actionAppliesTo == ActionAppliesTo.File 
                    ? _dataProvider.SourceFile as IPsiSourceFileWithLocation // Traverse config chain from file path
                    : null  // Traverse config chain from project path
            );
            
            // Perform formatting
            var formatter = new FormatterService(formatterOptions);
            
            var psiSourceFiles = 
                _actionAppliesTo == ActionAppliesTo.File ? _dataProvider.Document.GetPsiSourceFiles(solution).AsIReadOnlyList()
                    : _actionAppliesTo == ActionAppliesTo.Project ? _dataProvider.Project.GetAllProjectFiles(it => it.LanguageType.Is<ResxProjectFileType>()).SelectMany(file => file.ToSourceFiles().AsIReadOnlyList())
                        : _dataProvider.Solution.GetAllProjects().SelectMany(project => project.GetAllProjectFiles(it => it.LanguageType.Is<ResxProjectFileType>()).SelectMany(file => file.ToSourceFiles().AsIReadOnlyList()));

            foreach (var prjItem in psiSourceFiles)
            {
                foreach (var file in prjItem.GetPsiFiles<ResxLanguage>())
                {
                    var sourceFile = file.GetSourceFile();
                    if (sourceFile?.Document != null)
                    {
                        var oldText = sourceFile.Document.GetText();
                        var newText = formatter.FormatDocument(oldText);
                        file.ReParse(new TreeTextRange(new TreeOffset(0), new TreeOffset(oldText.Length)), newText);
                    }
                }
            }

            return null;
        }

        private enum ActionAppliesTo
        {
            File,
            Project,
            Solution
        }
    }
}