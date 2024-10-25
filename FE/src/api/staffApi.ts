import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Staff } from '@/types/Staff'

export type SearchStaffListRequest = {
  fieldId: number
  name: string
  isActive?: boolean
  limit: number
  offset: number
}

export interface CreateStaffRequest {
  userName: string
  name: string
  fieldId: number
  phoneNumber: string
  password: string
  isActive: boolean
}

export interface UpdateStaffStatusRequest {
  userId: number
  fieldId: number
  isActive: boolean
}
export interface UpdateStaffRequest extends CreateStaffRequest {
  id: number
}

export const searchStaffListApi = async (
  params: SearchStaffListRequest
): Promise<AxiosResponse<MfResponse<Staff[]>>> => {
  return await api.get('/staffs', { params })
}

export const getStaffByIdApi = async (
  staffId: number,
  fieldId: number
): Promise<AxiosResponse<MfResponse<Staff>>> => {
  return await api.get(`/staffs/${staffId}/field/${fieldId}`)
}

export const createStaffApi = async (
  params: CreateStaffRequest
): Promise<AxiosResponse<MfResponse<Staff>>> => {
  params.isActive = true
  return await api.post('/staffs', params)
}

export const changeStaffStatusApi = async (
  params: UpdateStaffStatusRequest
): Promise<AxiosResponse<MfResponse<Staff>>> => {
  return await api.put('/staffs', params)
}
