namespace YotanModCore.Consts
{
	public class ManRapeSleepConst
	{
		public const int Start = 0;
		public const int StartRape = 1;
		public const int StartDiscretlyRape = 2;
		public const int Insert = 3;
		public const int ChangeSpeed = 4;
		public const int Bust = 5;
		public const int SleepPowderSex = 6;
		public const int Leave = 10;

		public class SubState
		{
			public const int Raping = 1;
			public const int MaybeSleepPowder = 2;
			public const int DiscretlyRaping = 3;
			public const int Detected = 5;
		}

		public class Actions
		{
			public const int Rape = 1; // ok
			public const int SleepPowder = 2; // ok
			public const int Insert = 3; // ok
			public const int ChangeSpeed = 4; // ok
			public const int Bust = 5; // ok
			public const int FuckSleepPowder = 6; // ok
			public const int DiscretlyRape = 9; // ok
			public const int Leave = 10; // ok
		}
	}
}