using System.IO;
using System.Text;

namespace ResxFormatter;

public sealed class StringWriterWithEncoding : StringWriter
{
    public StringWriterWithEncoding(Encoding encoding)
    {
        Encoding = encoding;
        NewLine = "\n";
    }

    public override Encoding Encoding { get; }
}
