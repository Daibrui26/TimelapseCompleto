<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main page__main--explorar">

      <!-- Tabs -->
      <div class="explorar-tabs">
        <button
          class="explorar-tabs__btn"
          :class="{ 'explorar-tabs__btn--active': tabActiva === 'feed' }"
          @click="tabActiva = 'feed'"
        >
          🌍 Feed público
        </button>
        <button
          class="explorar-tabs__btn"
          :class="{ 'explorar-tabs__btn--active': tabActiva === 'mios' }"
          @click="tabActiva = 'mios'"
        >
          👤 Mis posts
        </button>
      </div>

      <!-- Composer: solo visible en "mis posts" o siempre -->
      <div class="post-composer">
        <div class="post-composer__header">
          <img
            src="@/assets/img/Perfil.png"
            alt="Avatar"
            class="post-composer__avatar"
          />
          <textarea
            v-model="nuevoTexto"
            class="post-composer__textarea"
            placeholder="Comparte un aprendizaje o vivencia..."
            maxlength="500"
          />
        </div>

        <div class="post-composer__footer">
          <div class="post-composer__actions-left">
            <!-- Adjuntar imagen -->
            <button
              type="button"
              class="post-composer__file-btn"
              @click="fileInputRef?.click()"
            >
              📎 Imagen
            </button>
            <input
              ref="fileInputRef"
              type="file"
              accept="image/*"
              class="post-composer__file-input"
              @change="onFileChange"
            />

            <!-- Preview archivo -->
            <div v-if="archivoSeleccionado" class="post-composer__file-preview">
              🖼️ {{ archivoSeleccionado.name }}
              <button
                type="button"
                class="post-composer__file-remove"
                @click="archivoSeleccionado = null"
              >×</button>
            </div>
          </div>

          <div class="post-composer__visibility">
            <button
              type="button"
              class="post-composer__vis-btn"
              :class="{ 'post-composer__vis-btn--active': visibilidad === 'publica' }"
              @click="visibilidad = 'publica'"
            >
              🌍 Público
            </button>
            <button
              type="button"
              class="post-composer__vis-btn"
              :class="{ 'post-composer__vis-btn--active': visibilidad === 'privada' }"
              @click="visibilidad = 'privada'"
            >
              🔒 Privado
            </button>
          </div>

          <button
            class="post-composer__submit"
            :disabled="!nuevoTexto.trim() || enviando"
            @click="publicarPost"
          >
            {{ enviando ? 'Publicando...' : 'Publicar' }}
          </button>
        </div>
      </div>

      <!-- Feed -->
      <div class="post-feed">
        <p v-if="loading" class="post-feed__loading">Cargando posts...</p>
        <p v-else-if="postsMostrados.length === 0" class="post-feed__empty">
          {{ tabActiva === 'mios' ? 'Aún no has publicado nada.' : 'No hay posts públicos todavía.' }}
        </p>

        <PostCard
          v-for="post in postsMostrados"
          :key="post.idPost"
          :post="post"
          @deleted="onPostDeleted"
        />
      </div>
    </main>

    <BottomNav />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import AppHeader from '@/components/AppHeader.vue'
import BottomNav from '@/components/BottomNav.vue'
import PostCard from '@/components/PostCard.vue'
import { useAuthStore } from '@/stores/auth'
import { useToast } from '@/composables/useToast'
import {
  getPosts,
  getPostsByUsuario,
  crearPost,
  type Post
} from '@/services/postService'

const authStore = useAuthStore()
const toast     = useToast()

const tabActiva  = ref<'feed' | 'mios'>('feed')
const nuevoTexto = ref('')
const visibilidad = ref<'publica' | 'privada'>('publica')
const enviando   = ref(false)
const loading    = ref(true)

const fileInputRef       = ref<HTMLInputElement | null>(null)
const archivoSeleccionado = ref<File | null>(null)

const postsFeed  = ref<Post[]>([])
const postsMios  = ref<Post[]>([])

const postsMostrados = computed(() =>
  tabActiva.value === 'feed' ? postsFeed.value : postsMios.value
)

onMounted(async () => {
  await cargarPosts()
})

async function cargarPosts() {
  loading.value = true
  try {
    const [feed, mios] = await Promise.all([
      getPosts(),
      getPostsByUsuario(authStore.usuario!.idUsuario)
    ])
    postsFeed.value = feed
    postsMios.value = mios
  } catch {
    toast.error('Error al cargar el feed.')
  } finally {
    loading.value = false
  }
}

function onFileChange(e: Event) {
  const input = e.target as HTMLInputElement
  archivoSeleccionado.value = input.files?.[0] ?? null
}

async function publicarPost() {
  if (!nuevoTexto.value.trim()) return
  enviando.value = true
  try {
    const nuevo = await crearPost(
      authStore.usuario!.idUsuario,
      nuevoTexto.value.trim(),
      visibilidad.value,
      archivoSeleccionado.value ?? undefined
    )

    // Añadir nombre de usuario manualmente si el backend no lo devuelve en el create
    nuevo.nombreUsuario = authStore.usuario!.nombre

    // Insertar al inicio
    postsMios.value.unshift(nuevo)
    if (visibilidad.value === 'publica') postsFeed.value.unshift(nuevo)

    nuevoTexto.value      = ''
    archivoSeleccionado.value = null
    if (fileInputRef.value) fileInputRef.value.value = ''

    toast.success('¡Post publicado!')
  } catch {
    toast.error('Error al publicar el post.')
  } finally {
    enviando.value = false
  }
}

function onPostDeleted(idPost: number) {
  postsFeed.value = postsFeed.value.filter(p => p.idPost !== idPost)
  postsMios.value = postsMios.value.filter(p => p.idPost !== idPost)
}
</script>