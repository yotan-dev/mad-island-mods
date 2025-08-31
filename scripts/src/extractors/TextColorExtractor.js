import assert from "assert";
import fs from "fs";
import { Constantify } from "../utils/Constantify.js";
import { ClassBuilder } from "../utils/ClassBuilder.js";
import { outDir } from "../config.js";

export class TextColorExtractor {
	// The game does not provide a proper name we can use as constant,
	// so we need to hardcode it here
	#colorsList = [
		{ r: 255, g: 0,   b: 0,   a: 255, name: 'Red' },
		{ r: 255, g: 255, b: 255, a: 255, name: 'White' },
		{ r: 11,  g: 207, b: 91,  a: 255, name: 'Green' },
		{ r: 25,  g: 225, b: 231, a: 255, name: 'Cyan' },
		{ r: 236, g: 125, b: 14,  a: 255, name: 'Orange' },
		{ r: 255, g: 93,  b: 233, a: 255, name: 'Pink' },
		{ r: 93,  g: 108, b: 255, a: 255, name: 'LightBlue' },
		{ r: 118, g: 2,   b: 18,  a: 255, name: 'DarkRed' },
		{ r: 44,  g: 141, b: 147, a: 255, name: 'Teal' },
		{ r: 88,  g: 137, b: 33,  a: 255, name: 'Olive' },
	];

	#getColorInfo(color) {
		const rgba = {
			r: Math.round(color.r * 255),
			g: Math.round(color.g * 255),
			b: Math.round(color.b * 255),
			a: Math.round(color.a * 255),
		};
		const colorInfo = this.#colorsList
			.find((c) => c.r === rgba.r && c.g === rgba.g && c.b === rgba.b && c.a === rgba.a);
		
		assert(colorInfo, `Color ${rgba.r} ${rgba.g} ${rgba.b} ${rgba.a} not found`);
		
		return colorInfo;
	}

	/**
	 * @param {import("../unity/Project.js").Project} project 
	 */
	async extract(project) {
		const fxManagerMeta = await project.loadMeta('Assets/Scripts/Assembly-CSharp/FXManager.cs');
		const scene01 = await project.loadContainer('Assets/Scenes/scene_01.unity');

		/** @type {import("../types.mi.js").FXManager} */
		const fxManagerObj = scene01.getMonoBehaviourByGuid(fxManagerMeta.guid);

		const colorList = fxManagerObj.textColors
			.map((color, index) => ({ index, colorInfo: this.#getColorInfo(color) }));

		const classBuilder = new ClassBuilder('YotanModCore.Consts', 'TextColor');
		for (const { colorInfo, index } of colorList.sort((a, b) => a.index - b.index)) {
			classBuilder.addIntConstField(Constantify.text(colorInfo.name), index, `RGBA(${colorInfo.r}, ${colorInfo.g}, ${colorInfo.b}, ${colorInfo.a})`);
		}

		console.log('Generating TextColor.cs...');
		fs.writeFileSync(`${outDir}TextColor.cs`, classBuilder.build(), 'utf-8');
	}
}
