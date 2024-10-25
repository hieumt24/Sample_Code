<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    modal
    :draggable="false"
    header="Xem các thay đổi"
    :style="{ width: '25rem' }"
  >
    <div>
      <h1>{{ message }}</h1>
      <div class="grid grid-cols-2 gap-4 mt-5">
        <Button @click="$emit('close')" label="Đóng" outlined />
        <Button @click="$emit('submit')" label="Đồng ý" />
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useI18n } from 'vue-i18n'
import Button from 'primevue/button'

const { t } = useI18n()

defineProps<{
  open: boolean
  message: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit'): void
}>()
</script>
