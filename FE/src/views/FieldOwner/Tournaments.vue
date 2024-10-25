<template>
  <div class="flex flex-wrap w-full">
    <h1>Tournaments</h1>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'

const layoutStore = useLayoutStore()
onMounted(() => {
  layoutStore.breadcrumb = [{ label: 'Tournaments' }]
})
</script>
