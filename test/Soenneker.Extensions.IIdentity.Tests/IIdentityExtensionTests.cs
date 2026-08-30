using System.Linq;
using System.Security.Claims;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Extensions.IIdentity.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class IdentityExtensionTests : HostedUnitTest
{
    public IdentityExtensionTests(Host host) : base(host)
    {
    }

    [Test]
    public async System.Threading.Tasks.Task Role_promotion_is_idempotent()
    {
        var identity = new ClaimsIdentity([new Claim("jobTitle", "Administrator, Billing")]);

        identity.AddRolesFromJobTitle();
        identity.AddRolesFromJobTitle();

        await Assert.That(identity.FindAll(ClaimTypes.Role).Count()).IsEqualTo(2);
    }

    [Test]
    public async System.Threading.Tasks.Task Malformed_roles_json_is_ignored()
    {
        var identity = new ClaimsIdentity([new Claim("roles", "not-json")]);

        identity.AddRolesFromRoles();

        await Assert.That(identity.FindAll(ClaimTypes.Role).Any()).IsFalse();
    }
}
