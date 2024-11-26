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

Build-Plugin "CraftColors" -OutFile "CraftColors.dll"
Build-Plugin "DisassembleItems" -OutFile "DisassembleItems.dll"
Build-Plugin "EnhanceWorkplaces" -OutFile "EnhanceWorkplaces.dll"
Build-Plugin "Gallery" -OutFile "Gallery.dll"
Build-Plugin "IncreaseZoom" -OutFile "IncreaseZoom.dll"
Build-Plugin "ItemSlotColor" -OutFile "ItemSlotColor.dll"
Build-Plugin "NpcStats" -OutFile "NpcStats.dll"
Build-Plugin "StackNearby" -OutFile "StackNearby.dll"
Build-Plugin "WarpBody" -OutFile "WarpBody.dll"
Build-Plugin "YoUnnoficialPatches" -OutFile "YoUnnoficialPatches.dll"
