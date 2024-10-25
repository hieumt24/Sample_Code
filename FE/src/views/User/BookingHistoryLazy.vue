<template>
  <div class="flex flex-col md:flex-row md:justify-between">
    <div class="">
      {{ bookingStoreUser.isLoadingHistory }}
      {{ bookingStoreUser.isAllLoaded }}
      {{ bookingStoreUser.searchBookingHistory.offset }}
      {{ bookingStoreUser.searchBookingHistory.limit }}
      {{ bookingStoreUser.totalHistory }}
    </div>

    <ul ref="bookingList" class="w-96 h-[calc(100vh-200px)] overflow-y-auto">
      <li
        v-for="booking in bookingStoreUser.bookings"
        :key="booking.id"
        class="w-full md:flex-row mb-5 block border border-gray-300 p-2"
      >
        <div>
          <div class="flex justify-between">
            <span>{{ booking.date }}</span>
            <span>{{ booking.fieldName }}</span>
          </div>
          <div>
            <span>{{ booking.startTime }} - {{ booking.endTime }}</span>
            <div>
              <i
                v-if="booking.status === BookingStatus.ACCEPTED"
                class="pi pi-check-circle text-green-500"
                style="font-size: 0.8rem"
              ></i>
              <i
                v-if="booking.status === BookingStatus.REJECTED"
                class="pi pi-ban text-red-500"
                style="font-size: 0.8rem"
              ></i>
              <i
                v-if="booking.status === BookingStatus.WAITING"
                class="pi pi-wave-pulse text-violet-500"
                style="font-size: 0.8rem"
              ></i>
              <i
                v-if="booking.status === BookingStatus.CANCELED"
                class="pi pi-wave-undo text-orange-500"
                style="font-size: 0.8rem"
              ></i>
              <span>{{ booking.status }}</span>
            </div>
          </div>
        </div>
      </li>
      <li v-if="bookingStoreUser.isLoadingHistory" class="h-10 w-full flex">
        <i class="pi pi-spin pi-spinner mx-auto" style="font-size: 2rem"></i>
      </li>
      <li ref="loadingTrigger" class="h-10 border border-gray-200"></li>
      <li v-if="bookingStoreUser.isAllLoaded" class="h-10 w-full flex">
        <h1>Đã load hết lịch sử booking</h1>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import AutoComplete from 'primevue/autocomplete'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { ref } from 'vue'
import { province } from '@/constants/area'
import { useI18n } from 'vue-i18n'
import { useFieldStore } from '@/stores/fieldStore'
import ActionButton from '@/components/buttons/ActionButton.vue'
import MultipleFieldMarkerMap from '@/components/maps/MultipleFieldMarkerMap.vue'
import SelectButton from 'primevue/selectbutton'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import PagingElement from '@/components/pagination/PagingElement.vue'
import { onMounted } from 'vue'
import { useFieldStoreUser } from '@/stores/fieldStoreUser'
import { useBookingStoreUser } from '@/stores/bookingStoreUser'
import { BookingStatus } from '@/constants/booking'
import { onUnmounted } from 'vue'

const { t } = useI18n()

const fieldStore = useFieldStoreUser()
const bookingStoreUser = useBookingStoreUser()

const filteredProvince = ref(province)
const searchDate = ref<'Specific date' | 'By range'>('Specific date')
const searchDateOptions = ref(['Specific date', 'By range'])
const loadingTrigger = ref(null)
const bookingList = ref(null)

let observer: IntersectionObserver | null = null

const schema = yup.object({
  email: yup.string().required(t('validation.required')).email(),
  password: yup.string().required(t('validation.required')).min(8, t('validation.passwordLength'))
})

const { meta, errors, defineField, handleSubmit } = useForm({
  initialValues: {
    email: '',
    password: ''
  },
  validationSchema: schema
})

const [date] = defineField('email')
const [fromDate] = defineField('password')
const [toDate] = defineField('password')
const [selectedProvince] = defineField('password')

const submit = handleSubmit((values) => {
  console.log(values)
})

onMounted(() => {
  bookingStoreUser.getBookingHistoryLazy()

  observer = new IntersectionObserver(handleIntersection, {
    root: bookingList.value,
    threshold: 1.0
  })

  if (loadingTrigger.value) {
    observer.observe(loadingTrigger.value)
  }
})

onUnmounted(() => {
  console.log('unmounted')
  if (observer) {
    observer.disconnect()
  }
})

const handleIntersection = (entries: IntersectionObserverEntry[]) => {
  const firstEntry = entries[0]

  if (
    firstEntry.isIntersecting &&
    !bookingStoreUser.isLoadingHistory &&
    !bookingStoreUser.isAllLoaded
  ) {
    bookingStoreUser.searchBookingHistory.offset += bookingStoreUser.searchBookingHistory.limit
    bookingStoreUser.getBookingHistoryLazy()
  }
}
</script>
