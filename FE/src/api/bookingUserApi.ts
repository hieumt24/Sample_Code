import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Booking } from '@/types/Booking'

export type GetBookingHistoryUserRequest = {
  date?: string
  startTime?: string
  endTime?: string
  fieldId?: number
  status?: string
  limit: number
  offset: number
}

export const getBookingHistoryUserApi = async (
  params: GetBookingHistoryUserRequest
): Promise<AxiosResponse<MfResponse<Booking[]>>> => {
  return await api.get('/bookings/history', { params })
}
