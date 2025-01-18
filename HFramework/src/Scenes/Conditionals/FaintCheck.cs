using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore.Consts;

namespace HFramework.Scenes.Conditionals
{
	public class FaintCheck : BaseConditional, IConditional
	{
		[XmlAttribute("actor")]
		public string Actor {
			get { return $"#{this.NpcId}"; }
			set {
				var act = new ConfigActor(value);
				this.NpcId = act.NpcId;
			}
		}

		[XmlIgnore]
		public int NpcId { get; set; } = NpcID.None;

		[XmlAttribute("value")]
		public bool ExpectedValue { get; set; } = false;

		public FaintCheck() {}

		public FaintCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public override bool Pass(IScene scene)
		{
			bool isFainted = true;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isFainted = isFainted && (actor.faint <= 0 || actor.life <= 0);
			}

			return isFainted == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			bool pass = true;
			if (from.npcID == this.NpcId)
				pass = pass && (from.faint <= 0 || from.life <= 0);

			if (to?.npcID == this.NpcId)
				pass = pass && (to.faint <= 0 || to.life <= 0);

			return pass == this.ExpectedValue;
		}
	}
}
