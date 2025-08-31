import path from "path";
import fs from "fs";
import { Constantify } from "../utils/Constantify.js";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { outDir } from "../config.js";

/**
 * Extracts AudioSE and AudioBGM constants from the project
 * generating AudioSE.cs and AudioBGM.cs files.
 */
export class SoundExtractor {
	async #extractSE(project, soundManager) {
		// Scann all AudioClip files, as SE files are audio clips.
		await project.scanDirectory('Assets/AudioClip');

		const seClass = new ClassBuilder('YotanModCore.Consts', 'AudioSE');

		soundManager.se.forEach((se, index) => {
			const meta = project.getMetaByGuid(se.guid);
			const fileName = path.basename(meta.filePath);
			const constName = Constantify.fileName(fileName);
			seClass.addIntConstField(constName, index, fileName);
		});

		fs.writeFileSync(outDir + '/AudioSE.cs', seClass.build(), 'utf-8');
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

		fs.writeFileSync(outDir + '/AudioBGM.cs', bgmClass.build(), 'utf-8');
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
