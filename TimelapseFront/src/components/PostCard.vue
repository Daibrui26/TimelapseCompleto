<template>
  <div class="post-card">

    <!-- Cabecera -->
    <div class="post-card__header">
      <div class="post-card__user">
        <img src="@/assets/img/Perfil.png" alt="Avatar" class="post-card__avatar" />
        <div class="post-card__user-info">
          <span class="post-card__username">{{ post.nombreUsuario }}</span>
          <span class="post-card__date">{{ formatFecha(post.fechaPublicacion) }}</span>
        </div>
      </div>

      <div class="post-card__meta">
        <span class="post-card__badge" :class="`post-card__badge--${post.visibilidad}`">
          {{ post.visibilidad === 'publica' ? '🌍 Público' : '🔒 Privado' }}
        </span>
        <button
          v-if="puedeEliminar"
          class="post-card__delete"
          title="Eliminar post"
          @click="handleDelete"
        >
          ✕
        </button>
      </div>
    </div>

    <!-- Texto -->
    <p class="post-card__body">{{ post.texto }}</p>

    <!-- Imagen adjunta -->
    <img
      v-if="post.urlArchivo"
      :src="post.urlArchivo"
      alt="Imagen del post"
      class="post-card__image"
      @click="lightboxUrl = post.urlArchivo!"
    />

    <!-- Footer -->
    <div class="post-card__footer">
      <button class="post-card__comments-toggle" @click="toggleComentarios">
        💬 {{ comentarios.length || post.totalComentarios }} comentario{{
          (comentarios.length || post.totalComentarios) !== 1 ? 's' : ''
        }}
        {{ comentariosAbiertos ? '▲' : '▼' }}
      </button>
    </div>

    <!-- Sección de comentarios -->
    <div v-if="comentariosAbiertos" class="post-comments">

      <!-- Formulario -->
      <div class="post-comments__form">
        <textarea
          v-model="nuevoComentario"
          class="post-comments__input"
          placeholder="Escribe un comentario..."
          rows="1"
          :disabled="enviandoComentario"
        />
        <button
          class="post-comments__send"
          :disabled="!nuevoComentario.trim() || enviandoComentario"
          @click="enviarComentario"
        >
          {{ enviandoComentario ? '...' : 'Enviar' }}
        </button>
      </div>

      <!-- Lista -->
      <p v-if="loadingComentarios" class="post-comments__empty">Cargando...</p>
      <p v-else-if="comentarios.length === 0" class="post-comments__empty">
        Sin comentarios todavía.
      </p>

      <div v-else class="post-comments__list">
        <div
          v-for="c in comentarios"
          :key="c.idComentarioPost"
          class="post-comments__item"
        >
          <div class="post-comments__item-header">
            <span class="post-comments__author">👤 {{ c.nombreUsuario }}</span>
            <div class="post-comments__comment-meta">
              <span class="post-comments__comment-date">
                {{ formatFecha(c.fechaComentario) }}
              </span>
              <button
                v-if="puedeEliminarComentario(c)"
                class="post-comments__comment-delete"
                @click="handleDeleteComentario(c.idComentarioPost)"
              >
                ✕
              </button>
            </div>
          </div>
          <p class="post-comments__text">{{ c.texto }}</p>
        </div>
      </div>

    </div>

    <!-- Lightbox -->
    <div v-if="lightboxUrl" class="post-lightbox" @click="lightboxUrl = ''">
      <img :src="lightboxUrl" class="post-lightbox__img" @click.stop />
      <button class="post-lightbox__close" @click="lightboxUrl = ''">✕</button>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useConfirm } from '@/composables/useConfirm'
import { useToast } from '@/composables/useToast'
import {
  getComentariosPost,
  crearComentarioPost,
  eliminarPost,
  eliminarComentarioPost,
  type Post,
  type ComentarioPost
} from '@/services/postService'

const props = defineProps<{ post: Post }>()
const emit  = defineEmits<{ deleted: [id: number] }>()

const authStore   = useAuthStore()
const { confirm } = useConfirm()
const toast       = useToast()

const comentariosAbiertos  = ref(false)
const loadingComentarios   = ref(false)
const comentarios          = ref<ComentarioPost[]>([])
const nuevoComentario      = ref('')
const enviandoComentario   = ref(false)
const lightboxUrl          = ref('')

const puedeEliminar = computed(() =>
  authStore.usuario?.idUsuario === props.post.idUsuario || authStore.isAdmin
)

function puedeEliminarComentario(c: ComentarioPost) {
  return authStore.usuario?.idUsuario === c.idUsuario || authStore.isAdmin
}

async function toggleComentarios() {
  comentariosAbiertos.value = !comentariosAbiertos.value
  if (comentariosAbiertos.value && comentarios.value.length === 0) {
    loadingComentarios.value = true
    try {
      comentarios.value = await getComentariosPost(props.post.idPost)
    } catch {
      toast.error('Error al cargar los comentarios.')
    } finally {
      loadingComentarios.value = false
    }
  }
}

async function enviarComentario() {
  if (!nuevoComentario.value.trim() || !authStore.usuario) return
  enviandoComentario.value = true
  try {
    const nuevo = await crearComentarioPost(
      props.post.idPost,
      authStore.usuario.idUsuario,
      nuevoComentario.value.trim()
    )
    nuevo.nombreUsuario = authStore.usuario.nombre
    comentarios.value.unshift(nuevo)
    nuevoComentario.value = ''
  } catch {
    toast.error('Error al enviar el comentario.')
  } finally {
    enviandoComentario.value = false
  }
}

async function handleDelete() {
  const ok = await confirm({
    title:       'Eliminar post',
    message:     '¿Seguro que quieres eliminar este post? Esta acción no se puede deshacer.',
    confirmText: 'Eliminar',
    danger:      true
  })
  if (!ok) return
  try {
    await eliminarPost(
      props.post.idPost,
      authStore.usuario!.idUsuario,
      authStore.isAdmin
    )
    emit('deleted', props.post.idPost)
    toast.success('Post eliminado.')
  } catch {
    toast.error('Error al eliminar el post.')
  }
}

async function handleDeleteComentario(idComentario: number) {
  const ok = await confirm({
    title:       'Eliminar comentario',
    message:     '¿Seguro que quieres eliminar este comentario?',
    confirmText: 'Eliminar',
    danger:      true
  })
  if (!ok) return
  try {
    await eliminarComentarioPost(
      idComentario,
      authStore.usuario!.idUsuario,
      authStore.isAdmin
    )
    comentarios.value = comentarios.value.filter(c => c.idComentarioPost !== idComentario)
    toast.success('Comentario eliminado.')
  } catch {
    toast.error('Error al eliminar el comentario.')
  }
}

function formatFecha(fecha: string): string {
  if (!fecha) return ''
  const d = new Date(fecha)
  const ahora = new Date()
  const diffMs = ahora.getTime() - d.getTime()
  const diffMin = Math.floor(diffMs / 60000)
  if (diffMin < 1)  return 'Ahora mismo'
  if (diffMin < 60) return `hace ${diffMin} min`
  const diffH = Math.floor(diffMin / 60)
  if (diffH < 24)   return `hace ${diffH}h`
  return d.toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' })
}
</script>