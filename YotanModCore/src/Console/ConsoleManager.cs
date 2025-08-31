using System.Collections.Generic;
using System.Linq;
using YotanModCore.Console.Commands;

namespace YotanModCore.Console
{
	public class ConsoleManager
	{
		public static ConsoleManager Instance { get; } = new ConsoleManager();

		private Dictionary<string, ConsoleCmd> Commands = [];

		internal void Init()
		{
			this.RegisterCmd("/faint", new FaintCmd());

			this.RegisterCmd("/heal", new HealCmd());
			this.RegisterCmd("/healhp", new HealCmd());
			this.RegisterCmd("/healst", new HealCmd());
			this.RegisterCmd("/healfood", new HealCmd());
			this.RegisterCmd("/healwater", new HealCmd());

			this.RegisterCmd("/makevirgin", new MakeVirginCmd());

			this.RegisterCmd("/moral", new MoralCmd());

			// this.RegisterCmd("/stun", new StunCmd());
		}

		/// <summary>
		/// Tries to handle the input.
		/// If a command doesn't exists, returns false
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public bool Handle(string input)
		{
			PLogger.LogDebug($"Handling input: {input}");
			var parts = input.Split(' ');
			if (parts.Length == 0)
				return false;

			var command = parts[0];
			if (!Commands.ContainsKey(command))
				return false;

			var args = parts.Skip(1).ToArray();

			var cmd = Commands[command];
			PLogger.LogDebug($"Executing command: {cmd}");
			cmd.Execute(command, args);
			return true;
		}

		/// <summary>
		/// Registers "cmd" as the handler for "command"
		/// </summary>
		/// <param name="command"></param>
		/// <param name="cmd"></param>
		public void RegisterCmd(string command, ConsoleCmd cmd)
		{
			Commands.Add(command, cmd);
		}
	}
}
