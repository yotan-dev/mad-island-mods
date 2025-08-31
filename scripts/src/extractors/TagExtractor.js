import assert from "assert";
import fs from "fs";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { constOutDir } from "../config.js";
import { Constantify } from "../utils/Constantify.js";

export class TagExtractor {
	/**
	 * @param {import("../unity/Project.js").Project} project
	 */
	async extract(project) {
		const tagManager = await project.loadContainer('ProjectSettings/TagManager.asset');

		/** @type {import("../types.mi.js").TagManager} */
		const tagManagerObj = tagManager.data[0].TagManager;
		assert(tagManagerObj);

		const tags = tagManagerObj.tags;
		assert(tags);

		console.log('Extracting Tags...');
		// Those are built-in tags shipped by unity -- https://docs.unity3d.com/Manual/Tags.html
		tags.unshift(
			'Untagged',
			'Respawn',
			'Finish',
			'EditorOnly',
			'MainCamera',
			'Player',
			'GameController',
		);

		const classBuilder = new ClassBuilder('YotanModCore.Consts', 'Tags');
		for (const name of [...tags].sort()) {
			classBuilder.addStringConstField(Constantify.text(name), name);
		}

		console.log('Generating Tags.cs...');
		fs.writeFileSync(`${constOutDir}Tags.cs`, classBuilder.build(), 'utf-8');
	}
}
