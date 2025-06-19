using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ResxFormatter.Options;

namespace ResxFormatter
{
    public class FormatterService(IFormatterOptions options)
    {
        private static readonly XName NameAttributeName = XNamespace.None.GetName(@"name");

        /// <summary>
        /// Execute styling from string input
        /// </summary>
        /// <param name="resxDocument"></param>
        /// <returns></returns>
        public string FormatDocument(string resxDocument)
        {
            XDocument document = XDocument.Parse(resxDocument, LoadOptions.None);
            var documentRoot = document.Root;
            
            var hasChanged = Sort(documentRoot!, options.SortOrder);
            if (!hasChanged)
            {
                return resxDocument;
            }

            using StringWriterWithEncoding writer = new StringWriterWithEncoding(Encoding.UTF8);
            document.Save(writer);
            return writer.ToString().Replace("\r\n", "\n");
        }
        
        private bool Sort(XElement root, StringComparison stringComparison)
        {
            var comparer = new DelegateComparer<string>((left, right) => string.Compare(left, right, stringComparison));
            
            var defaultNamespace = root.GetDefaultNamespace();
            var dataNodeName = defaultNamespace.GetName(@"data");

            var nodes = root
                .Elements(dataNodeName)
                .ToArray();

            var sortedNodes = nodes
                .OrderBy(GetName, comparer)
                .ToArray();

            return SortNodes(root, nodes, sortedNodes);
            
            static string GetName(XElement node)
            {
                return node.Attribute(NameAttributeName)?.Value.TrimStart('>') ?? string.Empty;
            }
        }
        
        private bool SortNodes(XElement root, XElement[] nodes, XElement[] sortedNodes)
        {
            if (nodes.SequenceEqual(sortedNodes))
                return false;

            foreach (var item in nodes)
            {
                item.Remove();
            }

            foreach (var item in sortedNodes)
            {
                root.Add(item);
            }

            return true;
        }
    }
}