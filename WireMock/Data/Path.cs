using System.Diagnostics.CodeAnalysis;

namespace WireMock.Data;

[ExcludeFromCodeCoverage]
public class Path
{
    public Matcher[]? Matchers { get; set; }
}