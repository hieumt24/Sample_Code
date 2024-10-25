import {
  changeStaffStatusApi,
  createStaffApi,
  getStaffByIdApi,
  searchStaffListApi,
  type CreateStaffRequest,
  type SearchStaffListRequest,
  type UpdateStaffStatusRequest
} from '@/api/staffApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Staff } from '@/types/Staff'
import { defineStore } from 'pinia'

interface State {
  search: SearchStaffListRequest
  staffs: Staff[]
  staff: Staff | undefined
  total: number
}

export const useStaffStore = defineStore('staffStore', {
  state: (): State => {
    return {
      search: {
        fieldId: 0,
        name: '',
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      staffs: [],
      staff: undefined,
      total: 0
    }
  },
  actions: {
    async getStaffList() {
      this.staffs = []
      this.total = 0
      const { data } = await searchStaffListApi(this.search)
      if (data.success) {
        this.staffs = data.data
        this.total = data.meta?.total ?? 0
      }
      return data
    },
    async changePage(page: number) {
      this.search.offset = (page - 1) * this.search.limit
      await this.getStaffList()
    },
    async getStaffById(staffId: number, fieldId: number) {
      const { data } = await getStaffByIdApi(staffId, fieldId)
      if (data.success) {
        this.staff = data.data
      }
      return data
    },

    async createStaff(request: CreateStaffRequest) {
      const { data } = await createStaffApi(request)
      if (data.success) {
        await this.getStaffList()
      }
      return data
    },
    async changeStaffStatus(status: boolean) {
      if (this.staff) {
        const request: UpdateStaffStatusRequest = {
          userId: this.staff.id,
          fieldId: this.staff.fieldId,
          isActive: status
        }
        const { data } = await changeStaffStatusApi(request)
        if (data.success) {
          this.staff = data.data
          await this.getStaffList()
        }
        return data
      }
    }
  }
})
