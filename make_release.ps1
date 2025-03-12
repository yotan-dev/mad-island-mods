Remove-Item -Path Release -Recurse
New-Item -ItemType Directory -Path Release

function Build-Plugin {
	param (
		$path,
		$OutFolder,
		[string[]]$OutFile
	)
	Write-Output ""
	Write-Output "================= Building $path =============="
	Start-Process "dotnet" -Wait -NoNewWindow -WorkingDirectory $path build
	Write-Output ""
	foreach ($file in $OutFile) {
		Write-Output "================= Copying $file =============="
		Copy-Item "$path/bin/Debug/netstandard2.1/$file" -Destination "Release/$OutFolder/$file"
		Write-Output ""
	}
	Write-Output "======================================================"
}

Build-Plugin "YotanModCore" -OutFolder / -OutFile "YotanModCore.dll"
Build-Plugin "HFramework" -OutFolder / -OutFile "HFramework.dll"

New-Item -ItemType Directory -Path Release/EnhancedIsland
Build-Plugin "EnhancedIsland" -OutFolder /EnhancedIsland/ -OutFile "EnhancedIsland.dll"
Copy-Item "EnhancedIsland/assets/*" -Destination "Release/EnhancedIsland/" -Recurse

Build-Plugin "Gallery" -OutFolder / -OutFile "Gallery.dll"
Build-Plugin "HExtensions" -OutFolder / -OutFile "HExtensions.dll"
Build-Plugin "YoUnnoficialPatches" -OutFolder / -OutFile "YoUnnoficialPatches.dll"
