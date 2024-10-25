import { defineStore } from 'pinia'

type BreadcrumbItem = {
  label?: string
  icon?: string
  router?: string
}

export interface State {
  apiLoadingCount: number
  breadcrumb: BreadcrumbItem[]
}

export const useLayoutStore = defineStore({
  id: 'layout',
  state: (): State => ({
    apiLoadingCount: 0,
    breadcrumb: []
  }),
  actions: {
    incrementApiLoadingCount() {
      this.apiLoadingCount++
    },
    decrementApiLoadingCount() {
      setTimeout(() => {
        this.apiLoadingCount--
      }, 50)
    }
  }
})
