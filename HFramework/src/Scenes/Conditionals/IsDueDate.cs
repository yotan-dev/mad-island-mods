using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes.Conditionals
{
	/// <summary>
	/// Checks if the given NPC is due to give birth. (Time remaining = 0)
	/// </summary>
	public class IsDueDate : BaseConditional, IConditional
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

		[XmlAttribute("value")]
		public bool ExpectedValue { get; set; } = false;

		public IsDueDate() { }

		public IsDueDate(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		private bool IsDueDateCheck(CommonStates common)
		{
			return common.pregnant[PregnantIndex.Father] != -1 && common.pregnant[PregnantIndex.TimeToBirth] == 0;
		}

		public override bool Pass(IScene scene)
		{
			bool isDueDate = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isDueDate = isDueDate || this.IsDueDateCheck(actor);
			}

			return isDueDate == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			bool pass = true;
			if (from.npcID == this.NpcId)
				pass = pass && this.IsDueDateCheck(from);

			if (to?.npcID == this.NpcId)
				pass = pass && this.IsDueDateCheck(to);

			return pass;
		}
	}
}
