// MIT License
//
// Copyright (c) 2019 Stefan Egli
// Copyright (c) 2025 Johannes Meyer zum Alten Borgloh

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ResxFormatter.Options;

namespace ResxFormatter
{
    public class FormatterService(IFormatterOptions options)
    {
        private static readonly XName NameAttributeName = XNamespace.None.GetName(@"name");
        private const string FakeSchema = "<schema />";
        private static string Original => ResXResourceWriterStub.ResourceSchemaWithComment;
        private static string OriginalComment { get; } = Comment(ResXResourceWriterStub.ResourceSchemaWithComment);
        private static string OriginalCommentContent { get; } = CommentContent(ResXResourceWriterStub.ResourceSchemaWithComment);
        private static string OriginalSchema { get; } = Schema(ResXResourceWriterStub.ResourceSchemaWithComment);

        /// <summary>
        /// Execute styling from string input
        /// </summary>
        /// <param name="resxDocument"></param>
        /// <returns></returns>
        public string FormatDocument(string resxDocument)
        {
            var document = XDocument.Parse(resxDocument, LoadOptions.None);
            
            var hasChanged = SortResx(document, options);
            if (!hasChanged)
            {
                return resxDocument;
            }

            using var writer = new StringWriterWithEncoding(Encoding.UTF8);
            document.Save(writer);
            return writer.ToString().Replace("\r\n", "\n");
        }
        
        private bool SortResx(XDocument document, IFormatterOptions formatterOptions)
        {
            var hasSchemaRemoved = false;
            var hasCommentRemoved = false;
            var toSave = new List<XNode>();
            var toSort = new List<XElement>();
            var comparer = StringComparer.FromComparison(options.SortOrder);

            foreach (var node in document.Root!.Nodes())
            {
                if (formatterOptions.RemoveXsdSchema)
                {
                    if (!hasSchemaRemoved && node is XElement { Name.LocalName: "schema" })
                    {
                        toSave.Add(XElement.Parse(FakeSchema));
                        hasSchemaRemoved = true;
                        continue;
                    }
                }

                if (formatterOptions.RemoveDocumentationComment)
                {
                    if (!hasCommentRemoved && node.NodeType == XmlNodeType.Comment)
                    {
                        hasCommentRemoved = true;
                        continue;
                    }
                }

                if (node is XElement { Name.LocalName: "data" or "metadata" } element)
                {
                    toSort.Add(element);
                }
                else
                {
                    toSave.Add(node);
                }
            }
            
            var sorted = toSort.
                OrderBy(GetName, comparer)
                .ToList();

            var hasCommentAdded = false;
            if (!formatterOptions.RemoveDocumentationComment && !HasDocumentationComment(document))
            {
                toSave.Insert(0, new XComment(OriginalCommentContent));
                hasCommentAdded = true;
            }

            var hasSchemaAdded = false;
            if (!formatterOptions.RemoveXsdSchema && !HasSchemaNode(document))
            {
                toSave.Insert(1, XElement.Parse(OriginalSchema));
                hasSchemaAdded = true;
            }

            var requiresSorting = !toSort.SequenceEqual(sorted);
            if (hasSchemaRemoved || hasCommentRemoved || hasCommentAdded || hasSchemaAdded || requiresSorting)
            {
                toSave.AddRange(sorted);
                document.Root.ReplaceNodes(toSave);
                return true;
            }

            return false;
        }

        private static string GetName(XElement node)
        {
            return node.Attribute(NameAttributeName)?.Value.TrimStart('>') ?? string.Empty;
        }
        
        private static bool HasDocumentationComment(XDocument document)
        {
            if (document.Root!.Nodes().FirstOrDefault(n => n.NodeType == XmlNodeType.Comment) is not XComment firstComment)
            {
                return false;
            }

            var value = RemoveWhiteSpace(firstComment.ToString());
            var schema = RemoveWhiteSpace(OriginalComment);
            return value == schema;

            string RemoveWhiteSpace(string text) => string.Join("", text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        private static bool HasSchemaNode(XDocument document)
        {
            if (document.Root!.Nodes().FirstOrDefault(n => n.NodeType == XmlNodeType.Element) is not XElement firstElement)
            {
                return false;
            }

            var value = RemoveWhiteSpace(firstElement.ToString());
            var schema = RemoveWhiteSpace(OriginalSchema);
            return value == schema;

            string RemoveWhiteSpace(string text) => string.Join("", text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        
        private static string Comment(string text)
        {
            var endOfComment = text.IndexOf("-->", StringComparison.Ordinal);
            return endOfComment > 0 ? text.Substring(0, endOfComment + 3) : text;
        }

        private static string CommentContent(string text)
        {
            var startOfComment = text.IndexOf("<!--", StringComparison.Ordinal);
            var endOfComment = text.IndexOf("-->", StringComparison.Ordinal);
            return startOfComment > 0 && endOfComment > 0 ? text.Substring(startOfComment + 4, endOfComment - startOfComment - 4) : text;
        }

        private static string Schema(string text)
        {
            var endOfComment = text.IndexOf("-->", StringComparison.Ordinal);
            return endOfComment > 0 ? text.Substring(endOfComment + 3).Trim() : text;
        }
    }
}