<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main page__main--capsulas">
      <section class="capsulas-list">
        <CapsuleItem
          v-for="capsula in capsulas"
          :key="capsula.id"
          :title="capsula.title"
          :date="capsula.date"
          :to="`/capsula/${capsula.id}`"
        />
      </section>
    </main>

    <BottomNav />
  </div>
</template>

<script setup lang="ts">
import AppHeader from '@/components/AppHeader.vue'
import BottomNav from '@/components/BottomNav.vue'
import CapsuleItem from '@/components/CapsuleItem.vue'

interface Capsula {
  id: number
  title: string
  date: string
}

const capsulas: Capsula[] = [
  { id: 1, title: 'Nombre Cápsula', date: 'Fecha Apertura' },
  { id: 2, title: 'Nombre Cápsula', date: 'Fecha Apertura' },
  { id: 3, title: 'Nombre Cápsula', date: 'Fecha Apertura' }
]
</script>