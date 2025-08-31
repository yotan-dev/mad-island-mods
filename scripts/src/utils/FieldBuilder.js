export class FieldBuilder {
	/** @type {string} */
	name;
	
	/** @type {number} */
	value;
	
	/** @type {string} */
	comment;

	/** @type {string[]} */
	attributes = [];

	constructor(name, value) {
		this.name = name;
		this.value = value;
		this.comment = '';
	}

	static intConst(name, value) {
		return new FieldBuilder(name, value);
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
		
		return `${attributesPart}public const int ${this.name} = ${this.value};${commentPart}`;
	}
}
