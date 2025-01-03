using System.Linq;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.AssWall
{
	public class AssWallSceneManager : ISceneManager
	{
		public static readonly AssWallSceneManager Instance = new AssWallSceneManager();

		private AssWallSceneManager() { }

		private bool IsUnlocked(int npcA, int npcB, InventorySlot.Type wallType)
		{
			return GalleryState.Instance.AssWall.Any((interaction) =>
			{
				return interaction.Character1.Id == npcA
					&& interaction.Character2.Id == npcB
					&& interaction.WallType == wallType
					;
			});
		}

		public void Unlock(GalleryChara player, GalleryChara girl, InventorySlot.Type wallType)
		{
			if (player == null || girl == null)
			{
				GalleryLogger.LogError($"AssWallSceneManager#Unlock: chara is null");
				return;
			}

			var desc = $"{player} x {girl} (WallType: {wallType})";

			if (this.IsUnlocked(player.Id, girl.Id, wallType))
			{
				GalleryLogger.LogDebug($"AssWallSceneManager#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"AssWallSceneManager#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.AssWall.Add(
				new AssWallInteractions(player, girl, wallType)
			);
		}

		// private GallerySceneInfo BuildSceneInfo(string charGroup, int npcB, bool dlc = false)
		// {
		// 	return new GallerySceneInfo() {
		// 		CharGroup = charGroup,
		// 		SceneType = SceneTypes.AssWall,
		// 		Name = "{npcA} fucks\n{npcB} on\nBack Toilet",
		// 		NpcA = new SceneNpc() { NpcID = NpcID.Man, Pregnant = false },
		// 		NpcB = new SceneNpc() { NpcID = npcB, Pregnant = false },
		// 		IsUnlocked = this.IsUnlocked(NpcID.Man, npcB, InventorySlot.Type.Toilet),
		// 		RequireDLC = dlc,
		// 		Play = (PlayData data) => {
		// 			// Put NPC into a dummy inventory slot
		// 			var npcItem = Managers.mn.itemMN.FindItem(Managers.mn.itemMN.NPCIDToItem(data.NpcB.npcID));
		// 			Managers.mn.itemMN.ItemToSlot(npcItem, Managers.mn.inventory.itemSlot[0], 1);
		// 			Managers.mn.inventory.itemSlot[0].common = data.NpcB;

		// 			// Hide entities
		// 			data.NpcA.gameObject.SetActive(false);
		// 			data.NpcB.gameObject.SetActive(false);

		// 			// Add NpcB to AssWall slot[0]
		// 			var invSlot = data.Prop.GetComponent<InventorySlot>();
		// 			Managers.mn.inventory.SlotToSlot(
		// 				Managers.mn.inventory.itemSlot[0],
		// 				invSlot.slots[0]
		// 			);

		// 			// Set AssWall as active inventory
		// 			Managers.mn.inventory.tmpSubInventory = invSlot;

		// 			Managers.mn.StartCoroutine(Managers.mn.gameMN.ToiletCheck(invSlot, 0));

		// 			return new AssWallScenePlayer(data.NpcA.GetComponent<CommonStates>(), invSlot.slots[0].common, invSlot)
		// 				.Play(0);
		// 		},
		// 		Prop = "gen_asswall_00", // _01 and _02 also works
		// 	};
		// }

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
				// this.BuildSceneInfo(CharGroups.NativeFemale, NpcID.FemaleNative),
				// this.BuildSceneInfo(CharGroups.NativeGirl, NpcID.NativeGirl, dlc: true),
			};
		}
	}
}
