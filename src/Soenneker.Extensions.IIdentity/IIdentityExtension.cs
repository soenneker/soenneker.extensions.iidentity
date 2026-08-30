using System;
using System.Security.Claims;
using System.Text.Json;
using Soenneker.Extensions.String;
using Soenneker.Utils.Json;

namespace Soenneker.Extensions.IIdentity;

// ReSharper disable once InconsistentNaming
/// <summary>
/// Represents the i identity extension.
/// </summary>
public static class IIdentityExtension
{
    private const string _jobTitleClaimType = "jobTitle";
    private const string _rolesClaimType = "roles";

    /// <summary>
    /// Adds role claims by parsing the comma-separated "jobTitle" claim into ClaimTypes.Role claims.
    /// Existing role claims with the same exact value are not duplicated.
    /// </summary>
    public static void AddRolesFromJobTitle(this System.Security.Principal.IIdentity? identity)
    {
        if (identity is not ClaimsIdentity ci)
            return;

        string? value = ci.FindFirst(_jobTitleClaimType)
                          ?.Value;

        if (value.IsNullOrWhiteSpace())
            return;

        ReadOnlySpan<char> span = value.AsSpan();

        var start = 0;

        for (var i = 0; i <= span.Length; i++)
        {
            if (i != span.Length && span[i] != ',')
                continue;

            ReadOnlySpan<char> slice = span.Slice(start, i - start);
            ReadOnlySpan<char> trimmed = slice.Trim();

            if (!trimmed.IsEmpty)
            {
                // If trimming didn't change it, avoid allocating a trimmed string.
                // Substring still allocates, but it's cheaper than Trim().ToString() when no trim needed.
                string role = trimmed.Length == slice.Length ? value.Substring(start, slice.Length) : trimmed.ToString();

                AddRoleIfMissing(ci, role);
            }

            start = i + 1;
        }
    }

    /// <summary>
    /// Adds role claims by parsing the JSON array in the "roles" claim into ClaimTypes.Role claims.
    /// Malformed JSON is ignored and existing role claims with the same exact value are not duplicated.
    /// </summary>
    public static void AddRolesFromRoles(this System.Security.Principal.IIdentity? identity)
    {
        if (identity is not ClaimsIdentity ci)
            return;

        string? value = ci.FindFirst(_rolesClaimType)
                          ?.Value;
        if (string.IsNullOrWhiteSpace(value))
            return;

        // Deserialize to array (cheaper than List<T>). Still allocates the strings (unavoidable).
        string[]? roles;

        try
        {
            roles = JsonUtil.Deserialize<string[]>(value);
        }
        catch (JsonException)
        {
            return;
        }

        if (roles is null || roles.Length == 0)
            return;

        for (var i = 0; i < roles.Length; i++)
        {
            string? roleStr = roles[i];

            if (roleStr.IsNullOrWhiteSpace())
                continue;

            ReadOnlySpan<char> roleSpan = roleStr.AsSpan();
            ReadOnlySpan<char> trimmed = roleSpan.Trim();

            if (trimmed.IsEmpty)
                continue;

            // If no trim occurred, reuse original string (zero allocation).
            string role = trimmed.Length == roleSpan.Length ? roleStr : trimmed.ToString();

            AddRoleIfMissing(ci, role);
        }
    }

    private static void AddRoleIfMissing(ClaimsIdentity identity, string role)
    {
        if (!identity.HasClaim(ClaimTypes.Role, role))
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
    }
}
