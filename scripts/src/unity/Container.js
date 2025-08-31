import assert from "assert";

/**
 * A container represents a structure that holds data (usually from a Unity YAML)
 * For example: scenes, prefabs, etc. which contains several objects,
 * like MonoBehaviours, Transforms, etc.
 */
export class Container {
	/** @type {string} */
	filePath;

	/** @type {import('./types').UnityYaml} */
	data;

	constructor(filePath, data) {
		this.filePath = filePath;
		this.data = data;
	}

	/**
	 * Finds a MonoBehaviour by its script GUID
	 * @param {string} guid 
	 * @returns {import('./types').MonoBehaviour}
	 */
	getMonoBehaviourByGuid(guid) {
		const mb = this.data
			.find((obj) => obj?.MonoBehaviour?.m_Script?.guid === guid)
			?.MonoBehaviour;

		assert(mb, `MonoBehaviour with GUID ${guid} not found in scene ${this.filePath}`);
		return mb;
	}
}
