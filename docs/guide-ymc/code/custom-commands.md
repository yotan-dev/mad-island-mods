# Custom commands

Yotan Mod Core provides a simple way to add custom console commands (the ones you can use by pressing enter and typing `/<command name>`) to the game.

To create your own command:

1. Create a class that inherits from `YotanModCore.Console.ConsoleCmd`.
2. Implement the `Execute` method.
3. Register the command using `ConsoleManager.Instance.RegisterCmd("/<commandName>", new YourCommandClass())`.


For example:


```C#
namespace YourNamespace
{
	public class YourCommandName : ConsoleCmd
	{
		public override void Execute(string command, string[] arguments)
		{
			// Do something
		}
	}
}
```

`Execute` receives 2 parameters:

- `command`: the command name (e.g. `/yourCommandName`)
- `arguments`: the arguments passed to the command (e.g. for `/yourCommandName arg1 arg2` it will be `["arg1", "arg2"]`) -- each space will become a new argument.

You can make use of [CommandUtils](/api/YotanModCore.Console.CommandUtils.html) to help you identify some useful things. For example, picking the `common` that the player is currently targetting, or converting an NPC ID to it, etc.

And register it:

```C#
ConsoleManager.Instance.RegisterCmd("/yourCommandName", new YourCommandName());
```

