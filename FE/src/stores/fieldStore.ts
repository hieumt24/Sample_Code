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
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Field } from '@/types/Field'
import type { MapBound } from '@/types/Map'
import { changeTimeToSeconds } from '@/utils/timeUtil'
import { defineStore } from 'pinia'

interface State {
  search: SearchFieldListRequest
  ownerSearch: OwnerSearchFieldListRequest
  fields: Field[]
  fieldsBound: Field[]
  field: Field | undefined
  freeSearch: string
  loading: boolean
  total: number
}

export const useFieldStore = defineStore('fieldStore', {
  state: (): State => {
    return {
      search: {
        name: '',
        address: '',
        province: '',
        district: '',
        commune: '',
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      ownerSearch: {
        name: '',
        address: '',
        limit: 30,
        offset: 0
      },
      fields: [],
      fieldsBound: [],
      field: undefined,
      freeSearch: '',
      loading: false,
      total: 0
    }
  },
  actions: {
    async getFieldList() {
      this.loading = true
      const { data } = await searchFieldList(this.search)
      this.fields = data.data
      this.total = data.meta?.total ?? 0
      this.loading = false
      return data
    },
    async getFieldById(id: number) {
      const { data } = await getFieldById(id)
      this.field = data.data
      return data
    },
    async createField(value: FieldCreateRequest) {
      const { data } = await createField(value)
      this.field = data.data
      return data
    },
    async updateField(value: FieldUpdateRequest) {
      const { data } = await updateField(value)
      this.field = data.data
      return data
    },

    async changeFieldStatus(id: number, value: FieldChangeStatus) {
      const { data } = await changeFieldStatus({ id: id, status: value.status })
      return data
    },
    async getFieldListByMapBound(mapBound: MapBound) {
      const { data } = await getFieldListByMapBound(mapBound)
      this.fieldsBound = data.data
      return data
    },

    // owner
    async changePageOwner(limit: number, offset: number) {
      this.ownerSearch.limit = limit
      this.ownerSearch.offset = offset
      await this.getOwnerFieldList()
    },
    async getOwnerFieldList() {
      this.loading = true
      const { data } = await getOwnerFieldList(this.ownerSearch)
      this.fields = data.data
      this.loading = false
      this.total = data.meta?.total ?? 0
      return data
    },
    //admin
    async changePageAdmin(limit: number, offset: number) {
      this.search.limit = limit
      this.search.offset = offset
      await this.getFieldList()
    }
  }
})
