<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    modal
    :draggable="false"
    header="Thông báo"
    :style="{ width: '25rem' }"
  >
    <div class="flex items-center gap-4 mb-8">
      Bạn chưa đăng nhập, hãy đăng nhập để sử dụng tính năng này
    </div>
    <div class="py-4 flex justify-evenly">
      <ActionButton class="mr-2" value="Quay lại" @click="onClose" :is-outlined="true" />
      <ActionButton
        class="mr-2"
        value="Đăng nhập"
        @click="$router.push({ name: 'login-screen' })"
      />
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import ActionButton from '@/components/buttons/ActionButton.vue'

defineProps<{
  open: boolean
}>()

const emit = defineEmits<(e: 'close') => void>()

const onClose = () => {
  emit('close')
}
</script>
