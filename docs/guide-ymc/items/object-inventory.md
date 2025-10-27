# Object inventory (StorageInventory)

This page will show you how to use the `StorageInventory` class, which allows you to make world have inventory to hold items.

Items like crafting stations and chests are examples of items that uses this.

In the original game, this class is called `InventorySlot`.

**Before you continue** make sure you know the basics of custom items, as this won't go through the basics.

- [Your first static item](./your-first-item.md)

You also should have an actual item that may be put into the world already.

Adding an inventory to your item simply requires you to add the `StorageInventory` script to the main object

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
|	|- Script: Storage Inventory -- Add this
|
|-- GameObject: NormalSpr -- Display sprite
|

```

And then configure it as in the next section.

## StorageInventory

This configures how the object inventory works.

Original settings:
- **Type**: This defines how the inventory should work, used for example to control custom interface for each kind of item (e.g.: highlight a slot, show a special UI, etc)
	- See [Inventory_SlotType](/notes/Inventory_SlotType.md)
- **Slots**: Leave it in 0. I am not sure if you can customize it (maybe to put initial items? Untested)
- **Size**: The number of slots this inventory has. For example, if Size = 10, it can hold 10 items.


Custom settings:
Currently, we don't have any custom setting.
