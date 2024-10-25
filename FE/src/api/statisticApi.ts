import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type {
  StatisticBookingByMonth,
  StatisticByBookingStatus,
  StatisticBySlot,
  StatisticByWeekDay,
  StatisticUserByMonth
} from '@/types/Statistics'

export type SearchBookingStatistic = {
  fromDate: string
  toDate: string
  fieldId?: number
}

export type SearchUserStatistic = {
  fromDate: string
  toDate: string
}

export const getBookingStatisticByMonthApi = async (
  params: SearchBookingStatistic
): Promise<AxiosResponse<MfResponse<StatisticBookingByMonth[]>>> => {
  return await api.get('/statistics/booking-monthly', { params })
}

export const getBookingStatisticByWeekDayApi = async (
  params: SearchBookingStatistic
): Promise<AxiosResponse<MfResponse<StatisticByWeekDay>>> => {
  return await api.get('/statistics/booking-week-day', { params })
}

export const getBookingStatisticStatusApi = async (
  params: SearchBookingStatistic
): Promise<AxiosResponse<MfResponse<StatisticByBookingStatus>>> => {
  return await api.get('/statistics/booking-status', { params })
}

export const getBookingStatisticSlotApi = async (
  params: SearchBookingStatistic
): Promise<AxiosResponse<MfResponse<StatisticBySlot>>> => {
  return await api.get('/statistics/booking-slots', { params })
}

export const getUserStatisticByMonth = async (
  params: SearchUserStatistic
): Promise<AxiosResponse<MfResponse<StatisticUserByMonth[]>>> => {
  return await api.get('/statistics/users-monthly', { params })
}
