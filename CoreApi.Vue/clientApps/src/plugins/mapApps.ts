import { URL, fileURLToPath } from 'node:url'
import path from 'path'
import { loadEnv } from 'vite'
import fs from "fs"

const clientAppsPath = `file:///${path.resolve('project')}`

interface IPage {
  name: string
  entry: string
  template?: string
  temporary?: string
  output: string
}
interface IAppMap {
  devUrl?: string
  mvcMode: boolean
  outputPath: string
  mvcTemplates: string
  mvcViews: string
  apps: IAppConfig[]
}
interface IAppConfig {
  devUrl: string
  name: string
  entry: string
  output: string,
  publishPath?: string,
  scssAdditional?: string
}
interface PageOptions {
  dir: string,
  baseRoute: string
}

let appConfig: IAppMap
let entryConfig: IPage[] = []
let dirConfig: PageOptions[] = []

const initialConfig = function (mode: ConfigEnv): void
{
  const env = loadEnv(mode.mode, process.cwd(), '')

  var data = fs.readFileSync(`./app.config.${mode.mode}.json`).toString();
    
  var keys = Object.keys(env);

  keys.forEach(k => {
    data = data.replaceAll(`{{${k}}}`, env[k]);
  });

  appConfig = JSON.parse(data)
  entryConfig = []
  dirConfig = []
  appConfig.apps.forEach(f => {
    const sFileInfo = path.parse(f.entry)
    const sPathInfo = path.parse(sFileInfo.dir)
    const template = fileURLToPath(new URL(`./templates/${sPathInfo.name}/${sFileInfo.name}.html`, clientAppsPath))
    entryConfig.push({
      name: f.name,
      entry: fileURLToPath(new URL(f.entry, clientAppsPath)),
      output: fileURLToPath(new URL(f.output, clientAppsPath)),
      template
    })

    const dirOption = {
      dir: `src/components/${f.name}/contents`,
      baseRoute: `${f.name}`,
    }

    dirConfig.push(dirOption)

    if (!appConfig.mvcMode && appConfig.devUrl === '' && !f.devUrl)
      appConfig.devUrl = f.devUrl

    //console.log('appConfig', appConfig)
    //console.log('dirConfig', dirConfig)
    //console.log('entryConfig', entryConfig)
  })
}

export { initialConfig, appConfig, entryConfig, dirConfig, type IAppMap, type IAppConfig, type IPage }
