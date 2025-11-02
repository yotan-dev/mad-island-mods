# Game info

[GameInfo](/api/YotanModCore.GameInfo.html) provides some useful information about the game, such as the game version, DLC status, etc.

You can use these to make your mod behave differently based on the running environment.

For example, if you want to support the main version and beta at the same time, or an old release, you can make use of Game Version data to check for compatibility.

Another option is when you are working with DLC-specific content, you may check for DLC status to properly handle it.

Game versions is manually mapped from the game's "Build date" (shown in the main menu), so we are not able to detect until I manually update it.

When a version is not mapped, we use some heuristics, and if everything fails, we default to 999.999.999.999 (as in, newer than any other release) -- I only expect this to happen when a new update comes.
