import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Field } from '@/types/Field'

export const getRecentFieldsByUserApi = async (): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/fields')
}
