import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { ReportField } from '@/types/Report'

export type ReportListRequest = {
  fieldId?: number
  status?: string
  limit: number
  offset: number
}

export type ReportCreateRequest = {
  fieldId: number
  reason: string
  reportType?: string
}

export type ReportUpdateRequest = {
  id: number
  status: string
  adminNotes: string
}

export const getReportDetailApi = async (
  id: number
): Promise<AxiosResponse<MfResponse<ReportField>>> => {
  return await api.get('/reports/' + id)
}

export const getReportsdAdminApi = async (
  params: ReportListRequest
): Promise<AxiosResponse<MfResponse<ReportField[]>>> => {
  return await api.get('/reports', { params })
}

export const createReportApi = async (
  value: ReportCreateRequest
): Promise<AxiosResponse<MfResponse<ReportField>>> => {
  return await api.post('/reports', value)
}

export const updateReportApi = async (
  value: ReportUpdateRequest
): Promise<AxiosResponse<MfResponse<ReportField>>> => {
  return await api.put('/reports', value)
}
