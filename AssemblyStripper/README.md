# Assembly Stripper
Simple tool to strip and patch relevant Mad Island or Unity assemblies and put them into Assemblies folder.

Usage (For Mad Island): `dotnet run v0.x.x.x "C:/path/to/Mad Island/folder`

It will update the files in `Assemblies/` folder.

These files can be commited.

**NEVER** commit original files.


This makes use of BepInEx assembly publicizer: https://github.com/BepInEx/BepInEx.AssemblyPublicizer


## Additional notes

For Assembly-CSharp.dll, some classes are removed to avoid issues when loading it on a Unity project for modding. Those classes are:
- GlitchEffectsManipulationExample
- LimitlessGlitch*
- Limitless_*
- CustomTexture

In case someone ever needs to mod those classes, this DLL won't work, and they will have to strip it themselves.
