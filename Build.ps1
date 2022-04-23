$BuildConfiguration = "Release"
$RunSledge = $false

foreach($Argument in $args) {
    if($Argument -eq "-Help") {
        Write-Host "
Usage: Build.ps1 [options]
Options:
    -Help: Show this help message
    -(Debug/Release): Build configuration (default: Release)
    -Run: Run Sledge after build (default: false)
"

        exit
    }

    if ($Argument -eq "-Release") {
        $BuildConfiguration = "Release"
    } elseif ($Argument -eq "-Debug") {
        $BuildConfiguration = "Debug"
    }

    if ($Argument -eq "-Run") {
        $RunSledge = $true
    }
}

$SledgeDir = $Env:SLEDGE_ROOT_DIR
if ( $SledgeDir.Length -eq 0 ) {
    Write-Output("[Error] SLEDGE_ROOT_DIR is Invalid")
    return
}

if (-not(Test-Path -Path "$SledgeDir\\sledge.exe" -PathType Leaf) -or -not(Test-Path -Path "$SledgeDir\\mods" -PathType Container)) {
    Write-Output("[Error] Unable to locate { sledge.exe || mods }")
    return
}

Write-Output("Copying Dependencies")

$SledgeDependencyDir = "$SledgeDir\mods\TeardownM\dependencies"
$MyDependencyDir = "$((Get-Item .).FullName)\TeardownM\dep\"

# Check if mod dependency directory exists
if (-not(Test-Path -Path $MyDependencyDir -PathType Container)) {
    Write-Output("[Error] Unable to locate $MyDependencyDir")
}

# Check if sledge\\mods\\TeardownM\\dependencies exists
if (-not(Test-Path -Path $SledgeDependencyDir -PathType Container)) {
    New-Item -Path $SledgeDependencyDir -ItemType Directory
    Write-Output("Creating TeardownM Directory in sledge/mods")
}

# Copy Dependencies
$RequiredDependencies = Get-ChildItem $MyDependencyDir
foreach($File in $RequiredDependencies) {
    # Compare file size, if different copy
    if (-not(Test-Path -Path "$SledgeDependencyDir\\$($File.Name)" -PathType Leaf)) {
        Copy-Item -Path $MyDependencyDir\\$($File.Name) -Destination $SledgeDependencyDir\\$($File.Name) -Force
        Write-Output("Copying $($File.Name)")
    }
}

# Build Mod
Write-Output("Building Mod")
Invoke-Command -ErrorAction Stop -ScriptBlock {
    dotnet build "$((Get-Item .).FullName)\TeardownM" /p:Configuration=Release /p:Platform="x64"
}

Copy-Item -Path "$((Get-Item .).FullName)\TeardownM\bin\x64\$BuildConfiguration\net6.0\TeardownM.dll" -Destination "$SledgeDir\mods\TeardownM.dll" -Force

if ($RunSledge -eq $true) {
    Write-Output("Starting Sledge")
    Start-Process -FilePath "$SledgeDir\sledge.exe" -WorkingDirectory $SledgeDir
}