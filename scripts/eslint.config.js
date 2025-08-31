import globals from "globals";
import js from "@eslint/js";
import stylistic from '@stylistic/eslint-plugin'

export default [
	{
		languageOptions: {
			ecmaVersion: 2022,
			sourceType: "module",
			globals: globals.node,
		},
	},
	js.configs.recommended,
	{
		plugins: {
			'@stylistic': stylistic,
		},
		rules: {
			'@stylistic/indent': ['error', 'tab'],
			'@stylistic/linebreak-style': ['error', 'unix'],
			'@stylistic/semi': ['error'],
			'@stylistic/space-infix-ops': ['error'],
		},
	},
];
