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
        :class="{
          'bg-blue-100 text-blue-500': isWaiting,
          'bg-green-100 text-green-500': bookingStore.booking?.status === BookingStatus.ACCEPTED,
          'bg-red-100 text-red-500': bookingStore.booking?.status === BookingStatus.REJECTED
        }"
      >
        <span v-if="isWaiting" class="mx-auto text-blue-500 bg-blue-100">{{
          t(`bookingStatus.${bookingStore.booking?.status}`)
        }}</span>
        <span
          v-if="bookingStore.booking?.status === BookingStatus.ACCEPTED"
          class="mx-auto text-green-500 bg-green-100"
          >{{ t(`bookingStatus.${bookingStore.booking?.status}`) }}</span
        >
        <span
          v-if="bookingStore.booking?.status === BookingStatus.REJECTED"
          class="mx-auto text-red-500 bg-red-100"
          >{{ t(`bookingStatus.${bookingStore.booking?.status}`) }}</span
        >
      </div>
    </div>
    <div class="ticket-details">
      <div class="detail-row">
        <span class="detail-label">Sân:</span>
        <span class="detail-value"
          >{{ bookingStore.booking?.fieldName }} -
          {{ bookingStore.booking?.partialFieldName }}</span
        >
      </div>
      <div class="detail-row">
        <span class="detail-label">Tiền cọc:</span>
        <span class="detail-value"
          >{{ Math.floor(bookingStore.booking?.depositAmount ?? 0).toLocaleString() }} VNĐ</span
        >
      </div>
      <div class="detail-row">
        <span class="detail-label">Người đặt:</span>
        <span class="detail-value">{{ bookingStore.booking?.userBookingName }}</span>
      </div>
      <div class="detail-row">
        <span class="detail-label">SĐT:</span>
        <span class="detail-value">{{ bookingStore.booking?.userBookingPhoneNumber }}</span>
      </div>
      <div class="detail-row">
        <span class="detail-label">Giờ đặt:</span>
        <span
          class="detail-value"
          v-tooltip.bottom="{
            value: formatDate(bookingStore.booking?.createdAt ?? ''),
            autoHide: false
          }"
        >
          {{ formatTimeAgo(bookingStore.booking?.createdAt ?? '') }}
        </span>
      </div>
      <div class="detail-row">
        <span class="detail-label">Cập nhập gần nhất:</span>
        <span
          class="detail-value"
          v-tooltip.bottom="{
            value: formatDate(bookingStore.booking?.lastUpdatedAt ?? ''),
            autoHide: false
          }"
        >
          {{ formatTimeAgo(bookingStore.booking?.lastUpdatedAt ?? '') }}
        </span>
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
          severity="danger"
          is-outlined
          v-if="isWaiting"
          value="Từ chối"
        />

        <ActionButton
          @click.stop="onAccept"
          class="bg-green-400 mx-auto ml-4"
          value="Chấp nhận"
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
import { formatDate, formatDateVietnamese, formatTimeAgo } from '@/utils/dateUtil'
import { BookingStatus } from '@/constants/booking'
import { ref } from 'vue'

const toast = useToast()
const { t } = useI18n()

const bookingStore = useBookingStore()

const isWaiting = ref(false)

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
      isWaiting.value = bookingStore.booking?.status === BookingStatus.WAITING
    }
  }
)
</script>
<style scoped>
.ticket-details {
  flex: 1;
}

.detail-row {
  margin-bottom: 0.5rem;
}

.detail-label {
  font-weight: bold;
  margin-right: 0.5rem;
}
</style>
