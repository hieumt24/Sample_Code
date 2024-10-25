import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Schedule } from '@/types/Schedule'

export type ScheduleListRequest = {
  fieldId: number
  startDate?: string
  endDate?: string
  limit: number
  offset: number
}

export type ScheduleCreateRequest = {
  startTime: string
  endTime: string
  reason: string
  fieldId: number
}

export type ScheduleUpdateRequest = {
  id: number
  startTime?: string
  endTime?: string
  reason?: string
}

export const getSchedule = async (
  params: ScheduleListRequest
): Promise<AxiosResponse<MfResponse<Schedule[]>>> => {
  return await api.get('/inactive-times', { params })
}

export const getScheduleById = async (id: number): Promise<AxiosResponse<MfResponse<Schedule>>> => {
  return await api.get('/inactive-times/' + id)
}

export const createSchedule = async (
  value: ScheduleCreateRequest
): Promise<AxiosResponse<MfResponse<Schedule>>> => {
  return await api.post('/inactive-times', value, {
    headers: { 'Content-Type': 'application/json' }
  })
}

export const updateSchedule = async (
  value: ScheduleUpdateRequest,
  id: number
): Promise<AxiosResponse<MfResponse<Schedule>>> => {
  return await api.put('/inactive-times/' + id, value)
}

export const deleteScheduleApi = async (
  id: number
): Promise<AxiosResponse<MfResponse<Schedule>>> => {
  return await api.delete('/inactive-times/' + id)
}
