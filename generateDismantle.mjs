import * as fs from 'fs';
import { CraftData } from "./CraftData.mjs";

const dismantleTable = {};

CraftData.filter((v) => v.craftDataId !== 24)
	.forEach((station) => {
		station.craftInfo.forEach((craft) => {
			if (!craft.resultName) {
				throw new Error("Missing craft.resultName");
			}

			const craftedItem = craft.resultName;
			if (dismantleTable[craft.resultName]) {
				console.error("Duplicate craft.resultName: " + craft.resultName);
				return;
			}

			dismantleTable[craft.resultName] = [];

			const table = dismantleTable[craftedItem];
			
			craft.required.forEach((req) => {
				if (req.itemData.itemId.includes('parts_')) {
					console.log(`Skipping body part: ${req.itemData.itemId}`);
					return;
				}
				table.push({ count: req.count, item: req.itemData.itemId });
			});
		});
	});


const finalTable = [];
Object.entries(dismantleTable).forEach(([itemId, items]) => {
	if (items.length === 0) {
		console.log(`Skipping "${itemId}" because it has no required items`);
		return;
	}

	finalTable.push({
		itemId,
		items: items.map((item) => ({ item: item.item, count: Number(item.count) })),
	});
});

fs.writeFileSync("DisassembleTable.json", JSON.stringify(finalTable, null, '\t'));
