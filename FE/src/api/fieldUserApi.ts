import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Field } from '@/types/Field'

export type SuperSearch = {
  startDateTime?: string
  province?: string
  district?: string
  commune?: string
  duration: number
  fromPrice?: number
  toPrice?: number
  fromStar?: number
  toStar?: number
}

export type SearchFieldsRecentlyUserRequest = {
  limit: number
  offset: number
}

export type SearchFieldsMostStarRequest = {
  longitude: number
  latitude: number
}

export type SearchNeabyFieldsRequest = {
  long: number
  lat: number
}

export type AvailableFields = {
  all: {
    data: Field[]
    total: number
  }
}
export const searchFieldsSuperApi = async (
  search: SuperSearch
): Promise<AxiosResponse<MfResponse<AvailableFields>>> => {
  return await api.get('/fields/search-options', { params: search })
}

export const searchFieldsRecentlyApi = async (
  search: SearchFieldsRecentlyUserRequest
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/fields/early', { params: search })
}

export const searchMostStarFieldApi = async (
  search: SearchFieldsMostStarRequest
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/fields/rating', { params: search })
}

export const searchNeabyFieldApi = async (
  search: SearchNeabyFieldsRequest
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/fields/scan', { params: search })
}

export const recommendedByFieldApi = async (
  id: number
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/fields/recommend/' + id)
}

//Search by free word
export const searchFieldListFreeWordApi = async (
  word: string
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get(`/fields/search?keyword=${word}`)
}
