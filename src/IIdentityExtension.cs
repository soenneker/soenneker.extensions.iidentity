using Soenneker.Extensions.String;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Soenneker.Extensions.IIdentity;

/// <summary>
/// A collection of helpful IIdentity (authentication, authorization) extension methods
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IIdentityExtension
{
    /// <summary>
    /// Adds role claims by parsing the comma-separated "jobTitle" claim into ClaimTypes.Role claims.
    /// </summary>
    public static void AddRolesFromJobTitle(this System.Security.Principal.IIdentity? identity)
    {
        AddRolesFromClaim(identity, "jobTitle");
    }

    /// <summary>
    /// Adds role claims by parsing the comma-separated "roles" claim into ClaimTypes.Role claims.
    /// </summary>
    public static void AddRolesFromRoles(this System.Security.Principal.IIdentity? identity)
    {
        AddRolesFromClaim(identity, "roles");
    }

    /// <summary>
    /// Adds role claims by parsing the comma-separated generic claim into ClaimTypes.Role claims.
    /// </summary>
    public static void AddRolesFromClaim(System.Security.Principal.IIdentity? identity, string claimType)
    {
        if (identity is not ClaimsIdentity claimsIdentity)
            return;

        string? value = claimsIdentity.FindFirst(claimType)?.Value;

        if (value.IsNullOrWhiteSpace())
            return;

        List<Claim>? claims = null;

        var start = 0;

        for (var i = 0; i <= value.Length; i++)
        {
            if (i == value.Length || value[i] == ',')
            {
                ReadOnlySpan<char> span = value.AsSpan(start, i - start).Trim();

                if (!span.IsEmpty)
                {
                    claims ??= new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Role, span.ToString()));
                }

                start = i + 1;
            }
        }

        if (claims is not null)
            claimsIdentity.AddClaims(claims);
    }
}