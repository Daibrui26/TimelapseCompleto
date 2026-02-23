<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main">
      <section class="card card--form">
        <form class="form" @submit.prevent="handleSubmit">
          <div class="form__group">
            <label for="nombre" class="form__label">Nombre Cápsula:</label>
            <input v-model="form.nombre" type="text" id="nombre" class="form__input" placeholder="Ej: Verano 2025" required />
          </div>
          <div class="form__group">
            <label for="participantes" class="form__label">Participantes:</label>
            <input v-model="form.participantes" type="text" id="participantes" class="form__input" placeholder="Añade participantes" />
          </div>
          <div class="form__group">
            <label for="recuerdos" class="form__label">Pon tus recuerdos aquí:</label>
            <textarea v-model="form.recuerdos" id="recuerdos" class="form__textarea" placeholder="Escribe tus recuerdos..." required></textarea>
          </div>

          <div class="btn-group btn-group--media">
            <button type="button" class="btn btn--add">
              <span class="btn__icon-add">📷</span> Añadir imagen
            </button>
            <button type="button" class="btn btn--add">
              <span class="btn__icon-add">🎥</span> Añadir vídeo
            </button>
          </div>

          <div class="form__group">
            <label for="fecha" class="form__label">Fecha:</label>
            <input v-model="form.fecha" type="date" id="fecha" class="form__input" required />
          </div>

          <hr class="form__separator" />

          <button type="submit" class="btn btn--submit">Sellar Cápsula</button>
          <RouterLink to="/home" class="btn btn--cancel">Cancelar</RouterLink>
        </form>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'

const router = useRouter()
const form = reactive({ nombre: '', participantes: '', recuerdos: '', fecha: '' })

function handleSubmit() {
  router.push('/home')
}
</script>