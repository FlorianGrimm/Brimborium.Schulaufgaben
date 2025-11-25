import { defineConfig } from 'orval';

export default defineConfig({
  petstore: {
    output: {
      //mode: 'tags-split',
      //mode: 'single',
      mode: 'split',
      namingConvention: 'PascalCase',
      target: 'projects/schulaufgaben/src/lib/api-client.ts',
      schemas: 'projects/schulaufgaben/src/lib/model',
      client: 'zod',
      mock: false,
    },
    input: {
      target: '../../artifacts/obj/SchulaufgabenEditorWeb/SchulaufgabenEditorWeb.json',
    },
  },
});