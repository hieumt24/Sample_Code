<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    modal
    :draggable="false"
    header="Thông tin"
    :style="{ width: '25rem' }"
  >
    <div>
      <h1 class="font-semibold mb-3">Cố định (Khuyến khích sử dụng):</h1>
      <p>
        Bạn sẽ phải cài đặt các slot, người dùng sẽ không thể tự điều chỉnh thời gian cho các lần
        đặt sân của họ, hệ thống sẽ hiển thị các slot bạn đã cài đặt để người dùng chọn.
      </p>
      <h1 class="font-semibold my-3">Không cố định:</h1>
      <p>
        Khi này người dùng sẽ tự chọn thời gian đặt sân của mình, người dùng sẽ được thấy lịch trống
        và không có các slot đã định sẵn.
      </p>
      <div class="mt-5 flex">
        <Button class="mx-auto" @click="$emit('close')" label="Đóng" outlined />
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
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()
</script>
