// Utilities
import { defineStore } from 'pinia'
import { useCookies } from 'vue3-cookies'
import axios from 'axios'

const cookies = useCookies()
const $http = axios.create({})
export const useAppStore = defineStore('vuexyStore', {
  state: () => ({
    count: 0,
  }),
  getters: {},
  actions: {
    async login(uid: string, pswd: string): Promise<void> {
      return new Promise((resolve, reject) => {
        $http.post('/', { uid, pswd })
          .then(r => {
            if (!r.data)
              reject()
            //cookies.set('ChicTripDashboardToken', r.data.token)
            resolve()
          })
          .catch(e => {
            reject()
          })
      })
    },
  },
})
