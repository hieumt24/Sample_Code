import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Field } from '@/types/Field'

export type SearchFieldListRequestAdmin = {
  name: string
  address: string
  province: string
  district: string
  commune: string
  status: string
  limit: number
  offset: number
}

export type FieldChangeStatusAdmin = {
  fieldId: number
  status: string
}

export const searchFieldListAdminApi = async (
  params: SearchFieldListRequestAdmin
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/admin/fields', { params })
}
