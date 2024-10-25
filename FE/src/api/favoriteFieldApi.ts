import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Field } from '@/types/Field'

export type CreateFavoriteFieldRequest = {
  fieldId: number
}

export type DeleteFavoriteFieldRequest = {
  fieldId: number
}

export const getMyFavoriteFieldsApi = async (): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/favorite-fields')
}

export const addToFavoriteApi = async (
  params: CreateFavoriteFieldRequest
): Promise<AxiosResponse<MfResponse<Field>>> => {
  return await api.post('/favorite-fields', params)
}

export const deleteFromFavoriteApi = async (
  fieldId: number
): Promise<AxiosResponse<MfResponse<Field>>> => {
  return await api.delete('/favorite-fields/field/' + fieldId)
}
