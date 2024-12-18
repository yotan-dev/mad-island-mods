using System;
using System.Collections;
using ExtendedHSystem;
using ExtendedHSystem.Scenes;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.Slave
{
	public class SlaveSceneEventHandler : SceneEventHandler
	{
		private readonly GalleryChara Player = null;

		private readonly GalleryChara Girl = null;

		private bool DidBust = false;

		public SlaveSceneEventHandler(CommonStates player, InventorySlot slaveObject) : base("yogallery_slave_handler")
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

		public override IEnumerable OnBusted(CommonStates from, CommonStates to, int specialFlag)
		{
			this.DidBust = true;
			return base.OnBusted(from, to, specialFlag);
		}

		public override IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			if (this.DidBust)
			{
				SlaveSceneManager.Instance.Unlock(this.Player, this.Girl);
			}
			else
			{
				var desc = $"{this.Player} x {this.Girl}";
				GalleryLogger.LogDebug($"SlaveSceneTracker#OnEnd: 'DidBust' ({this.DidBust}) not set -- event NOT unlocked for {desc}");
			}

			yield break;
		}
	}
}