using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace WireMock.Data;


public class WireMockMappingModel
{
    public string? Guid { get; }
    

    public DateTime UpdatedAt { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int Priority { get; set; }
    public required Request Request { get; set; }
    public required Response Response { get; set; }

    [JsonIgnore] public required string Raw { get; set; }
}

public class Request
{
    public Path? Path { get; set; }
    public string[]? Methods { get; set; }
    public string? HttpVersion { get; set; }
    public Header[]? Headers { get; set; }
}

public class Path
{
    public Matcher[]? Matchers { get; set; }
}


public class Header
{
    public string? Name { get; set; }
    public Matcher[]? Matchers { get; set; }
    public bool IgnoreCase { get; set; }
}

public class Matcher
{
    public string? Name { get; set; }
    public string? Pattern { get; set; }
    public bool IgnoreCase { get; set; }
}

public class Response
{
    public int StatusCode { get; set; }
    public string? BodyAsBytes { get; set; }
    public string? Body { get; set; }
    public JsonObject? BodyAsJson { get; set; }
    public JsonObject? Headers { get; set; }
}

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
