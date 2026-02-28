<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main page__main--capsulas">
      <p v-if="capsulaStore.loading" class="capsulas-list__msg">Cargando cápsulas...</p>
      <p v-else-if="capsulaStore.error" class="capsulas-list__msg">{{ capsulaStore.error }}</p>

      <div v-else-if="capsulaStore.total === 0" class="capsulas-empty">
        <p class="capsulas-empty__text">Aún no tienes ninguna cápsula del tiempo.</p>
        <p class="capsulas-empty__subtext">¡Crea tu primera cápsula y empieza a guardar recuerdos!</p>
        <RouterLink to="/crear-capsula" class="btn btn--submit capsulas-empty__btn">
          + Crear mi primera cápsula
        </RouterLink>
      </div>

      <section v-else class="capsulas-list">
        <CapsuleItem
          v-for="capsula in capsulaStore.capsulas"
          :key="capsula.idCapsula"
          :title="capsula.titulo"
          :date="capsula.fechaApertura"
          :to="`/capsula/${capsula.idCapsula}`"
          :emoji="getEmoji(capsula.idCapsula)"
        />
      </section>
    </main>

    <BottomNav />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
import BottomNav from '@/components/BottomNav.vue'
import CapsuleItem from '@/components/CapsuleItem.vue'
import { useAuthStore } from '@/stores/auth'
import { useCapsulaStore } from '@/stores/capsulas'

const authStore    = useAuthStore()
const capsulaStore = useCapsulaStore()

onMounted(async () => {
  if (authStore.usuario?.idUsuario) {
    await capsulaStore.fetchByUsuario(authStore.usuario.idUsuario)
  }
})

function getEmoji(id: number): string {
  try {
    const guardados = JSON.parse(localStorage.getItem('capsula_emojis') || '{}')
    return guardados[id] ?? '⏳'
  } catch {
    return '⏳'
  }
}
</script>