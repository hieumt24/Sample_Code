<template>
  <div class="flex h-10">
    <div class="flex items-center">
      <button
        class="mr-2 border text-green-500 border-green-500 rounded-md h-9 px-2 hover:bg-green-500 hover:text-white"
        @click.stop="goToToday"
      >
        Hôm nay
      </button>
      <CustomCalendar
        name="date"
        :label="$t('searchTab.date')"
        no-label
        v-model="selectedSpecificDate"
        @update:modelValue="onChangeDate"
      />
      <div class="flex ml-2">
        <button @click.stop="onPreviousWeek">
          <i class="pi pi-arrow-circle-left" style="font-size: 1.5rem"></i>
        </button>
        <div class="font-semibold mx-2">
          {{ formatDateVietnamese(fromDateWeek) }} ~ {{ formatDateVietnamese(toDateWeek) }}
        </div>
        <button @click.stop="onNextWeek">
          <i class="pi pi-arrow-circle-right" style="font-size: 1.5rem"></i>
        </button>
      </div>
    </div>
  </div>
  <div class="bg-white relative">
    <div
      v-if="bookingStore.loading"
      class="absolute h-full w-full bg-gray-200 flex items-center opacity-80 z-50 cursor-wait"
    >
      <i class="pi pi-spin pi-spinner mx-auto" style="font-size: 2rem"></i>
    </div>
    <div class="z-50" ref="test" style="height: 620px"></div>
  </div>

  <Dialog
    v-model:visible="openWarningTimeDialog"
    @hide="openWarningTimeDialog = false"
    modal
    :draggable="false"
    header="Thông báo"
    :style="{ width: '25rem' }"
  >
    <span class="text-surface-500 dark:text-surface-400 block mb-8">
      Bạn chỉ có thể đặt sân 1 tiếng, 1 tiếng 30 phút hoặc 2 tiếng.
    </span>
    <div class="flex justify-end gap-2">
      <Button
        type="button"
        label="Quay lại"
        severity="secondary"
        @click="openWarningTimeDialog = false"
      ></Button>
    </div>
  </Dialog>

  <Dialog
    v-model:visible="openPastTimeTimeDialog"
    @hide="openPastTimeTimeDialog = false"
    modal
    :draggable="false"
    header="Thông báo"
    :style="{ width: '25rem' }"
  >
    <span class="block mb-8 text-green-400">
      Lịch này ở quá khứ, xin vui lòng chọn lịch trong tương lai.
    </span>
    <div class="flex justify-end gap-2">
      <Button
        type="button"
        label="Quay lại"
        severity="secondary"
        @click="openPastTimeTimeDialog = false"
      ></Button>
    </div>
  </Dialog>

  <BookingDialog
    :open="bookingOpen"
    :date="selectedDate"
    :partial-id="partialFieldId"
    :init-start-time="startTimeBooking"
    :init-end-time="endTimeBooking"
    :disabled="true"
    @close="bookingOpen = false"
    @error-close="closeBookingDialog"
    @submit="onAfterSubmit"
  />
</template>

<script setup lang="ts">
import '@toast-ui/calendar/dist/toastui-calendar.min.css'
import { onMounted, readonly, ref } from 'vue'
import { BOOKING_TABLE_OPTIONS } from './types'
import type { EventInfo } from 'node_modules/@toast-ui/calendar/types/types/eventBus'
import type { EventObject } from 'node_modules/@toast-ui/calendar/types'
import Dialog from 'primevue/dialog'
import {
  getFirstDayOfWeek,
  getLastDayOfWeek,
  addDaysToDate,
  getCurrentDate,
  formatDateVietnamese,
  getFirstDayOfWeekBySpecificDate
} from '@/utils/dateUtil'
import { divideSchedule, INACTIVE_TIME, setEventColor } from '@/utils/bookingUtils'
import 'primeicons/primeicons.css'
import Button from 'primevue/button'
import { useBookingStore } from '@/stores/bookingStoreOwner'
import { getCurrentTime, isBefore } from '@/utils/timeUtil'
import BookingDialog from '../dialogs/BookingDialog.vue'
import { getDateFormatted, getDateFromDate } from '@/utils/dateUtil'
import { DATE_FORMAT_DD_MM_YYYY, isAfterDate } from '@/utils/dateUtil'
import { getTimeFromDate } from '@/utils/timeUtil'
import type { Booking } from '@/types/Booking'
import { useSlotStore } from '@/stores/slotStore'
import { computed } from 'vue'
import dayjs from 'dayjs'
import { changeTimeToSeconds } from '@/utils/timeUtil'
import { useFieldStore } from '@/stores/fieldStore'
import { useScheduleStore } from '@/stores/scheduleStore'
// @ts-ignore
import Calendar from '@toast-ui/calendar'
import CustomCalendar from '../calendar/CustomCalendar.vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const props = defineProps<{
  partialFieldId: number
  date?: string
  isFixedSlot: boolean
  fieldId: number
  fieldName?: string
  fromDate?: string
  toDate?: string
}>()

const bookingStore = useBookingStore()
const slotStore = useSlotStore()
const fieldStore = useFieldStore()
const scheduleStore = useScheduleStore()

const test = ref(null)
const calendar = ref<Calendar>()
const openWarningTimeDialog = ref(false)
const openPastTimeTimeDialog = ref(false)
const fromDateWeek = ref(props.fromDate ?? getFirstDayOfWeek())
const toDateWeek = ref(props.toDate ?? getLastDayOfWeek())
const bookingOpen = ref(false)

const selectedDate = ref('')
const selectedSpecificDate = ref(getCurrentDate())
const startTimeBooking = ref('')
const endTimeBooking = ref('')
const isReadOnlyCalendar = computed(() => toDateWeek.value < getCurrentDate())
const inactiveTimes = computed(() =>
  scheduleStore.schedules.map((s) => ({
    id: s.id,
    startDate: s.startTime.substring(0, 10),
    endDate: s.endTime.substring(0, 10),
    startTime: s.startTime.substring(11, 16),
    endTime: s.endTime.substring(11, 16)
  }))
)

const onChangeDate = () => {
  fromDateWeek.value = getFirstDayOfWeekBySpecificDate(selectedSpecificDate.value)
  toDateWeek.value = addDaysToDate(fromDateWeek.value, 6)
  calendar.value.setDate(selectedSpecificDate.value)
  fillCalendar()
}

const goToToday = () => {
  fromDateWeek.value = getFirstDayOfWeek()
  toDateWeek.value = getLastDayOfWeek()
  calendar.value.setDate(getCurrentDate())
  fillCalendar()
}

const onPreviousWeek = () => {
  fromDateWeek.value = addDaysToDate(fromDateWeek.value, -7)
  toDateWeek.value = addDaysToDate(toDateWeek.value, -7)

  calendar.value.prev()
  fillCalendar()
}

const onNextWeek = () => {
  fromDateWeek.value = addDaysToDate(fromDateWeek.value, 7)
  toDateWeek.value = addDaysToDate(toDateWeek.value, 7)
  calendar.value.next()
  fillCalendar()
}

const fillCalendar = async () => {
  calendar.value.clear()
  await fillInactiveTime()

  await bookingStore.getBookingInWeekByPartialField(
    props.partialFieldId,
    fromDateWeek.value,
    toDateWeek.value
  )

  const allBookings = bookingStore.bookingsInWeek
    .flatMap((b) => b.bookings)
    .map((b) => ({
      id: b.id,
      calendarId: 'cal1',
      title: t('bookingStatus.' + b.status),
      start: b.date + 'T' + b.startTime,
      end: b.date + 'T' + b.endTime,
      backgroundColor: setEventColor(b.status),
      isReadOnly: isBefore(b.date, new Date().toISOString().split('T')[0])
    }))
    .filter(
      (b) =>
        !scheduleStore.schedules.some(
          (i) =>
            (i.startTime > b.start && i.startTime < b.end) ||
            (i.endTime > b.start && i.endTime < b.end) ||
            (i.startTime === b.start && i.endTime === b.end) ||
            (i.startTime < b.start && i.endTime > b.end)
        )
    )

  calendar.value.createEvents(allBookings.map((b) => ({ ...b })))

  if (props.isFixedSlot) {
    await fillFixedSlot()
  }
}

const fillFixedSlot = async () => {
  await slotStore.getSlotByField(props.fieldId)

  const currentDateTemp = getCurrentDate() + 'T' + getCurrentTime()

  for (let i = 0; i < 7; i++) {
    const date = addDaysToDate(fromDateWeek.value, i)

    const bookingInThisDate = bookingStore.bookingsInWeek.find((b) => b.date === date)?.bookings
    const inactiveTimeThisDateBefore = inactiveTimes.value.find(
      (s) => s.startDate === date && s.endDate < date
    )

    const inactiveTimeThisDateAfter = inactiveTimes.value.find(
      (s) => s.startDate === date && s.endDate > date
    )

    const inactiveTimeThisDateInside = inactiveTimes.value.filter(
      (s) => s.startDate === date && s.endDate === date
    )

    const filteredSlots = slotStore.slots.filter(
      (s) =>
        !bookingInThisDate?.some((b) => {
          const slotStartTimeSeconds = changeTimeToSeconds(s.startTime)
          const slotEndTimeSeconds = changeTimeToSeconds(s.endTime)
          const bookingStartTimeSeconds = changeTimeToSeconds(b.startTime)
          const bookingEndTimeSeconds = changeTimeToSeconds(b.endTime)

          return (
            (slotStartTimeSeconds > bookingStartTimeSeconds &&
              slotStartTimeSeconds < bookingEndTimeSeconds) ||
            (slotEndTimeSeconds > bookingStartTimeSeconds &&
              slotEndTimeSeconds < bookingEndTimeSeconds) ||
            (bookingStartTimeSeconds === slotStartTimeSeconds &&
              bookingEndTimeSeconds === slotEndTimeSeconds) ||
            (slotStartTimeSeconds < bookingStartTimeSeconds &&
              slotEndTimeSeconds > bookingEndTimeSeconds)
          )
        }) &&
        !inactiveTimeThisDateInside.some((i) => {
          const slotStartTimeSeconds = changeTimeToSeconds(s.startTime)
          const slotEndTimeSeconds = changeTimeToSeconds(s.endTime)
          const inactiveStartTimeSeconds = changeTimeToSeconds(i.startTime)
          const inactiveEndTimeSeconds = changeTimeToSeconds(i.endTime)

          return (
            (slotStartTimeSeconds > inactiveStartTimeSeconds &&
              slotStartTimeSeconds < inactiveEndTimeSeconds) ||
            (slotEndTimeSeconds > inactiveStartTimeSeconds &&
              slotEndTimeSeconds < inactiveEndTimeSeconds) ||
            (inactiveStartTimeSeconds === slotStartTimeSeconds &&
              inactiveEndTimeSeconds === slotEndTimeSeconds) ||
            (slotStartTimeSeconds < inactiveStartTimeSeconds &&
              slotEndTimeSeconds > inactiveEndTimeSeconds)
          )
        }) &&
        !(
          inactiveTimeThisDateBefore &&
          changeTimeToSeconds(inactiveTimeThisDateBefore.endTime) > changeTimeToSeconds(s.startTime)
        ) &&
        !(
          inactiveTimeThisDateAfter &&
          changeTimeToSeconds(inactiveTimeThisDateAfter.startTime) < changeTimeToSeconds(s.endTime)
        )
    )

    calendar.value.createEvents(
      filteredSlots.map((s) => {
        const isPastTime = currentDateTemp > date + 'T' + s.startTime
        return {
          id: s.id + date,
          calendarId: 'cal1',
          title: isPastTime ? 'Đã qua' : 'Trống',
          start: date + 'T' + s.startTime,
          end: date + 'T' + s.endTime,
          backgroundColor: isPastTime ? '#cbd5e1' : '#6ee7b7',
          isReadOnly: isPastTime,
          customStyle: {
            border: 'none',
            fontSize: '15px'
          }
        }
      })
    )
  }
}

const fillInactiveTime = async () => {
  scheduleStore.search = {
    fieldId: 0,
    startDate: fromDateWeek.value,
    endDate: toDateWeek.value,
    limit: 20,
    offset: 0
  }

  scheduleStore.selectedFieldId = props.fieldId

  await scheduleStore.getScheduleList()
  scheduleStore.schedules = scheduleStore.schedules.flatMap((s) => divideSchedule(s))
  scheduleStore.schedules.map((x) => {
    console.log(x.startTime + ' ' + x.endTime)
    calendar.value.createEvents([
      {
        id: x.id + 'inactive',
        calendarId: 'cal1',
        title: 'Nghỉ',
        start: x.startTime,
        isReadOnly: true,
        end: x.endTime,
        color: '#000',
        backgroundColor: INACTIVE_TIME,
        dragBgColor: '#f9d9d9',
        borderColor: '#f9d9d9',
        customStyle: {
          border: 'none',
          display: 'flex',
          justifyContent: 'center',
          justifyItems: 'center',
          fontSize: '30px'
        }
      }
    ])
  })
}

const onAfterSubmit = async (newBooking: Booking) => {
  calendar.value.createEvents([
    {
      id: newBooking.id,
      calendarId: 'cal1',
      title: t('bookingStatus.' + newBooking.status),
      start: newBooking.date + 'T' + newBooking.startTime,
      end: newBooking.date + 'T' + newBooking.endTime,
      backgroundColor: setEventColor(newBooking.status),
      isReadOnly: false
    }
  ])
  await fillCalendar()
}

onMounted(async () => {
  const canlendarSettings = {
    defaultView: 'week',
    isReadOnly: false,
    usageStatistics: false,
    timezone: {
      zones: [
        {
          timezoneName: 'Asia/Ho_Chi_Minh',
          displayLabel: 'TIme'
        }
      ]
    },
    calendars: [
      {
        id: 'cal1',
        name: 'Personal',
        backgroundColor: '#03bd9e'
      }
    ],
    week: {
      dayNames: ['Chủ Nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'],
      hourStart: Math.floor(Number(fieldStore.field?.openTime.substring(0, 2))) ?? 6,
      startDayOfWeek: 1,
      hourEnd: Math.ceil(Number(fieldStore.field?.closeTime.substring(0, 2))) ?? 24,
      taskView: false,
      eventView: ['time']
    },
    gridSelection: true
  }

  calendar.value = new Calendar(test.value, canlendarSettings)

  await fillCalendar()

  calendar.value.on('clickEvent', (data: EventInfo) => {
    if (data.event.isReadOnly || isReadOnlyCalendar.value) {
      return
    }
    const startTimeOfBooking = dayjs(data.event.start + 0)
    const currentTime = dayjs()
    if (startTimeOfBooking.isBefore(currentTime)) {
      openPastTimeTimeDialog.value = true
      return
    }
    if (data.event.title === 'Trống') {
      startTimeBooking.value = dayjs(data.event.start + 0).format('HH:mm')
      endTimeBooking.value = dayjs(data.event.end + 0).format('HH:mm')
      selectedDate.value = dayjs(data.event.start + 0).format(DATE_FORMAT_DD_MM_YYYY)
      bookingOpen.value = true
    }
  })

  calendar.value.on('selectDateTime', (eventObj: EventObject) => {
    if (props.isFixedSlot) {
      return
    }
    const { start, end } = eventObj
    const startTime = new Date(start)
    const endTime = new Date(end)
    const diff = (endTime.getTime() - startTime.getTime()) / 1000

    if (diff > 7200 || diff < 3600) {
      openWarningTimeDialog.value = true
      return
    }

    selectedDate.value = getDateFromDate(startTime, DATE_FORMAT_DD_MM_YYYY)
    startTimeBooking.value = getTimeFromDate(startTime)
    endTimeBooking.value = getTimeFromDate(endTime)
    bookingOpen.value = true
  })
})

const closeBookingDialog = () => {
  bookingOpen.value = false
  fillCalendar()
}
</script>

<style>
.toastui-calendar-panel.toastui-calendar-time {
  height: 580px !important;
}
.toastui-calendar-timegrid {
  height: 100% !important;
}
.toastui-calendar-week-view-day-names {
  height: 50px !important;
}
.toastui-calendar-event-time-content {
  display: flex;
  flex-direction: column;
  justify-content: center;
  justify-items: center;
}
.toastui-calendar-template-time {
  display: flex;
  flex-direction: column;
  justify-items: center;
  justify-content: center;
}
</style>
