import path from 'node:path'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const API_TARGET = 'http://localhost:5167'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@/app': path.resolve(__dirname, './src/app'),
      '@/features': path.resolve(__dirname, './src/features'),
      '@/shared': path.resolve(__dirname, './src/shared'),
      '@/api': path.resolve(__dirname, './src/api'),
    },
  },
  server: {
    proxy: {
      '/api': {
        target: API_TARGET,
        changeOrigin: true,
      },
    },
  },
})
