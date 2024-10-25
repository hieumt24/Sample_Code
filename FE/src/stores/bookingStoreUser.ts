import {
  cencelBookingUserApi,
  getBookingInWeekByPartialFieldApi,
  getBookingRequestApi,
  postBookingRequest,
  type CreateBookingRequest,
  type GetListBookingInWeekByPartialField
} from '@/api/bookingApi'
import type { Booking } from '@/types/Booking'
import { defineStore } from 'pinia'
import { getDateFormatted, addDaysToDate } from '@/utils/dateUtil'
import { useFieldStore } from './fieldStore'
import { changeTimeToSeconds } from '@/utils/timeUtil'
import { getBookingHistoryUserApi, type GetBookingHistoryUserRequest } from '@/api/bookingUserApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'

type BookingTableItem = {
  date: string
  openTime: string
  closeTime: string
  bookings: Booking[]
}

interface State {
  searchInWeekByPartialField: GetListBookingInWeekByPartialField
  searchBookingHistory: GetBookingHistoryUserRequest
  bookings: Booking[]
  loading: boolean
  isBooking: boolean
  booking: Booking | undefined
  bookingsInWeek: BookingTableItem[]
  paymentLink: string
  totalHistory: number
  isLoadingHistory: boolean
  isAllLoaded: boolean
}

export const useBookingStoreUser = defineStore('bookingStoreUser', {
  state: (): State => {
    return {
      searchInWeekByPartialField: {
        partialFieldId: 0,
        date: '',
        startDate: '',
        endDate: ''
      },
      searchBookingHistory: {
        date: '',
        startTime: '',
        endTime: '',
        status: '',
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      bookings: [],
      loading: false,
      isBooking: false,
      booking: undefined,
      bookingsInWeek: [],
      paymentLink: '',
      isLoadingHistory: false,
      isAllLoaded: false,
      totalHistory: 0
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
      booking.startTime = changeTimeToSeconds(booking.startTime).toString()
      booking.endTime = changeTimeToSeconds(booking.endTime).toString()
      this.isBooking = true
      const { data } = await postBookingRequest(booking)
      this.isBooking = false
      // this.paymentLink = data.data.paymentLink

      console.log(data)
      return data
    },
    async cancelBooking(id: number) {
      const { data } = await cencelBookingUserApi({ id })
      console.log(data)
      return data
    },

    async getBookingHistoryLazy() {
      this.isLoadingHistory = true

      const { data } = await getBookingHistoryUserApi(this.searchBookingHistory)
      this.totalHistory = data.meta?.total ?? 0
      this.bookings = this.bookings.concat(data.data)

      this.isAllLoaded =
        this.searchBookingHistory.offset + this.searchBookingHistory.limit >= this.totalHistory

      this.isLoadingHistory = false
    },

    async getBookingHistory() {
      this.isLoadingHistory = true
      this.bookings = []
      const { data } = await getBookingHistoryUserApi(this.searchBookingHistory)
      this.totalHistory = data.meta?.total ?? 0
      this.bookings = data.data
      this.isAllLoaded =
        this.searchBookingHistory.offset + this.searchBookingHistory.limit >= this.totalHistory
      this.isLoadingHistory = false
    },

    async changePageHistoryBooking(page: number) {
      this.searchBookingHistory.offset = (page - 1) * this.searchBookingHistory.limit
      await this.getBookingHistory()
    },

    async getBookingById(bookingId: number) {
      const { data } = await getBookingRequestApi(bookingId)
      this.booking = data.data
      if (!data.success) {
        console.log(data.message)
      }
    }
  }
})
