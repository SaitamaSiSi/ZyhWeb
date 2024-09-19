export default {
  env: {
    browser: true,
    es2021: true,
    node: true,
  },
  extends: ['plugin:vue/vue3-essential', 'standard-with-typescript'],
  overrides: [],
  parser: 'vue-eslint-parser', // 新增
  parserOptions: {
    ecmaVersion: 'latest',
    extraFileExtensions: ['.vue'],
    parser: '@typescript-eslint/parser', // 新增
    project: ['tsconfig.json'],
    sourceType: 'module',
  },
  plugins: ['vue'],
  rules: {},
};
