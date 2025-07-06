using System;

namespace ResxFormatter.Options
{
    /// <summary>
    /// Options controls how formatter works.
    /// </summary>
    public interface IFormatterOptions
    {
        bool ImportResxFormatterEditorConfig { get; set;  }
        
        bool FormatOnSave { get; set; }
        
        StringComparison SortOrder { get; set; }
        
        string ConfigPath { get; set; }

        bool SearchToDriveRoot { get; set; }
        
        bool RemoveXsdSchema { get; set; }
        
        bool RemoveDocumentationComment { get; set; }
    }
}