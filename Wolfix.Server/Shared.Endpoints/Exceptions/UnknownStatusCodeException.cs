using System.Net;

namespace Shared.Endpoints.Exceptions;

public sealed class UnknownStatusCodeException(string endpointName, HttpStatusCode statusCode)
    : Exception($"Endpoint: {endpointName} -> Unknown status code: {statusCode}");