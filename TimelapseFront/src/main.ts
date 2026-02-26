import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router/index'
import '@/assets/styles/styles.css'

const app = createApp(App)

app.use(createPinia())  // ← esto faltaba
app.use(router)
app.mount('#app')