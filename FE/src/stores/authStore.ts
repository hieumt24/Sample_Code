import { defineStore } from 'pinia'
import {
  activeAccountApi,
  changePassApi,
  forgotPassApi,
  getMeApi,
  loginApi,
  logoutApi,
  registerApi,
  resetPassApi,
  verifyTokenApi,
  type ChangePassRequest,
  type LoginRequest,
  type RegisterRequest,
  type ResetPasswordRequest
} from '@/api/authApi'
import Cookies from 'js-cookie'
import { ACCESS_TOKEN_KEY, NAME, REFRESH_TOKEN_KEY, ROLE, USER_ID } from '@/constants/storage'
import type { Identity } from '@/types/Identity'
import type { AxiosError } from 'axios'
import router from '@/router'
import type { BadRequestResponse } from '@/api/types/BadRequestError'
import type { LoginResponse } from '@/api/types/LoginResponse'
import { useLocalStorage } from '@vueuse/core'
import { useNotificationStore } from './notificationStore'
import { NOTIFICATION_HUB } from '@/constants/env'
import { usePaymentStoreUser } from './paymentStoreUser'
// import type { BadRequestError } from '@/models/BadRequestError'

export interface State {
  loginError: boolean
  identity: Identity | null
  rootUrl: string
  name: string
  identified: boolean
  isLoggingIn: boolean
  isRegistering: boolean
  messageError: string | undefined
}

export const useAuthStore = defineStore({
  id: 'auth',
  state: (): State => ({
    loginError: false,
    messageError: undefined,
    identity: null,
    isLoggingIn: false,
    identified: false,
    isRegistering: false,
    name: useLocalStorage(NAME, '').value,
    rootUrl: '/'
  }),
  actions: {
    async login(params: LoginRequest) {
      try {
        this.loginError = false
        this.isLoggingIn = true
        const { data } = await loginApi(params)
        this.isLoggingIn = false
        await this.loginSuccessfully(data.data)
      } catch (e: unknown) {
        console.error(e)
        this.messageError = (e as AxiosError<BadRequestResponse>).response?.data.message
        this.loginError = true
        this.isLoggingIn = false
      }
    },

    async logout() {
      if (this.identity && Cookies.get(ACCESS_TOKEN_KEY)) await logoutApi()
      Cookies.remove(ACCESS_TOKEN_KEY)
      Cookies.remove(REFRESH_TOKEN_KEY)
      localStorage.clear()
      const notificationStore = useNotificationStore()
      notificationStore.stopConnection()
      this.identified = false
      window.location.replace(this.rootUrl)
    },

    async getMe() {
      const { data } = await getMeApi()

      if (data.success) this.identity = data.data
    },

    async register(params: RegisterRequest) {
      this.isRegistering = true
      const { data } = await registerApi(params)
      this.isRegistering = false
      return data
    },

    async changePass(params: ChangePassRequest) {
      const { data } = await changePassApi(params)
      if (data) {
        window.location.replace(this.rootUrl)
      }
    },

    async forgetPassword(email: string) {
      const { data } = await forgotPassApi(email)
      return data
    },

    async vorifyToken(token: string, userId: number) {
      const { data } = await verifyTokenApi({
        token: token,
        userId: userId
      })
      return data
    },

    async activeAccount(token: string, userId: number) {
      const { data } = await activeAccountApi({
        token: token,
        userId: userId
      })
      return data
    },

    async resetPassword(params: ResetPasswordRequest) {
      const { data } = await resetPassApi(params)
      return data
    },

    async loginSuccessfully(data: LoginResponse) {
      console.log('succeeded')

      Cookies.set(ACCESS_TOKEN_KEY, data.token)
      Cookies.set(REFRESH_TOKEN_KEY, data.refreshToken)
      Cookies.set(ROLE, data.user.roleName)
      Cookies.set(USER_ID, data.user.id + '')
      const notificationStore = useNotificationStore()
      notificationStore.startConnection(NOTIFICATION_HUB)
      await this.getMe()
      localStorage.setItem(NAME, data.user?.userName ?? '')
      this.name = data.user?.userName ?? ''
      this.identified = true
      localStorage.setItem('avatar', data.user?.avatar ?? '/noavatar.png')
      if (this.identity?.roleName === 'Player') {
        router.push('/user/home')
        const paymentStore = usePaymentStoreUser()
        paymentStore.getCurrentBalance()
      }
      if (this.identity?.roleName === 'Admin') router.push('/admin/home')
      if (this.identity?.roleName === 'Field') router.push('/field/home')
      if (this.identity?.roleName === 'Staff') router.push('/staff/home')
    }
  }
})
