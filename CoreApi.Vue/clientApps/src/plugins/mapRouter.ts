import type { App } from 'vue'
import moment from 'moment'
import { useCookies } from 'vue3-cookies'
import axios from 'axios'

import { loadFonts } from './webfontloader'
import vuetify from './vuetify'
import i18n from '@/i18n'

const { cookies } = useCookies()


export function registerPlugins(app: App, router: any, store: any) {
  loadFonts()
  app
    .use(vuetify)
    .use(router)
    .use(store)
    .use(i18n)

  app.config.globalProperties.$http = axios
  app.config.globalProperties.$moment = moment
  app.config.globalProperties.$cookies = cookies

}
