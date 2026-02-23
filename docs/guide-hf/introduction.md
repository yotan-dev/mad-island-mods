# Introduction

> [!WARNING]
> The entire Guide section for HFramework is focused on HFramework v2, currently a Work in Progress, and in a very early experimental API.
>
> It will very likely change with no prior notice and you may lose your work.
>
> This documentation is meant to guide modders in early adoption, testing and feedback gathering so we can refine into the final version.

HFramework is a framework to enable modders to create new sex interactions as well as customize existing Mad Island sexual content.

It provides Unity tooling for you to build your modded content and to easily integrate it into the game. This is possible thanks to HFramework
reimplementing the original sex system in a more customizable way.

You still need to have some knowledge about how the game sex scenes work, and we will be covering that in the guide, but once you have
the general idea, you can build your first few sex scenes by simply having a Unity project and a Spine animation.

HFramework contains 2 DLLs:

- **HFramework.dll**: The BepInEx injection plugin -- this is only meant by HFramework to get loaded into the game
- **HFrameworkLib.dll**: The main dll that provides the general API for mods and Unity tooling - You will be using that.

> [!WARNING]
> NEVER distribute `HFramework.dll`, `HFrameworkLib.dll` or other files from HFramework together with your mod.
> Tell your users to get the latest version from [https://github.com/yotan-dev/mad-island-mods/releases/latest](https://github.com/yotan-dev/mad-island-mods/releases/latest).
>
> Remember, other mods are also depending on it, so if you distribute it, collisions will happen and things will break.


## Versioning policy

As much as possible, HFramework follows semantic versioning for every API **not** marked as `[Experimental]`.

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


## Setting up Unity

If you are looking into creating custom items and things like that, you will need to create a special Unity Project.

1. Please follow [Minimal Unity Project](./minimal-unity-project.md) guide.
2. Once you have a Unity Project, you can install Yotan Mod Core following [Adding Yotan Mod Core to Unity project](./unity-project-yotan-mod-core.md) guide.

Now you can start creating your mod!
