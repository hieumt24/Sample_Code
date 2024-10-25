<template>
  <Dialog
    :visible="open"
    header="Xác nhận tìm đối"
    modal
    @update:visible="emit('close')"
    :draggable="false"
    :style="{ width: '30rem' }"
  >
    <div class="text-center">
      <p class="text-lg">Bạn đã đặt sân thành công trên hệ thống chưa?</p>
      <p class="text-lg text-gray-500">
        Nếu bạn muốn đặt sân, vui lòng
        <a @click="handleWantToBook" class="text-blue-500 cursor-pointer underline">nhấn vào đây</a>
      </p>
    </div>
    <div class="mt-4 flex justify-around">
      <Button label="Không đặt sân" severity="secondary" @click="handleNoBooking" />
      <Button label="Đã đặt sân" @click="handleBooked" />
    </div>
    <div class="mt-4 text-center"></div>
  </Dialog>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'

const props = defineProps<{
  open: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'open-create-without-booking-dialog'): void
}>()

const router = useRouter()

const handleBooked = () => {
  router.push('/user/booking-history')
}

const handleNoBooking = () => {
  emit('open-create-without-booking-dialog')
}
const handleWantToBook = () => {
  router.push('/user/home')
}
</script>
