<template>
  <div>
    <div class="admin-section-header">
      <div>
        <h1 class="admin-section-header__title">Usuarios</h1>
        <p class="admin-section-header__subtitle">{{ usuarios.length }} usuarios registrados</p>
      </div>
      <button class="admin-table__btn admin-table__btn--edit" style="padding: 10px 20px; font-size:14px" @click="abrirModalCrear">
        + Nuevo usuario
      </button>
    </div>

    <div class="admin-table">
      <div class="admin-table__search">
        <input
          v-model="busqueda"
          class="admin-table__search-input"
          placeholder="Buscar por nombre o email..."
        />
      </div>

      <p v-if="loading" class="admin-table__empty">Cargando...</p>
      <p v-else-if="error" class="admin-table__empty">{{ error }}</p>

      <table v-else>
        <thead>
          <tr>
            <th>ID</th>
            <th>Nombre</th>
            <th>Email</th>
            <th>Rol</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="usuariosFiltrados.length === 0">
            <td colspan="5" style="text-align:center; padding: 40px; color:#999">
              No se encontraron usuarios
            </td>
          </tr>
          <tr v-for="u in usuariosFiltrados" :key="u.idUsuario">
            <td>#{{ u.idUsuario }}</td>
            <td>{{ u.nombre }}</td>
            <td>{{ u.email }}</td>
            <td>
              <span class="admin-table__badge" :class="`admin-table__badge--${u.rol}`">
                {{ u.rol }}
              </span>
            </td>
            <td>
              <div class="admin-table__actions">
                <button class="admin-table__btn admin-table__btn--edit" @click="abrirModalEditar(u)">
                  Editar
                </button>
                <button class="admin-table__btn admin-table__btn--delete" @click="eliminarUsuario(u.idUsuario)">
                  Eliminar
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modal crear/editar -->
    <div v-if="modalAbierto" class="admin-modal-overlay" @click.self="cerrarModal">
      <div class="admin-modal">
        <h2 class="admin-modal__title">
          {{ modoEdicion ? 'Editar usuario' : 'Nuevo usuario' }}
        </h2>

        <form class="form" @submit.prevent="guardarUsuario">
          <div class="form__group">
            <label class="form__label">Nombre</label>
            <input v-model="form.nombre" type="text" class="form__input" required />
          </div>
          <div class="form__group">
            <label class="form__label">Email</label>
            <input v-model="form.email" type="email" class="form__input" required />
          </div>
          <div class="form__group">
            <label class="form__label">Contraseña{{ modoEdicion ? ' (dejar vacío para no cambiar)' : '' }}</label>
            <input v-model="form.contraseña" type="password" class="form__input" :required="!modoEdicion" />
          </div>
          <div class="form__group">
            <label class="form__label">Rol</label>
            <select v-model="form.rol" class="form__input">
              <option value="usuario">usuario</option>
              <option value="admin">admin</option>
            </select>
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

interface Usuario {
  idUsuario: number
  nombre: string
  email: string
  contraseña: string
  rol: string
}

const usuarios = ref<Usuario[]>([])
const loading = ref(true)
const error = ref('')
const busqueda = ref('')

const modalAbierto = ref(false)
const modoEdicion = ref(false)
const guardando = ref(false)
const errorModal = ref('')

const form = reactive({
  idUsuario: 0,
  nombre: '',
  email: '',
  contraseña: '',
  rol: 'usuario'
})

const usuariosFiltrados = computed(() => {
  const q = busqueda.value.toLowerCase()
  if (!q) return usuarios.value
  return usuarios.value.filter(u =>
    u.nombre.toLowerCase().includes(q) ||
    u.email.toLowerCase().includes(q)
  )
})

onMounted(async () => {
  await cargarUsuarios()
})

async function cargarUsuarios() {
  loading.value = true
  try {
    usuarios.value = await api.get<Usuario[]>('/Usuario')
  } catch {
    error.value = 'Error al cargar usuarios'
  } finally {
    loading.value = false
  }
}

function abrirModalCrear() {
  modoEdicion.value = false
  form.idUsuario = 0
  form.nombre = ''
  form.email = ''
  form.contraseña = ''
  form.rol = 'usuario'
  errorModal.value = ''
  modalAbierto.value = true
}

function abrirModalEditar(u: Usuario) {
  modoEdicion.value = true
  form.idUsuario = u.idUsuario
  form.nombre = u.nombre
  form.email = u.email
  form.contraseña = ''
  form.rol = u.rol
  errorModal.value = ''
  modalAbierto.value = true
}

function cerrarModal() {
  modalAbierto.value = false
}

async function guardarUsuario() {
  errorModal.value = ''
  guardando.value = true

  try {
    if (modoEdicion.value) {
      let contraseñaFinal = form.contraseña
      if (!contraseñaFinal) {
        const actual = await api.get<Usuario>(`/Usuario/${form.idUsuario}`)
        contraseñaFinal = actual.contraseña
      }
      await api.put(`/Usuario/${form.idUsuario}`, {
        idUsuario: form.idUsuario,
        nombre: form.nombre,
        email: form.email,
        contraseña: contraseñaFinal,
        rol: form.rol
      })
    } else {
      await api.post('/Usuario', {
        nombre: form.nombre,
        email: form.email,
        contraseña: form.contraseña,
        rol: form.rol
      })
    }
    await cargarUsuarios()
    cerrarModal()
  } catch (e: any) {
    errorModal.value = e?.message || 'Error al guardar'
  } finally {
    guardando.value = false
  }
}

async function eliminarUsuario(id: number) {
  if (!confirm('¿Seguro que quieres eliminar este usuario?')) return
  try {
    await api.delete(`/Usuario/${id}`)
    await cargarUsuarios()
  } catch {
    error.value = 'Error al eliminar el usuario'
  }
}
</script>