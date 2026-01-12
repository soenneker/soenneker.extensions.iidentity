using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Extensions.IIdentity.Tests;

[Collection("Collection")]
public class IdentityExtensionTests : FixturedUnitTest
{
    public IdentityExtensionTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
