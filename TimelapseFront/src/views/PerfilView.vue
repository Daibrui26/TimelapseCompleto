<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main page__main--perfil">
      <section class="card card--perfil">
        <div class="perfil-header">
          <h1 class="perfil-header__title">Perfil</h1>
          <button class="perfil-header__btn" @click="toggleEdit">
            {{ isEditing ? 'Guardar' : 'Editar' }}
          </button>
        </div>

        <div class="perfil-user">
          <div class="perfil-user__avatar">
            <img src="@/assets/img/Perfil.png" alt="Usuario" class="perfil-user__img" />
          </div>
          <h2 class="perfil-user__name">{{ perfil.nombre || 'Usuario' }}</h2>
          <p class="perfil-user__email">{{ perfil.email || 'correousuario@gmail.com' }}</p>
        </div>

        <div class="perfil-form">
          <div class="perfil-form__group">
            <label class="perfil-form__label">Nombre</label>
            <input v-model="perfil.nombre" type="text" class="perfil-form__input" :disabled="!isEditing" />
          </div>
          <div class="perfil-form__group">
            <label class="perfil-form__label">Correo</label>
            <input v-model="perfil.email" type="email" class="perfil-form__input" :disabled="!isEditing" />
          </div>
          <div class="perfil-form__group">
            <label class="perfil-form__label">Contraseña</label>
            <input v-model="perfil.password" type="password" class="perfil-form__input" :disabled="!isEditing" />
          </div>
          <div class="perfil-form__group">
            <label class="perfil-form__label">Fecha Nac.</label>
            <input v-model="perfil.fechaNac" type="date" class="perfil-form__input" :disabled="!isEditing" />
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import AppHeader from '@/components/AppHeader.vue'

const isEditing = ref(false)
const perfil = reactive({ nombre: '', email: '', password: '', fechaNac: '' })

function toggleEdit() {
  isEditing.value = !isEditing.value
}
</script>