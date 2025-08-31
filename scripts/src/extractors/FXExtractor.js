import path from "path";
import fs from "fs";
import { Constantify } from "../utils/Constantify.js";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { constOutDir } from "../config.js";
import assert from "assert";

/**
 * Extracts FX and Emote constants from the project
 * generating Emote.cs and FX.cs files.
 */
export class FXExtractor {
	// The game files has those duplicated, so we need to give them new constant names
	#overrides = {
		90: { from: 'Drugbug01Prefab', to: 'Drugbug01Prefab_B' },
		92: { from: 'SandImpact3', to: 'SandImpact3_B' },
	};

	async #extractFX(project, fxManager) {
		// Scann all PrefabInstance files, as FX files are prefabs.
		await project.scanDirectory('Assets/PrefabInstance');

		const fxClass = new ClassBuilder('YotanModCore.Consts', 'FX');

		fxManager.fx.forEach((fx, index) => {
			const meta = project.getMetaByGuid(fx.fxPrefab.guid);
			const fileName = path.basename(meta.filePath);
			let constName = Constantify.fileName(fileName);
			if (this.#overrides[index]) {
				assert(this.#overrides[index].from === constName, `Override name mismatch for index ${index}: expected ${this.#overrides[index].from}, got ${constName}`);
				constName = this.#overrides[index].to;
			}
			fxClass.addIntConstField(constName, index, fileName);
		});

		fs.writeFileSync(constOutDir + '/FX.cs', fxClass.build(), 'utf-8');
	}

	async #extractEmotes(project, fxManager) {
		// Scann all Sprite files, as Emote files are sprites.
		await project.scanDirectory('Assets/Sprite');

		const emoteClass = new ClassBuilder('YotanModCore.Consts', 'Emote');
		fxManager.emotionImages.forEach((emote, index) => {
			const meta = project.getMetaByGuid(emote.guid);
			const fileName = path.basename(meta.filePath);
			const constName = Constantify.fileName(fileName);
			emoteClass.addIntConstField(constName, index, fileName);
		});

		fs.writeFileSync(constOutDir + '/Emote.cs', emoteClass.build(), 'utf-8');
	}

	/**
	 * Extracts FX and Emote constants from the project
	 * @param {import("../unity/Project.js").Project} project
	 */
	async extract(project) {
		// Finds the GUID for FXManager.cs
		const fxManagerMeta = await project.loadMeta('Assets/Scripts/Assembly-CSharp/FXManager.cs');

		// Get FXManager from scene_01 (main scene))
		const scene01 = await project.loadContainer('Assets/Scenes/scene_01.unity');

		/** @type {import("../types.mi.js").FXManager} */
		const fxManager = scene01.getMonoBehaviourByGuid(fxManagerMeta.guid);

		console.log('Extracting FX...');
		await this.#extractFX(project, fxManager);

		console.log('Extracting Emotes...');
		await this.#extractEmotes(project, fxManager);
	}
}
