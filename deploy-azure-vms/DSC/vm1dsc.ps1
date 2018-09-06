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
  }
}