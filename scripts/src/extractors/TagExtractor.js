import assert from "assert";
import fs from "fs";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { outDir } from "../config.js";

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

		const layers = tagManagerObj.layers.filter((l) => !!l);
		assert(layers);

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
			classBuilder.addStringConstField(name, name);
		}

		console.log('Generating Tags.cs...');
		fs.writeFileSync(`${outDir}Tags.cs`, classBuilder.build(), 'utf-8');
	}
}
