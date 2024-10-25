import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Rate } from '@/types/Rate'

export type RateListRequest = {
  fieldId: number
  star?: number
  limit: number
  offset: number
}

export type RateCreateRequest = {
  bookingId: number
  star: number
  comment?: string
}

export type RateUpdateRequest = {
  bookingId: number
  star: number
  comment?: string
}

export type StarByFIeld = {
  average: number
  totalRate: number
  star1: number
  star2: number
  star3: number
  star4: number
  star5: number
}

export const getStarsByFieldApi = async (
  fieldId: number
): Promise<AxiosResponse<MfResponse<StarByFIeld>>> => {
  return await api.get('/fields/starts-all?fieldId=' + fieldId)
}

export const getRatesByFieldApi = async (
  params: RateListRequest
): Promise<AxiosResponse<MfResponse<Rate[]>>> => {
  return await api.get('/rates', { params })
}

export const getRatesByIdApi = async (id: number): Promise<AxiosResponse<MfResponse<Rate>>> => {
  return await api.get('/rates/' + id)
}

export const crateRateApi = async (
  value: RateCreateRequest
): Promise<AxiosResponse<MfResponse<Rate>>> => {
  return await api.post('/rates', value)
}

export const updateRateApi = async (
  value: RateUpdateRequest
): Promise<AxiosResponse<MfResponse<Rate>>> => {
  return await api.put('/rates', value)
}

export const deleteRateApi = async (
  bookingId: number
): Promise<AxiosResponse<MfResponse<Rate>>> => {
  return await api.delete('/rates/' + bookingId)
}
