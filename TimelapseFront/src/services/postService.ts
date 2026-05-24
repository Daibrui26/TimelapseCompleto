const API_BASE = '/api'

export interface Post {
  idPost: number
  texto: string
  urlArchivo?: string
  publicId?: string
  fechaPublicacion: string
  visibilidad: string
  idUsuario: number
  nombreUsuario: string
  totalComentarios: number
}

export interface ComentarioPost {
  idComentarioPost: number
  texto: string
  fechaComentario: string
  idUsuario: number
  idPost: number
  nombreUsuario: string
}

export async function getPosts(): Promise<Post[]> {
  const res = await fetch(`${API_BASE}/Post`)
  if (!res.ok) throw new Error('Error al obtener los posts')
  return res.json()
}

export async function getPostsByUsuario(idUsuario: number): Promise<Post[]> {
  const res = await fetch(`${API_BASE}/Post/usuario/${idUsuario}`)
  if (!res.ok) throw new Error('Error al obtener los posts del usuario')
  return res.json()
}

export async function getPost(id: number): Promise<Post> {
  const res = await fetch(`${API_BASE}/Post/${id}`)
  if (!res.ok) throw new Error('Error al obtener el post')
  return res.json()
}

export async function crearPost(
  idUsuario: number,
  texto: string,
  visibilidad: string,
  archivo?: File
): Promise<Post> {
  const formData = new FormData()
  formData.append('IdUsuario', String(idUsuario))
  formData.append('Texto', texto)
  formData.append('Visibilidad', visibilidad)
  if (archivo) formData.append('Archivo', archivo)

  const res = await fetch(`${API_BASE}/Post`, {
    method: 'POST',
    body: formData
  })
  if (!res.ok) throw new Error('Error al crear el post')
  return res.json()
}

export async function eliminarPost(
  idPost: number,
  idUsuario: number,
  esAdmin: boolean
): Promise<void> {
  const res = await fetch(
    `${API_BASE}/Post/${idPost}?idUsuario=${idUsuario}&esAdmin=${esAdmin}`,
    { method: 'DELETE' }
  )
  if (!res.ok) throw new Error('Error al eliminar el post')
}

export async function getComentariosPost(idPost: number): Promise<ComentarioPost[]> {
  const res = await fetch(`${API_BASE}/ComentarioPost/post/${idPost}`)
  if (!res.ok) throw new Error('Error al obtener comentarios')
  return res.json()
}

export async function crearComentarioPost(
  idPost: number,
  idUsuario: number,
  texto: string
): Promise<ComentarioPost> {
  const res = await fetch(`${API_BASE}/ComentarioPost`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ idPost, idUsuario, texto })
  })
  if (!res.ok) throw new Error('Error al crear el comentario')
  return res.json()
}

export async function eliminarComentarioPost(
  idComentarioPost: number,
  idUsuario: number,
  esAdmin: boolean
): Promise<void> {
  const res = await fetch(
    `${API_BASE}/ComentarioPost/${idComentarioPost}?idUsuario=${idUsuario}&esAdmin=${esAdmin}`,
    { method: 'DELETE' }
  )
  if (!res.ok) throw new Error('Error al eliminar el comentario')
}