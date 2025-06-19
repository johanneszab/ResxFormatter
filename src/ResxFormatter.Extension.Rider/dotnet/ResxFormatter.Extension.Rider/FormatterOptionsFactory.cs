using System;
using System.IO;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
using ResxFormatter.Options;

namespace ResxFormatter.Extension.Rider
{
    public static class FormatterOptionsFactory
    {
        public static IFormatterOptions FromSettings(
            IContextBoundSettingsStoreLive settings,
            ISolution? solution,
            IProject? project,
            IPsiSourceFileWithLocation? psiSourceFileWithLocation)
        {
            return FromSettings(
                settings: settings,
                solution: solution, 
                projectPath: project?.ProjectFileLocation?.FullPath, 
                sourceFilePath: psiSourceFileWithLocation?.Location.FullPath);
        }
        
        public static IFormatterOptions FromSettings(
            IContextBoundSettingsStoreLive settings,
            ISolution? solution,
            string? projectPath,
            string? sourceFilePath)
        {
             // Normalize paths as Rider may pass them in with differing separator chars
             projectPath = NormalizePath(projectPath);
             sourceFilePath = NormalizePath(sourceFilePath);

            // Load global settings
            IFormatterOptions formatterOptions = new FormatterOptions();
            
            formatterOptions.FormatOnSave = settings.GetValue((ResxFormatterSettings s) => s.FormatOnSave);
            formatterOptions.SortOrder = settings.GetValue((ResxFormatterSettings s) => s.SortOrder);
            formatterOptions.ConfigPath = settings.GetValue((ResxFormatterSettings s) => s.ConfigPath)?.FullPath;
            formatterOptions.SearchToDriveRoot = settings.GetValue((ResxFormatterSettings s) => s.SearchToDriveRoot);
            
            // Try finding settings in our project or solution
            if (!string.IsNullOrEmpty(projectPath) || !string.IsNullOrEmpty(sourceFilePath))
            {
                var searchToDriveRoot = settings.GetValue((ResxFormatterSettings s) => s.SearchToDriveRoot);
                
                var highestRootPath = solution is { IsTemporary: false }
                    ? (searchToDriveRoot ? Path.GetPathRoot(solution.SolutionFilePath.FullPath) : Path.GetDirectoryName(solution.SolutionFilePath.FullPath))
                    : string.Empty;

                var itemPath = sourceFilePath;
                
                var configPath = (!string.IsNullOrEmpty(itemPath) && itemPath.StartsWith(highestRootPath, StringComparison.OrdinalIgnoreCase))
                    ? GetConfigPathForProject(highestRootPath, itemPath)
                    : GetConfigPathForProject(projectPath ?? itemPath, itemPath);
                if (!string.IsNullOrEmpty(configPath))
                {
                    formatterOptions = ((FormatterOptions)formatterOptions).Clone();
                    formatterOptions.ConfigPath = configPath;
                }
            }

            return formatterOptions;
        }
        
        private static string GetConfigPathForProject(string highestRootPath, string path)
        {
            if (path.IsNullOrEmpty())
            {
                return null;
            }
            
            var currentDirectory = Path.GetDirectoryName(path);
            while (currentDirectory?.StartsWith(highestRootPath, StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                var configurationFilePath = Path.Combine(currentDirectory, "Settings.ResxFormatter");
                if (File.Exists(configurationFilePath))
                {
                    return configurationFilePath;
                }
                
                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            return null;
        }

        private static string NormalizePath(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return null;
            }

            return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}