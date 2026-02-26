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
          <div class="perfil-form__group" v-if="isEditing">
            <label class="perfil-form__label">Repetir Contraseña</label>
            <input v-model="perfil.confirmPassword" type="password" class="perfil-form__input" placeholder="Repite la nueva contraseña" />
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
import { ref, reactive, onMounted } from 'vue'
import AppHeader from '@/components/AppHeader.vue'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/services/api'

const authStore = useAuthStore()
const isEditing = ref(false)
const perfil = reactive({ nombre: '', email: '', password: '', confirmPassword: '', fechaNac: '' })

onMounted(() => {
  perfil.nombre = authStore.usuario?.nombre ?? ''
  perfil.email = authStore.usuario?.email ?? ''
})

async function toggleEdit() {
  if (!isEditing.value) {
    isEditing.value = true
    return
  }
  if (perfil.password && perfil.password !== perfil.confirmPassword) {
  alert('Las contraseñas no coinciden.')
  return
}

  try {
    // Si no ha escrito nueva contraseña, recuperamos la actual de la API
    let contraseñaFinal = perfil.password
    if (!contraseñaFinal) {
      const usuarioActual = await api.get<{ contraseña: string }>(`/Usuario/${authStore.usuario?.idUsuario}`)
      contraseñaFinal = usuarioActual.contraseña
    }

    await api.put(`/Usuario/${authStore.usuario?.idUsuario}`, {
      idUsuario: authStore.usuario?.idUsuario,
      nombre: perfil.nombre,
      email: perfil.email,
      contraseña: contraseñaFinal
    })

    authStore.setUsuario({
      idUsuario: authStore.usuario!.idUsuario,
      nombre: perfil.nombre,
      email: perfil.email
    })

    perfil.password = '' // limpiamos el campo tras guardar
    isEditing.value = false
    alert('Perfil actualizado correctamente.')
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Error al guardar los cambios.')
  }
}
</script>