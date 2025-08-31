export class FieldBuilder {
	/** @type {string} */
	name;
	
	/** @type {any} */
	value;

	/** @type {string} */
	type;
	
	/** @type {string} */
	comment;

	/** @type {string[]} */
	attributes = [];

	constructor(type, name, value) {
		this.type = type;
		this.name = name;
		this.value = value;
		this.comment = '';
	}

	static intConst(name, value) {
		return new FieldBuilder('int', name, value);
	}

	static stringConst(name, value) {
		return new FieldBuilder('string', name, `"${value}"`);
	}

	setComment(comment) {
		this.comment = comment;
		return this;
	}

	addAttribute(attr) {
		this.attributes.push(attr);
		return this;
	}

	build() {
		let commentPart = this.comment ? ` // ${this.comment}` : '';
		
		let attributesPart = this.attributes.join('\n');
		if (attributesPart) {
			attributesPart = `${attributesPart}\n`;
		}
		
		return `${attributesPart}public const ${this.type} ${this.name} = ${this.value};${commentPart}`;
	}
}
