import { defineStore } from 'pinia'
import type { PartialField } from '@/types/PartialField'
import {
  createPartialField,
  getPartialFieldByFieldId,
  getPartialFieldById,
  updatePartialField,
  type CreatePartialFieldRequest,
  type partialFieldSearchRequest,
  type UpdatePartialFieldRequest
} from '@/api/partialFieldApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'

interface State {
  searchPagination: partialFieldSearchRequest
  partialFields: PartialField[]
  partialField: PartialField | undefined
  total: number
  currentFieldId: number
  currentPage: number
  loading: boolean
  loadingPartial: boolean
}
export const usePartialFieldStore = defineStore('partialFieldStore', {
  state: (): State => {
    return {
      searchPagination: {
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      partialFields: [],
      partialField: undefined,
      total: 0,
      currentFieldId: 0,
      currentPage: 0,
      loading: false,
      loadingPartial: false
    }
  },
  actions: {
    async getPartialFieldByFieldId(fieldId: number) {
      this.loading = true
      const { data } = await getPartialFieldByFieldId(fieldId)
      this.loading = false
      this.partialFields = data.data
      this.total = data.meta?.total ?? 0
      return data
    },
    async getPartialFieldById(id: number) {
      this.loadingPartial = true
      const { data } = await getPartialFieldById(id)
      this.partialField = data.data
      this.loadingPartial = false
      return data
    },
    async changePage(page: number) {
      this.searchPagination.offset = (page - 1) * this.searchPagination.limit
      await this.getPartialFieldByFieldId(this.currentFieldId)
    },
    async createPartialField(value: CreatePartialFieldRequest) {
      const { data } = await createPartialField(value)
      this.partialField = data.data
      return data
    },
    async updatePartialField(value: UpdatePartialFieldRequest) {
      const { data } = await updatePartialField(value)
      this.partialField = data.data
      return data
    },
    async changePagePartialField(limit: number, offset: number) {
      this.searchPagination.limit = limit
      this.searchPagination.offset = offset
      await this.getPartialFieldByFieldId(this.currentFieldId)
    }
  }
})
