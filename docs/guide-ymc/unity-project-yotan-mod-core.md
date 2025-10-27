# Adding Yotan Mod Core to Unity project

Yotan Mod Core is a modding framework for Mad Island, it enables you to do several things
on the game without needing to dig through the code.

> [!NOTE]
> This guide is only explaining how to set it up to your Unity project.
> Once you start using it, any mod you create MUST depend on YotanModCore or it won't work.


## Pre requirements

1. A minimal unity project as in [Minimal Unity project](./minimal-unity-project.md)
2. Yotan Mod Core from [Github](https://github.com/yotan-dev/mad-island-mods)


## Installing

1. Download Yotan Mod Core from [Github](https://github.com/yotan-dev/mad-island-mods)
2. Extract it somewhere
3. Drag and drop the `YotanModCore.dll` file into your unity project Plugins folder

It should recompile your scripts and work just fine


## Updating

> [!WARNING]
> DON'T drag and drop the `YotanModCore.dll` file, it will cause issues.

If a new version of Yotan Mod Core is released, you should update your plugin as follows:

1. Download the new version from [Github](https://github.com/yotan-dev/mad-island-mods)
2. On Windows File Explorer, open your Unity Project and go to `Assets/Plugins` folder
3. Copy your newly dowloaded `YotanModCore.dll` into it, and accept replacing the old one
4. Go back to Unity and it should recompile and work in the new version


## Next steps

Now that you have Yotan Mod Core installed, you can start creating mods!

Check the side bar for more things you can do with Yotan Mod Core.
