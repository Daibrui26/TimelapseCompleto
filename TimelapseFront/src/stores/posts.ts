import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import {
  getPosts,
  getPostsByUsuario,
  crearPost,
  eliminarPost,
  type Post
} from '@/services/postService'

export const usePostStore = defineStore('posts', () => {

  // ── Estado ─────────────────────────────────────────────────────────────────
  const postsFeed = ref<Post[]>([])
  const postsMios = ref<Post[]>([])
  const loading   = ref(false)
  const error     = ref('')

  // ── Computed ───────────────────────────────────────────────────────────────
  const totalFeed = computed(() => postsFeed.value.length)
  const totalMios = computed(() => postsMios.value.length)

  const soloPublicos = computed(() =>
    postsFeed.value.filter(p => p.visibilidad === 'publica')
  )

  // ── Acciones ───────────────────────────────────────────────────────────────

  async function fetchFeed() {
    loading.value = true
    error.value   = ''
    try {
      postsFeed.value = await getPosts()
    } catch (e: any) {
      error.value = e?.message || 'Error al cargar el feed.'
    } finally {
      loading.value = false
    }
  }

  async function fetchMios(idUsuario: number) {
    loading.value = true
    error.value   = ''
    try {
      postsMios.value = await getPostsByUsuario(idUsuario)
    } catch (e: any) {
      error.value = e?.message || 'Error al cargar tus posts.'
    } finally {
      loading.value = false
    }
  }

  async function fetchTodos(idUsuario: number) {
    loading.value = true
    error.value   = ''
    try {
      const [feed, mios] = await Promise.all([
        getPosts(),
        getPostsByUsuario(idUsuario)
      ])
      postsFeed.value = feed
      postsMios.value = mios
    } catch (e: any) {
      error.value = e?.message || 'Error al cargar los posts.'
    } finally {
      loading.value = false
    }
  }

  async function publicar(
    idUsuario: number,
    nombre: string,
    texto: string,
    visibilidad: string,
    archivo?: File
  ) {
    const nuevo = await crearPost(idUsuario, texto, visibilidad, archivo)
    nuevo.nombreUsuario = nombre

    postsMios.value.unshift(nuevo)
    if (visibilidad === 'publica') postsFeed.value.unshift(nuevo)
  }

  async function eliminar(idPost: number, idUsuario: number) {
    await eliminarPost(idPost, idUsuario)
    postsFeed.value = postsFeed.value.filter(p => p.idPost !== idPost)
    postsMios.value = postsMios.value.filter(p => p.idPost !== idPost)
  }

  function reset() {
    postsFeed.value = []
    postsMios.value = []
    error.value     = ''
  }

  return {
    postsFeed, postsMios, loading, error,
    totalFeed, totalMios, soloPublicos,
    fetchFeed, fetchMios, fetchTodos, publicar, eliminar, reset
  }
})