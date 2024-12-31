namespace HFramework.Scenes.Conditionals
{
	public class LibidoCheck : IConditional
	{
		public int NpcId { get; private set; }
		public string Compare { get; private set; }
		public float ExpectedValue { get; private set; }


		public LibidoCheck(int npcId, string compare, float expectedValue)
		{
			this.NpcId = npcId;
			this.Compare = compare;
			this.ExpectedValue = expectedValue;
		}

		private bool CompareTo(float currentValue)
		{
			if (this.Compare == "==")
				return currentValue == this.ExpectedValue;
			else if (this.Compare == "!=")
				return currentValue != this.ExpectedValue;
			else if (this.Compare == ">=")
				return currentValue >= this.ExpectedValue;
			else if (this.Compare == "<=")
				return currentValue <= this.ExpectedValue;

			PLogger.LogError($"Unknown compare operator {this.Compare}");
			return false;
		}

		public bool Pass(IScene2 scene)
		{
			bool pass = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					pass = pass || this.CompareTo(actor.libido);
			}

			return pass;
		}

		public bool Pass(CommonStates from, CommonStates to)
		{
			bool pass = true;
			if (from.npcID == this.NpcId)
				pass = pass && this.CompareTo(from.libido);

			if (to?.npcID == this.NpcId)
				pass = pass && this.CompareTo(to.libido);

			return pass;
		}
	}
}
