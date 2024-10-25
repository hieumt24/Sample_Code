import {
  getBookingStatisticByMonthApi,
  getBookingStatisticSlotApi,
  getBookingStatisticStatusApi,
  getUserStatisticByMonth,
  type SearchBookingStatistic,
  type SearchUserStatistic
} from '@/api/statisticApi'
import type {
  StatisticBookingByMonth,
  StatisticByBookingStatus,
  StatisticBySlot,
  StatisticByWeekDay,
  StatisticUserByMonth
} from '@/types/Statistics'
import { defineStore } from 'pinia'

interface State {
  statisticBookingByMonth: StatisticBookingByMonth[]
  statisticBookingSlotByMonth: StatisticBySlot | undefined
  statisticBookingByWeekDay: StatisticByWeekDay[]
  statisticBookingByStatus: StatisticByBookingStatus | undefined
  statisticByBookingStatus: StatisticByBookingStatus[]
  statisticUserByMonth: StatisticUserByMonth[]
  searchStatistic: SearchBookingStatistic
  loading: boolean
  total: number
}

export const useStatisticOwnerStore = defineStore('statisticOwnerStore', {
  state: (): State => {
    return {
      statisticBookingByMonth: [],
      statisticBookingByWeekDay: [],
      statisticBookingSlotByMonth: undefined,
      statisticBookingByStatus: undefined,
      statisticByBookingStatus: [],
      statisticUserByMonth: [],
      searchStatistic: {
        fromDate: '',
        toDate: ''
      },
      loading: false,
      total: 0
    }
  },
  actions: {
    async getBookingByMonth() {
      if (this.searchStatistic.fieldId === 0) {
        return
      }
      this.loading = true
      const { data } = await getBookingStatisticByMonthApi(this.searchStatistic)
      this.loading = false
      this.statisticBookingByMonth = data.data ?? []
      this.total = data.meta?.total ?? 0
      return data
    },
    async getBookingSlotsByMonth() {
      if (this.searchStatistic.fieldId === 0) {
        return
      }
      this.loading = true
      const { data } = await getBookingStatisticSlotApi(this.searchStatistic)
      this.loading = false
      this.statisticBookingSlotByMonth = data.data ?? []
      this.total = data.meta?.total ?? 0
      return data
    },
    async getBookingByStatus() {
      if (this.searchStatistic.fieldId === 0) {
        return
      }
      this.loading = true
      const { data } = await getBookingStatisticStatusApi(this.searchStatistic)
      this.loading = false
      this.statisticBookingByStatus = data.data ?? []
      this.total = data.meta?.total ?? 0
      return data
    }
  }
})
