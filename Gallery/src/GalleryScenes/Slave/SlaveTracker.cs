using System;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.Slave
{
	public class SlaveTracker : BaseTracker
	{
		private readonly GalleryChara Player = null;

		private readonly GalleryChara Girl = null;

		public SlaveTracker(CommonStates player, InventorySlot slaveObject) : base()
		{
			this.Player = new GalleryChara(player);
			this.Girl = new GalleryChara(this.Object2Npc(slaveObject.GetComponent<ItemInfo>().itemKey));
		}

		private int Object2Npc(string obj)
		{
			switch (obj)
			{
				case "slave_giant_01": return NpcID.Giant;
				case "slave_shino_01": return NpcID.Shino;
				case "slave_sally_01": return NpcID.Sally;
				default: throw new Exception($"New slave: '{obj}'");
			}
		}


		public override void End()
		{
			if (this.Busted)
			{
				SlaveSceneManager.Instance.Unlock(this.Player, this.Girl);
			}
			else
			{
				var desc = $"{this.Player} x {this.Girl}";
				GalleryLogger.LogDebug($"SlaveSceneTracker#OnEnd: 'Busted' ({this.Busted}) not set -- event NOT unlocked for {desc}");
			}
		}
	}
}
