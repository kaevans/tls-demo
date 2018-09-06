# TLS 1.0 and .NET Framework demo

This demo shows that a project created with .NET 4.5 that makes TLS calls will fail when TLS 1.0 client is disabled via the registry. The code will succeed when the project is upgraded to at least .NET 4.6 or additional registry settings are applied. If neither option is possible, the code can be modified to affect the ServicePointManager settings.

The recommendation is to update code to target .NET 4.7 or later versions.

For more information, see [Transport Layer Security (TLS) best practices with the .NET Framework](https://docs.microsoft.com/en-us/dotnet/framework/network-programming/tls).

## Table of Contents

1. [Test Matrix](#test-matrix)
2. [Disable TLS 1.0](#disable-tls-1.0)
3. [Enable SystemDefaultTlsVersions and SchUseStrongCrypto in registry](#enable-systemdefaulttlsversions-and-schusestrongcrypto-in-registry)
4. [Deploy Azure Resources](#deploy-azure-resources)
5. [Sample Application](#sample-application)
6. [Enabling ServicePointManager](#enabling-servicepointmanager)

## Test matrix

Key Vault is used as an example in the sample project because it uses a TLS call. Any TLS call would fail in the same manner, this demo is not dependent on Key Vault.

The project is initially created using .NET 4.5 and Azure Key Vault 1.0. The following matrix proves that the failures observed are not dependent on the version of the Key Vault library. Rather, they are dependent on behavior that changed between .NET 4.5 and .NET 4.6. The failures can be remediated by updating the .NET Framework version for the application to at least .NET 4.6 or applying additional registry settings to enable a .NET application targeting .NET 4.5 to run on a system using .NET 4.6 or higher when TLS 1.0 is disabled.

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

This matrix shows the behavior of enabling the `SystemDefaultTlsVersions` and `SchUseStrongCrypto` flags in the registry, enabling an application targeting .NET 4.5 to run on a machine configured with .NET Framework 4.6 or later.

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
## Deploy Azure Resources

The solution includes an Azure Resource Manager template that provisions an Azure Key Vault, creates three Azure virtual machines enabled with system-assigned managed service identities, and adds access policies to the Key Vault for each VM to allow create, wrap, and unwrap access for keys. Each VM is configured with a an increasing number of registry settings applied via PowerShell DSC:

| VM Name  | TLS 1.0 Enabled  | Additional flags set  |
| -------- | ---------------- | --------------------- |
| vm1      | true             | false                 |
| vm2      | false            | false                 |
| vm3      | false            | true                  |

In this configuration, VM1 is used to test a base Windows 2016 Datacenter marketplace image where TLS 1.0 is not explicitly disabled. VM2 is used to confirm behavior when TLS 1.0 is explicitly disabled. VM3 is used to confirm what happens when TLS 1.0 is explicitly disabled and the `SystemDefaultTlsVersions` and `SchUseStrongCrypto` flags are set in the registry.

This deployment will enable you to test various scenarios such as enabling the ServicePointManager via configuration or recompiling the application to target .NET 4.6.

To deploy the ARM template, open the project in Visual Studio 2017 and edit the `azuredeploy.parameters.json` file to match your environment. Instead of provising an administrator password, you can link this to an existing Azure Key Vault to avoid storing credentials in source code. **Right-click** the deployment project and choose **Deploy / New**.

> Note! The virtual network subnet has a network security group applied that restricts RDP traffic to a source prefix range `47.184.40.0/24`. Find your IP address [Find your IP address](https://www.bing.com/search?q=what+is+my+ip) and then update this setting with your IP range. You can update this before deploying, or you can change the NSG rule after deployment via CLI, PowerShell, or the Azure Portal.

## Sample Application

Included in this repository is a sample .NET 4.5 application that uses the Azure Key Vault SDK to make a TLS call. This application leverages the Managed Service Identity capability of an Azure virtual machine, enabling the application to obtain an access token without requiring installation of a certificate to each machine or requiring use of a client ID and client secret.

The ARM template provisions three virtual machines, each with a system-assigned managed service identity, and grants each VM access to a newly created Azure Key Vault.

After provisioning the ARM template, look in the output for the property named `keyVault`. **Copy** the URL and **update** the Key Vault URL in the app.config of the project.

To run the sample, simply build it then copy the output (located in `bin/debug`) to each virtual machine to observe the different behavior in each VM.

## Enabling ServicePointManager

There are situations where it is not feasible for registry settings to be applied to the host virtual machine. If the .NET Framework version cannot be upgraded and the registry settings cannot be set, use the `ServicePointManager` class to add TLS 1.2 support and remove TLS 1.0 and SSL 3.0 as a last resort.

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