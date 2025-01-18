using System.Xml.Serialization;
using HFramework.ConfigFiles;
using YotanModCore.Consts;

namespace HFramework.Scenes.Conditionals
{
	public class FriendCheck : BaseConditional, IConditional
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

		public FriendCheck() {}


		public FriendCheck(int npcId, bool expectedValue)
		{
			this.NpcId = npcId;
			this.ExpectedValue = expectedValue;
		}

		public override bool Pass(IScene scene)
		{
			bool isFriend = true;
			foreach (var actor in scene.GetActors())
			{
				if (actor.npcID == this.NpcId)
					isFriend = isFriend && actor.Employed(false);
			}

			return isFriend == this.ExpectedValue;
		}

		public override bool Pass(CommonStates from, CommonStates to)
		{
			bool isFriend = true;
			if (from.npcID == this.NpcId)
				isFriend = isFriend && from.Employed(false);

			if (to?.npcID == this.NpcId)
				isFriend = isFriend && to.Employed(false);

			return isFriend == this.ExpectedValue;
		}
	}
}
