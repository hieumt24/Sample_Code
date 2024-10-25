import { getRecentFieldsByUserApi } from '@/api/userHomeApi'
import type { Field } from '@/types/Field'
import { defineStore } from 'pinia'

interface State {
  recentFields: Field[]
}

export const useUserHomeStore = defineStore('userHomeStore', {
  state: (): State => {
    return {
      recentFields: []
    }
  },
  actions: {
    async getRecentFieldsByUser() {
      const { data } = await getRecentFieldsByUserApi()
      this.recentFields = data.data
      return data
    }
  }
})
