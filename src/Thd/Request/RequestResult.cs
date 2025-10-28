using System.Net;
using System.Text.Json.Nodes;

namespace Thd.Request;

public record RequestResult(Uri InspectionUrl, JsonNode? Node, HttpStatusCode StatusCode);