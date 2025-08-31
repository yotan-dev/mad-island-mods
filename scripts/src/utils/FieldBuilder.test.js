import { describe, it, expect } from '@jest/globals';
import { FieldBuilder } from './FieldBuilder.js';

describe('FieldBuilder', () => {
	it('should build int const field with name and value', () => {
		expect(FieldBuilder.intConst('Test', 0).build()).toBe('public const int Test = 0;');
	});

	it('should build int const field with name, value and comment', () => {
		expect(
			FieldBuilder
				.intConst('Test3', 2)
				.setComment('This is a test')
				.build()
		).toBe('public const int Test3 = 2; // This is a test');
	});

	it('should build int const field with name, value and attribute', () => {
		expect(
			FieldBuilder
				.intConst('Test4', 3)
				.addAttribute('[Obsolete]')
				.build()
		).toBe('[Obsolete]\npublic const int Test4 = 3;');
	});

	it('should build int const field with name, value and multiple attributes', () => {
		expect(
			FieldBuilder
				.intConst('Test5', 4)
				.addAttribute('[Obsolete]')
				.addAttribute('[StrVal("Lalala")]')
				.build()
		).toBe('[Obsolete]\n[StrVal("Lalala")]\npublic const int Test5 = 4;');
	});
});
