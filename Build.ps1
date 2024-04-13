# 2024 / github.com/JustArion
# CorsairSDK Build Script

$ErrorActionPreference = 'Stop'
$workingDirectory = Get-Location
function WriteInfo($str)
{
    Write-Output "[*] $str"
}
function NotifyExec($str)
{
    WriteInfo("Exec | $str")
    Invoke-Expression $str
}


function ResetWorkingDirectory
{
    cd $workingDirectory
}

try
{
    # Set the Working Directory to the Script directory
    NotifyExec("cd $PSScriptRoot")

    # Generate the Interop bindings
    NotifyExec('.\ClangSharp\Generate.ps1')

    # Set the Working Directory to the Script directory
    NotifyExec("cd $PSScriptRoot")

    # Build Project Directory
    NotifyExec('cd src\CorsairSDK\')
    
    NotifyExec('dotnet build -c Release -o bin\Build')
    
    WriteInfo("The project has been built at location .\src\CorsairSDK\bin\Build\")
    
    NotifyExec('".\src\CorsairSDK\bin\Build\" | Set-Clipboard')
    
    WriteInfo("The path has been copied to your clipboard!")
}
finally
{
    ResetWorkingDirectory
}