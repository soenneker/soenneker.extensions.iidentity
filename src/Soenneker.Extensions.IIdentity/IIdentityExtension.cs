using System;
using System.Buffers;
using System.Security.Claims;
using Soenneker.Extensions.String;
using Soenneker.Utils.Json;

namespace Soenneker.Extensions.IIdentity;

// ReSharper disable once InconsistentNaming
public static class IIdentityExtension
{
    private const string _jobTitleClaimType = "jobTitle";
    private const string _rolesClaimType = "roles";

    /// <summary>
    /// Adds role claims by parsing the comma-separated "jobTitle" claim into ClaimTypes.Role claims.
    /// Highest-perf path: one scan to count segments, rent Claim[] from ArrayPool, add in one shot.
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

        // Count potential segments (commas + 1) so we can rent once.
        var segments = 1;

        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] == ',')
                segments++;
        }

        // Early out for the common "no comma, but maybe whitespace" case:
        // We'll still parse below, but this keeps the pooled rent size minimal.
        Claim[] rented = ArrayPool<Claim>.Shared.Rent(segments);

        var claimCount = 0;
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

                rented[claimCount++] = new Claim(ClaimTypes.Role, role);
            }

            start = i + 1;
        }

        if (claimCount > 0)
        {
            // AddClaims enumerates; safe to return array after this call.
            // We pass only the used portion via ArraySegment to avoid enumerating null tail.
            ci.AddClaims(new ArraySegment<Claim>(rented, 0, claimCount));
        }

        // Clear used slots so Claim references aren't kept alive by the pool.
        Array.Clear(rented, 0, claimCount);
        ArrayPool<Claim>.Shared.Return(rented);
    }

    /// <summary>
    /// Adds role claims by parsing the JSON array in the "roles" claim into ClaimTypes.Role claims.
    /// Uses pooled Claim[] and avoids creating List&lt;Claim&gt;.
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
        string[]? roles = JsonUtil.Deserialize<string[]>(value);
        if (roles is null || roles.Length == 0)
            return;

        Claim[] rented = ArrayPool<Claim>.Shared.Rent(roles.Length);

        var claimCount = 0;

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

            rented[claimCount++] = new Claim(ClaimTypes.Role, role);
        }

        if (claimCount > 0)
            ci.AddClaims(new ArraySegment<Claim>(rented, 0, claimCount));

        Array.Clear(rented, 0, claimCount);
        ArrayPool<Claim>.Shared.Return(rented);
    }
}