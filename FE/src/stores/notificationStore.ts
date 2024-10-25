// stores/notificationStore.ts
import { defineStore } from 'pinia'
import * as signalR from '@microsoft/signalr'
import Cookies from 'js-cookie'
import { ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY, USER_ID } from '@/constants/storage'
import { refreshTokenApi } from '@/api/authApi'
import { useAuthStore } from './authStore'
import type { Notification } from '@/types/Notification'
import {
  getNotifications,
  getUnreadNotificationsCount,
  markAllNotificationsAsRead,
  markNotificationAsRead,
  type NotificationGetRequest
} from '@/api/notificationApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'

interface State {
  notifications: Notification[]
  latestNotification: Notification | null
  isConnected: boolean
  connection: signalR.HubConnection | null
  notificationGetRequest: NotificationGetRequest
  loading: boolean
  unreadCount: number
  hasMoreNotifications: boolean
}

export const useNotificationStore = defineStore('notificationStore', {
  state: (): State => {
    return {
      notifications: [],
      latestNotification: null,
      isConnected: false,
      connection: null,
      notificationGetRequest: {
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      loading: false,
      unreadCount: 0,
      hasMoreNotifications: true
    }
  },

  actions: {
    async startConnection(url: string) {
      if (this.isConnected) return
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(url, {
          accessTokenFactory: () => this.getAccessToken(),
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets,
          headers: {
            'Access-Control-Allow-Origin': '*'
          },
          withCredentials: false
        })
        .withAutomaticReconnect()
        .build()
      this.connection.onclose(async (error) => {
        this.isConnected = false
        console.log('Connection closed due to error: ', error)
        console.log(error)
        await this.refreshToken()
        await this.startConnection(url)
      })

      try {
        await this.connection.start()
        this.isConnected = true
        console.log('Notification Hub Connected')
        this.connection.onreconnecting((error) => {
          console.log('Reconnecting')
          console.log(error)
        })
        this.connection.on('ReceiveNotification', (value: Notification) => {
          this.addNotification(value)
        })
      } catch (err) {
        console.error('Notification Hub Connection Error: ', err)
        if ((err as any).statusCode === 401) {
          await this.refreshToken()
        } else {
          Promise.resolve(err)
        }
      }
    },

    async stopConnection() {
      if (this.connection && this.isConnected) {
        await this.connection.stop()
        this.isConnected = false
        console.log('Notification Hub Disconnected')
      }
    },

    addNotification(notification: Notification) {
      this.notifications.unshift(notification)
      if (
        this.notifications.length > 1 &&
        this.notifications.length % this.notificationGetRequest.limit === 1
      ) {
        this.notifications.pop()
        this.hasMoreNotifications = true
      }
      this.latestNotification = notification
      this.unreadCount += 1
    },

    clearNotifications() {
      this.notifications = []
      this.latestNotification = null
    },
    async handleUnauthorized() {
      try {
        await this.refreshToken()
        // After refreshing the token, try to reconnect
        if (this.connection) {
          await this.connection.stop()
          await this.startConnection(this.connection.baseUrl)
        }
      } catch (error) {
        console.error('Failed to handle unauthorized error:', error)
        // Handle failed refresh (e.g., logout user, redirect to login page)
      }
    },
    getAccessToken() {
      return Cookies.get(ACCESS_TOKEN_KEY) ?? ''
    },
    async refreshToken() {
      const authStore = useAuthStore()
      const refreshToken = Cookies.get(REFRESH_TOKEN_KEY)
      if (!refreshToken) {
        throw new Error('No refresh token available')
      }
      const userId = Cookies.get(USER_ID)
      if (!userId) return
      const { data } = await refreshTokenApi({
        userId: Number(userId),
        refreshToken: refreshToken
      })
      if (!data.success) {
        return authStore.logout()
      }
      Cookies.set(ACCESS_TOKEN_KEY, data.data.token)
      Cookies.set(REFRESH_TOKEN_KEY, data.data.refreshToken)

      console.log('Token refreshed successfully')
    },
    async getNotificationList() {
      this.loading = true
      const { data } = await getNotifications(this.notificationGetRequest)
      this.notifications.push(...data.data)
      this.notificationGetRequest.offset += data.data.length
      this.loading = false
      this.hasMoreNotifications = this.notifications.length < (data.meta?.total ?? 0)
      return data
    },
    async getUnreadNotificationsCount() {
      const { data } = await getUnreadNotificationsCount()
      this.unreadCount = data.data
    },
    async markAsRead(id: number) {
      const { data } = await markNotificationAsRead(id)
      if (data.success) {
        const notification = this.notifications.find((x) => x.id === id)
        if (notification && !notification.isRead) {
          notification.isRead = true
          this.unreadCount--
        }
      }
    },
    async markAllAsRead() {
      const { data } = await markAllNotificationsAsRead()
      if (data.success) {
        const unreadNotifications = this.notifications.filter((x) => !x.isRead)
        if (unreadNotifications.length > 0) {
          unreadNotifications.forEach((notification) => {
            notification.isRead = true
          })
          this.unreadCount = 0
        }
      }
    }
  }
})
