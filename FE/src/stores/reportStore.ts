import { defineStore } from 'pinia'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import {
  createReportApi,
  getReportDetailApi,
  getReportsdAdminApi,
  updateReportApi,
  type ReportCreateRequest,
  type ReportListRequest,
  type ReportUpdateRequest
} from '@/api/reportApi'
import type { ReportField } from '@/types/Report'

interface State {
  search: ReportListRequest
  reports: ReportField[]
  report: ReportField | undefined
  loading: boolean
  total: number
}

export const useReportStore = defineStore('reportStore', {
  state: (): State => {
    return {
      search: {
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      reports: [],
      report: undefined,
      loading: false,
      total: 0
    }
  },
  actions: {
    async getReportsAdmin() {
      if (this.loading) return
      this.loading = true
      const { data } = await getReportsdAdminApi(this.search)
      this.reports = data.data
      this.total = data.meta?.total ?? 0
      this.loading = false
      return data
    },

    async createReport(report: ReportCreateRequest) {
      const { data } = await createReportApi(report)
      return data
    },

    async getReportById(id: number) {
      if (this.loading) return
      this.loading = true
      const { data } = await getReportDetailApi(id)
      this.report = data.data
      this.loading = false
      return data
    },

    async updateReport(report: ReportUpdateRequest) {
      if (this.loading) return
      this.loading = true
      const { data } = await updateReportApi(report)
      this.report = data.data
      this.loading = false
      return data
    },

    async changePageReport(page: number) {
      this.search.offset = (page - 1) * this.search.limit
      await this.getReportsAdmin()
    }
  }
})
