import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { viteStaticCopy } from 'vite-plugin-static-copy'

export default defineConfig({
  plugins: [
    vue(),
    viteStaticCopy({
      targets: [
        {
          src: 'src/assets/**/*', // 复制所有资源文件
          dest: 'assets'          // 目标目录
        }
      ]
    })
  ],
  server: {
    host: '0.0.0.0', // 允许外部访问
    port: 3000,      // 端口号
    open: true,      // 自动打开浏览器
    cors: true,      // 启用CORS
    allowedHosts: [
      'mpvk8690901.vicp.fun',  // 花生壳域名
      'localhost',              // 本地访问
      '.vicp.fun'              // 允许所有vicp.fun子域名
    ],
    proxy: {
      '/api': {
        target: 'http://mpvk8690901.vicp.fun:12575', // 代理到您的后端地址
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api/, '/api') // 保持路径不变
      }
    }
  }
})