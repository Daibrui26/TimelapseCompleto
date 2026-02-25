<template>
  <div class="capsula-item-wrapper">
    <div
      class="capsula-item"
      :class="{ 'capsula-item--shake': shaking }"
      @click="handleClick"
    >
      <span class="capsula-item__star">{{ emoji }}</span>
      <div class="capsula-item__content">
        <h3 class="capsula-item__title">{{ title }}</h3>
        <p class="capsula-item__date">{{ fechaFormateada }}</p>
      </div>
      <span v-if="bloqueada" class="capsula-item__lock">🔒</span>
    </div>

    <!-- Mensaje de bloqueada -->
    <div v-if="mostrarMensaje" class="capsula-item__locked-msg">
      <span class="capsula-item__locked-msg-icon">⏳</span>
      <span class="capsula-item__locked-msg-text">Esta cápsula se abrirá el</span>
      <span class="capsula-item__locked-msg-date">{{ fechaAperturaFormateada }}</span>
      <span class="capsula-item__locked-msg-text">— faltan {{ tiempoRestante }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

interface Props {
  title: string
  date: string
  to?: string
  emoji?: string
}

const props = withDefaults(defineProps<Props>(), {
  to: '#',
  emoji: '⏳'
})

const router = useRouter()
const shaking = ref(false)
const mostrarMensaje = ref(false)

// ── ¿Está bloqueada? ──────────────────────────────────────────────────────────
const bloqueada = computed(() => {
  if (!props.date) return false
  return new Date(props.date) > new Date()
})

// ── Fechas formateadas ────────────────────────────────────────────────────────
const fechaFormateada = computed(() => {
  if (!props.date) return ''
  return new Date(props.date).toLocaleDateString('es-ES', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
})

const fechaAperturaFormateada = computed(() => {
  if (!props.date) return ''
  return new Date(props.date).toLocaleDateString('es-ES', {
    day: '2-digit',
    month: 'long',
    year: 'numeric'
  })
})

// ── Tiempo restante ───────────────────────────────────────────────────────────
const tiempoRestante = computed(() => {
  if (!props.date) return ''
  const diff = new Date(props.date).getTime() - Date.now()
  if (diff <= 0) return ''

  const dias = Math.floor(diff / (1000 * 60 * 60 * 24))
  const años = Math.floor(dias / 365)
  const meses = Math.floor((dias % 365) / 30)
  const diasRestantes = dias % 30

  if (años >= 1) {
    return meses > 0
      ? `${años} año${años > 1 ? 's' : ''} y ${meses} mes${meses > 1 ? 'es' : ''}`
      : `${años} año${años > 1 ? 's' : ''}`
  }
  if (meses >= 1) {
    return diasRestantes > 0
      ? `${meses} mes${meses > 1 ? 'es' : ''} y ${diasRestantes} día${diasRestantes !== 1 ? 's' : ''}`
      : `${meses} mes${meses > 1 ? 'es' : ''}`
  }
  return `${dias} día${dias !== 1 ? 's' : ''}`
})

// ── Click handler ─────────────────────────────────────────────────────────────
function handleClick() {
  if (!bloqueada.value) {
    router.push(props.to)
    return
  }

  shaking.value = true
  mostrarMensaje.value = true

  setTimeout(() => { shaking.value = false }, 600)
  setTimeout(() => { mostrarMensaje.value = false }, 4000)
}
</script>