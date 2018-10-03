Configuration Main
{

Param ( [string] $nodeName )

Import-DscResource -ModuleName PSDesiredStateConfiguration

Node $nodeName
  {   
    WindowsFeature NetFramework46
    {
      Name = "Net-Framework-45-Core"
      Ensure = "Present"
    }
	Registry DisableTLS10Client 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.0\Client"
		ValueName = "Enabled"
		ValueData = "0"
		ValueType = "Dword"
	    Force = $true
	}
	Registry DisableTLS10ClientDefault 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.0\Client"
		ValueName = "DisabledByDefault"
		ValueData = "1"
		ValueType = "Dword" 
		Force = $true
	}
	Registry DisableTLS10Server 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.0\Server"
		ValueName = "Enabled"
		ValueData = "0"
		ValueType = "Dword" 
		Force = $true
	}
	Registry DisableTLS10ServerDefault 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.0\Server"
		ValueName = "DisabledByDefault"
		ValueData = "1"
		ValueType = "Dword" 
		Force = $true
	}
	Registry DisableTLS11Client 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.1\Client"
		ValueName = "Enabled"
		ValueData = "0"
		ValueType = "Dword" 
		Force = $true
	}	
	Registry DisableTLS11ClientDefault 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.1\Client"
		ValueName = "DisabledByDefault"
		ValueData = "1"
		ValueType = "Dword" 
		Force = $true
	}
	Registry DisableTLS11Server 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.1\Server"
		ValueName = "Enabled"
		ValueData = "0"
		ValueType = "Dword" 
		Force = $true
	}	  
	Registry DisableTLS11ServerDefault 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.1\Server"
		ValueName = "DisabledByDefault"
		ValueData = "1"
		ValueType = "Dword" 
		Force = $true
	}
	Registry EnableTLS12Client 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client"
		ValueName = "Enabled"
		ValueData = "1"
		ValueType = "Dword"
		Force = $true
	}
	Registry EnableTLS12ClientDefault 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client"
		ValueName = "DisabledByDefault"
		ValueData = "0"
		ValueType = "Dword"
		Force = $true
	}
	Registry EnableTLS12Server 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Server"
		ValueName = "Enabled"
		ValueData = "1"
		ValueType = "Dword" 
		Force = $true
	}
	Registry EnableTLS12ServerDefault 
	{
		Ensure = "Present"
		Key = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Server"
		ValueName = "DisabledByDefault"
		ValueData = "0"
		ValueType = "Dword"
		Force = $true
	}
	Registry EnableSystemDefaultTLS 
	{
		Ensure = "Present"
		Key = "HKLM:\SOFTWARE\Microsoft\.NETFramework\v4.0.30319"
		ValueName = "SystemDefaultTlsVersions"
		ValueData = "1"
		ValueType = "Dword"
		Force = $true
	}
	Registry EnableStrongCrypto 
	{
		Ensure = "Present"
		Key = "HKLM:\SOFTWARE\Microsoft\.NETFramework\v4.0.30319"
		ValueName = "SchUseStrongCrypto"
		ValueData = "1"
		ValueType = "Dword"
		Force = $true
	}
	Registry EnableSystemDefaultTLSWow64 
	{
		Ensure = "Present"
		Key = "HKLM:\SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v4.0.30319"
		ValueName = "SystemDefaultTlsVersions"
		ValueData = "1"
		ValueType = "Dword"
		Force = $true
	}
	Registry EnableStrongCryptoWow64 
	{
		Ensure = "Present"
		Key = "HKLM:\SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v4.0.30319"
		ValueName = "SchUseStrongCrypto"
		ValueData = "1"
		ValueType = "Dword"
		Force = $true
	}
  }
}