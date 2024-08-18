using System.Diagnostics.CodeAnalysis;

namespace WireMock.Data;

[ExcludeFromCodeCoverage]
public class Request
{
    public Path? Path { get; set; }
    public string[]? Methods { get; set; }
    public string? HttpVersion { get; set; }
    public Header[]? Headers { get; set; }
}