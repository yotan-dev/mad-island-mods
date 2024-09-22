using UnityEngine;
using UnityEngine.UI;
using YotanModCore;

namespace ItemSlotColor
{
	public class ColorTracker: MonoBehaviour
	{
		private static readonly Color32 Transparent = new Color32(0, 0, 0, 0);
		
		private ItemSlot Slot;

		private Image Image;

		private string ItemKey;

		private bool IsDyeable;


		public void Init(ItemSlot slot)
		{
			this.Slot = slot;
			this.Image = this.GetComponent<Image>();
		}

		private void UpdateItem()
		{
			this.ItemKey = this.Slot.itemKey;
			if (this.ItemKey == "") {
				this.Image.color = Transparent;
				return;
			}

			var itemData = Managers.mn.itemMN.FindItem(this.ItemKey);
			this.IsDyeable = Managers.mn.inventory.IsDyeable(itemData.itemType);
			if (itemData == null || !IsDyeable) {
				this.Image.color = Transparent;
				return;
			}
			
			this.Image.color = this.Slot.itemColor;
		}

		public void Update()
		{
			if (this.ItemKey != this.Slot.itemKey || (this.IsDyeable && this.Slot.itemColor != this.Image.color))
				this.UpdateItem();
		}
	}
}