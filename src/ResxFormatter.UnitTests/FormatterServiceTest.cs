using System;
using System.Text;
using System.Xml.Linq;
using ResxFormatter.Options;
using Xunit;

namespace ResxFormatter.UnitTests;

public class FormatterServiceTest
{
    private readonly FormatterService _formatterService = new(new FormatterOptions()
    {
        SortOrder = StringComparison.OrdinalIgnoreCase,
    });
    
    [Theory]
    [ResxFileData("TestData/unsorted.resx", "TestData/sorted.resx")]
    public void FormatsOrdinalIgnoreCase(XDocument testDocument, XDocument expectedDocument)
    {
        var testData = ToStringWithoutCarriageReturn(testDocument);
        var formatted = _formatterService.FormatDocument(testData);
        var expected = ToStringWithoutCarriageReturn(expectedDocument);
        Assert.Equal(formatted, expected);
    }

    private static string ToStringWithoutCarriageReturn(XDocument document)
    {
        using StringWriterWithEncoding writer = new StringWriterWithEncoding(Encoding.UTF8);
        document.Save(writer);
        return writer.ToString().Replace("\r\n", "\n");
    }
}
