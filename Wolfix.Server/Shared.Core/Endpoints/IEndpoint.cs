using Microsoft.AspNetCore.Routing;

namespace Shared.Core.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}