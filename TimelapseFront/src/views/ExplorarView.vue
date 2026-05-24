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

      <!-- Composer -->
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
        <p v-if="postStore.loading" class="post-feed__loading">Cargando posts...</p>
        <p v-else-if="postsMostrados.length === 0" class="post-feed__empty">
          {{ tabActiva === 'mios' ? 'Aún no has publicado nada.' : 'No hay posts públicos todavía.' }}
        </p>

        <PostCard
          v-for="post in postsMostrados"
          :key="post.idPost"
          :post="post"
          @deleted="postStore.eliminar($event, authStore.usuario!.idUsuario)"
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
import { usePostStore } from '@/stores/posts'
import { useToast } from '@/composables/useToast'

const authStore = useAuthStore()
const postStore = usePostStore()
const toast     = useToast()

const tabActiva   = ref<'feed' | 'mios'>('feed')
const nuevoTexto  = ref('')
const visibilidad = ref<'publica' | 'privada'>('publica')
const enviando    = ref(false)

const fileInputRef        = ref<HTMLInputElement | null>(null)
const archivoSeleccionado = ref<File | null>(null)

const postsMostrados = computed(() =>
  tabActiva.value === 'feed' ? postStore.postsFeed : postStore.postsMios
)

onMounted(async () => {
  await postStore.fetchTodos(authStore.usuario!.idUsuario)
})

function onFileChange(e: Event) {
  const input = e.target as HTMLInputElement
  archivoSeleccionado.value = input.files?.[0] ?? null
}

async function publicarPost() {
  if (!nuevoTexto.value.trim()) return
  enviando.value = true
  try {
    await postStore.publicar(
      authStore.usuario!.idUsuario,
      authStore.usuario!.nombre,
      nuevoTexto.value.trim(),
      visibilidad.value,
      archivoSeleccionado.value ?? undefined
    )
    nuevoTexto.value          = ''
    archivoSeleccionado.value = null
    if (fileInputRef.value) fileInputRef.value.value = ''
    toast.success('¡Post publicado!')
  } catch {
    toast.error('Error al publicar el post.')
  } finally {
    enviando.value = false
  }
}
</script>