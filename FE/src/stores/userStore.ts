import {
  banUser,
  changeProfileUser,
  getAllUser,
  getUserById,
  type BanUserRequest,
  type ChangeProfileUserRequest,
  type SearchUserRequest
} from '@/api/userApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Profile } from '@/types/Profile'
import { defineStore } from 'pinia'

interface State {
  search: SearchUserRequest
  users: Profile[]
  user: Profile | undefined
  freeSearch: string
  loading: boolean
  total: number
}

export const profileStore = defineStore('profileStore', {
  state: (): State => {
    return {
      search: {
        userName: '',
        email: '',
        phoneNumber: '',
        status: '',
        fromCreateDate: '',
        toCreateDate: '',
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      users: [],
      user: undefined,
      freeSearch: '',
      loading: false,
      total: 0
    }
  },
  actions: {
    searchUser(search: SearchUserRequest) {
      this.search = search
    },
    async getData(id: number) {
      const { data } = await getUserById(id)
      this.user = data.data
      return data
    },
    async getUserList() {
      const { data } = await getAllUser(this.search)
      this.users = data.data
      this.total = data.meta?.total ?? 0
      return data
    },
    async banUser(value: BanUserRequest) {
      const { data } = await banUser(value)
      return data
    },
    async changePageUser(limit: number, offset: number) {
      this.search.limit = limit
      this.search.offset = offset
      await this.getUserList()
    },
    async changeProfileUser(value: ChangeProfileUserRequest, id: number) {
      const { data } = await changeProfileUser(value, id)
      return data
    }
  }
})
