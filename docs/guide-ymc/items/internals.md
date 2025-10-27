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
