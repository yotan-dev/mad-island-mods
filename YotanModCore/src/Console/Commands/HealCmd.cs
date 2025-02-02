namespace YotanModCore.Console.Commands
{
	/// <summary>
	/// Heals player HP, Stamina, Food and Water
	/// 
	/// Syntax: /heal or /healhp or /healst or /healfood or /healwater
	/// </summary>
	public class HealCmd : ConsoleCmd
	{
		public override void Execute(string command, string[] arguments)
		{
			var player = CommonUtils.GetActivePlayer();
			if (command == "/heal" || command == "/healhp")
				player.CommonLifeChange(player.maxLife);
			if (command == "/heal" || command == "/healfood")
				Managers.mn.gameMN.FoodChange(player, player.maxFood);
			if (command == "/heal" || command == "/healwater")
				Managers.mn.gameMN.WaterChange(player, player.maxWater);
			if (command == "/heal" || command == "/healst")
			{
				player.FaintChange(player.maxFaint);
				Managers.mn.gameMN.LivesChange(3);
			}
		}
	}
}
