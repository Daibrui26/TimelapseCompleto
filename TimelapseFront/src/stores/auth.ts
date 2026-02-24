import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface UsuarioSesion {
  idUsuario: number
  nombre: string
  email: string
}

export const useAuthStore = defineStore('auth', () => {
  const usuario = ref<UsuarioSesion | null>(null)

  const isLoggedIn = computed(() => usuario.value !== null)
  const nombreUsuario = computed(() => usuario.value?.nombre ?? '')

  function setUsuario(data: UsuarioSesion) {
    usuario.value = data
  }

  function logout() {
    usuario.value = null
  }

  return { usuario, isLoggedIn, nombreUsuario, setUsuario, logout }
})