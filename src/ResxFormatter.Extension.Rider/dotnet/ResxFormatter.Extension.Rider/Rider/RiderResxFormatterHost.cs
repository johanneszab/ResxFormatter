using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.Rd.Tasks;
using JetBrains.Rider.Backend.Features;
using JetBrains.Rider.Model;

namespace ResxFormatter.Extension.Rider.Rider
{
    [SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
    public class RiderResxFormatterHost
    {
        [NotNull]
        private readonly Lifetime _lifetime;

        [NotNull]
        private readonly SolutionModel _solutionModel;

        [NotNull]
        private readonly ISolution _solution;

        [NotNull]
        private readonly DocumentManager _documentManager;

        public RiderResxFormatterHost(
            [NotNull] Lifetime lifetime,
            [NotNull] SolutionModel solutionModel,
            [NotNull] ISolution solution,
            [NotNull] DocumentManager documentManager
        )
        {
            _lifetime = lifetime;
            _solutionModel = solutionModel;
            _solution = solution;
            _documentManager = documentManager;

            var rdSolutionModel = solutionModel.TryGetCurrentSolution();
            if (rdSolutionModel != null)
            {
                var rdResxFormatterModel = rdSolutionModel.GetResxFormatterModel();
                rdResxFormatterModel.PerformReformat.SetAsync(PerformReformatHandler);
            }
        }

        private Task<RdResxFormatterFormattingResult> PerformReformatHandler(
            Lifetime requestLifetime,
            RdResxFormatterFormattingRequest request
        )
        {
            return Task.Run(
                    () =>
                    {
                        _lifetime.ThrowIfNotAlive();

                        // Fetch settings
                        var settings = _solution
                            .GetSettingsStore()
                            .SettingsStore.BindToContextLive(
                                _lifetime,
                                ContextRange.Smart(_solution.ToDataContext())
                            );
                        var formatterOptions = FormatterOptionsFactory.FromSettings(
                            settings,
                            _solution,
                            null,
                            request.FilePath
                        );

                        // Bail out early if needed
                        if (!formatterOptions.FormatOnSave)
                            return new RdResxFormatterFormattingResult(false, false, "");

                        // Perform styling
                        var formatter = new FormatterService(formatterOptions);

                        var formattedText = formatter
                            .FormatDocument(request.DocumentText);

                        if (request.DocumentText == formattedText)
                        {
                            return new RdResxFormatterFormattingResult(true, false, "");
                        }
                        return new RdResxFormatterFormattingResult(true, true, formattedText);
                    },
                    requestLifetime
                )
                .ToRdTask();
        }
    }
}
