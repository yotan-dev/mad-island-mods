using System;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;

namespace ItemSlotColor
{
	public class ColorTracker: MonoBehaviour
	{
		private static readonly Color32 Transparent = new Color32(0, 0, 0, 0);
		
		private ItemSlot? Slot;

		private Image? Image;

		private string? ItemKey;

		private bool IsDyeable;


		public void Init(ItemSlot slot)
		{
			this.Slot = slot;
			this.Image = this.GetComponent<Image>();
		}

		private void UpdateItem(ItemSlot slot, Image image)
		{
			this.ItemKey = slot.itemKey;
			if (this.ItemKey == "") {
				image.color = Transparent;
				return;
			}

			var itemData = Managers.mn.itemMN.FindItem(this.ItemKey);
			if (itemData == null) {
				image.color = Transparent;
				return;
			}

			this.IsDyeable = Managers.mn.inventory.IsDyeable(itemData.itemType);
			if (!IsDyeable) {
				image.color = Transparent;
				return;
			}
			
			image.color = slot.itemColor;
		}

		public void Update()
		{
			// While this was never supposed to happen, it does happen in a certain case during Entking event
			if (this.Slot == null || this.Image == null)
				return;

			try {
				if (this.ItemKey != this.Slot.itemKey || (this.IsDyeable && this.Slot.itemColor != this.Image.color))
					this.UpdateItem(this.Slot, this.Image);
			} catch (Exception e) {
				PLogger.LogError("Error in ColorTracker.Update()");
				PLogger.LogError(e);
			}
		}
	}
}
