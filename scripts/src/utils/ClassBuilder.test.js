import { describe, it, expect } from '@jest/globals';
import { ClassBuilder } from './ClassBuilder.js';
import { FieldBuilder } from './FieldBuilder.js';

describe('ClassBuilder', () => {
	it('should build class with namespace, class name and int const fields', () => {
		const cb = new ClassBuilder('YotanModCore.Consts', 'SampleClass');
		cb.addIntConstField('Test', 0, 'This is a test');
		cb.addIntConstField('Test2', 1);
		expect(cb.build()).toMatchSnapshot();
	});

	it('should build class with namespace, class name and int const fields with attributes', () => {
		const cb = new ClassBuilder('YotanModCore.Consts', 'SampleClass');
		cb.addRawField(FieldBuilder.intConst('Test', 0).addAttribute('[Obsolete]').build());
		expect(cb.build()).toMatchSnapshot();
	});
});
