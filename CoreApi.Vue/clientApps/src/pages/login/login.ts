import App from '@pages/login/login.vue'
import { VDataTableServer } from 'vuetify/labs/VDataTable'
import { createApp } from 'vue'

import { registerPlugins } from '@plugins/mapRouter'
import { createPinia } from 'pinia'
import { createPersistedState } from 'pinia-plugin-persistedstate'

import VueDatePicker from '@vuepic/vue-datepicker'
import '@vuepic/vue-datepicker/dist/main.css'


import loginRoute from '@pages/login/loginRoute'


const app = createApp(App)
const router = loginRoute
const pinia = createPinia()
pinia.use(createPersistedState())

registerPlugins(app, router, pinia)

app.component(
  'VDataTableServer', VDataTableServer,
)

app.component('VueDatePicker', VueDatePicker)

app.mount('#app')
