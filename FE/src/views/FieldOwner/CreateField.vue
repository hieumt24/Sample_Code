<template>
  <div class="md:px-10">
    <FieldForm @back="back" :is-create="true" />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import FieldForm from '@/components/forms/FieldForm.vue'
import router from '@/router'

const layoutStore = useLayoutStore()

onMounted(() => {
  layoutStore.breadcrumb = [{ label: 'My Field' }, { label: 'Add Field' }]
})

const back = () => {
  router.push('/field/list')
}
</script>
