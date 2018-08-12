# TLS 1.0 and .NET Framework demo

This demo shows that a project created with .NET 4.5 that makes TLS calls will fail when TLS 1.0 client is disabled via the registry. The code will succeed when the project is upgraded to at least .NET 4.6 or registry settings are applied. If neither option is possible, the code can be modified to affect the ServicePointManager settings.

The recommendation is to update code to .NET 4.7 or later versions.

For more information, see [Transport Layer Security (TLS) best practices with the .NET Framework](https://docs.microsoft.com/en-us/dotnet/framework/network-programming/tls).

## Table of Contents

1. [Test Matrix](#test-matrix)
2. [Disable TLS 1.0](#disable-tls-1.0)
3. [Enable SystemDefaultTlsVersions and SchUseStrongCrypto in registry](#enable-systemdefaulttlsversions-and-schusestrongcrypto-in-registry)
4. [Use ServicePointManager](#use-servicepointmanager)
5. [Deploy Azure virtual machines](#deploy-azure-virtual-machines)

## Test matrix

Key Vault is used as an example because it uses a TLS call. Any TLS call would fail in the same manner, this demo is not dependent on Key Vault.

The project is initially created using .NET 4.5 and Azure Key Vault 1.0. The following matrix shows that the failure is not dependent on the version of the Key Vault library, it is dependent on the .NET Framework version being upgraded or the registry settings being applied.

The `SystemDefaultTlsVersions` and `SchUseStrongCrypto` flags can be set in the registry to enable .NET 4.5 applications to successfully make TLS calls without upgrading the application to target .NET Framework 4.6 or later.

| .NET version  | Key Vault version  | Registry flags set  | Result  |
| ------------- | ------------------ | ------------------- | ------- |
| 4.5           | 1.0                | false               | Fail    |
| 4.5.1         | 1.0                | false               | Fail    |
| 4.5.2         | 1.0                | false               | Fail    |
| 4.6           | 1.0                | false               | Succeed |
| 4.5.2         | 3.0                | false               | Fail    |
| 4.5           | 1.0                | true                | Succeed |
| 4.5.1         | 1.0                | true                | Succeed |
| 4.5.2         | 1.0                | true                | Succeed |

## Disable TLS 1.0

Use the following registry settings were used to disable TLS 1.0. Reboot the machine after applying changes.

```
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL]
"EventLogging"=dword:00000001

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Ciphers]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Ciphers\RC4 128/128]
"Enabled"="0"

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Ciphers\RC4 40/128]
"Enabled"="0"

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Ciphers\RC4 56/128]
"Enabled"="0"

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\CipherSuites]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Hashes]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\KeyExchangeAlgorithms]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\SSL 2.0]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\SSL 2.0\Client]
"DisabledByDefault"=dword:00000001

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\SSL 3.0]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\SSL 3.0\Client]
"Enabled"="0"

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\SSL 3.0\Server]
"Enabled"="0"

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.0]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.0\Client]
"Enabled"=dword:00000000
"DisabledByDefault"=dword:00000001

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.0\Server]
"Enabled"=dword:00000000
"DisabledByDefault"=dword:00000001

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.1]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.1\Client]
"DisabledByDefault"=dword:00000000

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.1\Server]
"DisabledByDefault"=dword:00000000

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2]

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client]
"DisabledByDefault"=dword:00000000

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Server]
"DisabledByDefault"=dword:00000000
```

## Enable SystemDefaultTlsVersions and SchUseStrongCrypto in registry

If the .NET framework version cannot be upgraded, use the following registry settings. Reboot the machine after applying these settings.

```
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v4.0.30319]
"SystemDefaultTlsVersions"=dword:00000001
"SchUseStrongCrypto"=dword:00000001

[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v4.0.30319]
"SystemDefaultTlsVersions"=dword:00000001
"SchUseStrongCrypto"=dword:00000001
```

## Use ServicePointManager

If the .NET Framework version cannot be upgraded and the registry settings cannot be set, use the `ServicePointManager` class to add TLS 1.2 support and remove TLS 1.0 and SSL 3.0 as a last resort.

````csharp
// add TLS 1.2
System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
// remove insecure TLS options
System.Net.ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
System.Net.ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
````

This sample enables demonstrating this by using a configuration setting. This setting is `false` by default, setting to `true` will set the `ServicePointManager` settings.

```xml
<add key="ServicePointManagerConfigEnabled" value="true"/>
```

## Deploy Azure virtual machines

The solution includes an Azure Resource Manager template that will provision the specified number of Azure virtual machines. An example environment:

| VM Name  | TLS 1.0 Enabled  | Additional flags set  |
| -------- | ---------------- | --------------------- |
| vm1      | true             | false                 |
| vm2      | false            | false                 |
| vm3      | false            | true                  |

In this configuration, vm1 is used to test a base Windows 2016 Datacenter marketplace image where TLS 1.0 is not explicitly disabled. vm2 is used to confirm behavior when TLS 1.0 is explicitly disabled. vm3 is used to confirm what happens when TLS 1.0 is explicitly disabled and the `SystemDefaultTlsVersions` and `SchUseStrongCrypto` flags are set in the registry.