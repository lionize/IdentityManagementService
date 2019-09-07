param(
    [switch]$Latest,
    [string]$Version
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Publish -parameters @{"Latest"=$Latest;"Version"=$Version}
