// Plugins
import { URL, fileURLToPath } from 'node:url'
import vue from '@vitejs/plugin-vue'
import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify'
import { defineConfig, loadEnv } from 'vite'
import { appConfig, dirConfig, entryConfig, initialConfig } from './src/plugins/mapApps'
import { inputParse, intergeMvc } from './src/plugins/integrate'
import { createSvgIconsPlugin } from 'vite-plugin-svg-icons'
import path from 'path'

// Utilities

let isTarget = false
let mpaConfig = null
let scssAdditional = ``;

// https://vitejs.dev/config/
export default defineConfig((mode) => {

  initialConfig(mode);

  appConfig.apps.forEach(app => {
    if (app.scssAdditional) {
      if (!app.scssAdditional.trim().endsWith("?"))
        app.scssAdditional = `${app.scssAdditional};`;
      scssAdditional += app.scssAdditional;
    }
  });
  console.log("root",path.resolve(__dirname, '../'));
  //process.env = { ...process.env, ...loadEnv(mode, process.cwd()) };
  return {
    base: `${appConfig.mvcMode ? '' : `/${appConfig.outputPath}/`}`,
    //base:`/`,
    plugins: [
      vue({
        template: { transformAssetUrls },
      }),
      // https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vite-plugin
      vuetify({
        autoImport: true,
      }),
      intergeMvc(entryConfig),
      createSvgIconsPlugin({
        iconDirs: [path.resolve(process.cwd(), 'src/assets/svg')],
        symbolId: 'icon-[name]',
      }),
    ],
    define: { 'process.env': {} },
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
        '@components': fileURLToPath(new URL('./src/components', import.meta.url)),
        '@pages': fileURLToPath(new URL('./src/pages', import.meta.url)),
        '@assets': fileURLToPath(new URL('./src/assets', import.meta.url)),
        '@plugins': fileURLToPath(new URL('./src/plugins', import.meta.url)),
        '@themeConfig': fileURLToPath(new URL('./themeConfig.ts', import.meta.url)),
      },
      extensions: [
        '.js',
        '.json',
        '.jsx',
        '.mjs',
        '.ts',
        '.tsx',
        '.vue',
      ],
    },
    build: {
      sourcemap: true,
      outDir: `${appConfig.outputPath}/`,
      chunkSizeWarningLimit: 5000,
      assetsDir: 'assets',
      emptyOutDir: true,
      rollupOptions: {
        input: inputParse(entryConfig),
      },
      modulePreload: {
        resolveDependencies: (filename, deps, { hostId, hostType }) => {
          mpaConfig = null

          //�o��O���F�����e�A�sĶ���ɮ׬O�֡A�ӳ]�wmpaConfig�A�o��]�w�����U�ӷ|���W����U����renderBuiltUrl�o�Ӥ�k
          appConfig.apps.forEach(f => {
            const appTemp = f.entry.toLowerCase().replace("../", "").replace(".cshtml", "")
            const hostkey = hostId.toLowerCase().replace(".html", "")
            //console.log('hostkey-appTemp', hostkey, appTemp)
            isTarget = appTemp == hostkey
            if (isTarget) mpaConfig = f
          })
          console.log('resolveDependencies-', filename, hostId, hostType)

          return deps
        },
      },
      watch: {  
        ignored: ['./templates/**', '**/Views/**'],
      },
    },
    experimental: {
      //�]�w�귽�ɸ��|
      renderBuiltUrl(filename: string, { hostType }: { hostType: 'js' | 'css' | 'html' }) {
        if (!mpaConfig) return { relative: true }
        if (!mpaConfig.publishPath) return { relative: true }

        return `${mpaConfig.publishPath}${filename}`
      },
    },
    server: {
      port: 3000,
      watch: {  
        ignored: ['./templates/**', '**/Views/**'],
      },
      fs: {
        allow: ['..'] // 允許存取專案上一層目錄
      }
    },
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: `@import "./src/assets/regist/scss/_color.scss"; @import "./src/assets/regist/scss/_variables.scss";${scssAdditional}`,
        },
      }
    }
  };
})
