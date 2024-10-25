import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Payment } from '@/types/Payment'

export type PaymentListRequest = {
  fieldId?: number
  date?: number
  limit: number
  offset: number
}

export type PaymentCreateRequest = {
  amount: number
  description?: string
}

export const getListPaymentApi = async (
  params: PaymentListRequest
): Promise<AxiosResponse<MfResponse<Payment[]>>> => {
  return await api.get('/transactions', { params })
}

export const getCurrentBalanceApi = async (): Promise<AxiosResponse<MfResponse<number>>> => {
  return await api.get('/transactions/amount')
}
export const getPaymentByIdApi = async (
  payId: number
): Promise<AxiosResponse<MfResponse<Payment>>> => {
  return await api.get('/transactions/' + payId)
}

export const createPaymentApi = async (
  value: PaymentCreateRequest
): Promise<AxiosResponse<MfResponse<string>>> => {
  return await api.post('/transactions', value)
}

export const cancelPaymentLinkApi = async (
  paymentId: number
): Promise<AxiosResponse<MfResponse<Payment>>> => {
  return await api.put(`/transactions/${paymentId}/cancel-paymentlink`)
}
