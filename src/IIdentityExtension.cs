using System.Collections.Generic;
using System.Security.Claims;
using Soenneker.Extensions.String;

namespace Soenneker.Extensions.IIdentity;

/// <summary>
/// A collection of helpful IIdentity (authentication, authorization) extension methods
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IIdentityExtension
{
    public static void AddRolesFromJobTitle(this System.Security.Principal.IIdentity? identity)
    {
        if (identity is not ClaimsIdentity claimsIdentity)
            return;

        Claim? jobTitle = claimsIdentity.FindFirst("jobTitle");

        if (jobTitle == null)
            return;

        List<string> roles = jobTitle.Value.FromCommaSeparatedToList();

        var claims = new List<Claim>();

        for (var i = 0; i < roles.Count; i++)
        {
            string role = roles[i];
            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }

        if (claims.Count > 0)
            claimsIdentity.AddClaims(claims);
    }
}
