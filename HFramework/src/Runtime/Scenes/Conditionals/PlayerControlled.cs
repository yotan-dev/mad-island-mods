using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes.Conditionals
{
	/// <summary>
	/// Checks if a specific NPC is currently controlled by the player.
	/// </summary>
	public class PlayerControlled : BaseConditional, IConditional
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

		public PlayerControlled() { }

		public PlayerControlled(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		private bool IsPlayerControlled(CommonStates common)
		{
			return Managers.mn.gameMN.IsPreparingControlledCharacter(common) // Player is switching to control common
				|| Managers.mn.gameMN.IsControlledNpc(common); // Player is controlling this NPC
		}

		public override bool Pass(IScene scene)
		{
			bool isPlayerControlled = false;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isPlayerControlled = isPlayerControlled || this.IsPlayerControlled(actor);
			}

			return isPlayerControlled == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			bool isPlayerControlled = false;
			if (from.npcID == this.NpcId)
				isPlayerControlled = isPlayerControlled || this.IsPlayerControlled(from);

			if (to?.npcID == this.NpcId)
				isPlayerControlled = isPlayerControlled || this.IsPlayerControlled(to);

			return isPlayerControlled == this.ExpectedValue;
		}
	}
}
