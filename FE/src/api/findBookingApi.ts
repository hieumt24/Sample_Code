import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { PartialField } from '@/types/PartialField'

export type GetAvailableRequest = {
  fieldId: number
  date: string
  startTime?: number
  duration?: number
}

export const getAvailableByFieldApi = async (
  params: GetAvailableRequest
): Promise<AxiosResponse<MfResponse<PartialField[]>>> => {
  return await api.get(`/partial-fields/field/${params.fieldId}/available`, { params })
}
