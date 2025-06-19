using JetBrains.Application.Parts;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Psi.Resx;
using JetBrains.ReSharper.Psi.Resx.Tree;
using JetBrains.TextControl;

namespace ResxFormatter.Extension.Rider;

[ContextActionDataBuilder(typeof (ResxContextActionDataProvider), Instantiation.DemandAnyThreadSafe)]
internal class ResxContextActionDataBuilder : ContextActionDataBuilderBase<ResxLanguage, IResxFile>
{
    public override IContextActionDataProvider BuildFromPsi(
        ISolution solution,
        ITextControl textControl,
        IResxFile psiFile)
    {
        return (IContextActionDataProvider) new ResxContextActionDataProvider(solution, textControl, psiFile);
    }
}