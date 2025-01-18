using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes.Conditionals
{
	public class PregnantCheck : BaseConditional, IConditional
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

		public PregnantCheck() { }

		public PregnantCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public override bool Pass(IScene scene)
		{
			bool isPregnant = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isPregnant = isPregnant || CommonUtils.IsPregnant(actor);
			}
			
			return isPregnant == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			bool pass = true;
			if (from.npcID == this.NpcId)
				pass = pass && CommonUtils.IsPregnant(from) == this.ExpectedValue;

			if (to?.npcID == this.NpcId)
				pass = pass && CommonUtils.IsPregnant(to) == this.ExpectedValue;

			return pass;
		}
	}
}
