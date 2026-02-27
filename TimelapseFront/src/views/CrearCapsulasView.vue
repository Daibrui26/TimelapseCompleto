<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main page__main--crear-capsula">
      <section class="card card--crear-capsula">

        <!-- Cabecera -->
        <div class="crear-capsula__header">
          <h1 class="crear-capsula__title">Nueva Cápsula</h1>
          <p class="crear-capsula__subtitle">Guarda tus recuerdos y ábrelos en el futuro</p>
        </div>

        <form class="form" @submit.prevent="handleSubmit">

          <!-- Selector de emoji -->
          <div class="form__group">
            <div class="emoji-selector">
              <span class="emoji-selector__label">Elige un icono para tu cápsula</span>
              <div class="emoji-selector__grid">
                <button
                  v-for="emoji in EMOJIS"
                  :key="emoji"
                  type="button"
                  class="emoji-selector__option"
                  :class="{ 'emoji-selector__option--selected': form.emoji === emoji }"
                  @click="form.emoji = emoji"
                >
                  {{ emoji }}
                </button>
              </div>
              <div class="emoji-selector__preview">
                <span>{{ form.emoji }}</span>
                <span>Icono seleccionado</span>
              </div>
            </div>
          </div>

          <!-- Título -->
          <div class="form__group">
            <label for="titulo" class="form__label">Título de la cápsula</label>
            <input
              v-model="form.titulo"
              type="text"
              id="titulo"
              class="form__input"
              placeholder="Ej: Verano 2025"
              required
            />
          </div>

          <!-- Descripción -->
          <div class="form__group">
            <label for="descripcion" class="form__label">Descripción</label>
            <textarea
              v-model="form.descripcion"
              id="descripcion"
              class="form__textarea"
              placeholder="Describe qué hay en esta cápsula..."
            ></textarea>
          </div>

          <!-- Fecha de apertura -->
          <div class="form__group">
            <label for="fechaApertura" class="form__label">Fecha de apertura</label>
            <input
              v-model="form.fechaApertura"
              type="date"
              id="fechaApertura"
              class="form__input"
              :min="minDate"
              required
            />
          </div>

          <!-- Visibilidad -->
          <div class="form__group">
            <label class="form__label">Visibilidad</label>
            <div class="visibility-selector">
              <button
                type="button"
                class="visibility-selector__option"
                :class="{ 'visibility-selector__option--selected': form.visibilidad === 'privada' }"
                @click="form.visibilidad = 'privada'"
              >
                <span class="visibility-selector__option-icon">🔒</span>
                Privada
              </button>
              <button
                type="button"
                class="visibility-selector__option"
                :class="{ 'visibility-selector__option--selected': form.visibilidad === 'publica' }"
                @click="form.visibilidad = 'publica'"
              >
                <span class="visibility-selector__option-icon">🌍</span>
                Pública
              </button>
            </div>
          </div>

          <!-- Participantes -->
          <div class="form__group">
            <label class="form__label">Participantes</label>
            <div class="participantes">
              <div class="participantes__search">
                <input
                  v-model="participanteInput"
                  type="text"
                  class="participantes__input"
                  placeholder="Buscar usuario por nombre..."
                  @input="buscarUsuarios"
                  @keydown.enter.prevent="añadirPorNombre"
                />
                <button type="button" class="participantes__btn-add" @click="añadirPorNombre">+</button>
              </div>

              <!-- Sugerencias de búsqueda -->
              <div v-if="sugerencias.length > 0" class="participantes__list">
                <button
                  v-for="u in sugerencias"
                  :key="u.idUsuario"
                  type="button"
                  class="participantes__tag"
                  style="cursor:pointer"
                  @click="seleccionarUsuario(u)"
                >
                  👤 {{ u.nombre }}
                  <span style="font-size:11px; opacity:0.7">{{ u.email }}</span>
                </button>
              </div>

              <!-- Participantes añadidos -->
              <div v-if="participantesSeleccionados.length > 0" class="participantes__list">
                <span
                  v-for="p in participantesSeleccionados"
                  :key="p.idUsuario"
                  class="participantes__tag"
                >
                  👤 {{ p.nombre }}
                  <button type="button" class="participantes__tag-remove" @click="quitarParticipante(p.idUsuario)">×</button>
                </span>
              </div>
              <span v-else-if="sugerencias.length === 0" class="participantes__empty">
                Sin participantes adicionales
              </span>
            </div>
          </div>

          <!-- Subir archivos -->
          <div class="form__group">
            <label class="form__label">Archivos (imágenes, vídeos, documentos)</label>
            <div
              class="upload-area"
              :class="{ 'upload-area--dragover': isDragging }"
              @click="triggerFileInput"
              @dragover.prevent="isDragging = true"
              @dragleave="isDragging = false"
              @drop.prevent="onDrop"
            >
              <span class="upload-area__icon">📎</span>
              <p class="upload-area__text">Haz clic o arrastra archivos aquí</p>
              <p class="upload-area__hint">Imágenes, vídeos y documentos (máx. 10 archivos)</p>
              <input
                ref="fileInputRef"
                type="file"
                multiple
                class="upload-area__input"
                accept="image/*,video/*,.pdf,.doc,.docx"
                @change="onFileChange"
              />
            </div>

            <!-- Lista de archivos seleccionados -->
            <div v-if="archivos.length > 0" class="file-list">
              <div v-for="(file, index) in archivos" :key="index" class="file-list__item">
                <span class="file-list__icon">{{ getFileIcon(file) }}</span>
                <span class="file-list__name">{{ file.name }}</span>
                <button type="button" class="file-list__remove" @click="quitarArchivo(index)">×</button>
              </div>
            </div>
          </div>

          <hr class="form__separator" />

          <!-- Mensaje de error -->
          <p v-if="errorMsg" style="color: #C85C5C; font-size: 14px; text-align:center;">
            {{ errorMsg }}
          </p>

          <!-- Acciones -->
          <div class="crear-capsula-actions">
            <button type="submit" class="btn btn--submit" :disabled="loading">
              {{ loading ? 'Sellando...' : '🔒 Sellar Cápsula' }}
            </button>
            <RouterLink to="/home" class="btn btn--cancel">Cancelar</RouterLink>
          </div>

        </form>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
import { api } from '@/services/api'
import { useAuthStore } from '@/stores/auth'

interface Usuario {
  idUsuario: number
  nombre: string
  email: string
}

interface CapsulaCreada {
  idCapsula: number
}

const router = useRouter()
const authStore = useAuthStore()

// ── Emojis disponibles ────────────────────────────────────────────────────────
const EMOJIS = ['⏳', '📦', '🌟', '💌', '🎁', '🏖️', '🎓', '❤️', '🌍', '📷', '🎵', '🏡']

// ── Estado del formulario ─────────────────────────────────────────────────────
const form = reactive({
  titulo: '',
  descripcion: '',
  fechaApertura: '',
  visibilidad: 'privada',
  emoji: '⏳'
})

const minDate = computed(() => {
  const d = new Date()
  d.setDate(d.getDate() + 1)
  return d.toISOString().split('T')[0]
})

// ── Archivos ──────────────────────────────────────────────────────────────────
const archivos = ref<File[]>([])
const fileInputRef = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)

function triggerFileInput() {
  fileInputRef.value?.click()
}

function onFileChange(e: Event) {
  const input = e.target as HTMLInputElement
  if (input.files) addFiles(Array.from(input.files))
}

function onDrop(e: DragEvent) {
  isDragging.value = false
  if (e.dataTransfer?.files) addFiles(Array.from(e.dataTransfer.files))
}

function addFiles(nuevos: File[]) {
  const todos = [...archivos.value, ...nuevos]
  archivos.value = todos.slice(0, 10)
}

function quitarArchivo(index: number) {
  archivos.value.splice(index, 1)
}

function getFileIcon(file: File): string {
  if (file.type.startsWith('image/')) return '🖼️'
  if (file.type.startsWith('video/')) return '🎥'
  if (file.type.includes('pdf')) return '📄'
  return '📎'
}

function getTipo(file: File): string {
  if (file.type.startsWith('image/')) return 'imagen'
  if (file.type.startsWith('video/')) return 'video'
  return 'documento'
}

// ── Participantes ─────────────────────────────────────────────────────────────
const participanteInput = ref('')
const participantesSeleccionados = ref<Usuario[]>([])
const sugerencias = ref<Usuario[]>([])
let busquedaTimeout: ReturnType<typeof setTimeout> | null = null

async function buscarUsuarios() {
  if (busquedaTimeout) clearTimeout(busquedaTimeout)
  const q = participanteInput.value.trim()
  if (q.length < 2) { sugerencias.value = []; return }

  busquedaTimeout = setTimeout(async () => {
    try {
      const resultados = await api.get<Usuario[]>(`/Usuario/search?nombre=${encodeURIComponent(q)}`)
      // Filtrar los ya añadidos y el propio usuario
      sugerencias.value = resultados.filter(
        u => u.idUsuario !== authStore.usuario?.idUsuario &&
             !participantesSeleccionados.value.some(p => p.idUsuario === u.idUsuario)
      )
    } catch {
      sugerencias.value = []
    }
  }, 300)
}

function seleccionarUsuario(u: Usuario) {
  if (!participantesSeleccionados.value.some(p => p.idUsuario === u.idUsuario)) {
    participantesSeleccionados.value.push(u)
  }
  participanteInput.value = ''
  sugerencias.value = []
}

function añadirPorNombre() {
  if (sugerencias.value.length > 0) {
    seleccionarUsuario(sugerencias.value[0])
  }
}

function quitarParticipante(id: number) {
  participantesSeleccionados.value = participantesSeleccionados.value.filter(p => p.idUsuario !== id)
}

// ── Envío ─────────────────────────────────────────────────────────────────────
const loading = ref(false)
const errorMsg = ref('')

async function handleSubmit() {
  errorMsg.value = ''
  loading.value = true

  try {
    const hoy = new Date().toISOString()

    // 1. Crear la cápsula
    const nuevaCapsula = await api.post<CapsulaCreada>('/Capsula', {
      titulo: form.titulo,
      descripcion: form.descripcion,
      fechaCreacion: hoy,
      fechaApertura: new Date(form.fechaApertura).toISOString(),
      estado: 'cerrada',
      visibilidad: form.visibilidad
    })

    const idCapsula = nuevaCapsula.idCapsula

    // 2. Asociar el creador a la cápsula (rol: creador)
    await api.post('/UsuarioCapsula', {
      idUsuario: authStore.usuario!.idUsuario,
      idCapsula,
      rol: 'creador'
    })

    // 3. Añadir participantes adicionales (rol: participante)
    if (participantesSeleccionados.value.length > 0) {
      await Promise.all(
        participantesSeleccionados.value.map(u =>
          api.post('/UsuarioCapsula', {
            idUsuario: u.idUsuario,
            idCapsula,
            rol: 'participante'
          })
        )
      )
    }

    // 4. Subir archivos
    if (archivos.value.length > 0) {
      await Promise.all(
        archivos.value.map(file => {
          const formData = new FormData()
          formData.append('IdCapsula', String(idCapsula))
          formData.append('Tipo', getTipo(file))
          formData.append('Archivo', file)
          return fetch('/api/Contenido/archivo', {
            method: 'POST',
            body: formData
          })
        })
      )
    }

    // 5. Guardar emoji en localStorage
    const emojisGuardados = JSON.parse(localStorage.getItem('capsula_emojis') || '{}')
    emojisGuardados[idCapsula] = form.emoji
    localStorage.setItem('capsula_emojis', JSON.stringify(emojisGuardados))

    router.push('/tus-capsulas')
  } catch (e: any) {
    errorMsg.value = e?.message || 'Ha ocurrido un error. Inténtalo de nuevo.'
  } finally {
    loading.value = false
  }
}
</script>