# Data loading

You can load your custom data into yotan mod core in several ways, depending on your needs:

1. A simple custom asset bundle
2. A custom asset bundle with scripts
3. A BepInEx plugin that interacts with Yotan Mod Core

Below you will find how to use each of the above methods, also the guides on other sections should show you
some complete examples.


## Simple custom asset bundle

This method is ideal when existing components are enough and you only want to set them different.
You simply produce a new asset bundle from the unity project and put it into `BepInEx/CustomBundles/` folder (may be in a subfolder)

Examples:
- Making a new wall item with custom sprite and differen stats


### Steps

First create your asset bundle:

1. Select your `YMCDataLoader`
2. On the Inspect window, at the very bottom, click on the first dropdown of `Asset Bundle`
3. Click on `new`
4. Give it a name (I will call mine `my_first_item`)
5. Add the same asset bundle to all related objects of your mod (prefabs, textures, etc)
6. Right click the folder and select `Build AssetBundles`

Now go to your Unity Project `AssetBundles` folder through Windows file explorer and copy the asset bundle there,
this is the file you will need to put in your game/distribute to others.

Put the asset bundle generated in the previous steps into `BepInEx/CustomBundles/` folder, you can also create a folder with your mod name if you want.

You should be ready to go.


## Custom asset bundle with scripts

This method is ideal when you need custom Unity scripts, but don't need to use BepInEx to touch game's code.
You add your asset bundle and dll containing the scripts into `BepInEx/CustomBundles/` folder (may be in a subfolder)

Examples:
- Making a special wall that when touched will explode

### Steps

You can create a custom asset bundle with scripts by following the steps below.

> [!WARNING]
> When updating the DLL used in Unity, DO IT VIA EXPLORER, never drag and drop it into Unity,
> otherwise, references will break and you will have to remap every script.


First, let's create your script DLL and get it into Unity:

1. Create a new C# DLL project and add a reference to Mad Island's `Assembly-CSharp.dll`
2. On this DLL, build your scripts, extending Unity's `MonoBehaviour`
3. Compile the DLL
4. Copy the compiled DLL into your Unity Project `Assets/Plugins` folder
5. Switch to Unity window and wait for it to reload.


Now, we can build our custom item/whatever:

1. Create a new folder in the `Assets` folder of your mod
2. Create the needed objects/textures/etc
3. You can load your custom scripts as any other Unity script, you may use the `Add Component` button or drag the script from your Plugins folder


Finally, once completed, we can build our asset bundle:

1. Select your `YMCDataLoader`
2. On the Inspect window, at the very bottom, click on the first dropdown of `Asset Bundle`
3. Click on `new`
4. Give it a name (I will call mine `my_first_item`)
5. Add the same asset bundle to all related objects of your mod (prefabs, textures, etc)
6. Right click the folder and select `Build AssetBundles`

Now go to your Unity Project `AssetBundles` folder through Windows file explorer and copy the asset bundle there,
you should copy this AssetBundle file AND your compiled DLL into a folder. Those 2 files must be distributed together.

Put those files into `BepInEx/CustomBundles/` folder, you can also create a folder with your mod name if you want.

You should be ready to go.


## BepInEx plugin

> [NOTE]
> TODO

- Used when other cases are not enough, and you need to touch the game's code
- You put everything in your BepInEx plugin and use YMC's API to interact with it
