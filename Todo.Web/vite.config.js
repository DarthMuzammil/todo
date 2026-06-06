import path from 'node:path'
import tailwindcss from '@tailwindcss/vite'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const API_TARGET = 'http://127.0.0.1:5167'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  test: {
    environment: 'jsdom',
    setupFiles: './vitest.setup.js',
    globals: true,
    css: true,
    exclude: ['**/node_modules/**', '**/e2e/**', '**/dist/**'],
  },
  esbuild: {
    jsx: 'automatic',
  },
  resolve: {
    alias: {
      '@/app': path.resolve(import.meta.dirname, './src/app'),
      '@/features': path.resolve(import.meta.dirname, './src/features'),
      '@/shared': path.resolve(import.meta.dirname, './src/shared'),
      '@/api': path.resolve(import.meta.dirname, './src/api'),
      '@/test': path.resolve(import.meta.dirname, './src/test'),
      '@/lib': path.resolve(import.meta.dirname, './src/lib'),
    },
  },
  server: {
    proxy: {
      '/api': {
        target: API_TARGET,
        changeOrigin: true,
      },
      '/hubs': {
        target: API_TARGET,
        changeOrigin: true,
        ws: true,
      },
    },
  },
})
