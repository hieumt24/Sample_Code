import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { PartialField } from '@/types/PartialField'

export type partialFieldSearchRequest = {
  limit: number
  offset: number
}

export type CreatePartialFieldRequest = {
  name: string
  fieldId: number
  description?: string
}

export interface UpdatePartialFieldRequest extends CreatePartialFieldRequest {
  id: number
  status?: string
}

export const getPartialFieldByFieldId = async (
  fieldId: number
): Promise<AxiosResponse<MfResponse<PartialField[]>>> => {
  return await api.get('/partial-fields/field/' + fieldId + '?limit=20')
}

export const getPartialFieldById = async (
  id: number
): Promise<AxiosResponse<MfResponse<PartialField>>> => {
  return await api.get('/partial-fields/' + id)
}

export const createPartialField = async (
  value: CreatePartialFieldRequest
): Promise<AxiosResponse<MfResponse<PartialField>>> => {
  return await api.post('/partial-fields', value, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}

export const updatePartialField = async (
  value: UpdatePartialFieldRequest
): Promise<AxiosResponse<MfResponse<PartialField>>> => {
  return await api.put('/partial-fields/' + value.id, value, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}
