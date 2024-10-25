<template>
  <FieldForm @back="back" :is-create="true" />
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import FieldForm from '@/components/forms/FieldForm.vue'
import router from '@/router'

const layoutStore = useLayoutStore()

onMounted(() => {
  layoutStore.breadcrumb = [{ label: 'Add Field' }]
})

const back = () => {
  router.push('/admin/fields')
}
</script>
