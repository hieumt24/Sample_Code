<template>
  <Dialog
    :visible="open"
    :header="bookingStoreUser.booking?.fieldName"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="ticket-dialog"
  >
    <div class="ticket">
      <div class="ticket-header">
        <h1>{{ fieldStore.field?.address }}</h1>
      </div>

      <div class="ticket-body">
        <div class="ticket-image">
          <div v-if="isLoading" class="loading-overlay">
            <i class="pi pi-spin pi-spinner" style="font-size: 2rem"></i>
          </div>
          <img :src="fieldStore.field?.cover" :alt="fieldStore.field?.name" />
        </div>

        <div class="ticket-details">
          <div class="detail-row">
            <span class="detail-label">Sân:</span>
            <span class="detail-value">{{ bookingStoreUser.booking?.partialFieldName }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">Ngày:</span>
            <span class="detail-value">{{ bookingStoreUser.booking?.date }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">Giờ bắt đầu:</span>
            <span class="detail-value">{{ bookingStoreUser.booking?.startTime }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">Giờ kết thúc:</span>
            <span class="detail-value">{{ bookingStoreUser.booking?.endTime }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">Tiền cọc:</span>
            <span class="detail-value"
              >{{
                Math.floor(bookingStoreUser.booking?.depositAmount ?? 0).toLocaleString()
              }}
              VNĐ</span
            >
          </div>
          <div class="detail-row">
            <span class="detail-label">Giờ đặt:</span>
            <span
              class="detail-value"
              v-tooltip.bottom="{
                value: formatDate(bookingStoreUser.booking?.createdAt ?? ''),
                autoHide: false
              }"
            >
              {{ formatTimeAgo(bookingStoreUser.booking?.createdAt ?? '') }}
            </span>
          </div>
          <div class="detail-row">
            <span class="detail-label">Cập nhập gần nhất:</span>
            <span
              class="detail-value"
              v-tooltip.bottom="{
                value: formatDate(bookingStoreUser.booking?.lastUpdatedAt ?? ''),
                autoHide: false
              }"
            >
              {{ formatTimeAgo(bookingStoreUser.booking?.lastUpdatedAt ?? '') }}
            </span>
          </div>
        </div>
      </div>

      <div class="ticket-footer">
        <div class="footer-actions">
          <router-link
            :to="`/user/field/${bookingStoreUser.booking?.fieldId}`"
            target="_blank"
            rel="noopener"
            class="p-0"
          >
            <Button label="Xem sân" class="h-full" />
          </router-link>
          <Button
            v-if="
              bookingStoreUser.booking?.status === 'ACCEPTED' && !bookingStoreUser.booking?.isRated
            "
            icon="pi pi-star"
            label="Đánh giá"
            class="p-button-success h-full"
            @click="onRating"
          />
          <Button
            v-if="bookingStoreUser.booking?.status === 'ACCEPTED' && !isBookingInThePast"
            @click="onFindOpponent"
            icon="pi pi-users"
            label="Tạo tìm đối"
            class="p-button-success h-full"
          />
          <Button
            v-if="
              bookingStoreUser.booking?.status === 'ACCEPTED' && bookingStoreUser.booking?.isRated
            "
            icon="pi pi-star-fill"
            label="Đánh giá của bạn"
            class="p-button-success h-full"
            @click="onDetailRating"
          />
          <Button
            v-if="bookingStoreUser.booking?.status === BookingStatus.WAITING"
            label="Huỷ"
            icon="pi pi-times"
            severity="danger"
            outlined
            class="h-full"
            @click="onOpenConfirmDialogCancel"
          />
        </div>

    <div class="my-2 mt-5 flex justify-between border-t border-gray-300 pt-4">
      <ActionButton
        @click.stop="emit('close')"
        class="bg-green-400 mr-2"
        is-outlined
        value="Đóng"
      />
      <div>
        <div
          class="booking-status"
          :class="{
            'text-lime-600': bookingStoreUser.booking?.status === BookingStatus.ACCEPTED,
            'text-red-600': bookingStoreUser.booking?.status === BookingStatus.REJECTED,
            'text-blue-600': bookingStoreUser.booking?.status === BookingStatus.WAITING,
            'text-gray-600': bookingStoreUser.booking?.status === BookingStatus.CANCELED
          }"
        >
          <i
            :class="{
              'pi pi-check-circle text-green-500':
                bookingStoreUser.booking?.status === BookingStatus.ACCEPTED,
              'pi pi-ban text-red-500': bookingStoreUser.booking?.status === BookingStatus.REJECTED,
              'pi pi-wave-pulse text-violet-500':
                bookingStoreUser.booking?.status === BookingStatus.WAITING,
              'pi pi-undo text-gray-400':
                bookingStoreUser.booking?.status === BookingStatus.CANCELED
            }"
            style="font-size: 0.8rem"
          ></i>
          <span class="font-bold ml-2"
            >Trạng thái: {{ t(`bookingStatus.${bookingStoreUser.booking?.status}`) }}</span
          >
        </div>
      </div>
    </div>
  </Dialog>

  <RatingFieldDialog
    :open="showRatingDialog"
    @close="showRatingDialog = false"
    :booking-id="id ?? 0"
    :field-id="bookingStoreUser.booking?.fieldId ?? 0"
    :is-create="isCreateRate"
  />

  <ConfirmDialog
    :open="isConfirmCancelDiablog"
    :message="'Bạn có chắc chắn muốn huỷ lịch này?'"
    @close="isConfirmCancelDiablog = false"
    @submit="onCancelBooking"
  />

  <OpponentFindingWithBookingCreateDialog
    :open="showOpponentFindingCreateDialog"
    @close="showOpponentFindingCreateDialog = false"
    :booking-id="bookingStoreUser.booking?.id"
  />
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { computed, watch } from 'vue'
import { ref } from 'vue'
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import type { Booking } from '@/types/Booking'
import { useBookingStoreUser } from '@/stores/bookingStoreUser'
import { useFieldStore } from '@/stores/fieldStore'
import Button from 'primevue/button'
import OpponentFindingWithBookingCreateDialog from './OpponentFindingWithBookingCreateDialog.vue'
import { BookingStatus } from '@/constants/booking'
import RatingFieldDialog from './RatingFieldDialog.vue'
import ConfirmDialog from './ConfirmDialog.vue'
import { useToast } from 'primevue/usetoast'
import { formatDate, formatTimeAgo } from '@/utils/dateUtil'

const { t } = useI18n()
const toast = useToast()

const bookingStoreUser = useBookingStoreUser()
const fieldStore = useFieldStore()
const partialFieldStore = usePartialFieldStore()
const showOpponentFindingCreateDialog = ref(false)
const showRatingDialog = ref(false)
const isLoading = ref(false)
const isConfirmCancelDiablog = ref(false)
const isCreateRate = ref(false)
const isBookingInThePast = computed(() => {
  const booking = bookingStoreUser.booking
  if (!booking) return false

  const bookingDateTime = new Date(`${booking.date}T${booking.startTime}`)
  const now = new Date()

  return bookingDateTime < now
})
const props = defineProps<{
  open: boolean
  id?: number
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', booking: Booking): void
}>()

const onFindOpponent = () => {
  showOpponentFindingCreateDialog.value = true
}

const onRating = () => {
  isCreateRate.value = true
  showRatingDialog.value = true
}

const onDetailRating = () => {
  isCreateRate.value = false
  showRatingDialog.value = true
}

const onOpenConfirmDialogCancel = () => {
  isConfirmCancelDiablog.value = true
}

const onCancelBooking = async () => {
  await bookingStoreUser.cancelBooking(props.id ?? 0).then((data) => {
    if (data.success) {
      isConfirmCancelDiablog.value = false
      emit('close')
      toast.add({
        severity: 'success',
        summary: 'Thành công',
        detail: 'Đã huỷ lịch đặt thành công'
      })
      bookingStoreUser.getBookingHistory()
    }
  })
}

watch(
  () => props.open,
  async (value) => {
    if (value && props.id) {
      isLoading.value = true
      fieldStore.field = undefined
      bookingStoreUser.booking = undefined
      await bookingStoreUser.getBookingById(props.id).then(async () => {
        await fieldStore.getFieldById(bookingStoreUser.booking?.fieldId ?? 1)
      })
      isLoading.value = false
    }
  }
)
</script>
<style scoped>
.ticket-dialog {
  max-width: 800px;
}

.ticket {
  display: flex;
  flex-direction: column;
  background-color: #f8f9fa;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.ticket-header {
  background-color: #4caf50;
  color: white;
  padding: 1rem;
  text-align: center;
}

.ticket-body {
  display: flex;
  padding: 1rem;
}

.ticket-image {
  flex: 0 0 40%;
  position: relative;
}

.ticket-image img {
  width: 100%;
  height: auto;
  object-fit: cover;
  border-radius: 4px;
}

.loading-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: rgba(255, 255, 255, 0.7);
}

.ticket-details {
  flex: 1;
  padding-left: 1rem;
}

.detail-row {
  margin-bottom: 0.5rem;
}

.detail-label {
  font-weight: bold;
  margin-right: 0.5rem;
}

.ticket-actions {
  display: flex;
  justify-content: center;
  padding: 1rem;
  gap: 1rem;
}

.ticket-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background-color: #e9ecef;
}

.footer-actions {
  display: flex;
  gap: 0.5rem;
}

.booking-status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: bold;
}
</style>
