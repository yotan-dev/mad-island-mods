# v1 to v2 migration guide

This docs explains how a mod auther can migrate from YotanModCore v1.x.x to v2.0.0.

## `GameVersion` type change

`GameInfo.GameVersion` has changed its type from `int` to `System.Version`, this
aims at supporting the new versioning schema added in Mad Island v0.4.3.1.

From now on, versions are referred as `<Major>.<Minor>.<Build>.<Patch>`, aligning
witch C#'s own naming:
https://learn.microsoft.com/en-us/dotnet/api/system.version?view=net-9.0


### Migration

The change on v2.0.0 simply replaces the return types to `System.Version`.

If you were using `GameInfo.ToVersion` already, simply recompiling targetting the
new assemblies should be enough. Otherwise, keep reading...

If you had a direct `int` comparison like that:

```C#
if (GameInfo.GameVersion < 4300 /* v0.4.3.0 */) {
	/* Do something */
}
```

You will get errors when depending on the new version.

Instead, you should switch to use one of Yotan Mod Core's helpers for version, like:

```C#
if (GameInfo.GameVersion < GameInfo.ToVersion("0.4.3")) {
	/* Do something */
}

// or

if (GameInfo.GameVersion < GameInfo.ToVersion("0.4.3.0")) {
	/* Do something */
}
```

## `ConstMenuItem` removed

`ConstMenuItem` was deprecated since v1.6.0 (2025.02.15) in favor of `MenuItem` and it is now removed.

### Migrating

You can replace `ConstMenuItem` usages with `MenuItem` like below.

If you only used `textId` to get the translated text, simply replace the class:

```C#
// from
new ConstMenuItem(PropPanelConst.Text.Insert, () => { /* ... */ });

// to
new MenuItem(PropPanelConst.Text.Insert, () => { /* ... */ });
```

If you used `textId` as some guidance on your code (E.g. to know which action was used),
you can use the new `MetaId` field for it:

```C#
// from
new ConstMenuItem(PropPanelConst.Text.Insert, () => { /* ... */ });

// to -- Note the 3rd parameter, but you can use whatever you want there
new MenuItem(PropPanelConst.Text.Insert, () => { /* ... */ }, PropPanelConst.Text.Insert);
```

Note that you are not required to give the same value to MetaId, you can create your own constant and
split presentation from actual, meaningful, information. Consider if it is the change to refactor :)
