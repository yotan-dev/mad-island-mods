using YotanModCore.Consts;

namespace YotanModCore.Console.Commands
{
	/// <summary>
	/// Resets basic sex info so character is considered virgin again.
	/// 
	/// Syntax: /makevirgin [friendId]
	/// If friendID is not specified, the selected NPC is used, if none, then active player is used
	/// </summary>
	public class MakeVirginCmd : ConsoleCmd
	{
		public override void Execute(string command, string[] arguments)
		{
			CommonStates common = CommandUtils.GetTarget(arguments, 0);
			if (common == null)
				return;

			common.sexInfo[SexInfoIndex.FirstSex] = -1;
			common.sexInfo[SexInfoIndex.SexCount] = 0;
			common.sexInfo[SexInfoIndex.Pregnancies] = 0;
			common.sexInfo[SexInfoIndex.Deliveries] = 0;
			common.sexInfo[SexInfoIndex.Masturbation] = 0;
		}
	}
}
