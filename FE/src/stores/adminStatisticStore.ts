import {
  getBookingStatisticByMonthApi,
  getBookingStatisticStatusApi,
  getUserStatisticByMonth,
  type SearchBookingStatistic,
  type SearchUserStatistic
} from '@/api/statisticApi'
import type {
  StatisticBookingByMonth,
  StatisticByBookingStatus,
  StatisticByWeekDay,
  StatisticUserByMonth
} from '@/types/Statistics'
import { defineStore } from 'pinia'

interface State {
  searchStatistic: SearchBookingStatistic
  statisticBookingByMonth: StatisticBookingByMonth[]
  statisticBookingByWeekDay: StatisticByWeekDay[]
  statisticBookingByStatus: StatisticByBookingStatus | undefined
  statisticByBookingStatus: StatisticByBookingStatus[]
  statisticUserByMonth: StatisticUserByMonth[]
  loading: boolean
  total: number
}

export const useStatisticAdminStore = defineStore('statisticAdminStore', {
  state: (): State => {
    return {
      searchStatistic: {
        fromDate: '',
        toDate: ''
      },
      statisticBookingByMonth: [],
      statisticBookingByWeekDay: [],
      statisticBookingByStatus: undefined,
      statisticByBookingStatus: [],
      statisticUserByMonth: [],
      loading: false,
      total: 0
    }
  },
  actions: {
    async getBookingByMonth() {
      this.loading = true
      const { data } = await getBookingStatisticByMonthApi(this.searchStatistic)
      this.loading = false
      this.statisticBookingByMonth = data.data ?? []
      this.total = data.meta?.total ?? 0
      return data
    },
    async getUserByMonth() {
      this.loading = true
      const { data } = await getUserStatisticByMonth(this.searchStatistic)
      this.loading = false
      this.statisticUserByMonth = data.data ?? []
      this.total = data.meta?.total ?? 0
      return data
    },
    async getBookingByStatus() {
      this.loading = true
      const { data } = await getBookingStatisticStatusApi(this.searchStatistic)
      this.loading = false
      this.statisticBookingByStatus = data.data ?? []
      this.total = data.meta?.total ?? 0
      return data
    }
  }
})
