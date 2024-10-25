<template>
  <Dialog
    :visible="open"
    :header="isWaiting ? 'Lịch chờ xác nhận' : 'Lịch đã xác nhận'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[600px] mx-20"
  >
    <div class="w-full flex justify-between">
      <div class="w-full flex items-center">
        <span class="mr-5">{{ formatDateVietnamese(bookingStore.booking?.date) }},</span>
        <span class="font-semibold">
          Thời gian:
          {{ bookingStore.booking?.startTime }} - {{ bookingStore.booking?.endTime }}
        </span>
      </div>
      <div
        class="font-semibold w-40 flex items-center rounded-full p-1"
        :class="isWaiting ? 'bg-blue-100' : 'bg-red-100'"
      >
        <span v-if="isWaiting" class="mx-auto text-blue-500 bg-blue-100">{{
          t(`bookingStatus.${bookingStore.booking?.status}`)
        }}</span>
        <span v-if="!isWaiting" class="mx-auto text-red-500 bg-red-100">{{
          t(`bookingStatus.${bookingStore.booking?.status}`)
        }}</span>
      </div>
    </div>

    <div class="my-2 flex justify-between mt-10">
      <ActionButton
        @click.stop="emit('close')"
        class="bg-green-400 mx-2"
        is-outlined
        value="Đóng"
      />
      <div>
        <ActionButton
          @click.stop="onReject"
          class="bg-green-400 mx-auto"
          is-outlined
          v-if="isWaiting"
          value="Tù chối"
        />

        <ActionButton
          @click.stop="onAccept"
          class="bg-green-400 mx-auto ml-4"
          value="CHấp nhận"
          v-if="isWaiting"
        />
      </div>
    </div>
  </Dialog>
  <Toast />
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useBookingStore } from '@/stores/bookingStoreOwner'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { computed, watch } from 'vue'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'
import { formatDateVietnamese } from '@/utils/dateUtil'
import { BookingStatus } from '@/constants/booking'

const toast = useToast()
const { t } = useI18n()

const bookingStore = useBookingStore()

const isWaiting = computed(() => bookingStore.booking?.status === BookingStatus.WAITING)

const props = defineProps<{
  open: boolean
  id: number
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'afterSubmit'): void
}>()

const onAccept = async () => {
  await bookingStore
    .acceptBookingStatusOwner(props.id)
    .then(() => {
      toast.add({
        severity: 'success',
        summary: 'Chấp nhận lịch đặt',
        detail: 'Đã chấp nhận lịch đặt sân',
        life: 3000
      })
      emit('afterSubmit')
    })
    .catch(() => {
      toast.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Có lỗi xảy ra, vui lòng thử lại',
        life: 3000
      })
    })
}

const onReject = async () => {
  await bookingStore
    .rejectBookingStatusOwner(props.id)
    .then(() => {
      toast.add({
        severity: 'info',
        summary: 'Từ chối lịch đặt',
        detail: 'Đã từ chối lịch đặt sân',
        life: 3000
      })
      emit('afterSubmit')
    })
    .catch(() => {
      toast.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Có lỗi xảy ra, vui lòng thử lại',
        life: 3000
      })
    })
}

watch(
  () => props.id,
  async (id) => {
    if (id) {
      await bookingStore.getBookingById(id)
    }
  }
)
</script>
