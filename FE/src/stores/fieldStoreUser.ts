import {
  type SearchFieldsMostStarRequest,
  searchFieldsRecentlyApi,
  searchMostStarFieldApi,
  type SearchFieldsRecentlyUserRequest,
  searchFieldListFreeWordApi,
  searchNeabyFieldApi,
  type SearchNeabyFieldsRequest,
  type SuperSearch,
  searchFieldsSuperApi,
  recommendedByFieldApi
} from '@/api/fieldUserApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Field } from '@/types/Field'
import { defineStore } from 'pinia'

interface State {
  bookedFields: Field[]
  recommendedFields: Field[]
  fields: Field[]
  mostStarFields: Field[]
  superSearchFields: Field[]
  freewordFields: Field[]
  nearbyFields: Field[]
  field: Field | undefined
  mostStarField: Field | undefined
  searchRecently: SearchFieldsRecentlyUserRequest
  searchMostStar: SearchFieldsMostStarRequest
  searchNearby: SearchNeabyFieldsRequest
  superSearch: SuperSearch
  currentPage: number
  loading: boolean
  loadingMostStar: boolean
  loadingNearbyFields: boolean
  total: number
  totalBooked: number
  totalSuper: number
  loadingFreeword: boolean
  loadingSuperSearch: boolean
  loadedFreeWord: boolean
}

export const useFieldStoreUser = defineStore('fieldStoreUser', {
  state: (): State => {
    return {
      bookedFields: [],
      recommendedFields: [],
      fields: [],
      mostStarFields: [],
      freewordFields: [],
      superSearchFields: [],
      nearbyFields: [],
      field: undefined,
      mostStarField: undefined,
      searchRecently: {
        limit: 4,
        offset: 0
      },
      superSearch: {
        startDateTime: '',
        province: '',
        district: '',
        commune: '',
        duration: 5400 //90 minutes
      },
      searchMostStar: {
        latitude: 0,
        longitude: 0
      },
      searchNearby: {
        lat: 0,
        long: 0
      },
      loading: false,
      loadingMostStar: false,
      loadingFreeword: false,
      loadingNearbyFields: false,
      loadedFreeWord: false,
      loadingSuperSearch: false,
      total: 0,
      currentPage: 1,
      totalBooked: 0,
      totalSuper: 0
    }
  },
  actions: {
    async getSuperSearchField() {
      this.superSearchFields = []
      this.loadingSuperSearch = true
      const { data } = await searchFieldsSuperApi(this.superSearch)
      this.superSearchFields = data.data.all.data
      this.totalSuper = data.meta?.total ?? 0
      this.loadingSuperSearch = false
      return data
    },
    async getRecentBookingFields() {
      this.bookedFields = []
      this.loading = true
      const { data } = await searchFieldsRecentlyApi(this.searchRecently)
      this.bookedFields = data.data
      this.totalBooked = data.meta?.total ?? 0
      this.loading = false
      return data
    },
    async getMostStarBookingFields() {
      this.mostStarFields = []
      this.loadingMostStar = true
      const { data } = await searchMostStarFieldApi(this.searchMostStar)
      this.mostStarFields = data.data
      this.loadingMostStar = false
      return data
    },
    async getNearbyFields() {
      this.nearbyFields = []
      this.loadingNearbyFields = true
      const { data } = await searchNeabyFieldApi(this.searchNearby)
      this.nearbyFields = data.data
      this.loadingNearbyFields = false
      return data
    },
    async recommendLikelyField(id: number) {
      this.recommendedFields = []
      const { data } = await recommendedByFieldApi(id)
      this.recommendedFields = data.data
      return data
    },
    async changePageRecentlyBooking(page: number) {
      this.searchRecently.offset = (page - 1) * this.searchRecently.limit
      await this.getRecentBookingFields()
    },
    //Free search
    async searchFieldListFreeWord(keyword: string) {
      this.freewordFields = []
      if (!keyword || keyword === '') return
      this.loadedFreeWord = false
      this.freewordFields = []
      this.loadingFreeword = true
      const { data } = await searchFieldListFreeWordApi(keyword)
      this.freewordFields = data?.data ?? []
      this.loadedFreeWord = true
      this.loadingFreeword = false
      return data
    },
    //Free search
    async getSuperSearchFieldToFreeWordPage() {
      this.loadedFreeWord = false
      this.freewordFields = []
      this.loadingFreeword = true
      const { data } = await searchFieldsSuperApi(this.superSearch)
      this.freewordFields = data.data.all.data
      this.loadedFreeWord = true
      this.loadingFreeword = false
      return data
    }
  }
})
