using Soenneker.Extensions.String;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Soenneker.Extensions.Enumerable;
using Soenneker.Utils.Json;

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
        if (identity is not ClaimsIdentity claimsIdentity)
            return;

        string? value = claimsIdentity.FindFirst("jobTitle")?.Value;

        if (value.IsNullOrWhiteSpace())
            return;

        ReadOnlySpan<char> span = value.AsSpan();
        var claims = new List<Claim>();
        var start = 0;

        for (var i = 0; i <= span.Length; i++)
        {
            if (i == span.Length || span[i] == ',')
            {
                ReadOnlySpan<char> slice = span.Slice(start, i - start).Trim();

                if (!slice.IsEmpty)
                    claims.Add(new Claim(ClaimTypes.Role, slice.ToString()));

                start = i + 1;
            }
        }

        if (claims.Count > 0)
            claimsIdentity.AddClaims(claims);
    }

    /// <summary>
    /// Adds role claims by parsing the comma-separated "roles" claim into ClaimTypes.Role claims.
    /// </summary>
    public static void AddRolesFromRoles(this System.Security.Principal.IIdentity? identity)
    {
        if (identity is not ClaimsIdentity claimsIdentity)
            return;

        string? value = claimsIdentity.FindFirst("roles")?.Value;

        if (value.IsNullOrWhiteSpace())
            return;

        var roles = JsonUtil.Deserialize<List<string>>(value);

        if (roles.IsNullOrEmpty())
            return;

        List<Claim> claims = new(roles.Count);

        for (var i = 0; i < roles.Count; i++)
        {
            string role = roles[i];

            if (role.HasContent())
            {
                ReadOnlySpan<char> span = role.AsSpan().Trim();

                if (!span.IsEmpty)
                    claims.Add(new Claim(ClaimTypes.Role, span.ToString()));
            }
        }

        if (claims.Count > 0)
            claimsIdentity.AddClaims(claims);
    }
}