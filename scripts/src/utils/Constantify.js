export class Constantify {
	/**
	 * 
	 * @param {string} str 
	 * @returns {string}
	 */
	static word(str) {
		return str.charAt(0).toUpperCase() + str.slice(1);
	}

	/**
	 * 
	 * @param {string} str 
	 * @returns {string}
	 */
	static text(str) {
		str = str.replace(/ /g, '_');

		const parts = str.split('_');
		for (let i = 0; i < parts.length; i++) {
			parts[i] = parts[i].split('-').map((subPart) => Constantify.word(subPart)).join('');
		}

		const combined = parts.map((part) => Constantify.word(part)).join('');

		if (/^([0-9])/.test(combined)) {
			return '_' + combined;
		}

		return combined;
	}
}
