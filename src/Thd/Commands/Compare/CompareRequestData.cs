using System.Net;

namespace Thd.Commands.Compare;

public record CompareRequestData(ResolvedUrl UrlActual, ResolvedUrl UrlExpected, HttpStatusCode? ExpectedStatusCode);