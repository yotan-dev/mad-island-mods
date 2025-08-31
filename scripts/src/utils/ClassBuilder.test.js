import { describe, test, expect } from '@jest/globals';
import { ClassBuilder } from './ClassBuilder.js';

describe('ClassBuilder', () => {
	test('build', () => {
		const cb = new ClassBuilder('YotanModCore.Consts', 'SampleClass');
		cb.addIntConstField('Test', 0, 'This is a test');
		cb.addIntConstField('Test2', 1);
		expect(cb.build()).toMatchSnapshot();
	});
});
