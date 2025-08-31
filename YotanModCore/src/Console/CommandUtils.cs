namespace YotanModCore.Console
{
	public static class CommandUtils
	{
		/// <summary>
		/// Gets command target Common, following this order:
		/// 1. argument at friendIdArgIndex is a friendId
		/// 2. Selected NPC
		/// 3. Active player
		/// </summary>
		/// <param name="args"></param>
		/// <param name="friendIdArgIndex"></param>
		/// <returns></returns>
		public static CommonStates GetTarget(string[] args, int friendIdArgIndex)
		{
			CommonStates common = null;
			if (args.Length > friendIdArgIndex)
			{
				if (int.TryParse(args[friendIdArgIndex], out int friendId))
					common = Managers.mn.npcMN.GetFriend(friendId);

				if (common == null)
					return null;
			}

			if (common == null && Managers.mn.gameMN.menuNPC != null)
				common = Managers.mn.gameMN.menuNPC;

			if (common == null)
				common = CommonUtils.GetActivePlayer();

			return common;
		}

		/// <summary>
		/// Gets command target Common, following this order:
		/// 1. Selected NPC
		/// 2. Active player
		/// </summary>
		public static CommonStates GetTarget()
		{
			CommonStates common = null;
			if (common == null && Managers.mn.gameMN.menuNPC != null)
				common = Managers.mn.gameMN.menuNPC;

			if (common == null)
				common = CommonUtils.GetActivePlayer();

			return common;
		}
	}
}
