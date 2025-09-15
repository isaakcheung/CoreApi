import fs from 'fs'
import { URL, fileURLToPath } from 'node:url'
import path from 'path'

import type { IAppMap, IPage } from '@plugins/mapApps'

const data = require('../../app.config.json')

const appConfig: IAppMap = JSON.parse(JSON.stringify(data))

const name = 'mvc-interge-plugin'

const exProcess = (e: any) => {
  if (e)
    console.log(e)
}

const projectPath = `file:///${path.resolve('project')}`

let isFirstTimeBuild = true

const intergeMvc = (pages: IPage[]) => {
  pages.forEach(f => {
    const sFileInfo = path.parse(f.entry)
    const sPathInfo = path.parse(sFileInfo.dir)
    const oFileInfo = path.parse(f.output)
    const oPathInfo = path.parse(oFileInfo.dir)

    f.template = fileURLToPath(new URL(`./templates/${sPathInfo.name}/${sFileInfo.name}.html`, projectPath))
    f.temporary = fileURLToPath(new URL(`${appConfig.outputPath}/templates/${sPathInfo.name}/${sFileInfo.name}.html`, projectPath))

    fs.mkdir(oFileInfo.dir, { recursive: true }, exProcess)
    fs.mkdir(path.parse(f.template).dir, { recursive: true }, exProcess)
  })

  return ({
    name,
    pages,
    buildStart() {
      if (!isFirstTimeBuild) return;
      isFirstTimeBuild=false;
      pages.forEach(f => {
        const sourceFile: string = f.entry || ''
        const targetFile: string = f.template || ''

        fs.copyFile(sourceFile, targetFile, exProcess)
        console.log(`template => ${targetFile}`)
      })
      console.log('templates copy done')
    },
    closeBundle() {
      console.log('closeBundle')

      // fs.cp(appConfig.outputPath, appConfig.mvcViews, { recursive: true }, exProcess);
      pages.forEach(f => {
        const oFileInfo = path.parse(f.output)

        console.log(`view => ${f.output}`)
        fs.copyFile(f.temporary || '', f.output, exProcess)
      })
      console.log('output copy done')
    },
  })
}

const inputParse = function (pages: IPage[]) {
  const result: any = {}

  pages.forEach(f => {
    result[f.name] = f.template
  })

  return result
}

export { intergeMvc, inputParse, appConfig }
