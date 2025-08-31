import assert from "assert";

export class Scene {
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
