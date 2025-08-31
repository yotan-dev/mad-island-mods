export class ClassBuilder {
	/** @type {string} */
	namespace;
	
	/** @type {string} */
	className;

	/** @type {string[]} */
	fields = [];

	constructor(namespace, className) {
		this.namespace = namespace;
		this.className = className;
	}

	addIntConstField(name, value, comment = '') {
		let commentPart = comment ? ` // ${comment}` : '';
		this.fields.push(`public const int ${name} = ${value};${commentPart}`);
		return this;
	}

	/**
	 * 
	 * @returns {string}
	 */
	build() {
		const fieldList = this.fields
			.map((fld) => `\t\t${fld}`)
			.join('\n\n');

		return `// Automatically generated - Do NOT edit.
namespace ${this.namespace}
{
	public static class ${this.className}
	{
${fieldList}
	}
}
`;
	}
}
