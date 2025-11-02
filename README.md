> [!CAUTION]
> This repository contains mods for the **ADULT GAME** Mad Island.
>
> Thus, **BEWARE** that when browsing this repository content you are likely to find text, code and _maybe_ images that are not suitable for a general audience.
>
> As much as possible, I tried to keep images either zipped or not in the repository at all, but be cautios.
>

# Mad Island Mods

Mods for Mad Island game.


## Before you begin

> [!WARNING]
> Read this before you continue. Don't report a bug to the developers if you can't reproduce it without mods.
>

Before you start, you need to know and understand that:

1. Every mod here can potentially break your game. They are changing the game code in ways the devs did not expect/know;
2. If you find a bug, check BepInEx console to check if a mod is not causing issues.
3. **Before** you report a bug to the game developer, try reproducing the bug without mods.

Let's not make the life of the developers harder. And responsibly enjoy a modded game :)


## How to install/use

See [End user guide](https://yotan-dev.github.io/mad-island-mods/) for info and instructions on how to install/use mods.


## Utilities

### AssemblyStripper

Utilitary tool to strip relevant Mad Island dlls into Assemblies folder. Stripped DLLs does not contain original/copyrighted code.


## Support

If you find a bug, have questions or feature requests, feel free to open an issue. Feature requests may not be implemented by me.


## Contributing

Contributions are welcome. For small changes/bug fixes, just fork it and open a PR and I will review.

For bigger changes/new features, please open an issue for discussion first.


## Development Setup

1. Clone this repo

- You can build any mod by opening a shell in its folder and running `dotnet build`
- You can build all mods by opening a PowerShell and running:
```PowerShell
Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process
.\make_release.ps1
```
DLLs will be put in Release folder


**NEVER** commit original game files.


## License

- Some code is copied/very similar to original decompiled source, those are kinda of propietary to Mad Island developers;
- Most of the code is made by me and is free to be reused as long as kept free and open source (be it as git repo or source distributed together with your mod).


## Project status

I think most of the mods have reached their "end state", while I do think I will come back from time to time, please note that I may not always be active.

Feel free to contribute, reviewing PRs are easier than making the changes, so it is much appreciated :)
