using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi.ContentModel;
using JetBrains.ReSharper.Psi.Resx.Tree;
using JetBrains.TextControl;

namespace ResxFormatter.Extension.Rider;

public class ResxContextActionDataProvider(
    ISolution? solution,
    ITextControl? textControl,
    IResxFile? rsxFile) : 
    CachedContextActionDataProviderBase<IResxFile>(solution, textControl, rsxFile),
    ISupportsContentModelForkTranslation<ResxContextActionDataProvider>
{
    public ResxContextActionDataProvider? TryTranslateToCurrentFork(
        IContentModelForkTranslator translator)
    {
        IResxFile currentFork = translator.TryTranslateNodeToCurrentFork<IResxFile>(this.PsiFile);
        return currentFork == null ? (ResxContextActionDataProvider) null : new ResxContextActionDataProvider(this.Solution, translator.TranslateTextControl(this.TextControl), currentFork);
    }
}
