<template>
  <div v-if="total" class="flex items-center">
    <span>Trang {{ currentPage }}</span>
    <Paginator
      @page="onPageChange"
      :rows="limit ?? LIMIT_PER_PAGE"
      :totalRecords="total"
    ></Paginator>
  </div>
</template>

<script setup lang="ts">
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import Paginator, { type PageState } from 'primevue/paginator'
import { ref } from 'vue'

defineProps<{
  total: number
  limit?: number
}>()
const currentPage = ref(1)
const emit = defineEmits<(e: 'changePage', page: number) => void>()

const onPageChange = (pageState: PageState) => {
  emit('changePage', pageState.page + 1)
  currentPage.value = pageState.page + 1
}

const goToFirstPage = () => {
  emit('changePage', 1)
  resetPageNumberDisplay()
}

const resetPageNumberDisplay = () => {
  currentPage.value = 1
}
defineExpose({
  goToFirstPage,
  resetPageNumberDisplay
})
</script>

<style scoped>
.p-paginator-first {
  cursor: e-resize !important;
}
</style>
