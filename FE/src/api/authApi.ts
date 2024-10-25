import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { RefreshResponse } from './types/ResreshResponse'
import type { Identity } from '@/types/Identity'
import type { LoginResponse } from './types/LoginResponse'

export type LoginRequest = {
  email: string
  password: string
}

export type LogoutRequest = {
  email: string
  password: string
}

export type VerifyPasswordTokenRequest = {
  token: string
  userId: number
}

export type ActiveAccountRequest = {
  token: string
  userId: number
}

export type ResetPasswordRequest = {
  email: string
  newPassword: string
  confirmPassword: string
}

export type RegisterRequest = {
  userName: string
  email: string
  role: number
  phoneNumber: string
  confirmPassword: string
  password: string
}

export type RefreshRequest = {
  userId: number
  refreshToken: string
}

export type ChangePassRequest = {
  email: string
  oldPassword: string
  newPassword: string
  confirmPassword: string
}

export const loginApi = async (
  params: LoginRequest
): Promise<AxiosResponse<MfResponse<LoginResponse>>> => {
  return await api.post('auths/login', params)
}

export const logoutApi = async (): Promise<AxiosResponse<MfResponse<Object>>> => {
  return await api.post('auths/logout')
}

export const registerApi = async (
  params: RegisterRequest
): Promise<AxiosResponse<MfResponse<Object>>> => {
  return await api.post('auths/register', { ...params, phoneNumber: params.phoneNumber.toString() })
}

export const changePassApi = async (
  params: ChangePassRequest
): Promise<AxiosResponse<MfResponse<Object>>> => {
  return await api.post('auths/change-password', params)
}

export const forgotPassApi = async (email: string): Promise<AxiosResponse<MfResponse<Object>>> => {
  return await api.post('auths/forget-password', { email: email })
}

export const verifyTokenApi = async (
  params: VerifyPasswordTokenRequest
): Promise<AxiosResponse<MfResponse<Object>>> => {
  return await api.get('auths/verify-token', { params })
}

export const activeAccountApi = async (
  params: ActiveAccountRequest
): Promise<AxiosResponse<MfResponse<Object>>> => {
  return await api.post('auths/active-account', params)
}

export const resetPassApi = async (
  params: ResetPasswordRequest
): Promise<AxiosResponse<MfResponse<Object>>> => {
  return await api.post('auths/new-password', params)
}

export const refreshTokenApi = async (
  params: RefreshRequest
): Promise<AxiosResponse<MfResponse<RefreshResponse>>> => {
  return await api.post('auths/refresh-token', params)
}

export const getMeApi = async (): Promise<AxiosResponse<MfResponse<Identity>>> => {
  return await api.post('auths/me')
}
