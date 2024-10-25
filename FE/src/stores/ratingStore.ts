import { defineStore } from 'pinia'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Rate } from '@/types/Rate'
import {
  crateRateApi,
  deleteRateApi,
  getRatesByFieldApi,
  getRatesByIdApi,
  getStarsByFieldApi,
  updateRateApi,
  type RateCreateRequest,
  type RateListRequest,
  type RateUpdateRequest,
  type StarByFIeld
} from '@/api/rateApi'

interface State {
  searchRateByField: RateListRequest
  rates: Rate[]
  rate: Rate | undefined
  loading: boolean
  total: number
  starByField: StarByFIeld
}

export const useRateStore = defineStore('rateStore', {
  state: (): State => {
    return {
      searchRateByField: {
        fieldId: 0,
        star: 0,
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      rates: [],
      rate: undefined,
      loading: false,
      total: 0,
      starByField: {
        average: 0,
        totalRate: 0,
        star1: 0,
        star2: 0,
        star3: 0,
        star4: 0,
        star5: 0
      }
    }
  },
  actions: {
    async getStarByField(fieldId: number) {
      const { data } = await getStarsByFieldApi(fieldId)
      this.starByField = data.data
      return data
    },

    async getRateByField() {
      if (this.loading) return
      this.loading = true
      const { data } = await getRatesByFieldApi({
        ...this.searchRateByField,
        star: this.searchRateByField.star === 0 ? undefined : this.searchRateByField.star
      })
      this.rates = data.data ?? []
      this.loading = false
      return data
    },

    async createRate(rate: RateCreateRequest) {
      const { data } = await crateRateApi(rate)
      return data
    },

    async getRateById(id: number) {
      if (this.loading) return
      this.loading = true
      const { data } = await getRatesByIdApi(id)
      this.rate = data.data
      this.loading = false
      return data
    },

    async updateRate(rate: RateUpdateRequest) {
      if (this.loading) return
      this.loading = true
      const { data } = await updateRateApi(rate)
      this.rate = data.data
      this.loading = false
      return data
    },

    async changePageRate(page: number) {
      this.searchRateByField.offset = (page - 1) * this.searchRateByField.limit
      await this.getRateByField()
    },

    async deleteRate(id: number) {
      const { data } = await deleteRateApi(id)
      this.rate = data.data
    }
  }
})
