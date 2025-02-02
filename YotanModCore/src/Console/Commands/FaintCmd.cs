using YotanModCore.Consts;

namespace YotanModCore.Console.Commands
{
	/// <summary>
	/// Faints character.
	/// 
	/// Syntax: /faint [friendId]
	/// If friendID is not specified, the selected NPC is used, if none, then active player is used
	/// </summary>
	public class FaintCmd : ConsoleCmd
	{
		public override void Execute(string command, string[] arguments)
		{
			CommonStates common = CommandUtils.GetTarget(arguments, 0);

			common.FaintChange(-common.maxFaint);
		}
	}
}
