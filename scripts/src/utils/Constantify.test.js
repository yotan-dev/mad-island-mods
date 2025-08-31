import { describe, test, expect } from '@jest/globals';

import { Constantify } from './Constantify.js';

describe('Constantify', () => {
	describe('word', () => {
		test.each([
			['test', 'Test'],
			['TEST', 'TEST'],
			['test1', 'Test1'],
		])('%s -> %s', (input, expected) => {
			expect(Constantify.word(input)).toBe(expected);
		});
	});

	describe('text', () => {
		test.each([
			['test-text', 'TestText'],
			['Test-text', 'TestText'],
			['test Text', 'TestText'],
			['1test-text', '_1testText'],
			['test-1-text', 'Test1Text'],
			['ba-ku_test', 'BaKuTest'],
			['Chicken?', 'Chicken'],
			['Az&Bz', 'AzBz'],
		])('%s -> %s', (input, expected) => {
			expect(Constantify.text(input)).toBe(expected);
		});
	});

	describe('fileName', () => {
		test.each([
			['test.ogg', 'Test'],
			['test.mp3.asset', 'Test'],
			['test', 'Test'],
		])('%s -> %s', (input, expected) => {
			expect(Constantify.fileName(input)).toBe(expected);
		});
	});
});
