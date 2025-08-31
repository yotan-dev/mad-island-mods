import { ClassBuilder } from "../utils/ClassBuilder.js";
import { Constantify } from "../utils/Constantify.js";
import { FieldBuilder } from "../utils/FieldBuilder.js";
import { assert } from "console";
import * as fs from "fs";
import { outDir } from "../config.js";

export class NpcIdExtractor {
	// from SaveManger::LoadDLC
	#DlcNpcIDs = new Set([
		2,   // Riona
		3,   // Little
		14,  // Native Boy
		16,  // Native Girl
		140, // LargeNativeGirl
		141, // LargeNativeBoy
		142, // UnderGroundGirl
		143, // UnderGroundBoy
	]);

	// There are a few cases where 2 different NPCs have the same name, so we manually override them here.
	#rename = {
		30: 'Spider2',
		113: 'Cassie2',
		149: 'Giant2',
	};

	/**
	 * 
	 * @param {import("../unity/Project.js").Project} project 
	 */
	async extract(project) {
		const npcStatsMeta = await project.loadMeta('Assets/Scripts/Assembly-CSharp/StatsData.cs');
		const metas = await project.scanDirectory('Assets/Resources/npcstats');

		const npcList = [];
		for (const meta of metas) {
			if (!meta.filePath.endsWith('.prefab'))
				continue;

			const npcId = meta.filePath.match(/\/(\d+).prefab$/)[1];
			assert(npcId, `No NPC ID found in ${meta.filePath}`);

			const npcStats = await project.loadContainer(meta.filePath);

			/** @type {import("../types.mi.js").NpcStats} */
			const npcStatsObj = npcStats.getMonoBehaviourByGuid(npcStatsMeta.guid);
			
			let npcName = npcStatsObj.localizedName[1];
			npcName = this.#rename[npcId] ?? npcName;
			npcList.push({ npcId: parseInt(npcId, 10), npcName, constant: Constantify.text(npcName) });
		}

		npcList.sort((a, b) => a.npcId - b.npcId);

		const npcIdBuilder = new ClassBuilder('YotanModCore.Consts', 'NpcIDs');

		for (const { npcId, npcName, constant } of npcList) {
			const field = FieldBuilder
				.intConst(constant, npcId)
				.addAttribute(`[StrVal("${npcName}")]`)
				.setComment(this.#DlcNpcIDs.has(npcId) ? ' // DLC' : '')
				.build();
			npcIdBuilder.addRawField(field);
		}

		fs.writeFileSync(`${outDir}NpcID.cs`, npcIdBuilder.build());
	}
}
