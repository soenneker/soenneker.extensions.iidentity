[![](https://img.shields.io/nuget/v/soenneker.extensions.iidentity.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.iidentity/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.iidentity/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.iidentity/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.iidentity.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.iidentity/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.iidentity/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.iidentity/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.IIdentity
A collection of helpful IIdentity (authentication, authorization) extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.IIdentity
```

## Quick start

```csharp
using Soenneker.Extensions.IIdentity;

// Given an existing System.Security.Principal.IIdentity? named identity:
identity.AddRolesFromJobTitle();
```

## Common operations

- `AddRolesFromJobTitle()` - Adds role claims by parsing the comma-separated "jobTitle" claim into ClaimTypes.Role claims. Highest-perf path: one scan to count segments, rent Claim[] from ArrayPool, add in one shot.
- `AddRolesFromRoles()` - Adds role claims by parsing the JSON array in the "roles" claim into ClaimTypes.Role claims. Uses pooled Claim[] and avoids creating List<Claim>.
