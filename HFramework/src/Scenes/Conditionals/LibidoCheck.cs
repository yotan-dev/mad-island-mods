using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes.Conditionals
{
	public class LibidoCheck : BaseConditional, IConditional
	{
		[XmlAttribute("actor")]
		public string Actor
		{
			get { return $"#{this.NpcId}"; }
			set
			{
				var act = new ConfigActor(value);
				this.NpcId = act.NpcId;
			}
		}

		[XmlIgnore]
		public int NpcId { get; set; } = NpcID.None;

		[XmlAttribute("compare")]
		public string Compare { get; set; } = "==";

		[XmlAttribute("value")]
		public float ExpectedValue { get; set; } = 0f;

		public LibidoCheck() { }

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

		public override bool Pass(IScene scene)
		{
			bool pass = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					pass = pass || this.CompareTo(actor.libido);
			}

			return pass;
		}

		public override bool Pass(CommonStates from, CommonStates to)
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
