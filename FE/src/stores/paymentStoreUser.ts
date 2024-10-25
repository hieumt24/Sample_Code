import {
  cancelPaymentLinkApi,
  createPaymentApi,
  getCurrentBalanceApi,
  getListPaymentApi,
  getPaymentByIdApi,
  type PaymentCreateRequest,
  type PaymentListRequest
} from '@/api/paymentStoreApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Payment } from '@/types/Payment'
import { defineStore } from 'pinia'

interface State {
  searchPayment: PaymentListRequest
  myPayments: Payment[]
  myPaymentDetail: Payment | undefined
  loading: boolean
  loadingDetail: boolean
  requestingAddMoney: boolean
  requestingGetBalance: boolean
  total: number
  balance: number
}

export const usePaymentStoreUser = defineStore('paymentStoreUser', {
  state: (): State => {
    return {
      searchPayment: {
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      myPayments: [],
      myPaymentDetail: undefined,
      loading: false,
      loadingDetail: false,
      requestingAddMoney: false,
      requestingGetBalance: false,
      total: 0,
      balance: 0
    }
  },
  actions: {
    updatePayment(updatedItem: Payment) {
      const index = this.myPayments.findIndex((x) => x.id === updatedItem.id)
      if (index !== -1) {
        this.myPayments.splice(index, 1, updatedItem)
      }
    },
    async getMyPayments() {
      this.loading = true
      this.myPayments = []
      const { data } = await getListPaymentApi(this.searchPayment)
      this.myPayments = data.data
      this.total = data.meta?.total ?? 0
      this.loading = false
      return data
    },

    async changePagePaymentHistory(page: number) {
      this.searchPayment.offset = (page - 1) * this.searchPayment.limit
      await this.getMyPayments()
    },

    async getDetailPayment(id: number) {
      this.loadingDetail = true
      const { data } = await getPaymentByIdApi(id)
      if (data.success) {
        this.myPaymentDetail = data.data
        this.updatePayment(data.data)
      }
      this.loadingDetail = false
      return data
    },

    async getCurrentBalance() {
      this.requestingGetBalance = true
      const { data } = await getCurrentBalanceApi()
      this.balance = data.data
      this.requestingGetBalance = false
      localStorage.setItem('balance', data.data.toString())
      return data
    },

    async addMoney(params: PaymentCreateRequest) {
      this.requestingAddMoney = true
      const { data } = await createPaymentApi(params)
      this.requestingAddMoney = false
      return data
    },
    async cancelPaymentLink(paymentId: number) {
      this.loadingDetail = true
      const { data } = await cancelPaymentLinkApi(paymentId)
      if (data.success) {
        this.myPaymentDetail = data.data
        this.updatePayment(data.data)
      }
      this.loadingDetail = false
      return data
    }
  }
})
