using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore.Consts;

namespace HFramework.Scenes.Conditionals
{
	public class PerfumeCheck : BaseConditional, IConditional
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

		public PerfumeCheck() { }

		public PerfumeCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public override bool Pass(IScene scene)
		{
			bool hasPerfume = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					hasPerfume = hasPerfume || actor.debuff.perfume > 0f;
			}

			return hasPerfume == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			bool hasPerfume = false;
			if (from.npcID == this.NpcId)
				hasPerfume = hasPerfume || (from.debuff.perfume > 0f);

			if (to?.npcID == this.NpcId)
				hasPerfume = hasPerfume || (to.debuff.perfume > 0f);

			return hasPerfume == this.ExpectedValue;
		}
	}
}
