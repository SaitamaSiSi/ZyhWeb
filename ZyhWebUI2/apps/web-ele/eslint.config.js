import globals from "globals";
import pluginJs from "@eslint/js";
import tseslint from "typescript-eslint";
import pluginVue from "eslint-plugin-vue";

export default [
  {files: ["**/*.{js,mjs,cjs,ts,vue}"]},
  {languageOptions: { globals: {...globals.browser, ...globals.node} }},
  pluginJs.configs.recommended,
  ...tseslint.configs.recommended,
  ...pluginVue.configs["flat/essential"],
  {
    files: ["**/*.vue", "**/*.ts"], languageOptions: {parserOptions: {parser: tseslint.parser}},
    rules: {
      'vue/multi-word-component-names': 'off',
      '@typescript-eslint/no-unused-expressions': 'off',
      'vue/no-unused-components': 'off',
      '@typescript-eslint/no-unused-vars': 'off',
      '@typescript-eslint/no-namespace': 'off',
      'no-undef': 'error'
    }
  },
];
