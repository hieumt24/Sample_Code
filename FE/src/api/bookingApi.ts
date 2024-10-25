import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Booking } from '@/types/Booking'

export type GetListBookingInWeekByPartialField = {
  partialFieldId: number
  date?: string
  status?: string
  startDate?: string
  endDate?: string
  startTime?: string
  endTime?: string
}

export type CreateBookingRequest = {
  date: string
  startTime: string
  endTime: string
  partialFieldId: number
}

export type UpdateBookingStatus = {
  id: number
  status: string
}

export type CancelBookingStatusUser = {
  id: number
}

export type SearchBookingByFieldOwner = {
  date?: string
  startTime?: string
  endTime?: string
  fieldId: number
  status?: string
  limit: number
  offset: number
}

export const getBookingInWeekByPartialFieldApi = async (
  params: GetListBookingInWeekByPartialField
): Promise<AxiosResponse<MfResponse<Booking[]>>> => {
  return await api.get('/bookings/busy', { params })
}

export const getBookingInWeekByPartialFieldByOwnerApi = async (
  params: GetListBookingInWeekByPartialField
): Promise<AxiosResponse<MfResponse<Booking[]>>> => {
  return await api.get('/bookings', { params })
}

export const postBookingRequest = async (
  params: CreateBookingRequest
): Promise<AxiosResponse<MfResponse<Booking>>> => {
  return await api.post('/bookings', params)
}

export const getBookingRequestApi = async (
  id: number
): Promise<AxiosResponse<MfResponse<Booking>>> => {
  return await api.get('/bookings/' + id)
}

export const cencelBookingUserApi = async (
  params: CancelBookingStatusUser
): Promise<AxiosResponse<MfResponse<Booking>>> => {
  return await api.put('/bookings/rejection', params)
}

export const updateBookingStatusOwnerApi = async (
  params: UpdateBookingStatus
): Promise<AxiosResponse<MfResponse<Booking>>> => {
  return await api.put('/bookings/status', params)
}

export const getBookingByFieldOwnerApi = async (
  params: SearchBookingByFieldOwner
): Promise<AxiosResponse<MfResponse<Booking[]>>> => {
  return await api.get(`/bookings/field/${params.fieldId}`, { params })
}
