using System.Linq;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.Daruma
{
	public class DarumaSceneManager : ISceneManager
	{
		public static readonly DarumaSceneManager Instance = new DarumaSceneManager();

		private DarumaSceneManager()
		{
			
		}

		private bool IsUnlocked(int npcA, int npcB)
		{
			return GalleryState.Instance.AssWall.Any((interaction) =>
			{
				return interaction.Character1.Id == npcA
					&& interaction.Character2.Id == npcB
					;
			});
		}

		public void Unlock(GalleryChara player, GalleryChara girl)
		{
			if (player == null || girl == null)
			{
				GalleryLogger.LogError($"DarumaSceneManager#Unlock: chara is null");
				return;
			}

			var desc = $"{player} x {girl}";

			if (this.IsUnlocked(player.Id, girl.Id))
			{
				GalleryLogger.LogDebug($"DarumaSceneManager#Unlock: already unlocked for {desc}");
				return;
			}

			GalleryLogger.LogDebug($"DarumaSceneManager#Unlock: event UNLOCKED for {desc}");
			SaveFile.GalleryState.Instance.Daruma.Add(
				new CharacterInteraction(player, girl)
			);
		}

		private GallerySceneInfo BuildSceneInfo(int npcB, bool dlc = false)
		{
			return new GallerySceneInfo() {
				CharGroup = GallerySceneInfo.CharGroups.Man,
				SceneType = GallerySceneInfo.SceneTypes.Daruma,
				Name = "{npcA} fucks\n{npcB} on\nCushion (Daruma)",
				NpcA = new GallerySceneInfo.SceneNpc() { NpcID = NpcID.Man, Pregnant = false },
				NpcB = new GallerySceneInfo.SceneNpc() { NpcID = npcB, Pregnant = false },
				IsUnlocked = this.IsUnlocked(NpcID.Man, npcB),
				RequireDLC = dlc,
				// Play = (GallerySceneInfo.PlayData data) => {
				// 	// Put NPC into a dummy inventory slot
				// 	var npcItem = Managers.mn.itemMN.FindItem(Managers.mn.itemMN.NPCIDToItem(data.NpcB.npcID));
				// 	Managers.mn.itemMN.ItemToSlot(npcItem, Managers.mn.inventory.itemSlot[0], 1);
				// 	Managers.mn.inventory.itemSlot[0].common = data.NpcB;

				// 	// Hide entities
				// 	data.NpcA.gameObject.SetActive(false);
				// 	data.NpcB.gameObject.SetActive(false);
					
				// 	// Add NpcB to Daruma slot[0]
				// 	var invSlot = data.Prop.GetComponent<InventorySlot>();
				// 	Managers.mn.inventory.SlotToSlot(
				// 		Managers.mn.inventory.itemSlot[0],
				// 		invSlot.slots[0]
				// 	);

				// 	// Set Daruma as active inventory
				// 	Managers.mn.inventory.tmpSubInventory = invSlot;

				// 	Managers.mn.StartCoroutine(Managers.mn.gameMN.ToiletCheck(invSlot, 0));

				// 	// @TODO:
				// 	// return new DarumaScenePlayer(data.NpcA.GetComponent<CommonStates>(), invSlot.slots[0].common, invSlot)
				// 	// 	.Play(0);
				// },
				Prop = "deco_labo_cushion_01", // There are many others that also work
			};
		}

		public GallerySceneInfo[] GetScenes()
		{
			return new GallerySceneInfo[] {
				// this.BuildSceneInfo(NpcID.FemaleNative),
				// this.BuildSceneInfo(NpcID.NativeGirl, dlc: true),
			};
		}
	}
}