import {
  changeFieldStatus,
  createField,
  getFieldById,
  getFieldListByMapBound,
  getOwnerFieldList,
  searchFieldList,
  updateField,
  type FieldChangeStatus,
  type FieldCreateRequest,
  type FieldUpdateRequest,
  type OwnerSearchFieldListRequest,
  type SearchFieldListRequest
} from '@/api/fieldApi'
import { searchFieldListAdminApi, type SearchFieldListRequestAdmin } from '@/api/fieldStoreAdmin'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Field } from '@/types/Field'
import type { MapBound } from '@/types/Map'
import { changeTimeToSeconds } from '@/utils/timeUtil'
import { defineStore } from 'pinia'

interface State {
  search: SearchFieldListRequestAdmin
  fields: Field[]
  field: Field | undefined
  loading: boolean
  total: number
}

export const useFieldStoreAdmin = defineStore('fieldStoreAdmin', {
  state: (): State => {
    return {
      search: {
        name: '',
        address: '',
        province: '',
        district: '',
        commune: '',
        status: '',
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      fields: [],
      field: undefined,
      loading: false,
      total: 0
    }
  },
  actions: {
    async getFieldList() {
      this.loading = true
      const { data } = await searchFieldListAdminApi(this.search)
      this.fields = data.data
      this.total = data.meta?.total ?? 0
      this.loading = false
      return data
    },
    async updateField(value: FieldUpdateRequest) {
      const { data } = await updateField(value)
      this.field = data.data
      return data
    },
    async getFieldById(id: number) {
      const { data } = await getFieldById(id)
      this.field = data.data
      return data
    },
    //admin
    async changePageAdmin(limit: number, offset: number) {
      this.search.limit = limit
      this.search.offset = offset
      await this.getFieldList()
    },
    async changeFieldStatus(status: string) {
      if (this.field) {
        const { data } = await changeFieldStatus({
          id: this.field.id,
          status: status
        })
        if (data.success) {
          this.field = data.data
        } else {
          await this.getFieldById(this.field.id)
        }
        return data
      }
    }
  }
})
