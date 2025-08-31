import assert from 'assert';
import fs from 'fs/promises';
import * as yaml from 'js-yaml';
import { Container } from './Container.js';

/** @typedef {{ guid: string; filePath: string; }} FileMeta */

export class Project {
	/** @type {string} */
	path;

	/** @type {Map<string, FileMeta>} */
	#guidCache = new Map();

	/** @type {Map<string, FileMeta>} */
	#fileMetaCache = new Map();

	constructor(path) {
		this.path = path;
	}

	/**
	 * Loads a YAML file from Unity into a JS object
	 * @param {string} filePath path relative to the project
	 * @returns {Promise<import('./types').UnityYaml>}
	 */
	async #loadUnityYaml(filePath) {
		// console.debug(`Loading YAML for ${filePath}`);

		/**
		 * Credits: https://github.com/nodeca/js-yaml/issues/100#issuecomment-752067249
		 */
		const types = {};

		let file = await fs.readFile(`${this.path}/${filePath}`, 'utf8');

		// remove the unity tag line
		file = file.replace(/%TAG.+\r?\n?/, '');

		// replace each subsequent tag with the full line + map any types
		file = file.replace(/!u!([0-9]+).+/g, (match, p1) => {

			// create our mapping for this type
			if (!(p1 in types)) {
				const type = new yaml.Type(`tag:unity3d.com,2011:${p1}`, {
					kind: 'mapping',
					construct: function (data) {
						return data || {}; // in case of empty node
					},
					instanceOf: Object
				});
				types[p1] = type;
			}

			return `!<tag:unity3d.com,2011:${p1}>`;
		});

		// create our schema
		const schema = yaml.DEFAULT_SCHEMA.extend(Object.values(types));

		// parse our yaml
		const objAr = yaml.loadAll(file, null, { schema });

		// objAr will be something like:
		// [
		//     {
		//         "GameObject": {
		//             ...
		//         }
		//     },
		//     {
		//         "Transform": {
		//             ...
		//         }
		//     },
		//     ...
		// ]
		// console.debug(`Loaded YAML for ${filePath}`);
		return objAr;
	}

	/**
	 * 
	 * @param {string} mainFilePath 
	 * @returns {Promise<import('./types').FileMeta>}
	 */
	async loadMeta(mainFilePath) {
		// console.debug(`Loading meta for ${mainFilePath}`);

		const metaFile = await this.#loadUnityYaml(`ExportedProject/${mainFilePath}.meta`);
		/** @type {import('./types').UnityMeta} */
		const metadata = metaFile[0];

		assert(metadata.guid, `No GUID found in meta file ${mainFilePath}.meta`);

		const fileMeta = { guid: metadata.guid, filePath: mainFilePath };
		this.#guidCache.set(metadata.guid, fileMeta);
		this.#fileMetaCache.set(mainFilePath, fileMeta);

		// console.debug(`Loaded meta for ${mainFilePath}`);
		return fileMeta;
	}


	/**
	 * Scans a directory for .meta files, loading them into memory
	 * @param {string} dirPath 
	 * @returns {Promise<import('./types').FileMeta[]>}
	 */
	async scanDirectory(dirPath) {
		console.debug(`Scanning directory ${dirPath}`);

		const files = await fs.readdir(`${this.path}/ExportedProject/${dirPath}`);

		const readPromises = files
			.filter((file) => file.endsWith('.meta'))
			.map(async (file) => {
				const mainFileName = file.substring(0, file.length - 5);
				const filePath = `${dirPath}/${mainFileName}`;

				// Don't re-read files we've already read
				if (this.#fileMetaCache.has(filePath)) {
					return this.#fileMetaCache.get(filePath);
				}

				return this.loadMeta(filePath);
			});
		const metas = await Promise.all(readPromises);

		console.debug(`Scanned ${files.length} files in ${dirPath}`);
		return metas;
	}

	/**
	 * Gets a meta by its GUID
	 * @param {string} guid 
	 * @returns {FileMeta}
	 */
	getMetaByGuid(guid) {
		const meta = this.#guidCache.get(guid);
		assert(meta, `No meta found for GUID ${guid}`);

		return meta;
	}

	async loadContainer(filePath) {
		const file = await this.#loadUnityYaml(`ExportedProject/${filePath}`);
		return new Container(filePath, file);
	}
}
