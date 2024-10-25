import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Profile } from '@/types/Profile'

export type ProfileRequest = {
  name: string
  address: string
  latitude: number
  longitude: number
  status: string
  description: string
  cover: File | undefined
}

export type SearchUserRequest = {
  userName: string
  email: string
  phoneNumber: string
  status: string
  fromCreateDate: string
  toCreateDate: string
  limit: number
  offset: number
}

export type BanUserRequest = {
  id: number
  status: string
}

export type ChangeProfileUserRequest = {
  userName: string
  phoneNumber: string
  avatar: File | undefined
}

export const getUserById = async (id: number): Promise<AxiosResponse<MfResponse<Profile>>> => {
  return await api.get('/users/' + id)
}

export const getAllUser = async (
  params: SearchUserRequest
): Promise<AxiosResponse<MfResponse<Profile[]>>> => {
  return await api.get('users/search', { params })
}

export const banUser = async (
  value: BanUserRequest
): Promise<AxiosResponse<MfResponse<Profile>>> => {
  return await api.put('/users/status', value, {
    headers: { 'Content-Type': 'application/json' }
  })
}

export const changeProfileUser = async (
  value: ChangeProfileUserRequest,
  id: number
): Promise<AxiosResponse<MfResponse<Profile>>> => {
  return await api.put('/users/' + id, value, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}
