# Yotan Mod Core API docs

This file describes how to use some of the APIs available in Yotan Mod Core.

In case of doubt, check the code directly.


## Console commands (DebugTool)

This module allows you to register new commands for the game console (opened with Enter).

Every command should be a class that inherits `YotanModCore.Console.ConsoleCmd` and
implements `Execute`.

After you have a command class, you can register it with `YotanModCore.Console.ConsoleManager#RegisterCmd`.

When the player types the `/command` specified in `RegisterCmd`, the framework will split the command and arguments
part and send it to `Execute` as is, there you can handle the parameters and perform whatever you need.

In the example below, players may type `/sample` or `/sample2` to execute `SampleCmd`.

```CS
public class SampleCmd : ConsoleCmd
{
	public override void Execute(string command, string[] arguments)
	{
		// command = /sample or /sample2
		// arguments = any other value sent after the command, separated by space.
	}
}

// ... somewhere else ...
ConsoleManager.Instance.RegisterCmd("/sample", new SampleCmd());
ConsoleManager.Instance.RegisterCmd("/sample2", new SampleCmd());
//

```

Some example inputs:

| Input                | `command`  | `arguments`       |
| -------------------- | ---------- | ----------------- |
| `/sample`            | `/sample`  | []                |
| `/sample2`           | `/sample2` | []                |
| `/sample test`       | `/sample`  | [`test`]          |
| `/sample test test2` | `/sample`  | [`test`, `test2`] |
| `/sample 1 2`        | `/sample`  | [`1`, `2`]        |

Note that arguments are always string, if you need an int, you have to cast it.
