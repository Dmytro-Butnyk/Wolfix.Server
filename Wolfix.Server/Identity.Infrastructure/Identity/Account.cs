using Identity.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Identity;

public sealed class Account : IdentityUser<Guid>
{
    //todo: обдумать как сделать
    public AccountAuthProvider AuthProvider { get; init; } = AccountAuthProvider.Custom;
}