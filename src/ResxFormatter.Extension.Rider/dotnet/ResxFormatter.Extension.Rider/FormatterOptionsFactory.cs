using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
using ResxFormatter.Options;

namespace ResxFormatter.Extension.Rider
{
    public static class FormatterOptionsFactory
    {
        private static readonly MemoryCache EditorConfigSettingsCache = MemoryCache.Default;
        private static readonly CacheItemPolicy CacheItemPolicy = new()
        {
            SlidingExpiration = TimeSpan.FromHours(1),
        };
        
        private static readonly Dictionary<StringComparer, StringComparison> ComparerToComparison = 
            new()
            {
                [StringComparer.CurrentCulture] = StringComparison.CurrentCulture,
                [StringComparer.CurrentCultureIgnoreCase] = StringComparison.CurrentCultureIgnoreCase,
                [StringComparer.InvariantCulture] = StringComparison.InvariantCulture,
                [StringComparer.InvariantCultureIgnoreCase] = StringComparison.InvariantCultureIgnoreCase,
                [StringComparer.Ordinal] = StringComparison.Ordinal,
                [StringComparer.OrdinalIgnoreCase] = StringComparison.OrdinalIgnoreCase
            };
        
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
            
            formatterOptions.ImportResxFormatterEditorConfig = settings.GetValue((ResxFormatterSettings s) => s.ImportResxFormatterEditorConfig);
            formatterOptions.FormatOnSave = settings.GetValue((ResxFormatterSettings s) => s.FormatOnSave);
            formatterOptions.SortOrder = settings.GetValue((ResxFormatterSettings s) => s.SortOrder);
            formatterOptions.RemoveXsdSchema = settings.GetValue((ResxFormatterSettings s) => s.RemoveXsdSchema);
            formatterOptions.RemoveDocumentationComment = settings.GetValue((ResxFormatterSettings s) => s.RemoveDocumentationComment);
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

            // Try finding ResxFormatter settings in .editorconfig.
            var cacheKey = Path.GetDirectoryName(sourceFilePath) ?? sourceFilePath;
            ResxEditorConfigSettings? editorConfig = EditorConfigSettingsCache.Get(cacheKey) as ResxEditorConfigSettings;
            if (editorConfig == null)
            {
                editorConfig = new ResxEditorConfigSettings(sourceFilePath);
                EditorConfigSettingsCache.Add(new CacheItem(cacheKey, editorConfig), CacheItemPolicy);
            }

            if (editorConfig.IsActive)
            {
                formatterOptions.SortOrder = ComparerToComparison[editorConfig.Comparer];
                formatterOptions.RemoveXsdSchema = editorConfig.RemoveXsdSchema;
                formatterOptions.RemoveDocumentationComment = editorConfig.RemoveDocumentationComment;
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