<template>
  <div>
    <div class="admin-section-header">
      <div>
        <h1 class="admin-section-header__title">Cápsulas</h1>
        <p class="admin-section-header__subtitle">{{ capsulas.length }} cápsulas en total</p>
      </div>
    </div>

    <div class="admin-table">
      <div class="admin-table__search">
        <input
          v-model="busqueda"
          class="admin-table__search-input"
          placeholder="Buscar por título o estado..."
        />
      </div>

      <p v-if="loading" class="admin-table__empty">Cargando...</p>
      <p v-else-if="error" class="admin-table__empty">{{ error }}</p>

      <table v-else>
        <thead>
          <tr>
            <th>ID</th>
            <th>Título</th>
            <th>Estado</th>
            <th>Visibilidad</th>
            <th>Apertura</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="capsulasFiltradas.length === 0">
            <td colspan="6" style="text-align:center; padding:40px; color:#999">
              No se encontraron cápsulas
            </td>
          </tr>
          <tr v-for="c in capsulasFiltradas" :key="c.idCapsula">
            <td>#{{ c.idCapsula }}</td>
            <td>{{ c.titulo }}</td>
            <td>
              <span class="admin-table__badge" :class="`admin-table__badge--${c.estado}`">
                {{ c.estado }}
              </span>
            </td>
            <td>
              <span class="admin-table__badge" :class="`admin-table__badge--${c.visibilidad}`">
                {{ c.visibilidad }}
              </span>
            </td>
            <td>{{ formatFecha(c.fechaApertura) }}</td>
            <td>
              <div class="admin-table__actions">
                <button class="admin-table__btn admin-table__btn--edit" @click="abrirModalEditar(c)">
                  Editar
                </button>
                <button class="admin-table__btn admin-table__btn--delete" @click="eliminarCapsula(c.idCapsula)">
                  Eliminar
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal editar -->
    <div v-if="modalAbierto" class="admin-modal-overlay" @click.self="cerrarModal">
      <div class="admin-modal">
        <h2 class="admin-modal__title">Editar cápsula</h2>

        <form class="form" @submit.prevent="guardarCapsula">
          <div class="form__group">
            <label class="form__label">Título</label>
            <input v-model="form.titulo" type="text" class="form__input" required />
          </div>
          <div class="form__group">
            <label class="form__label">Descripción</label>
            <textarea v-model="form.descripcion" class="form__textarea"></textarea>
          </div>
          <div class="form__group">
            <label class="form__label">Estado</label>
            <select v-model="form.estado" class="form__input">
              <option value="cerrada">cerrada</option>
              <option value="abierta">abierta</option>
            </select>
          </div>
          <div class="form__group">
            <label class="form__label">Visibilidad</label>
            <select v-model="form.visibilidad" class="form__input">
              <option value="privada">privada</option>
              <option value="publica">publica</option>
            </select>
          </div>
          <div class="form__group">
            <label class="form__label">Fecha de apertura</label>
            <input v-model="form.fechaApertura" type="date" class="form__input" required />
          </div>

          <p v-if="errorModal" style="color:#C85C5C; font-size:14px">{{ errorModal }}</p>

          <div class="admin-modal__actions">
            <button type="submit" class="btn btn--submit" style="flex:1" :disabled="guardando">
              {{ guardando ? 'Guardando...' : 'Guardar' }}
            </button>
            <button type="button" class="btn btn--cancel" style="flex:1" @click="cerrarModal">
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, reactive } from 'vue'
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

const capsulas = ref<Capsula[]>([])
const loading = ref(true)
const error = ref('')
const busqueda = ref('')
const modalAbierto = ref(false)
const guardando = ref(false)
const errorModal = ref('')

const form = reactive({
  idCapsula: 0,
  titulo: '',
  descripcion: '',
  fechaCreacion: '',
  fechaApertura: '',
  estado: 'cerrada',
  visibilidad: 'privada'
})

const capsulasFiltradas = computed(() => {
  const q = busqueda.value.toLowerCase()
  if (!q) return capsulas.value
  return capsulas.value.filter(c =>
    c.titulo.toLowerCase().includes(q) ||
    c.estado.toLowerCase().includes(q)
  )
})

onMounted(async () => {
  await cargarCapsulas()
})

async function cargarCapsulas() {
  loading.value = true
  try {
    capsulas.value = await api.get<Capsula[]>('/Capsula')
  } catch {
    error.value = 'Error al cargar cápsulas'
  } finally {
    loading.value = false
  }
}

function abrirModalEditar(c: Capsula) {
  form.idCapsula = c.idCapsula
  form.titulo = c.titulo
  form.descripcion = c.descripcion
  form.fechaCreacion = c.fechaCreacion
  form.fechaApertura = c.fechaApertura.split('T')[0]
  form.estado = c.estado
  form.visibilidad = c.visibilidad
  errorModal.value = ''
  modalAbierto.value = true
}

function cerrarModal() {
  modalAbierto.value = false
}

async function guardarCapsula() {
  errorModal.value = ''
  guardando.value = true
  try {
    await api.put(`/Capsula/${form.idCapsula}`, {
      idCapsula: form.idCapsula,
      titulo: form.titulo,
      descripcion: form.descripcion,
      fechaCreacion: form.fechaCreacion,
      fechaApertura: new Date(form.fechaApertura).toISOString(),
      estado: form.estado,
      visibilidad: form.visibilidad
    })
    await cargarCapsulas()
    cerrarModal()
  } catch (e: any) {
    errorModal.value = e?.message || 'Error al guardar'
  } finally {
    guardando.value = false
  }
}

async function eliminarCapsula(id: number) {
  if (!confirm('¿Seguro que quieres eliminar esta cápsula?')) return
  try {
    await api.delete(`/Capsula/${id}`)
    await cargarCapsulas()
  } catch {
    error.value = 'Error al eliminar la cápsula'
  }
}

function formatFecha(fecha: string): string {
  if (!fecha) return ''
  return new Date(fecha).toLocaleDateString('es-ES', {
    day: '2-digit', month: 'short', year: 'numeric'
  })
}
</script>