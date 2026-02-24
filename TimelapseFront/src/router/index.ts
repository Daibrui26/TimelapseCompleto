import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { RouteRecordRaw } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
  {
    path: '/',
    name: 'Index',
    component: () => import('@/views/IndexView.vue'),
  },
  {
    path: '/iniciar-sesion',
    name: 'Login',
    component: () => import('@/views/LoginView.vue'),
    meta: { public: true }
  },
  {
    path: '/registro',
    name: 'Register',
    component: () => import('@/views/RegisterView.vue'),
    meta: { public: true }
  },
  {
    path: '/home',
    name: 'Home',
    component: () => import('@/views/HomeView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/menu',
    name: 'Menu',
    component: () => import('@/views/MenuView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/perfil',
    name: 'Perfil',
    component: () => import('@/views/PerfilView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/tus-capsulas',
    name: 'TusCapsulas',
    component: () => import('@/views/TusCapsulaView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/crear-capsula',
    name: 'CrearCapsula',
    component: () => import('@/views/CrearCapsulasView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/contacto',
    name: 'Contacto',
    component: () => import('@/views/ContactoView.vue'),
    meta: { requiresAuth: true }
  }
]
})

router.beforeEach((to) => {
  const authStore = useAuthStore()

  // Ruta protegida y no está logueado → redirigir a login
  if (to.meta.requiresAuth && !authStore.isLoggedIn) {
    return { name: 'login' }
  }

  // Ya está logueado e intenta acceder a login/register → redirigir a home
  if (to.meta.public && authStore.isLoggedIn) {
    return { name: 'home' }
  }
})

export default router