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
          <button type="submit" class="btn btn--submit btn--login">Iniciar sesión</button>
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
import { reactive } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
import AppFooter from '@/components/AppFooter.vue'

const router = useRouter()
const form = reactive({ email: '', password: '' })

function handleLogin() {
  router.push('/home')
}
</script>