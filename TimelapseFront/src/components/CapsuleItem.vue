<template>
  <RouterLink :to="to" class="capsula-item">
    <span class="capsula-item__star">★</span>
    <div class="capsula-item__content">
      <h3 class="capsula-item__title">{{ title }}</h3>
      <p class="capsula-item__date">{{ date }}</p>
    </div>
  </RouterLink>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'

interface Props {
  title: string
  date: string
  to?: string
}

withDefaults(defineProps<Props>(), {
  to: '#'
})
</script>