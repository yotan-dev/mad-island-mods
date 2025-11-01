# Destroyable objects (CDefenceInfo)

> [!WARNING]
> This is an [experimental API (see versioning policy)](../introduction.md#versioning-policy) and may change in the future. Feedback is welcome.
>

This page will show you how to use the `CDefenceInfo` class, which allows you to make world objects destroyable by raids/etc, and may also cause damage on contact.

Items like walls, chests and crafting stations are examples of items that uses this.

**Before you continue** make sure you know the basics of custom items, as this won't go through the basics.

- [Your first static item](./your-first-item.md)

You also should have an actual item that may be put into the world already.

Making the object destroyable requires you to do the following steps:

1. Add a disabled broken sprite (`BrokenSpr`) as a children of the main object
2. Add a disabled triggerable box collider (`BrokenColl`) for the broken area (as a separate children object)
3. Add the `CDefenceInfo` script to the main object

Suppose you have the following item:

```
GameObject: myitem_obj
|	|- Component: Box Collider (not a trigger)
|	|- Script: Custom Item Info
|
|-- GameObject: NormalSpr -- Display sprite

```

It must now look like:

```
GameObject: myitem_obj
|	|- Component: Box Collider (not a trigger)
|	|- Script: Custom Item Info
|	|- Script: Destroyable Object -- Add this
|
|-- GameObject: NormalSpr -- Display sprite
|
|	** Add these **
|-- GameObject: BrokenSpr (Disabled) -- Sprite when broken
|-- GameObject: BrokenColl (Disabled) -- Collider when broken
	|	|- Component: Box Collider (trigger)

```

## CDefenceInfo

This configures how the object breaks down.

Original settings:
- **Item Info**: The main item object (`myitem_obj`)
- **Active**: Unknown. Set it to `true`
- **Defence Type**: Unknown. Set it to `Defence`
- **Attack**: Damaged caused to NPCs who attacks/touches it (0 = no damage)
- **Active Objects**: Add the children objects that should be active when the object is **functional**
  - In this example, this would be `NormalSpr`
- **Broken Objects**: Add the children objects that should be active when the object is **broken**
  - In this example, this would be `BrokenSpr` and `BrokenColl`
- **Destroy FX**: Unknown. Set it to `4`
- **Destryo SE**: Unknown. Set it to `39`
- **Coll Offset**: Unknown. Set it to `(0, 0, 0)`

Custom settings:
- **DefenceDamagedSE**: SE played when using custom code for your defence logic. (See below)
	- Values can be found in [AudioSE](/api/YotanModCore.Consts.AudioSE.html)
	- Default: `AudioSE.HitMeat01`
- **IgnoresContact**: Whether this defence performs "contact" logic.
	- Originally, this is always false. But this gives you the power to make items that NPCs ignore, but can still be damaged by code (and repaired)
	- Default: `false`

## Customizing

You can implement some custom logic for your defence info by using some events and custom methods provided by YotanModCore.

### Events

- `DefenceActiveStateChanged`: Called when the defence becomes active or inactive (e.g. because it was destroyed)
- `DefenceRepaired`: Called when the defence is repaired


### Methods

@TODO: Add more information about the overridable methods, and what it means to interrupt those steps.

The following methods handle specific situations in the lifecycle of the object, and may also be overriden
to implement custom logic.

- `ChangeLife`: Changes the life of the structure, handling its sounds and destroy state change
- `IsAlive`: Returns whether the structure is functional or not
- `IsDestroyed`: Returns whether the structure is broken or not
- `OnDefenceAttacked`: Called when something attacks this structure
	- Note: this method allows you to create custom logic if desired.
	- You should return `true` if you did so and want the code to stop there.
	- If you return `false`, original damage/counter attack will be triggered.
- `DefenceCounterAttack`: Called when calculating how the attacked structure should "Counter Attack"
	- Note: this method allows you to create custom logic if desired.
	- You should return `true` if you did so and want the code to stop there.
	- If you return `false`, original counter attack will be triggered.
- `OnDefenceTouched`: Called when something touches this structure
	- Note: this method allows you to create custom logic if desired.
	- You should return `true` if you did so and want the code to stop there.
	- If you return `false`, original touch logic will be triggered.


