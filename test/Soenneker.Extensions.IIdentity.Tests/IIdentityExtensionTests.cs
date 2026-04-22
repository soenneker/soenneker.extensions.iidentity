using Soenneker.Tests.HostedUnit;

namespace Soenneker.Extensions.IIdentity.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class IdentityExtensionTests : HostedUnitTest
{
    public IdentityExtensionTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
