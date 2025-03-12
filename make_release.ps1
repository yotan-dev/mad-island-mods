Remove-Item -Path Release -Recurse
New-Item -ItemType Directory -Path Release

function Build-Plugin {
	param (
		$path,
		[string[]]$OutFile
	)
	Write-Output ""
	Write-Output "================= Building $path =============="
	Start-Process "dotnet" -Wait -NoNewWindow -WorkingDirectory $path build
	Write-Output ""
	foreach ($file in $OutFile) {
		Write-Output "================= Copying $file =============="
		Copy-Item "$path/bin/Debug/netstandard2.1/$file" -Destination "Release/$file"
		Write-Output ""
	}
	Write-Output "======================================================"
}

Build-Plugin "YotanModCore" -OutFile "YotanModCore.dll"
Build-Plugin "HFramework" -OutFile "HFramework.dll"

Build-Plugin "EnhancedIsland" -OutFile "EnhancedIsland.dll"
Build-Plugin "Gallery" -OutFile "Gallery.dll"
Build-Plugin "HExtensions" -OutFile "HExtensions.dll"
Build-Plugin "YoUnnoficialPatches" -OutFile "YoUnnoficialPatches.dll"
