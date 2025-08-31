namespace YotanModCore.Console.Commands
{
	/// <summary>
	/// Changes active friendId moral by <moral>
	/// 
	/// Syntax: /moral [moral] [friendId]
	/// </summary>
	public class MoralCmd : ConsoleCmd
	{
		public override void Execute(string command, string[] arguments)
		{
			if (arguments.Length == 0 || !int.TryParse(arguments[0], out int moral))
			{
				PLogger.LogError($"Invalid moral value: {(arguments.Length > 0 ? arguments[0] : "null")}");
				return;
			}

			CommonStates common = CommandUtils.GetTarget(arguments, 1);
			if (common == null)
				return;

			common.MoralChange(-common.moral + moral);
		}
	}
}
