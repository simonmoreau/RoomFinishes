param ($Configuration, $TargetName, $ProjectDir, $TargetPath, $TargetDir)
write-host $Configuration
write-host $TargetName
write-host $ProjectDir
write-host $TargetPath
write-host $TargetDir

function CopyToAddinFolder($revitVersion) {
	
    $addinFolder = ($env:APPDATA + "\Autodesk\REVIT\Addins\" + $revitVersion)

    if (Test-Path $addinFolder) {
        try {
            # Remove previous versions
            if (Test-Path ($addinFolder  + "\" + $TargetName + ".addin")) { Remove-Item ($addinFolder  + "\" + $TargetName + ".addin") }
            if (Test-Path ($addinFolder  + "\" + $TargetName)) { Remove-Item ($addinFolder  + "\" + $TargetName) -Recurse }
            
            # create the AlignTag folder
            New-Item -ItemType Directory -Path ($addinFolder  + "\" + $TargetName)

            # Copy the addin file
            xcopy /Y ($ProjectDir + $TargetName + ".addin") ($addinFolder)
            xcopy /Y ($TargetDir + "\*.dll*") ($addinFolder  + "\" + $TargetName)
            copy-item ($ProjectDir + "HelpFile\*") ($addinFolder  + "\" + $TargetName) -force -recurse
        }
        catch {
            Write-Host "Something went wrong"
        }
    }
}

# sign the dll
$cert=Get-ChildItem -Path Cert:\CurrentUser\My -CodeSigningCert
Set-AuthenticodeSignature -FilePath $TargetPath -Certificate $cert -IncludeChain All -TimestampServer "http://timestamp.comodoca.com/authenticode"

if ( $Configuration -eq "Debug") { 
    $revitVersion = "2022"
} else { 
    $revitVersion = $Configuration
}

CopyToAddinFolder $revitVersion

$addinFolder = ($env:APPDATA + "\Autodesk\REVIT\Addins\" + $revitVersion)

# Zip the package
# This path point to someplace on my laptop where is saved all release of the plugin
$ReleasePath="G:\My Drive\05 - Travail\Revit Dev\RoomFinishes\Release\Current"
$bunldeFolder = $ReleasePath + "\RoomFinishing.bundle"

# Remove previous release
if (Test-Path $bunldeFolder) {  Remove-Item $bunldeFolder -Recurse }

# Create bundle folder
New-Item -ItemType Directory -Path $bunldeFolder
xcopy /Y ($ProjectDir + "\PackageContents.xml") ($bunldeFolder)
New-Item -ItemType Directory -Path ($bunldeFolder + "\Contents")

$revitVersions = "2019","2020","2021","2022"

Foreach ($version in $revitVersions) {

    $versionFolder = $bunldeFolder + "\Contents\" + $version
    New-Item -ItemType Directory -Path $versionFolder
    $sourceFolder = (get-item $TargetDir).parent.FullName + "\" + $version

    if (Test-Path $sourceFolder) { 
        copy-item ($sourceFolder + "\*.dll") $versionFolder -force -recurse
        copy-item ($ProjectDir + "HelpFile\*") $versionFolder -force -recurse
        copy-item ($ProjectDir + $TargetName + ".addin") $versionFolder -force -recurse
    }
}

$ReleaseZip = ($ReleasePath + "\" + $TargetName + ".zip")
if (Test-Path $ReleaseZip) { Remove-Item $ReleaseZip }

if ( Test-Path -Path $ReleasePath ) {
  7z a -tzip $ReleaseZip $bunldeFolder
}


