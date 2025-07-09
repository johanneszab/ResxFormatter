// MIT License
//
// Copyright (c) 2019 Stefan Egli

using System;
using JetBrains.Util.Logging;

namespace ResxFormatter.Options;

public interface IFormatSettings
{
    StringComparer Comparer { get; }
    bool RemoveDocumentationComment { get; }
    bool RemoveXsdSchema { get; }
    bool SortEntries { get; }
}

public class ResxEditorConfigSettings : IFormatSettings
{
    public ResxEditorConfigSettings(string targetFile = "dummy.resx")
    {
        var isActive = false;
        try
        {
            var parser = new EditorConfig.Core.EditorConfigParser();
            var settings = parser.Parse(targetFile).Properties;
            if (settings.TryGetValue("resx_formatter_sort_entries", out var sortEntries))
            {
                isActive = true;
                this.SortEntries = IsEnabled(sortEntries);
            }

            if (settings.TryGetValue("resx_formatter_remove_xsd_schema", out var removeSchema))
            {
                isActive = true;
                this.RemoveXsdSchema = IsEnabled(removeSchema);
            }

            if (settings.TryGetValue("resx_formatter_remove_documentation_comment", out var removeComment))
            {
                isActive = true;
                this.RemoveDocumentationComment = IsEnabled(removeComment);
            }

            if (this.SortEntries && settings.TryGetValue("resx_formatter_sort_comparer", out var comparerString))
            {
                this.Comparer = Comparer(comparerString);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to parse EditorConfig file:\n" + ex.ToString());
        }

        this.IsActive = isActive;
        return;

        bool IsEnabled(string setting) => "true" == setting;

        StringComparer Comparer(string comparerString)
        {
            return comparerString switch
            {
                nameof(StringComparer.InvariantCulture) => StringComparer.InvariantCulture,
                nameof(StringComparer.InvariantCultureIgnoreCase) => StringComparer.InvariantCultureIgnoreCase,
                nameof(StringComparer.OrdinalIgnoreCase) => StringComparer.OrdinalIgnoreCase,
                _ => StringComparer.Ordinal
            };
        }
    }

    public StringComparer Comparer { get; private set; } = StringComparer.OrdinalIgnoreCase;
    public bool IsActive { get; }
    public bool RemoveDocumentationComment { get; }
    public bool RemoveXsdSchema { get; }
    public bool SortEntries { get; }
}