import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router/index'
import '@/assets/scss/main.scss'


const app = createApp(App)

app.use(createPinia())  // ← esto faltaba
app.use(router)
app.mount('#app')