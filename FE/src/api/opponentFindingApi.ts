import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { OpponentFinding, RequestOfOpponentFindingResponse } from '@/types/OpponentFinding'

export type SearchOpponentFindingRequest = {
  status: string
  province: string
  district: string
  commune: string
  fromDate: string
  toDate: string
  fromTime: string
  toTime: string
  limit: number
  offset: number
}

export type OpponentFindingRequestPagination = {
  IsSortDescByCreatedAt: boolean
  limit: number
  offset: number
}

export type OpponentFindingFilterKey = keyof Omit<SearchOpponentFindingRequest, 'limit' | 'offset'>

export type CreateRequestingOpponentFindingRequest = {
  opponentFindingId: number
  message: string
}

export type OpponentFindingWithoutBookingCreateRequest = {
  content: string
  fieldName: string | undefined
  fieldAddress: string | undefined
  fieldProvince: string
  fieldDistrict: string | undefined
  fieldCommune: string | undefined
  startTime: string
  endTime: string
  date: string
}

export type OpponentFindingWithBookingCreateRequest = {
  bookingId: number
  content: string
}

export type OverlapCheckRequest = {
  date: string
  startTime: string
  endTime: string
}

export const searchOpponentFindings = async (
  params: SearchOpponentFindingRequest,
  { signal }: { signal?: AbortSignal } = {}
): Promise<AxiosResponse<MfResponse<OpponentFinding[]>>> => {
  return await api.get('/opponent-findings', { params, signal: signal })
}

export const getOpponentFindingById = async (
  id: number,
  { signal }: { signal?: AbortSignal } = {}
): Promise<AxiosResponse<MfResponse<OpponentFinding>>> => {
  return await api.get('/opponent-findings/' + id, { signal: signal })
}

export const createRequestingOpponentFinding = async (
  value: CreateRequestingOpponentFindingRequest
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse>>> => {
  return await api.post('/opponent-findings/requesting', value)
}

export const getRequestingOpponentFindingList = async (
  opponentFindingId: number,
  params: OpponentFindingRequestPagination
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse[]>>> => {
  return await api.get('/opponent-findings/' + opponentFindingId + '/requesting', { params })
}

export const acceptRequestOpponentFinding = async (
  requestId: number
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse[]>>> => {
  return await api.post('/opponent-findings/requesting/' + requestId + '/accept')
}

export const getMyOpponentFindings = async (
  params: SearchOpponentFindingRequest,
  { signal }: { signal?: AbortSignal } = {}
): Promise<AxiosResponse<MfResponse<OpponentFinding[]>>> => {
  return await api.get('/opponent-findings/my-history', { params, signal: signal })
}

export const getMyOpponentFindingRequests = async (
  params: SearchOpponentFindingRequest,
  { signal }: { signal?: AbortSignal } = {}
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse[]>>> => {
  return await api.get('/opponent-findings/my-history-request', { params, signal: signal })
}

export const getMyRequestByOpponentFindingId = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse>>> => {
  return await api.get('/opponent-findings/my-request?opponentFindingId=' + opponentFindingId)
}
export const getRequestById = async (
  requestId: number
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse>>> => {
  return await api.get('opponent-findings/requesting/' + requestId)
}

export const cancelOpponentFinding = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<OpponentFinding>>> => {
  return await api.put(`/opponent-findings/${opponentFindingId}/cancel-post`)
}

export const cancelAcceptedRequest = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<OpponentFinding>>> => {
  return await api.put(`/opponent-findings/${opponentFindingId}/cancel-matching`)
}

export const createOpponentFindingWithoutBooking = async (
  value: OpponentFindingWithoutBookingCreateRequest
): Promise<AxiosResponse<MfResponse<OpponentFinding>>> => {
  return await api.post('/opponent-findings/not-booking', value)
}

export const createOpponentFindingWithBooking = async (
  value: OpponentFindingWithBookingCreateRequest
): Promise<AxiosResponse<MfResponse<OpponentFinding>>> => {
  return await api.post('/opponent-findings', value)
}

export const checkOverlapRequest = async (
  params: OverlapCheckRequest
): Promise<AxiosResponse<MfResponse<boolean>>> => {
  return await api.get('/opponent-findings/requests-overlap', { params })
}

export const cancelRequestOpponentFinding = async (
  requestId: number
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse>>> => {
  return await api.post('/opponent-findings/requesting/' + requestId + '/cancel')
}

export const restoreCancelledOverlapRequest = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse[]>>> => {
  return await api.put(`/opponent-findings/${opponentFindingId}/restore-overlap-request`)
}

export const restoreCancelledOverlapOpponentFinding = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<OpponentFinding[]>>> => {
  return await api.put(`/opponent-findings/${opponentFindingId}/restore-overlap-findings`)
}

export const restoreFindingState = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<OpponentFinding>>> => {
  return await api.put(`/opponent-findings/${opponentFindingId}/restore-finding`)
}

export const checkAcceptedRequest = async (
  params: OverlapCheckRequest
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse>>> => {
  return await api.get('/opponent-findings/request-was-accepted', { params })
}

export const checkAcceptedPost = async (
  params: OverlapCheckRequest
): Promise<AxiosResponse<MfResponse<OpponentFinding[]>>> => {
  return await api.get('/opponent-findings/opponent-finding-accepted', { params })
}

export const checkOverlapOpponentFinding = async (
  params: OverlapCheckRequest
): Promise<AxiosResponse<MfResponse<OpponentFinding[]>>> => {
  return await api.get('/opponent-findings/opponent-finding-overlap', { params })
}

export const checkOverlapOpponentFindingCanRestore = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<OpponentFinding[]>>> => {
  return await api.get(`/opponent-findings/${opponentFindingId}/check-can-restore-overlap-post`)
}

export const checkOverlapRequestCanRestore = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse[]>>> => {
  return await api.get(`/opponent-findings/${opponentFindingId}/check-can-restore-overlap-request`)
}

export const restoreOverlapPostById = async (
  opponentFindingId: number
): Promise<AxiosResponse<MfResponse<OpponentFinding>>> => {
  return await api.put(`/opponent-findings/${opponentFindingId}/restore-overlap-post`)
}

export const restoreOverlapRequestById = async (
  requestId: number
): Promise<AxiosResponse<MfResponse<RequestOfOpponentFindingResponse>>> => {
  return await api.put(`/opponent-findings/${requestId}/restore-single-overlap-request`)
}
