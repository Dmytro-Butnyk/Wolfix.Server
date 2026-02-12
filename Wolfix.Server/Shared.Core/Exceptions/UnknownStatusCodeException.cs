using System.Net;

namespace Shared.Core.Exceptions;

public sealed class UnknownStatusCodeException(string endpointGroupName, string endpointName, HttpStatusCode statusCode)
    : Exception($"Endpoint group: {endpointGroupName} -> Endpoint: {endpointName} -> Unknown status code: {statusCode}");