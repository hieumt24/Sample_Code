import { API_BASE_URL, NOTIFICATION_HUB } from '@/constants/env'
import { ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY, USER_ID } from '@/constants/storage'
import { useLayoutStore } from '@/stores/layoutStore'
import axios from 'axios'
import Cookies from 'js-cookie'
import { refreshTokenApi } from '@/api/authApi'
import { useAuthStore } from '@/stores/authStore'
import { useNotificationStore } from '@/stores/notificationStore'

const api = axios.create({
  baseURL: API_BASE_URL
  // withCredentials: true
})

api.interceptors.request.use((config) => {
  const layoutStore = useLayoutStore()
  layoutStore.incrementApiLoadingCount()
  const accessToken = Cookies.get(ACCESS_TOKEN_KEY)
  if (accessToken && !config.url?.includes('/login')) {
    config.headers.Authorization = `Bearer ${accessToken}`
  }
  return config
})

interface ExpiredTokenRequest {
  resolve: (value?: unknown | PromiseLike<unknown>) => void
  reject: (error: unknown) => unknown
}

let refreshingToken = false
let expiredTokenRequests: ExpiredTokenRequest[] = []

const processExpiredTokenRequests = (error: unknown) => {
  expiredTokenRequests.forEach((req) => {
    if (error) {
      req.reject(error)
    } else {
      req.resolve()
    }
  })
  expiredTokenRequests = []
}

api.interceptors.response.use(
  (response) => {
    const layoutStore = useLayoutStore()
    layoutStore.decrementApiLoadingCount()
    return response
  },
  async (error) => {
    const authStore = useAuthStore()
    const layoutStore = useLayoutStore()
    layoutStore.decrementApiLoadingCount()

    const status = error.response?.status
    switch (status) {
      case 401: {
        if (error.config.url?.includes('/login')) {
          break
        }

        const config = error.config
        if (
          !error.config.url?.includes('/refresh-token') &&
          !error.config.url?.includes('/logout') &&
          !config._retry
        ) {
          console.log('refresh token')
          if (refreshingToken) {
            try {
              await new Promise((resolve, reject) => {
                expiredTokenRequests.push({ resolve, reject })
              })
              return api(config)
            } catch (error) {
              return Promise.resolve(error)
            }
          }

          config._retry = true

          const refreshToken = Cookies.get(REFRESH_TOKEN_KEY)
          if (!refreshToken) {
            return authStore.logout()
          }
          if (!refreshingToken) {
            try {
              refreshingToken = true
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

              const notificationStore = useNotificationStore()
              notificationStore.stopConnection()
              await notificationStore.startConnection(NOTIFICATION_HUB)

              processExpiredTokenRequests(null)
              return api(config)
            } catch (e) {
              window.location.replace('/')
              Cookies.remove(ACCESS_TOKEN_KEY)
              Cookies.remove(REFRESH_TOKEN_KEY)
              processExpiredTokenRequests(e)
              return authStore.logout()
            } finally {
              refreshingToken = false
            }
          }
        }
        break
      }

      case 403:
        // Cookies.remove(ACCESS_TOKEN_KEY)
        // window.location.replace('/403')
        console.log('server error 404 violate')
        break
      case 404:
        console.log('server error 404')
        break
      case 409:
        console.log('server error 409')
        break
      case 500:
        console.log('server error 500')
        break
      case 503:
        console.log('server error 503')
        break
    }
    return Promise.resolve(error.response)
  }
)

export default api
