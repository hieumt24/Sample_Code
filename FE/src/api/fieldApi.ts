import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Field } from '@/types/Field'
import type { MapBound } from '@/types/Map'

export type SearchFieldListRequest = {
  name: string
  address: string
  province: string
  district: string
  commune: string
  fromPrice?: number
  toPrice?: number
  fromStar?: number
  toStar?: number
  limit: number
  offset: number
}

export type OwnerSearchFieldListRequest = {
  name: string
  address: string
  status?: string
  limit: number
  offset: number
}

export type FieldCreateRequest = {
  name: string
  address: string
  latitude: number
  longitude: number
  description: string
  province: string
  district: string
  commune: string
  phoneNumber: string
  openTime: number | string
  closeTime: number | string
  cover: File | undefined
  avatar: File | undefined
  isFixedSlot: boolean
  price: number
  deposit: number
}

export type FieldChangeStatus = {
  id: number
  status: string
}

export interface FieldUpdateRequest extends FieldCreateRequest {
  id: number
}

export const searchFieldList = async (
  params: SearchFieldListRequest
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/fields', { params })
}

export const getFieldById = async (id: number): Promise<AxiosResponse<MfResponse<Field>>> => {
  return await api.get('/fields/' + id)
}

export const createField = async (
  value: FieldCreateRequest
): Promise<AxiosResponse<MfResponse<Field>>> => {
  return await api.post('/fields', value, { headers: { 'Content-Type': 'multipart/form-data' } })
}

export const updateField = async (
  value: FieldCreateRequest
): Promise<AxiosResponse<MfResponse<Field>>> => {
  return await api.put('/fields', value, { headers: { 'Content-Type': 'multipart/form-data' } })
}

export const getOwnerFieldList = async (
  params: OwnerSearchFieldListRequest
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get('/fields/owner', { params })
}

export const changeFieldStatus = async (
  params: FieldChangeStatus
): Promise<AxiosResponse<MfResponse<Field>>> => {
  return await api.put('/fields/status', params)
}

export const getFieldListByMapBound = async (
  mapBound: MapBound
): Promise<AxiosResponse<MfResponse<Field[]>>> => {
  return await api.get(
    `/fields/map?FromLatitude=${mapBound._sw.lat}&ToLatitude=${mapBound._ne.lat}&FromLongitude=${mapBound._sw.lng}&ToLongitude=${mapBound._ne.lng}`
  )
}
