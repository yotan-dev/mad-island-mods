import assert from "assert";
import fs from "fs";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { outDir } from "../config.js";
import { Constantify } from "../utils/Constantify.js";

export class LayerExtractor {
	/**
	 * @param {import("../unity/Project.js").Project} project 
	 */
	async extract(project) {
		const tagManager = await project.loadContainer('ProjectSettings/TagManager.asset');

		/** @type {import("../types.mi.js").TagManager} */
		const tagManagerObj = tagManager.data[0].TagManager;
		assert(tagManagerObj);

		const layers = tagManagerObj.layers.filter((l) => !!l);
		assert(layers);

		console.log('Extracting Layers...');

		const classBuilder = new ClassBuilder('YotanModCore.Consts', 'Layers');
		for (const name of [...layers].sort()) {
			classBuilder.addStringConstField(Constantify.text(name), name);
		}

		console.log('Generating Layers.cs...');
		fs.writeFileSync(`${outDir}Layers.cs`, classBuilder.build(), 'utf-8');
	}
}
