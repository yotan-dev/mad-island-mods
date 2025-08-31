import path from "path";
import fs from "fs";
import { Constantify } from "../utils/Constantify.js";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { constOutDir } from "../config.js";
import assert from "assert";

/**
 * Extracts AudioSE and AudioBGM constants from the project
 * generating AudioSE.cs and AudioBGM.cs files.
 */
export class SoundExtractor {
	// the game files has those duplicated, so we need to give them new constant names.
	#seOverrides = {
		90: { from: 'BreakGlass01', to: 'BreakGlass01_B' },
		160: { from: 'AcidDamage02', to: 'AcidDamage02_B' },
		220: { from: 'Arrow02', to: 'Arrow02_B' },
	};

	async #extractSE(project, soundManager) {
		// Scann all AudioClip files, as SE files are audio clips.
		await project.scanDirectory('Assets/AudioClip');

		const seClass = new ClassBuilder('YotanModCore.Consts', 'AudioSE');

		soundManager.se.forEach((se, index) => {
			const meta = project.getMetaByGuid(se.guid);
			const fileName = path.basename(meta.filePath);
			let constName = Constantify.fileName(fileName);
			if (this.#seOverrides[index]) {
				assert(this.#seOverrides[index].from === constName, `Override name mismatch for index ${index}: expected ${this.#seOverrides[index].from}, got ${constName}`);
				constName = this.#seOverrides[index].to;
			}
			seClass.addIntConstField(constName, index, fileName);
		});

		fs.writeFileSync(constOutDir + '/AudioSE.cs', seClass.build(), 'utf-8');
	}

	async #extractBGM(project, soundManager) {
		// Scann all AudioClip files, as BGM files are audio clips.
		await project.scanDirectory('Assets/AudioClip');

		const bgmClass = new ClassBuilder('YotanModCore.Consts', 'AudioBGM');
		soundManager.bgm.forEach((bgm, index) => {
			const meta = project.getMetaByGuid(bgm.guid);
			const fileName = path.basename(meta.filePath);
			const constName = Constantify.fileName(fileName);
			bgmClass.addIntConstField(constName, index, fileName);
		});

		fs.writeFileSync(constOutDir + '/AudioBGM.cs', bgmClass.build(), 'utf-8');
	}

	/**
	 * Extracts AudioSE and AudioBGM constants from the project
	 * @param {import("../unity/Project.js").Project} project
	 */
	async extract(project) {
		// Finds the GUID for SoundManager.cs
		const soundManagerMeta = await project.loadMeta('Assets/Scripts/Assembly-CSharp/SoundManager.cs');

		// Get SoundManager from scene_01 (main scene))
		const scene01 = await project.loadContainer('Assets/Scenes/scene_01.unity');

		/** @type {import("../types.mi.js").SoundManager} */
		const soundManager = scene01.getMonoBehaviourByGuid(soundManagerMeta.guid);

		console.log('Extracting AudioSE...');
		await this.#extractSE(project, soundManager);

		console.log('Extracting AudioBGM...');
		await this.#extractBGM(project, soundManager);
	}
}
