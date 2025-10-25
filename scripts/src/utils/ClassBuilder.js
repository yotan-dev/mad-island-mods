import { FieldBuilder } from "./FieldBuilder.js";

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
		this.fields.push(FieldBuilder.intConst(name, value).setComment(comment).build());
		return this;
	}

	addStringConstField(name, value, comment = '') {
		this.fields.push(FieldBuilder.stringConst(name, value).setComment(comment).build());
		return this;
	}

	addRawField(field) {
		this.fields.push(field);
		return this;
	}

	/**
	 * 
	 * @returns {string}
	 */
	build() {
		const fieldList = this.fields
			.map((fld) => {
				return fld.split('\n')
					.map((line) => `\t\t${line}`)
					.join('\n');
			})
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
