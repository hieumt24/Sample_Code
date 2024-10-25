import {
  addToFavoriteApi,
  deleteFromFavoriteApi,
  getMyFavoriteFieldsApi
} from '@/api/favoriteFieldApi'
import {
  type SearchFieldsMostStarRequest,
  searchFieldsRecentlyApi,
  searchMostStarFieldApi,
  type SearchFieldsRecentlyUserRequest
} from '@/api/fieldUserApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Field } from '@/types/Field'
import { useLocalStorage } from '@vueuse/core'
import { defineStore } from 'pinia'

interface State {
  favoriteFieldIds: string
  loadingFavorite: boolean
  favoriteFields: Field[]
  favoriteField: Field | undefined
  totalFavotite: number
}

export const useFavoriteFieldStore = defineStore('favoriteFieldStore', {
  state: (): State => {
    return {
      favoriteFieldIds: useLocalStorage('favoriteFields', 0 + '').value,
      loadingFavorite: false,
      favoriteFields: [],
      favoriteField: undefined,
      totalFavotite: 0
    }
  },
  actions: {
    async getMyFavoriteFields() {
      this.loadingFavorite = true
      const { data } = await getMyFavoriteFieldsApi()
      this.favoriteFields = data.data
      this.totalFavotite = data.meta?.total ?? 0
      this.favoriteFieldIds = data.data.map((field: Field) => '[' + field.id + ']').join(',')

      localStorage.setItem(
        'favoriteFields',
        data.data.map((field: Field) => '[' + field.id + ']').join(',')
      )
      this.loadingFavorite = false
      return data
    },
    async addToFavoriteFields(fieldId: number) {
      this.loadingFavorite = true
      const { data } = await addToFavoriteApi({ fieldId: fieldId })
      this.favoriteField = data.data
      this.getMyFavoriteFields()
      this.loadingFavorite = false
      return data
    },
    async deleteFromFavoriteFields(fieldId: number) {
      this.loadingFavorite = true
      const { data } = await deleteFromFavoriteApi(fieldId)
      this.favoriteField = data.data
      this.getMyFavoriteFields()
      this.loadingFavorite = false
      return data
    }
  }
})
