import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: "https://localhost:7041", // tu ASP.NET
        changeOrigin: true,
        secure: false, // cert de desarrollo
      },
    },
  },
});