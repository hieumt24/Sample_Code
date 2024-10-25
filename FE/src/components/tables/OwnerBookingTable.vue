<template>
  <div class="bg-white relative">
    <div
      v-if="isLoading"
      class="absolute h-full w-full bg-gray-200 flex items-center opacity-80 z-50 cursor-wait"
    >
      <i class="pi pi-spin pi-spinner mx-auto" style="font-size: 2rem"></i>
    </div>
    <div class="z-50" ref="test" style="height: 500px"></div>
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
      Chỉ có thể thêm lịch 1 tiếng, 1 tiếng 30 phút hoặc 2 tiếng.
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

  <BookingDialogOwner
    :open="bookingOpen"
    :date="selectedDate"
    :partial-id="partialFieldId"
    :init-start-time="startTimeBooking"
    :init-end-time="endTimeBooking"
    :disabled="isFixedSlot"
    @close="bookingOpen = false"
    @submit="onAfterSubmit"
  />

  <Dialog
    v-model:visible="openPastTimeTimeDialog"
    @hide="openPastTimeTimeDialog = false"
    modal
    :draggable="false"
    header="Thông báo"
    :style="{ width: '25rem' }"
  >
    <span class="block mb-8 text-green-400"
      >Lịch này ở quá khứ, xin vui lòng chọn lịch trong tương lai.</span
    >
    <div class="flex justify-end gap-2">
      <Button
        type="button"
        label="Quay lại"
        severity="secondary"
        @click="openPastTimeTimeDialog = false"
      ></Button>
    </div>
  </Dialog>

  <BookingAcceptDialog
    :open="openAceptBookingDialog"
    :id="selectedBookingId"
    @close="openAceptBookingDialog = false"
    @after-submit="onAfterAcceptBookingUser"
  ></BookingAcceptDialog>

  <BookingAcceptDialog
    :open="openDetailBooking"
    :id="selectedDetailBooking"
    @close="openDetailBooking = false"
    @after-submit="onAfterAccept"
  ></BookingAcceptDialog>
</template>

<script setup lang="ts">
import '@toast-ui/calendar/dist/toastui-calendar.min.css'
import { computed, onMounted, ref, watch } from 'vue'
import type { EventInfo } from 'node_modules/@toast-ui/calendar/types/types/eventBus'
import type { EventObject } from 'node_modules/@toast-ui/calendar/types'
import Dialog from 'primevue/dialog'
import {
  DATE_FORMAT_DD_MM_YYYY,
  getFirstDayOfWeek,
  getLastDayOfWeek,
  addDaysToDate,
  getDateFromDate,
  getCurrentDate
} from '@/utils/dateUtil'
import { divideSchedule, INACTIVE_TIME, setEventColor } from '@/utils/bookingUtils'
import 'primeicons/primeicons.css'
import Button from 'primevue/button'
import { useBookingStore } from '@/stores/bookingStoreOwner'
import { changeTimeToSeconds, getCurrentTime, isBefore, getTimeFromDate } from '@/utils/timeUtil'
import BookingDialog from '../dialogs/BookingDialog.vue'
import type { Booking } from '@/types/Booking'
import type { BookingTableItem } from '@/stores/bookingStoreOwner'
import {} from 'vue'
import BookingAcceptDialog from '../dialogs/BookingAcceptDialog.vue'
import BookingDialogOwner from '../dialogs/BookingDialogOwner.vue'
import { useSlotStore } from '@/stores/slotStore'
import dayjs from 'dayjs'
// @ts-ignore
import Calendar from '@toast-ui/calendar'
import { useScheduleStore } from '@/stores/scheduleStore'
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
const scheduleStore = useScheduleStore()

const isLoading = ref(false)
const test = ref(null)
const calendar = ref<Calendar>()
const openWarningTimeDialog = ref(false)
const fromDateWeek = ref(props.fromDate ?? getFirstDayOfWeek())
const toDateWeek = ref(props.toDate ?? getLastDayOfWeek())
const bookingOpen = ref(false)
const openDetailBooking = ref(false)
const selectedDate = ref('')
const startTimeBooking = ref('')
const endTimeBooking = ref('')
const bookingInWeek = ref<BookingTableItem[]>([])
const openAceptBookingDialog = ref(false)
const selectedBookingId = ref(0)
const selectedDetailBooking = ref(0)
const isReadOnlyCalendar = computed(() => toDateWeek.value < getCurrentDate())
const openPastTimeTimeDialog = ref(false)
const inactiveTimes = computed(() =>
  scheduleStore.schedules.map((s) => ({
    id: s.id,
    startDate: s.startTime.substring(0, 10),
    endDate: s.endTime.substring(0, 10),
    startTime: s.startTime.substring(11, 16),
    endTime: s.endTime.substring(11, 16)
  }))
)

const fillCalendar = async () => {
  isLoading.value = true
  calendar.value.clear()

  await fillInactiveTime()

  await bookingStore
    .getBookingInWeekByPartialFieldOwner(props.partialFieldId, fromDateWeek.value, toDateWeek.value)
    .then((data: BookingTableItem[]) => {
      bookingInWeek.value = data
    })
  isLoading.value = false

  const allBookings = bookingInWeek.value
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
    const bookingInThisDate = bookingInWeek.value.find((b) => b.date == date)?.bookings

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
          calendarId: props.partialFieldId.toString(),
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
      title: newBooking.status,
      start: newBooking.date + 'T' + newBooking.startTime,
      end: newBooking.date + 'T' + newBooking.endTime,
      backgroundColor: setEventColor(newBooking.status),
      isReadOnly: false
    }
  ])
  await fillCalendar()
}

const onAfterAcceptBookingUser = async () => {
  openAceptBookingDialog.value = false
  await fillCalendar()
}

const onAfterAccept = async () => {
  openDetailBooking.value = false
  await fillCalendar()
}

onMounted(() => {
  const tableSettings = {
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
      hourStart: 7,
      startDayOfWeek: 1,
      hourEnd: 23,
      taskView: false,
      eventView: ['time']
    }
  }

  calendar.value = new Calendar(test.value, tableSettings)

  fillCalendar()

  calendar.value.on('clickEvent', (data: EventInfo) => {
    if (data.event.isReadOnly || isReadOnlyCalendar.value) {
      return
    }
    // const startTimeOfBooking = dayjs(data.event.start + 0)
    // const currentTime = dayjs()
    // if (startTimeOfBooking.isBefore(currentTime)) {
    //   openPastTimeTimeDialog.value = true
    //   return
    // }
    switch (data.event?.title) {
      case 'Trống': {
        startTimeBooking.value = dayjs(data.event.start + 0).format('HH:mm')
        endTimeBooking.value = dayjs(data.event.end + 0).format('HH:mm')
        selectedDate.value = dayjs(data.event.start + 0).format(DATE_FORMAT_DD_MM_YYYY)
        bookingOpen.value = true
        break
      }
      case 'Chờ xác nhận': {
        openAceptBookingDialog.value = true
        selectedBookingId.value = data.event?.id
        break
      }
      case 'Thành công': {
        openDetailBooking.value = true
        selectedDetailBooking.value = data.event?.id
        break
      }
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

  calendar.value.setTheme({
    week: {
      timeGrid: {
        borderRight: '1px solid #e5e5e5',
        height: '1000px'
      },
      timeGridHalfHourLine: {
        borderBottom: '1px dotted #e5e5e5'
      },
      weekend: {
        backgroundColor: 'rgba(255, 64, 64, 0.05)'
      },
      dayGrid: {
        borderRight: 'none',
        height: '1000px'
      }
    }
  })
})

watch(
  () => [props.fromDate, props.partialFieldId],
  () => {
    if (props.fromDate === undefined || props.toDate === undefined) return
    fromDateWeek.value = props.fromDate
    toDateWeek.value = props.toDate
    calendar.value.setDate(fromDateWeek.value)
    fillCalendar()
  }
)
</script>

<style>
.toastui-calendar-panel.toastui-calendar-time {
  height: 1000px !important;
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
