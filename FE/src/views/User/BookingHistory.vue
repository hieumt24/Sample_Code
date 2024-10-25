<template>
  <div class="container mx-auto px-4 pb-8">
    <h1 class="text-3xl font-bold mb-4">Lịch sử đặt sân</h1>

    <div class="flex flex-col lg:flex-row gap-8">
      <!-- Left Column: Search and Filters -->
      <div class="lg:w-1/3 max-w-[420px]">
        <div
          class="bg-white p-6 rounded-lg shadow-md grid grid-cols-2 md:grid-cols-1 gap-5 lg:grid-cols-2"
        >
          <h2 class="text-xl font-semibold mb-4 col-span-2">Tìm kiếm</h2>
          <CustomCalendar
            label="Từ ngày"
            maxDateToday
            v-model="bookingStoreUser.searchBookingHistory.startTime"
            class="col-span-1"
          />
          <CustomCalendar
            label="Đến ngày"
            maxDateToday
            v-model="bookingStoreUser.searchBookingHistory.endTime"
            class="col-span-1"
          />
          <div class="col-span-1">
            <h3 class="font-medium mb-2">Trạng thái</h3>
            <Dropdown
              v-model="selectedStatus"
              :options="bookingStatusOptionsTest"
              optionLabel="name"
              placeholder="Chọn trạng thái"
            />
          </div>
          <div class="col-span-2 grid grid-cols-2 gap-4 mt-5">
            <Button
              icon="pi pi-undo"
              label="Đặt lại"
              class="bg-green-400 col-span-1 w-44 ml-0.5"
              outlined
              @click="onReset"
            />
            <Button
              icon="pi pi-search"
              label="Tìm"
              class="bg-green-400 col-span-1 w-44 mr-2"
              @click="onSearch"
            />
          </div>
        </div>
      </div>

      <!-- Right Column: Booking List -->
      <div class="lg:w-2/3">
        <div class="bg-white rounded-lg shadow-md">
          <div class="p-4 border-b flex justify-between items-center">
            <h2 class="text-xl font-semibold">Danh sách</h2>
            <PagingElement
              :limit="bookingStoreUser.searchBookingHistory.limit"
              :total="bookingStoreUser.totalHistory"
              @change-page="bookingStoreUser.changePageHistoryBooking"
            />
          </div>

          <div
            v-if="bookingStoreUser.isLoadingHistory"
            class="flex justify-center items-center p-8"
          >
            <i class="pi pi-spin pi-spinner" style="font-size: 2rem"></i>
          </div>

          <div
            v-if="bookingStoreUser.bookings.length === 0 && !bookingStoreUser.isLoadingHistory"
            class="flex justify-center items-center p-8"
          >
            <p>Không có dữ liệu</p>
          </div>

          <ul v-else class="divide-y">
            <li
              v-for="booking in bookingStoreUser.bookings"
              :key="booking.id"
              @click="onBookingDetail(booking.id)"
              class="p-4 hover:bg-gray-100 cursor-pointer transition duration-150 ease-in-out my-0.5"
              :class="{
                'border-l-4 border-l-lime-500': booking?.status === BookingStatus.ACCEPTED,
                'border-l-4 border-l-red-500': booking?.status === BookingStatus.REJECTED,
                'border-l-4 border-l-blue-500': booking?.status === BookingStatus.WAITING,
                'border-l-4 border-l-gray-400': booking?.status === BookingStatus.CANCELED
              }"
            >
              <div class="flex justify-between items-center">
                <div>
                  <p class="font-medium">{{ booking.fieldName }}</p>
                  <p class="text-sm text-gray-500">
                    {{ formatDateVietnamese(booking.date) }} ({{ booking.startTime }} -
                    {{ booking.endTime }})
                  </p>
                </div>
                <div class="flex items-center">
                  <span
                    :class="{
                      'text-lime-600': booking.status === BookingStatus.ACCEPTED,
                      'text-red-600': booking.status === BookingStatus.REJECTED,
                      'text-blue-600': booking.status === BookingStatus.WAITING,
                      'text-gray-600': booking.status === BookingStatus.CANCELED
                    }"
                    class="mr-2"
                  >
                    <i
                      v-if="booking.status === BookingStatus.ACCEPTED"
                      class="pi pi-check-circle text-green-500"
                      style="font-size: 0.8rem"
                    ></i>
                    <i
                      v-else-if="booking.status === BookingStatus.REJECTED"
                      class="pi pi-ban text-red-500"
                      style="font-size: 0.8rem"
                    ></i>
                    <i
                      v-else-if="booking.status === BookingStatus.WAITING"
                      class="pi pi-wave-pulse text-violet-500"
                      style="font-size: 0.8rem"
                    ></i>
                    <i
                      v-else-if="booking.status === BookingStatus.CANCELED"
                      class="pi pi-undo text-gray-400"
                      style="font-size: 0.8rem"
                    ></i>
                    {{ t(`bookingStatus.${booking.status}`) }}
                  </span>
                  <div class="h-10 w-10 rounded-full border border-gray-300 overflow-hidden">
                    <img
                      :src="booking.fieldAvatar || '/default-field-avt.png'"
                      alt="field"
                      class="h-full w-full object-cover"
                    />
                  </div>
                </div>
              </div>
              <div
                v-if="booking.status === BookingStatus.ACCEPTED"
                class="mt-2 text-sm text-green-500"
              >
                {{ booking.isRated ? 'Đã đánh giá' : 'Chưa đánh giá' }}
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>

  <BookingHistoryUserDialog
    :open="isOpenBookingDetail"
    :id="selectedBookingId"
    @close="isOpenBookingDetail = false"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import PagingElement from '@/components/pagination/PagingElement.vue'
import { onMounted } from 'vue'
import { useBookingStoreUser } from '@/stores/bookingStoreUser'
import { BookingStatus } from '@/constants/booking'
import Dropdown from 'primevue/dropdown'
import { bookingStatusOptionsTest } from '@/constants/options'
import BookingHistoryUserDialog from '@/components/dialogs/BookingHistoryUserDialog.vue'
import { formatDateVietnamese } from '@/utils/dateUtil'
import Button from 'primevue/button'
import type { DropdownType } from '@/constants/types'

const { t } = useI18n()

const bookingStoreUser = useBookingStoreUser()

const selectedStatus = ref<DropdownType>()
const isOpenBookingDetail = ref(false)
const selectedBookingId = ref(0)

const onBookingDetail = (bookingId: number) => {
  selectedBookingId.value = bookingId
  isOpenBookingDetail.value = true
}

const onSearch = () => {
  if (selectedStatus.value) {
    bookingStoreUser.searchBookingHistory.status = selectedStatus.value.code + ''
  }
  bookingStoreUser.getBookingHistory()
}

const onReset = () => {
  bookingStoreUser.searchBookingHistory.startTime = undefined
  bookingStoreUser.searchBookingHistory.endTime = undefined
  bookingStoreUser.searchBookingHistory.status = undefined
  bookingStoreUser.getBookingHistory()
}

onMounted(async () => {
  await bookingStoreUser.getBookingHistory()
})
</script>
