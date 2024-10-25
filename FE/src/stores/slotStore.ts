import {
  createSlotApi,
  getSlotByIdApi,
  searchSlotsByFieldApi,
  setFixedSlotApi,
  updateSlotApi,
  type FixedSlotSetting,
  type SlotCreateRequest,
  type SlotUpdateRequest
} from '@/api/slotApi'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { Slot } from '@/types/Slot'
import { changeTimeToSeconds } from '@/utils/timeUtil'
import { defineStore } from 'pinia'

interface State {
  slots: Slot[]
  slot: Slot | undefined
  loading: boolean
  total: number
}

export const useSlotStore = defineStore('slotStore', {
  state: (): State => {
    return {
      slots: [],
      slot: undefined,
      loading: false,
      total: 0
    }
  },
  actions: {
    async getSlotByField(fieldId: number) {
      this.loading = true
      const { data } = await searchSlotsByFieldApi(fieldId)
      // Sort the slots by start time
      this.slots = data.data.slice().sort((a: Slot, b: Slot) => {
        return changeTimeToSeconds(a.startTime) - changeTimeToSeconds(b.startTime)
      })
      this.total = data.meta?.total ?? 0
      this.loading = false
      return data
    },
    async getSlotById(id: number) {
      const { data } = await getSlotByIdApi(id)
      this.slot = data.data
      return data
    },
    async createSlot(value: SlotCreateRequest) {
      const { data } = await createSlotApi(value)
      this.slot = data.data
      return data
    },
    async updateSlot(value: SlotUpdateRequest) {
      const { data } = await updateSlotApi(value)
      this.slot = data.data
      return data
    },
    async setFixedSlot(value: FixedSlotSetting) {
      const { data } = await setFixedSlotApi(value)
      return data
    }
  }
})
