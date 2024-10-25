import {
  createSchedule,
  deleteScheduleApi,
  getSchedule,
  getScheduleById,
  updateSchedule,
  type ScheduleCreateRequest,
  type ScheduleListRequest,
  type ScheduleUpdateRequest
} from '@/api/scheduleApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Schedule } from '@/types/Schedule'
import { getDateFormatted } from '@/utils/dateUtil'
import { defineStore } from 'pinia'

interface State {
  search: ScheduleListRequest
  schedules: Schedule[]
  schedule: Schedule | undefined
  total: number
  loading: boolean
  selectedFieldId: number
}

export const useScheduleStore = defineStore('scheduleStore', {
  state: (): State => {
    return {
      search: {
        limit: LIMIT_PER_PAGE,
        fieldId: 0,
        startDate: undefined,
        endDate: undefined,
        offset: 0
      },
      schedules: [],
      schedule: undefined,
      total: 0,
      selectedFieldId: 0,
      loading: false
    }
  },
  actions: {
    async getScheduleList() {
      this.loading = true
      const { data } = await getSchedule({ ...this.search, fieldId: this.selectedFieldId })
      this.schedules = data.data
      this.total = data.meta?.total ?? 0
      this.loading = false
      return data
    },
    async getScheduleById(id: number) {
      const { data } = await getScheduleById(id)
      this.schedule = data.data
      return data
    },
    async createSchedule(value: ScheduleCreateRequest) {
      const { data } = await createSchedule(value)
      this.schedule = data.data
      return data
    },
    async deleteSchedule(id: number) {
      const { data } = await deleteScheduleApi(id)
      this.schedule = data.data
      return data
    },
    async updateSchedule(value: ScheduleUpdateRequest) {
      const { data } = await updateSchedule(value, value.id)
      this.schedule = data.data
      return data
    },
    async changePage(limit: number, offset: number) {
      this.search.limit = limit
      this.search.offset = offset
      await this.getScheduleList()
    }
  }
})
