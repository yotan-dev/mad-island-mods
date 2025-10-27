# Crafting stations

This guide will show you how to create a new crafting station.

**Before you continue** make sure you know the basics of custom items, as this won't go through the basics.

- [Your first static item](./your-first-item.md)


Creating a crafting station requires the following steps:

1. Create the world object
2. Create the inventory item
3. Register the item to a YMCDataLoader


## Creating the world item

Create a prefab for how your crafting station shows in the world.

The following is the minimal structure for your world object (the details of each script will be explained below):

```
GameObject: mybench_obj
|	|- Component: Box Collider (not a trigger)
|	|- Script: Custom Item Info
|	|- Script: Storage Inventory
|
|-- GameObject: Sprite -- Display sprite

```

> [!NOTE]
> Official crafting stations are usually destroyable, we are not doing this here.
> If you want to make it destroyable, check [Destroyable objects](./destroyable-objects.md)


### Custom Item Info

These are the basic information about this item.

Do note that the `Item Key` defined here will be used when creating [recipes](./craft-recipes.md) for this station.


### Storage Inventory

This represents the inventory of the station, you need to put items here to use them on recipes.

- **Type**: Should be `Bench`
- **Size**: The maximum number of items that can be stored in this bench
- **Slots**: Leave it as 0


## Create the inventory item

Create a prefab to define general information of your item and how it shows on inventory.

The following is the minimal structure for your inventory item (the details of each script will be explained below):

```
GameObject: mybench
|	|- Script: Custom Item Data

```

### Custom Item Data

General item data.

It is very important to set:

- **Item Type**: `Prop`
- **Sub Type**: `Chest`

See [Guide: Your first static item](./your-first-item.md) for details.


## Register the item to a YMCDataLoader

Add your inventory item to your YMCDataLoader.
