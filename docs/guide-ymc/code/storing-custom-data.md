# Storing custom data

> [!WARNING]
> This is an [experimental API (see versioning policy)](../introduction.md#versioning-policy) and may change in the future. Feedback is welcome.
>

Depending on your mod needs, you may want to store custom information into game entities, like NPCs, objects, etc.
And you may also want to store them into the save files so you can continue using them later.

Yotan Mod Core brings what we call DataStores and SaveData to help you with this.

In a nutshell, DataStores are used to store data in memory, during gameplay and SaveData is the representation in the save file.
Read the sections below to learn how each of them works and how to implement it.

Currently, we only support those for NPCs and Game general data, but let me know if you need for other places and we can extend it.

You can see a fullly implemented example on the main repository, "Samples" project.


# Overall process

Overall, you have to do the following steps to have your custom data in the game:

1. Create a DataStore class for you custom data (may have multiple fields). This should extend from the right `DataStore` interface.
2. **(Optional)** If you want to store the data in the save file, create a SaveData class for it and make it implement `ISaveData`.
3. During plugin initialization, register the DataStore and SaveData (if any) using `DataStoreManager.RegisterDataStore`.


# Data Stores

A Data Store is the main point for controlling your custom data. Your code will access instances of it when it wants to read/write data,
and it will also be the one responsible for loading and saving relevant info.

Your Data Store class should implement the interface based on what kind of data are you storing:

- `IGameDataStore` for general game data, which has a single intance for the game session (e.g.: Time of day, etc.)
- `ICommonSDataStore` for data that is specific to a CommonStates instance (e.g.: NPC life, etc.)

You are free to add custom methods to your class, and you will be able to call them later on whenever you need,
since the methods to interact with custom data will return you the DataStore instance.


## Example

In this example, we will create a DataStore to store how much damage a NPC has taken during their life time,
and we will have a custom method that tells whether his is "battle hardened" because he already took too much damage.

For now, we are not taking care of saving it (we will do it later).

```C#
using YotanModCore.DataStore;

public class DamageTakenStore : ICommonSDataStore
{
	public float DamageTaken { get; set; } = 0f;

	public void Initialize(CommonStates commonStates) {
		/* We can do some initialization here */
	}

	public void OnLoad(ISaveData data) {
		/* This is triggered when we load a save file, for each DataStore. */
	}

	public ISaveData OnSave() {
		/* This is triggered when we save the game, for each DataStore. */
		return null;
	}

	public bool IsBattleHardened() {
		return DamageTaken > 100;
	}
}
```

Now, let's register it:

```C#
DataStoreManager.RegisterDataStore(typeof(DamageTakenStore), () => new DamageTakenStore());
```

This tells Yotan Mod Core to track this DataStore, the first parameter is the type of the DataStore, the second parameter is a factory method that returns a new instance of the DataStore.

Now, we are ready to use it in our general code.

We can access the store by calling `GetData<DamageTakenStore>()` on any CommonStates instance, for example:

```C#
using YotanModCore.DataStore; // GetData is a ExtensionMethod from YotanModCore.DataStore

//...
var store = commonStates.GetData<DamageTakenStore>();
store.DamageTaken += 10f;

if (store.IsBattleHardened()) {
	// Do something
}
```

`GetData` will fetch this CommonState's `DamageTakenStore`, creating a new store if needed or returning the existing one. Once you have its reference, you can use as desired.


# Save Data

Save Data is the representation of the custom data into a save file, it is a separate class that extends `ISaveData`.

The recommended pattern is to create your SaveData class as a nested class of your DataStore class, to make it easier to keep them synced.

When dealing with SaveData, you will need to implement your DataStore `OnLoad` and `OnSave` in order to move information between Save and Game states. You can use this moment to perform transformations if needed. For example, if there is a better format for the save file or for the game state, you can do the conversion here.

## Example

Extending on the example from DataStore, we will now save the damage taken value into the save file.

```C#
using YotanModCore.DataStore;

public class DamageTakenStore : ICommonSDataStore
{
	public class Data : ISaveData
	{
		public float DamageTaken;

		public Type GetStoreType()
		{
			return typeof(DamageTakenStore); // This MUST be the type of the DataStore class.
		}
	}

	public float DamageTaken { get; set; } = 0f;

	public void Initialize(CommonStates commonStates) {
		/* We can do some initialization here */
	}

	public void OnLoad(ISaveData data) {
		// Copy values from Data to DataStore
		this.DamageTaken = ((Data)data).DamageTaken;
	}

	public ISaveData OnSave() {
		// Copy values from DataStore to Data
		return new Data { DamageTaken = DamageTaken };
	}

	public bool IsBattleHardened() {
		return DamageTaken > 100;
	}
}
```

And update the registration:

```C#
DataStoreManager.RegisterDataStore(typeof(DamageTakenStore), () => new DamageTakenStore(), /* new */typeof(DamageTakenStore.Data));
```

The 3rd parameter of `RegisterDataStore` indicates the Type of the SaveData class.


# Available DataStores

Yotan Mod Core provides a few DataStores out of the box:

- `GameDataStore` for general game data, which has a single intance for the game session (e.g.: Time of day, etc.) -- This data is linked to the current `GameManager` instance (you can access it through `Managers.gameMN`)
- `CommonSDataStore` for data that is specific to a CommonStates instance (e.g.: NPC life, etc.) -- This data is linked to `CommonStates` instances


# Auto-cleanup

If you remove a mod that previously added a DataStore, or a mod you have been using updated and no longer registers a given DataStore, it will be automatically cleaned up on the game load.

You are likely to see errors like that on your BepInEX console when this happens:

```
[Error  :YotanModCore] Invalid custom data type: <YotanModCore.KillCountStore.Data>. Ignoring it. (Probably missing/removed/broken mod)
```

This is normal if you removed a mod or linked to an update. It might be an issue if:

1. The new version of the mod is actually broken
2. Something else went really wrong and you were not supposed to lose this info.

But overall, YotanModCore will ignore it and if you save it, these data will be lost forever. So keep it in mind.
