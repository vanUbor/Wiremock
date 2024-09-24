import { defineConfig } from 'vite';

export default defineConfig({
    resolve: {
        alias: {
            '/@modules/': '/node_modules/'
        }
    }
});