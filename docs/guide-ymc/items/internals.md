# Internals

This page explains some internals of how Items work in Mad Island.

## Reserved slots

The game keeps all inventory-related data in a single big array, where certain ranges are reserved for certain things.

This list may be innacurate but it looks something like:

- 100 .. 999: Crafting recipes (for the current crafting station)
- 1000 .. ???: Shop items


## Craft/Buy handling

When a player opens a shop or crafting station, the game checks the cost of each item and enable/disable its button.

The button, when clicked, triggers the `ItemSlot::SlotClick`, which will pass it to either `InventoryManager::CraftItem` or `ShopManager::TradeItem`.

The slot itself holds the craft/shop info and is required that it also has the related `ItemData`.


## Random notes on Food items

This is mostly a brain dump regarding food items, I did not take the time to fully document it yet.

ItemData
- Type : Food
- Sub Type:  Cook (Can be put on table?)

FoodInfo
Food - Hunger
Water - water
Life - heal


item obj may be null if not put on table

world object (put on table)

ItemInfo - all 0
Box Collider - trigger
CookInfo -> Defence 1


**Raw meat:**
Type = fOod / Subtype = none

**water bottle:**

full => ItemData = Bottle/None ; Tool tYpe = durability ; life = 3
emtpy => ItemData = Bottle/None ; Tool tYpe = none ; life = 0

**growing potion:**
consumables/none
