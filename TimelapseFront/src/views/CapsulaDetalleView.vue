<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main page__main--capsula-detalle">

      <p v-if="loading" class="capsulas-list__msg">Cargando cápsula...</p>
      <p v-else-if="error" class="capsulas-list__msg">{{ error }}</p>

      <template v-else-if="capsula">

        <!-- Cabecera -->
        <section class="card card--perfil" style="flex-direction: column; align-items: flex-start;">
          <div class="capsula-detalle__header">
            <span class="capsula-detalle__emoji">{{ emoji }}</span>
            <div class="capsula-detalle__info">
              <h1 class="capsula-detalle__titulo">{{ capsula.titulo }}</h1>
              <div class="capsula-detalle__badges">
                <span
                  class="capsula-detalle__badge"
                  :class="`capsula-detalle__badge--${capsula.visibilidad}`"
                >
                  {{ capsula.visibilidad === 'publica' ? '🌍 Pública' : '🔒 Privada' }}
                </span>
              </div>
            </div>
          </div>

          <div class="capsula-fechas" style="width:100%">
            <div class="capsula-fechas__item">
              <p class="capsula-fechas__label">Creada el</p>
              <p class="capsula-fechas__valor">{{ formatFecha(capsula.fechaCreacion) }}</p>
            </div>
            <div class="capsula-fechas__item">
              <p class="capsula-fechas__label">Abierta el</p>
              <p class="capsula-fechas__valor">{{ formatFecha(capsula.fechaApertura) }}</p>
            </div>
          </div>
        </section>

        <!-- Descripción -->
        <section v-if="capsula.descripcion" class="card capsula-seccion">
          <h2 class="capsula-seccion__titulo">Descripción</h2>
          <p class="capsula-seccion__texto">{{ capsula.descripcion }}</p>
        </section>

        <!-- Participantes -->
        <section v-if="participantes.length > 0" class="card capsula-seccion">
          <h2 class="capsula-seccion__titulo">Participantes</h2>
          <div class="capsula-participantes">
            <span
              v-for="p in participantes"
              :key="p.idUsuario"
              class="capsula-participantes__tag"
            >
              👤 {{ p.nombre }}
            </span>
          </div>
        </section>

        <!-- Contenido -->
        <section class="card capsula-seccion">
          <h2 class="capsula-seccion__titulo">Contenido</h2>

          <p v-if="loadingContenido" class="capsula-archivos__empty">Cargando archivos...</p>
          <p v-else-if="contenido.length === 0" class="capsula-archivos__empty">
            Esta cápsula no tiene archivos adjuntos.
          </p>

          <div v-else class="capsula-archivos">
            <div class="capsula-archivos__grid">
              <div v-for="item in contenido" :key="item.idContenido" class="capsula-archivos__item">

                <template v-if="item.tipo === 'imagen'">
                  <img
                    :src="item.urlArchivo"
                    alt="Imagen"
                    class="capsula-archivos__img"
                    @click="abrirLightbox(item.urlArchivo!)"
                  />
                </template>

                <template v-else-if="item.tipo === 'video'">
                  <video
                    :src="item.urlArchivo"
                    class="capsula-archivos__video"
                    controls
                    preload="metadata"
                  />
                </template>

                <template v-else-if="item.tipo === 'texto'">
                  <div class="capsula-archivos__doc" style="background: #f5f5f5;">
                    <span class="capsula-archivos__doc-icon">📝</span>
                    <p class="capsula-archivos__doc-name">{{ item.contenidoTexto }}</p>
                  </div>
                </template>

                <template v-else>
                  <a :href="item.urlArchivo" target="_blank" rel="noopener" class="capsula-archivos__doc">
                    <span class="capsula-archivos__doc-icon">{{ getDocIcon(item.urlArchivo) }}</span>
                    <span class="capsula-archivos__doc-name">{{ getNombreArchivo(item.urlArchivo) }}</span>
                  </a>
                </template>

              </div>
            </div>
          </div>
        </section>

      </template>
    </main>

    <!-- Lightbox -->
    <div v-if="lightboxUrl" class="lightbox" @click="lightboxUrl = ''">
      <img :src="lightboxUrl" class="lightbox__img" @click.stop />
      <button class="lightbox__close" @click="lightboxUrl = ''">✕</button>
    </div>

    <BottomNav />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
import BottomNav from '@/components/BottomNav.vue'
import { api } from '@/services/api'

interface Capsula {
  idCapsula: number
  titulo: string
  descripcion: string
  fechaCreacion: string
  fechaApertura: string
  estado: string
  visibilidad: string
}

interface Contenido {
  idContenido: number
  tipo: string
  contenidoTexto?: string
  urlArchivo?: string
  fechaSubida: string
}

interface Participante {
  idUsuario: number
  nombre: string
  rol: string
}

interface UsuarioCapsula {
  idUsuarioCapsula: number
  idUsuario: number
  idCapsula: number
  rol: string
}

interface Usuario {
  idUsuario: number
  nombre: string
  email: string
}

const route = useRoute()
const router = useRouter()

const capsula = ref<Capsula | null>(null)
const contenido = ref<Contenido[]>([])
const participantes = ref<Participante[]>([])
const loading = ref(true)
const loadingContenido = ref(true)
const error = ref('')
const lightboxUrl = ref('')

const emoji = computed(() => {
  try {
    const id = Number(route.params.id)
    const guardados = JSON.parse(localStorage.getItem('capsula_emojis') || '{}')
    return guardados[id] ?? '⏳'
  } catch {
    return '⏳'
  }
})

onMounted(async () => {
  const id = Number(route.params.id)

  try {
    const data = await api.get<Capsula>(`/Capsula/${id}`)

    if (new Date(data.fechaApertura) > new Date()) {
      router.replace('/tus-capsulas')
      return
    }

    capsula.value = data

    const [contenidoData, ucData] = await Promise.all([
      api.get<Contenido[]>(`/Contenido/capsula/${id}`),
      api.get<UsuarioCapsula[]>(`/UsuarioCapsula/capsula/${id}`)
    ])

    contenido.value = contenidoData
    loadingContenido.value = false

    const usuarios = await Promise.all(
      ucData.map(uc => api.get<Usuario>(`/Usuario/${uc.idUsuario}`))
    )
    participantes.value = ucData.map((uc, i) => ({
      idUsuario: uc.idUsuario,
      nombre: usuarios[i].nombre,
      rol: uc.rol
    }))

  } catch {
    error.value = 'No se pudo cargar la cápsula.'
  } finally {
    loading.value = false
  }
})

function formatFecha(fecha: string): string {
  if (!fecha) return ''
  return new Date(fecha).toLocaleDateString('es-ES', {
    day: '2-digit', month: 'long', year: 'numeric'
  })
}

function getNombreArchivo(url?: string): string {
  if (!url) return 'Archivo'
  return url.split('/').pop()?.split('?')[0] ?? 'Archivo'
}

function getDocIcon(url?: string): string {
  if (!url) return '📎'
  if (url.includes('.pdf')) return '📄'
  if (url.match(/\.(doc|docx)/)) return '📝'
  return '📎'
}

function abrirLightbox(url: string) {
  lightboxUrl.value = url
}
</script>

<style scoped>
.lightbox {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.85);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  cursor: zoom-out;
}
.lightbox__img {
  max-width: 90vw;
  max-height: 90vh;
  object-fit: contain;
  border-radius: 8px;
  cursor: default;
}
.lightbox__close {
  position: absolute;
  top: 20px;
  right: 24px;
  background: none;
  border: none;
  color: #fff;
  font-size: 28px;
  cursor: pointer;
  opacity: 0.8;
  transition: opacity 0.2s;
}
.lightbox__close:hover { opacity: 1; }
</style>