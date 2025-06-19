using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Xunit.Sdk;

namespace ResxFormatter.UnitTests;

public class ResxFileDataAttribute : DataAttribute
{
    private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);
    
    private readonly string _expectedDataPath;
    private readonly string _testDataPath;

    /// <summary>
    /// Load data from a resx files as the data source for a theory
    /// </summary>
    /// <param name="testDataPath">The absolute or relative path to the resx file to load</param>
    /// <param name="expectedDataPath">The absolute or relative path to the resx file to load</param>
    public ResxFileDataAttribute(string testDataPath, string expectedDataPath)
    {
        _testDataPath = testDataPath;
        _expectedDataPath = expectedDataPath;
    }

    /// <inheritDoc />
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        ArgumentNullException.ThrowIfNull(testMethod);

        var expectedDataAbsolutePath = Path.IsPathRooted(_expectedDataPath)
            ? _expectedDataPath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), _expectedDataPath);

        if (!File.Exists(expectedDataAbsolutePath))
        {
            throw new ArgumentException($"Could not find file at path: {expectedDataAbsolutePath}");
        }
        
        var testDataAbsolutePath = Path.IsPathRooted(_testDataPath)
            ? _testDataPath
            : Path.GetRelativePath(Directory.GetCurrentDirectory(), _testDataPath);

        if (!File.Exists(testDataAbsolutePath))
        {
            throw new ArgumentException($"Could not find file at path: {testDataAbsolutePath}");
        }

        XDocument testDocument = LoadResxFromFile(_testDataPath);
        XDocument expectedDocument = LoadResxFromFile(_expectedDataPath);

        var objectsList = new List<object[]>();
        objectsList.Add([testDocument, expectedDocument]);
        return objectsList;
    }
    
    private XDocument LoadResxFromFile(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream, Utf8WithoutBom, true);
        return XDocument.Load(reader);
    }
}