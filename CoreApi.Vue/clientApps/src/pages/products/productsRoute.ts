// Composables

import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/products',
    component: () => import(/* webpackChunkName: "products" */ '@components/products/contents/products.vue')
  },
  {
    path: '/products/:id',
    component: () => import(/* webpackChunkName: "products" */ '@components/products/contents/product.vue')
  },
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})
export default router;
