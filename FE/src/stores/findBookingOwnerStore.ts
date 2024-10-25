import type { Booking } from '@/types/Booking'
import { defineStore } from 'pinia'
import { getAvailableByFieldApi, type GetAvailableRequest } from '@/api/findBookingApi'
import type { PartialField } from '@/types/PartialField'

export type BookingTableItem = {
  date: string
  openTime: string
  closeTime: string
  bookings: Booking[]
}

interface State {
  searchAvailableRequest: GetAvailableRequest
  partialFields: PartialField[]
  loading: boolean
}

export const useFindBookingOwnerStore = defineStore('findBookingOwner', {
  state: (): State => {
    return {
      searchAvailableRequest: {
        fieldId: 0,
        date: '',
        startTime: 0
      },
      partialFields: [],
      loading: false
    }
  },
  actions: {
    async getAvailableByField(params: GetAvailableRequest) {
      this.loading = true
      const { data } = await getAvailableByFieldApi(params)
      this.partialFields = data.data
      this.loading = false
      return data
    }
  }
})
