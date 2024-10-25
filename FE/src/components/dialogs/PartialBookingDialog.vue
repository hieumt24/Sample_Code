<template>
  <Dialog
    :visible="open"
    :header="fieldName ?? 'Lịch đặt sân'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[900px] mx-20"
  >
    <SuperBookingTable
      :partial-field-id="partialId"
      :field-id="fieldId"
      :date="date"
      :is-fixed-slot="isFixedSlot"
    />
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { watch } from 'vue'
import { useBookingStore } from '@/stores/bookingStoreOwner'
import SuperBookingTable from '../tables/SuperBookingTable.vue'

const bookingStore = useBookingStore()

const props = defineProps<{
  open: boolean
  partialId: number
  fieldId: number
  isFixedSlot: boolean
  fieldName?: string
  date?: string
}>()

const emit = defineEmits<(e: 'close') => void>()

const onClose = () => {
  emit('close')
}
</script>
