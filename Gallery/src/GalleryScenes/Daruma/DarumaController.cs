#nullable enable

using System.Collections;
using System.Linq;
using Gallery.ConfigFiles;
using Gallery.SaveFile;
using Gallery.SaveFile.Containers;
using YotanModCore;

namespace Gallery.GalleryScenes.Daruma
{
	public class DarumaController : BaseController
	{
		public override void Unlock(GalleryChara[] charas)
		{
			if (charas.Length < 2)
			{
				PLogger.LogError($"DarumaController: Not enough actors. Expected 2, got {charas.Length}");
				return;
			}

			var desc = $"{charas[0]} x {charas[1]}";

			var actors = new GalleryActor[2] { new(charas[0]), new(charas[1]) };
			if (this.IsUnlocked(actors))
			{
				PLogger.LogInfo($"DarumaController: Already unlocked: {desc}");
				return;
			}

			PLogger.LogInfo($"DarumaController: Unlocking: {desc}");
			SaveFile.GalleryState.Instance.Daruma.Add(
				new CharacterInteraction(charas[0], charas[1])
			);
		}

		public override bool IsUnlocked(GalleryActor[] actors)
		{
			if (actors.Length < 2)
			{
				PLogger.LogError($"DarumaController: Not enough actors. Expected 2, got {actors.Length}");
				return false;
			}

			return GalleryState.Instance.Daruma.Any((interaction) =>
			{
				return interaction.Character1.Id == actors[0].NpcId
					&& interaction.Character2.Id == actors[1].NpcId
					;
			});
		}

		protected override IEnumerator GetScene(PlayData playData)
		{
			if (playData.Actors.Count < 2)
			{
				PLogger.LogError($"DarumaController: Not enough actors. Expected 2, got {playData.Actors.Count}");
				yield break;
			}

			playData.Actors[1].sex = CommonStates.SexState.Daruma;

			// Put NPC into a dummy inventory slot
			var npcItem = Managers.mn.itemMN.FindItem(Managers.mn.itemMN.NPCIDToItem(playData.Actors[1].npcID));
			Managers.mn.itemMN.ItemToSlot(npcItem, Managers.mn.inventory.itemSlot[0], 1);
			Managers.mn.inventory.itemSlot[0].common = playData.Actors[1];

			// Hide entities
			playData.Actors[0].gameObject.SetActive(false);
			playData.Actors[1].gameObject.SetActive(false);

			// Add Actors[1] (female) to AssWall slot[0]
			var invSlot = playData.Prop.GetComponent<InventorySlot>();
			Managers.mn.inventory.SlotToSlot(
				Managers.mn.inventory.itemSlot[0],
				invSlot.slots[0]
			);

			// Set AssWall as active inventory
			Managers.mn.inventory.tmpSubInventory = invSlot;

			yield return null;

			Managers.mn.gameMN.BodyRackCheck(invSlot, 0, true);

			this.Scene = new HFramework.Scenes.Daruma(playData.Actors[0], playData.Actors[1], invSlot);
		}
	}
}
