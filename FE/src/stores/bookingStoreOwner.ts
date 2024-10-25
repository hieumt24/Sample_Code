import {
  getBookingByFieldOwnerApi,
  getBookingInWeekByPartialFieldApi,
  getBookingInWeekByPartialFieldByOwnerApi,
  getBookingRequestApi,
  postBookingRequest,
  updateBookingStatusOwnerApi,
  type CreateBookingRequest,
  type GetListBookingInWeekByPartialField,
  type SearchBookingByFieldOwner
} from '@/api/bookingApi'
import type { Booking } from '@/types/Booking'
import { defineStore } from 'pinia'
import {
  getLastDayOfWeek,
  getFirstDayOfWeek,
  getDateFormatted,
  addDaysToDate
} from '@/utils/dateUtil'
import { useFieldStore } from './fieldStore'
import { changeTimeToSeconds } from '@/utils/timeUtil'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import { BookingStatus } from '@/constants/booking'

export type BookingTableItem = {
  date: string
  openTime: string
  closeTime: string
  bookings: Booking[]
}

interface State {
  searchInWeekByPartialField: GetListBookingInWeekByPartialField
  searchBookByFieldByOwner: SearchBookingByFieldOwner
  bookings: Booking[]
  bookingsByField: Booking[]
  loading: boolean
  tableLoading: boolean
  isBooking: boolean
  booking: Booking | undefined
  bookingsInWeek: BookingTableItem[]
  paymentLink: string
  totalTable: number
}

export const useBookingStore = defineStore('bookingStore', {
  state: (): State => {
    return {
      searchInWeekByPartialField: {
        partialFieldId: 0,
        date: '',
        startDate: '',
        endDate: ''
      },
      searchBookByFieldByOwner: {
        fieldId: 0,
        date: undefined,
        startTime: undefined,
        endTime: undefined,
        status: undefined,
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      bookings: [],
      bookingsByField: [],
      loading: false,
      tableLoading: false,
      isBooking: false,
      booking: undefined,
      bookingsInWeek: [],
      paymentLink: '',
      totalTable: 0
    }
  },
  actions: {
    async getBookingInWeekByPartialField(partialFieldId: number, fromDate: string, toDate: string) {
      this.bookingsInWeek = []
      const fieldStore = useFieldStore()
      if (this.loading) return

      this.loading = true

      const { data } = await getBookingInWeekByPartialFieldApi({
        partialFieldId: partialFieldId,
        startDate: getDateFormatted(fromDate),
        endDate: getDateFormatted(toDate)
      })

      for (let i = 0; i < 7; i++) {
        const date = addDaysToDate(fromDate, i)
        const openTime = fieldStore.field?.openTime ?? '00:00'
        const closeTime = fieldStore.field?.closeTime ?? '23:00'
        const bookings = data.data.filter((booking) => booking.date === date)
        this.bookingsInWeek.push({ date, openTime, closeTime, bookings })
      }
      this.loading = false
      return data
    },

    async createABooking(booking: CreateBookingRequest) {
      this.isBooking = true
      const { data } = await postBookingRequest(booking)
      this.isBooking = false
      return data
    },

    //OWNER
    async getBookingInWeekByPartialFieldOwner(
      partialFieldId: number,
      fromDate: string,
      toDate: string
    ) {
      const bookingsInWeekOwner: BookingTableItem[] = []
      const fieldStore = useFieldStore()

      const { data } = await getBookingInWeekByPartialFieldByOwnerApi({
        partialFieldId: partialFieldId,
        startDate: getDateFormatted(fromDate),
        endDate: getDateFormatted(toDate)
      })

      for (let i = 0; i < 7; i++) {
        const date = addDaysToDate(fromDate, i)
        const openTime = fieldStore.field?.openTime ?? '00:00'
        const closeTime = fieldStore.field?.closeTime ?? '23:00'
        const bookings = data.data.filter(
          (booking) =>
            booking.date === date &&
            (booking.status == BookingStatus.WAITING || booking.status == BookingStatus.ACCEPTED)
        )
        bookingsInWeekOwner.push({ date, openTime, closeTime, bookings })
      }

      return bookingsInWeekOwner
    },

    async getBookingById(bookingId: number) {
      const { data } = await getBookingRequestApi(bookingId)
      this.booking = data.data
      if (!data.success) {
        console.log(data.message)
      }
      return data
    },

    async getBookingByFieldId() {
      this.tableLoading = true
      const { data } = await getBookingByFieldOwnerApi({
        ...this.searchBookByFieldByOwner
      })
      this.bookingsByField = data.data
      this.totalTable = data.meta?.total ?? 0
      this.tableLoading = false
      return data
    },

    async acceptBookingStatusOwner(bookingId: number) {
      const { data } = await updateBookingStatusOwnerApi({ id: bookingId, status: 'ACCEPTED' })
      this.booking = data.data
      if (!data.success) {
        console.log(data.message)
      }
    },

    async rejectBookingStatusOwner(bookingId: number) {
      const { data } = await updateBookingStatusOwnerApi({ id: bookingId, status: 'REJECTED' })
      this.booking = data.data
      if (!data.success) {
        console.log(data.message)
      }
    },

    async changePageTableOwner(limit: number, offset: number) {
      this.searchBookByFieldByOwner.limit = limit
      this.searchBookByFieldByOwner.offset = offset
      await this.getBookingByFieldId()
    }
  }
})
