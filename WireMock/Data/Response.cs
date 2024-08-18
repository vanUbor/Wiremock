using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace WireMock.Data;

[ExcludeFromCodeCoverage]
public class Response
{
    public int StatusCode { get; set; }
    public string? BodyAsBytes { get; set; }
    public string? Body { get; set; }
    public JsonObject? BodyAsJson { get; set; }
    public JsonObject? Headers { get; set; }
}