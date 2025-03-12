using System.Collections.Generic;
using YotanModCore;

namespace EnhancedIsland.RequirementChecker
{
	public class CraftRequirements
	{
		public class Requirement
		{
			public string ItemKey;

			public int Required;

			public int Current;

			public Requirement(string itemKey)
			{
				this.ItemKey = itemKey;
			}
		}

		private Dictionary<string, Requirement> RequirementDict = new();

		private List<Requirement> Requirements = new();

		public bool MissingRequirement
		{
			get
			{
				foreach (var req in this.Requirements)
				{
					if (req.Required > req.Current)
					{
						return true;
					}
				}

				return false;
			}
		}

		public void AddRequirement(Requirement req)
		{
			this.RequirementDict.Add(req.ItemKey, req);
			this.Requirements.Add(req);
		}

		public List<Requirement> GetRequirementList()
		{
			return this.Requirements;
		}

		private void CheckInventory(ItemSlot[] inventory, int offset, int size)
		{
			for (int i = 0; i < size; i++)
			{
				var itemSlot = inventory[i + offset];

				if (itemSlot.itemKey != null && this.RequirementDict.TryGetValue(itemSlot.itemKey, out var req))
				{
					req.Current += itemSlot.stack;
				}
			}
		}

		public void CheckRequirements()
		{
			var invManager = Managers.mn.inventory;
			if (invManager.craftPanelID == 0)
			{
				this.CheckInventory(invManager.itemSlot, 0, invManager.baseCount);
			}
			else
			{
				this.CheckInventory(invManager.itemSlot, 50, invManager.tmpSubInventory.size);

				int i = 0;
				while (this.MissingRequirement && i < invManager.refInventory.Count)
				{
					this.CheckInventory(invManager.refInventory[i].slots, 0, invManager.refInventory[i].slots.Length);
					i++;
				}
			}

		}
	}
}