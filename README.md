[![](https://img.shields.io/nuget/v/soenneker.extensions.iidentity.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.iidentity/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.iidentity/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.iidentity/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.iidentity.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.iidentity/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.iidentity/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.iidentity/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.IIdentity
Promotes selected identity claims into standard .NET role claims for authorization.

## Installation

```bash
dotnet add package Soenneker.Extensions.IIdentity
```

## Promote a comma-separated job title claim

```csharp
using Soenneker.Extensions.IIdentity;

identity.AddRolesFromJobTitle();
```

Given a `jobTitle` claim such as `"Administrator, Billing"`, `AddRolesFromJobTitle()` adds `ClaimTypes.Role` claims for `Administrator` and `Billing`. Values are split on commas, trimmed, and blank entries are skipped.

## Promote a JSON roles claim

```csharp
identity.AddRolesFromRoles();
```

`AddRolesFromRoles()` expects the first `roles` claim to contain a JSON string array:

```json
["Administrator", "Billing"]
```

Blank elements are skipped. Malformed JSON and an empty array add no roles.

Both methods:

- Modify the existing identity only when it is a `ClaimsIdentity`; `null` and other `IIdentity` implementations are unchanged.
- Read only the first source claim of the relevant type.
- Preserve the source claim and add standard role claims alongside it.
- Do not add an exact duplicate of an existing role claim, making repeated calls idempotent.

These methods turn claim text directly into authorization roles. Call them only after the identity and source claims have been authenticated and validated against a trusted issuer. They do not verify token signatures, issuers, audiences, or which role names your application permits.
