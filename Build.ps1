$BuildConfiguration = "Release"
$DebugPreference = "SilentlyContinue"
$InformationPreference = "Continue"
$VerbosePreference = "SilentlyContinue"
$RunSledge = $false
$NBR = $false
$Log = $false

foreach($Argument in $args) {
    if($Argument -eq "-Help") {
        Write-Host "
Usage: Build.ps1 [options]
Options:
    -Help: Show this help message
    -(Debug/Release): Build configuration (default: Release)
    -Run: Run Sledge after build (default: false)
    -PSDebug: Outputs debug information, useful for debugging build scripts (default: false)
    -PSVerbose: Outputs everything, use this if you have issues (default: false)
    -NBR: Don't build, just run sledge (default: false)
    -Log: Logs build output to a file (default: false)
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

    if ($Argument -eq "-PSDebug") {
        $DebugPreference = "Continue"
    }

    if ($Argument -eq "-PSVerbose") {
        $VerbosePreference = "Continue"
    }

    if ($Argument -eq "-NBR") {
        $NBR = $true
    }

    if ($Argument -eq "-Log") {
        $Log = $true
    }
}

if ($Log -eq $true) {
    if (Test-Path "$((Get-Item .).FullName)\Build.log" -PathType Leaf) {
        Remove-Item "$((Get-Item .).FullName)\Build.log" -ErrorAction SilentlyContinue
    }

    Start-Transcript -Path "$((Get-Item .).FullName)\Build.log" -UseMinimalHeader -Append
}

Write-Verbose("Building Sledge with configuration: $BuildConfiguration")
Write-Verbose("Run Sledge: $RunSledge")

$SledgeDir = $Env:SLEDGE_ROOT_DIR
Write-Verbose("Getting Sledge Directory")
if ( $SledgeDir.Length -eq 0 ) {
    Write-Host("[Error] SLEDGE_ROOT_DIR is Invalid")
    return
}

Write-Verbose("Checking for Sledge.exe and mods folder")
if (-not(Test-Path -Path "$SledgeDir\\sledge.exe" -PathType Leaf) -or -not(Test-Path -Path "$SledgeDir\\mods" -PathType Container)) {
    Write-Host("[Error] Unable to locate { sledge.exe || mods }")
    return
}

Write-Verbose("Checking for bin\sledgelib.dll")
if (-not(Test-Path -Path "$SledgeDir\bin\sledgelib.dll" -PathType Leaf)) {
    Write-Host("[Error] Unable to locate `"$SledgeDir/bin/sledgelib.dll`", sledge must have failed copying it")
    return
}

$SledgeDependencyDir = "$SledgeDir\mods\TeardownM\dependencies"
$TeardownMDir = "$((Get-Item .).FullName)\TeardownM"
$TeardownMDeps = "$TeardownMDir\dep\"

Write-Debug "SledgeDir: $SledgeDir"
Write-Debug "TeardownM Dir: $TeardownMDir"
Write-Debug "TeardownM Dep Dir: $SledgeDependencyDir"

# Check if mod dependency directory exists
Write-Verbose("Checking for TeardownM dependencies")
if (-not(Test-Path -Path $TeardownMDeps -PathType Container)) {
    Write-Host("[Error] Unable to locate $TeardownMDeps")
}

Write-Verbose("Checking for sledge/mods/TeardownM/dependencies")
if (-not(Test-Path -Path $SledgeDependencyDir -PathType Container)) {
    New-Item -Path $SledgeDependencyDir -ItemType Directory
    Write-Host("Creating TeardownM Directory in sledge/mods")
}

# Check if the file has been deleted
Write-Debug("Checking for deleted dpendencies")
$Dependencies = Get-ChildItem -Path $SledgeDependencyDir -Filter *.dll
foreach ($File in $Dependencies) {
    if (-not(Test-Path -Path "$TeardownMDeps\\$($File.Name)" -PathType Leaf)) {
        Write-Host("Deleteing $($File.Name) (No longer exists)")
        Remove-Item -Path "$SledgeDependencyDir\\$($File.Name)" -Force
    }
}

# Copy Dependencies
Write-Debug("Copying Dependencies")
$CopiedDependencies = 0
$RequiredDependencies = Get-ChildItem -Path $TeardownMDeps -Filter *.dll
foreach($File in $RequiredDependencies) {
    Write-Verbose("Copy Dependency | Found File: $File")

    # Copy the file if it doesn't exist
    if (-not(Test-Path -Path "$SledgeDependencyDir\\$($File.Name)" -PathType Leaf)) {
        Copy-Item -Path $TeardownMDeps\\$($File.Name) -Destination $SledgeDependencyDir\\$($File.Name) -Force
        Write-Host("Copying $($File.Name)")
        $CopiedDependencies += 1
    }

    # Compare Hashes and copy if they don't match
    $RequiredFileHash = Get-FileHash -Path $File.FullName -Algorithm SHA256
    $SledgeFileHash = Get-FileHash -Path "$SledgeDependencyDir/$($File.Name)" -Algorithm SHA256
    if ($RequiredFileHash.Hash -ne $SledgeFileHash.Hash) {
        Copy-Item -Path $TeardownMDeps\\$($File.Name) -Destination $SledgeDependencyDir\\$($File.Name) -Force
        Write-Host("Copying $($File.Name) (New Hash)")
        $CopiedDependencies += 1
    }
}

if ($CopiedDependencies -ne 0) {
    Write-Host("Copied $CopiedDependencies Dependencies")
} else {
    Write-Debug("No Dependencies Needed to be Copied")
}

function GetGamePath([string]$GameName) {
    if ($GameName.Length -eq 0) {
        return $false
    }

    # Open registry key
    $SteamPath = (Get-ItemProperty -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam -Name InstallPath).InstallPath
    Write-Debug("Steam Path: $SteamPath")

    $GamePath = "$SteamPath\steamapps\common\$GameName"
    # Check if teardown.exe exists
    if (Test-Path -Path "$GamePath\$GameName.exe" -PathType Leaf) {
        return $GamePath
    } else {
        $ConfigFile = "$SteamPath\steamapps\libraryfolders.vdf"
        if (Test-Path -Path $ConfigFile -PathType Leaf) {
            Select-String -Path $ConfigFile -Pattern '[A-Z]:(.*)(?=")' -CaseSensitive | ForEach-Object {
                $Match = $_.Matches[0].Value
                if (Test-Path -Path "$Match\steamapps\common\$GameName\$GameName.exe" -PathType Leaf) {
                    $GamePath = "$Match\steamapps\common\$GameName"
                    return $GamePath
                }
            }
        } else {
            Write-Host("Unable to locate $SteamPath\steamapps\libraryfolders.vdf Path")
            return $false
        }
    }
}

Write-Verbose("Getting Game Path")
$TeardownPath = GetGamePath("Teardown")
if ($TeardownPath -eq $false) {
    Write-Host("[Error] Unable to locate game")
    return
}

Write-Debug("Teardown Path: $TeardownPath")

$TeardownMMods = @()
$Directories = Get-ChildItem -Path $TeardownMDir\lua |Where-Object {$_.PSIsContainer}
foreach ($Dir in $Directories) {
    $TeardownMMods += $Dir;
}

# Delete files/folders if they no longer exist in TeardownM/lua
Write-Verbose("Checking for deleted files/folders")
$Files = Get-ChildItem -Path $TeardownPath\mods -Recurse -Force -Include "*"
foreach($File in $Files) {
    $DirName = $File.Name
    if ($DirName.Length -eq 0 -or -not $File.DirectoryName) { continue }
    $ModName = $File.DirectoryName.Substring($TeardownMDir.Length + 12)

    $FoundMod = $false
    foreach ($Mod in $TeardownMMods) {
        if ($Mod.Name -eq $ModName) {
            $FoundMod = $true
            break
        }
    }

    if ($FoundMod -eq $false) { continue }

    $Directories = Get-ChildItem -Path $TeardownPath\mods\$ModName -Recurse |Where-Object {$_.PSIsContainer}
    foreach ($Dir in $Directories) {
        # Remove everything before ModName
        $RelativeDir = $Dir.FullName.Substring($TeardownPath.Length + 6)
        if (-not(Test-Path -Path "$TeardownMDir\lua\$RelativeDir" -PathType Container)) {
            Write-Host("Deleting $RelativeDir (No longer exists)")
            Remove-Item -Path $Dir -Recurse -Force
        }
    }

    $Filepath = "$ModName\$($File.Name)"
    Write-Verbose("Checking $Filepath")
    if (-not(Test-Path -Path "$TeardownMDir\lua\$Filepath" -PathType Leaf)) {
        Write-Host("Deleting File $Filepath (No longer exists)")
        Remove-Item -Path $File -Force
    }
}

Write-Debug("Copying Lua Mods")
$Files = Get-ChildItem -Path $TeardownMDir\lua -Recurse -Force
$CopiedFiles = 0
foreach($File in $Files) {
    $DirName = $File.Directory

    # Remove everything before lua
    if ($DirName.Length -eq 0) { continue }

    $CleanName = $DirName.ToString().Substring($TeardownMDir.Length + 5)
    Write-Verbose("Copy Lua | $CleanName\$($File.Name)")

    # Create the directory if it doesn't exist
    if (-not(Test-Path -Path "$TeardownPath\mods\$CleanName" -PathType Container)) {
        Write-Debug("Creating Directory: $TeardownPath\mods\$CleanName")
        New-Item -Path "$TeardownPath\mods\$CleanName" -ItemType Directory
        Write-Host("Creating $CleanName Directory in lua")
    }

    # Copy the file if it doesn't exist
    if (-not(Test-Path -Path "$TeardownPath\mods\$CleanName\$($File.Name)" -PathType Leaf)) {
        Copy-Item -Path $File -Destination "$TeardownPath\mods\$CleanName" -Force
        Write-Host("Copying $($File.Name)")
        $CopiedFiles += 1
    }

    # Compare Hashes
    $RequiredFileHash = Get-FileHash -Path $File.FullName -Algorithm SHA256
    $SledgeFileHash = Get-FileHash -Path "$TeardownPath\mods\$CleanName\$($File.Name)" -Algorithm SHA256
    if ($RequiredFileHash.Hash -ne $SledgeFileHash.Hash) {
        Copy-Item -Path $File -Destination "$TeardownPath\mods\$CleanName" -Force
        Write-Host("Copying $($File.Name) (New Hash)")
        $CopiedFiles += 1
    }
}

if ($CopiedFiles -ne 0) {
    Write-Host("Copied $CopiedFiles Files")
} else {
    Write-Debug("No Files Needed to be Copied")
}

if ($NBR -eq $true) {
    Start-Process -FilePath "$SledgeDir\sledge.exe" -WorkingDirectory $SledgeDir
    if ($Log -eq $true) {
        Stop-Transcript
    }

    exit
}

$Output = ""

#Build Mod
Write-Host("Building Mod")
Invoke-Command -OutBuffer $Output  -ScriptBlock {
    dotnet build "$((Get-Item .).FullName)\TeardownM" /p:Configuration=Release /p:Platform="x64" | Out-Host
}

Copy-Item -Path "$((Get-Item .).FullName)\TeardownM\bin\x64\$BuildConfiguration\net6.0\TeardownM.dll" -Destination "$SledgeDir\mods\TeardownM.dll" -Force

if ($RunSledge -eq $true) {
    Write-Host("Starting Sledge")
    Start-Process -FilePath "$SledgeDir\sledge.exe" -WorkingDirectory $SledgeDir
}

if ($Log -eq $true) {
    Stop-Transcript
}