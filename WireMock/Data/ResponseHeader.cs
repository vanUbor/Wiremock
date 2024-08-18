using System.Diagnostics.CodeAnalysis;

namespace WireMock.Data;

[ExcludeFromCodeCoverage]
public class ResponseHeader
{
    public string? Content_Type { get; set; }
    public string? Content_Encoding { get; set; }
    public string? Date { get; set; }
    public string? Transfer_Encoding { get; set; }
    public string? Connection { get; set; }
    public string[]? Vary { get; set; }
    public string? CF_Cache_Status { get; set; }
    public string? Report_To { get; set; }
    public string? NEL { get; set; }
    public string? Server { get; set; }
    public string? CF_RAY { get; set; }
    public string? Alt_Svc { get; set; }
}