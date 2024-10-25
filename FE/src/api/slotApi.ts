import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Slot } from '@/types/Slot'

export type SearchSlotListRequest = {
  fieldId: string
}

export type SlotCreateRequest = {
  fieldId: number
  startTime: number
  endTime: number
}

export type SlotUpdateRequest = {
  id: number
  fieldId: number
  startTime: number
  endTime: number
}

export type FixedSlotSetting = {
  id: number
  isFixedSlot: boolean
}

export const searchSlotsByFieldApi = async (
  fieldId: number
): Promise<AxiosResponse<MfResponse<Slot[]>>> => {
  return await api.get('/slots/field/' + fieldId)
}

export const getSlotByIdApi = async (id: number): Promise<AxiosResponse<MfResponse<Slot>>> => {
  return await api.get('/slots/' + id)
}

export const createSlotApi = async (
  values: SlotCreateRequest
): Promise<AxiosResponse<MfResponse<Slot>>> => {
  return await api.post('/slots', values)
}

export const updateSlotApi = async (
  value: SlotUpdateRequest
): Promise<AxiosResponse<MfResponse<Slot>>> => {
  return await api.put('/slots', value)
}

export const setFixedSlotApi = async (
  value: FixedSlotSetting
): Promise<AxiosResponse<MfResponse<Slot>>> => {
  return await api.put('/fields/fixed-slot', value)
}
