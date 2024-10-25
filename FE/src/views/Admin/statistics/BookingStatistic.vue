<template>
  <div>
    <h1>Booking statistic</h1>
  </div>
</template>
<script setup lang="ts">
import DataTable, { type DataTablePageEvent } from 'primevue/datatable'
import Column from 'primevue/column'
import IconButton from '@/components/buttons/IconButton.vue'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import { onMounted } from 'vue'
import type { TableColumn } from '@/components/tables/types'

defineProps<{
  title?: string
  noDetail?: boolean
  noEdit?: boolean
  loading?: boolean
  noDelete?: boolean
  total: number
  ban?: boolean
  headers: TableColumn[]
  items: { [key: string]: any }[]
}>()

const emit = defineEmits<{
  (e: 'detail', id: number): void
  (e: 'edit', id: number): void
  (e: 'delete', id: number): void
  (e: 'changePage', offset: number, limit: number): void
  (e: 'ban', id: number): void
}>()

onMounted(() => {
  console.log('Booking statistic mounted')
})
</script>
