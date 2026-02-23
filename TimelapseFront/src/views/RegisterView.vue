<template>
  <div class="page">
    <AppHeader variant="public" logo-link="/" />

    <main class="page__main">
      <section class="card card--form">
        <h1 class="card__title card__title--main">CREAR CUENTA</h1>
        <p class="card__subtitle card__subtitle--main">
          Únete a Timelapse y comienza a guardar tus recuerdos.
        </p>

        <form class="form" @submit.prevent="handleRegister">
          <div class="form__group">
            <label for="nombre" class="form__label">Nombre completo:</label>
            <input v-model="form.nombre" type="text" id="nombre" class="form__input" placeholder="Ej: Juan Pérez" required />
          </div>
          <div class="form__group">
            <label for="email" class="form__label">Correo electrónico:</label>
            <input v-model="form.email" type="email" id="email" class="form__input" placeholder="ejemplo@correo.com" required />
          </div>
          <div class="form__group">
            <label for="password" class="form__label">Contraseña:</label>
            <input v-model="form.password" type="password" id="password" class="form__input" placeholder="Mínimo 8 caracteres" required />
          </div>
          <div class="form__group">
            <label for="confirm-password" class="form__label">Confirmar contraseña:</label>
            <input v-model="form.confirmPassword" type="password" id="confirm-password" class="form__input" placeholder="Repite tu contraseña" required />
          </div>
          <div class="form__group">
            <label for="fecha-nac" class="form__label">Fecha de nacimiento:</label>
            <input v-model="form.fechaNac" type="date" id="fecha-nac" class="form__input" required />
          </div>

          <hr class="form__separator" />

          <button type="submit" class="btn btn--submit">Registrarse</button>
          <RouterLink to="/" class="btn btn--cancel">Cancelar</RouterLink>
        </form>

        <p class="welcome__text" style="margin-top: 20px">
          ¿Ya tienes cuenta?
          <RouterLink to="/iniciar-sesion" class="link">Inicia sesión</RouterLink>
        </p>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'

const router = useRouter()
const form = reactive({ nombre: '', email: '', password: '', confirmPassword: '', fechaNac: '' })

function handleRegister() {
  router.push('/home')
}
</script>