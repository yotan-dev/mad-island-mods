# Introduction

Mod Core contains many features, some of them simply require you to reference YotanModCore.dll, while other will require
a Unity Project to be created following some specific instructions.

Before starting, it is important to understand what each file from Mod Core means/does:

- **YotanModCore.dll**: The main dll and the framework itself -- this is where the interface for mods is located at and the file you will need in your mods
- **YotanModCoreLoader.dll**: A BepInEx plugin to inject our framework into the game -- You should never refer/touch this file in your mods

> [!WARNING]
> NEVER distribute `YotanModCore.dll` or other files from Yotan Mod Core together with your mod.
> Tell your users to get the latest version from [https://github.com/yotan-dev/mad-island-mods/releases/latest](https://github.com/yotan-dev/mad-island-mods/releases/latest).
>
> Remember, other mods are also depending on it, so if you distribute it, collisions will happen and things will break.


## Versioning policy

As much as possible, YotanModCore follows semantic versioning for every API **not** marked as `[Experimental]`.

By API, I mean any public class/method/constant/variable.


What this means:

- Major version changes (v2.x.x -> v3.x.x) will potentially remove/make a change that won't always work to an API
- Minor version changes (v2.1.x -> v2.2.x) will not make changes that affect the usage of an API, as long as this API is **not experimental**
- Patch version changes (v2.1.1 -> v2.1.2) will not make changes that affect the usage of an API, even for **experimental** APIs

Experimental APIs are APIs that are not yet stable, and marked with the `[Experimental]` attributes, they are usually used when we are hoping for test/feedback
before making them stable.


Whenever possible, we will have:

1. At least 1 minor marking an API As obsolete before a Major that changes it
2. A proper migration guide when APIs are changing


## Simple mods

If you are simply making a code mod, and only need some utility stuff from Mod Core, like constants, game data, etc, you can simply reference YotanModCore.dll and start coding.

To do that:

1. Create a new BepInEx plugin project following their docs -- https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/index.html
2. Download the latest version of Yotan Mod Core
3. Copy YotanModCore.dll to your project, for example, in an `assemblies` folder
4. Add a reference to YotanModCore.dll by opening your mod's `.csproj`  file and adding the following:

```xml
<ItemGroup>
	<Reference Include="YotanModCore">
		<HintPath>./assemblies/YotanModCore.dll</HintPath>
	</Reference>
</ItemGroup>
```

5. Save it and you should be ready to reference `YotanModCore.` in your code.


## More complex mods

If you are looking into creating custom items and things like that, you will need to create a special Unity Project.

1. Please follow [Minimal Unity Project](./minimal-unity-project.md) guide.
2. Once you have a Unity Project, you can install Yotan Mod Core following [Adding Yotan Mod Core to Unity project](./unity-project-yotan-mod-core.md) guide.

Now you can start creating your mod!
