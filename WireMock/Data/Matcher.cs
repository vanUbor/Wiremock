using System.Diagnostics.CodeAnalysis;

namespace WireMock.Data;

[ExcludeFromCodeCoverage]
public class Matcher
{
    public string? Name { get; set; }
    public string? Pattern { get; set; }
    public bool IgnoreCase { get; set; }
}