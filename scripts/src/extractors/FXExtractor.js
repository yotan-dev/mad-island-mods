import path from "path";
import fs from "fs";
import { Constantify } from "../utils/Constantify.js";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { outDir } from "../config.js";

/**
 * Extracts FX and Emote constants from the project
 * generating Emote.cs and FX.cs files.
 */
export class FXExtractor {
	async #extractFX(project, fxManager) {
		// Scann all PrefabInstance files, as FX files are prefabs.
		await project.scanDirectory('Assets/PrefabInstance');

		const fxClass = new ClassBuilder('YotanModCore.Consts', 'FX');

		fxManager.fx.forEach((fx, index) => {
			const meta = project.getMetaByGuid(fx.fxPrefab.guid);
			const fileName = path.basename(meta.filePath);
			const constName = Constantify.fileName(fileName);
			fxClass.addIntConstField(constName, index, fileName);
		});

		fs.writeFileSync(outDir + '/FX.cs', fxClass.build(), 'utf-8');
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

		fs.writeFileSync(outDir + '/Emote.cs', emoteClass.build(), 'utf-8');
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
