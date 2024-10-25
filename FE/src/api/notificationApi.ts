import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { Notification } from '@/types/Notification'

export type NotificationGetRequest = {
  limit: number
  offset: number
}

export const getNotifications = async (
  params: NotificationGetRequest
): Promise<AxiosResponse<MfResponse<Notification[]>>> => {
  return await api.get('/notifications', { params })
}

export const getUnreadNotificationsCount = async (): Promise<AxiosResponse<MfResponse<number>>> => {
  return await api.get('/notifications/unread')
}

export const markNotificationAsRead = async (
  id: number
): Promise<AxiosResponse<MfResponse<number>>> => {
  return await api.put(`/notifications/${id}/is-read`)
}

export const markAllNotificationsAsRead = async (): Promise<AxiosResponse<MfResponse<any>>> => {
  return await api.put(`/notifications/mark-all-as-read`)
}
