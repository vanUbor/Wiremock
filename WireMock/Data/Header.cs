using System.Diagnostics.CodeAnalysis;

namespace WireMock.Data;

[ExcludeFromCodeCoverage]
public class Header
{
    public string? Name { get; set; }
    public Matcher[]? Matchers { get; set; }
    public bool IgnoreCase { get; set; }
}