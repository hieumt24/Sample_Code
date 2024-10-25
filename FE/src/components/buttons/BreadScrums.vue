<template>
  <div>
    <Breadcrumb class="!border-none" :home="home" :model="layoutStore.breadcrumb" />
  </div>
</template>

<script setup lang="ts">
import Breadcrumb from 'primevue/breadcrumb'
import { useLayoutStore } from '@/stores/layoutStore'
import { ref } from 'vue'

defineProps<{
  values?: string[]
}>()

const home = ref({
  icon: 'pi pi-home'
})

const layoutStore = useLayoutStore()
</script>
