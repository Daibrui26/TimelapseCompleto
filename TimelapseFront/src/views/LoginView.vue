<template>
  <div class="page">
    <AppHeader variant="public" logo-link="/" />

    <main class="page__main page__main--login">
      <section class="card card--login">
        <h1 class="login__title">BIENVENIDO A<br />TIMELAPSE</h1>
        <p class="login__subtitle">Inicia sesión para acceder a tus cápsulas del tiempo</p>

        <form class="form form--login" @submit.prevent="handleLogin">
          <div class="form__group">
            <label for="email" class="form__label">Correo electrónico</label>
            <input v-model="form.email" type="email" id="email" class="form__input" placeholder="tu@email.com" required />
          </div>
          <div class="form__group">
            <label for="password" class="form__label">Contraseña</label>
            <input v-model="form.password" type="password" id="password" class="form__input" placeholder="••••••••" required />
          </div>
          <a href="#" class="login__forgot">¿Olvidaste tu contraseña?</a>
          <p v-if="error" class="form__error">{{ error }}</p>
          <button type="submit" class="btn btn--submit btn--login" :disabled="loading">
           {{ loading ? 'Entrando...' : 'Iniciar sesión' }}
          </button>
        </form>

        <div class="login__divider">
          <span class="login__divider-text">o</span>
        </div>

        <p class="login__register">
          ¿No tienes cuenta?
          <RouterLink to="/registro" class="link">Regístrate aquí</RouterLink>
        </p>
      </section>
    </main>

    <AppFooter />
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
import AppFooter from '@/components/AppFooter.vue'
import { useAuthStore } from '@/stores/auth'
import { authService } from '@/services/authservice'

const router = useRouter()
const authStore = useAuthStore()

const form = reactive({ email: '', password: '' })
const error = ref('')
const loading = ref(false)

async function handleLogin() {
  error.value = ''
  loading.value = true

  try {
    const response = await authService.login({
      email: form.email,
      contraseña: form.password   // tu form usa "password", la API espera "contraseña"
    })

    authStore.setUsuario({
      idUsuario: response.idUsuario,
      nombre: response.nombre,
      email: response.email
    })

    router.push('/home')
  } catch (err: unknown) {
    error.value = err instanceof Error ? err.message : 'Email o contraseña incorrectos'
  } finally {
    loading.value = false
  }
}
</script>