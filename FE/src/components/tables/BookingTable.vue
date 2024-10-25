<template>
  <div class="grid grid-cols-3 md:grid-cols-5 xl:grid-cols-7 w-full h-full overflow-x-auto">
    <div
      v-for="bookingInDay in bookingStore.bookingsInWeek"
      :key="bookingInDay.date"
      class="h-80 col-span-1 border border-gray-300 flex flex-col"
    >
      <div class="h-6 bg-slate-500 flex flex-none">
        <span class="font-semibold mx-auto text-white text-center">{{ bookingInDay.date }}</span>
      </div>

      <div class="grow">
        <div
          v-for="booking in bookingInDay.bookings"
          class="bg-green-700 text-white"
          :key="booking.id"
        >
          {{ booking.startTime }} - {{ booking.endTime }}
        </div>
      </div>
      <div class="flex-none flex p-1">
        <ActionButton
          @click.stop="onBookingOpen(bookingInDay.date)"
          icon="ticket"
          class="bg-green-400 mx-auto"
          :is-outlined="true"
          value="Book"
        />
      </div>
    </div>
    <div v-if="bookingStore.loading" class="h-80 col-span-7 bg-gray-200 flex items-center">
      <i class="pi pi-spin pi-spinner mx-auto" style="font-size: 2rem"></i>
    </div>
  </div>

  <BookingDialog
    :open="bookingOpen"
    :date="selectedDate"
    :partial-id="partialId"
    @close="bookingOpen = false"
  ></BookingDialog>
</template>
<script setup lang="ts">
import { useBookingStore } from '@/stores/bookingStoreOwner'
import Button from 'primevue/button'
import ActionButton from '@/components/buttons/ActionButton.vue'
import BookingDialog from '../dialogs/BookingDialog.vue'
import { ref } from 'vue'

defineProps<{
  title?: string
  partialId: number
}>()

const bookingStore = useBookingStore()

const bookingOpen = ref(false)
const selectedDate = ref('')

const onBookingOpen = (date: string) => {
  selectedDate.value = date
  bookingOpen.value = true
}
</script>
