import {
  acceptRequestOpponentFinding,
  cancelAcceptedRequest,
  cancelOpponentFinding,
  cancelRequestOpponentFinding,
  checkAcceptedPost,
  checkAcceptedRequest,
  checkOverlapOpponentFinding,
  checkOverlapOpponentFindingCanRestore,
  checkOverlapRequest,
  checkOverlapRequestCanRestore,
  createOpponentFindingWithBooking,
  createOpponentFindingWithoutBooking,
  createRequestingOpponentFinding,
  getMyOpponentFindingRequests,
  getMyOpponentFindings,
  getMyRequestByOpponentFindingId,
  getOpponentFindingById,
  getRequestById,
  getRequestingOpponentFindingList,
  restoreCancelledOverlapOpponentFinding,
  restoreCancelledOverlapRequest,
  restoreFindingState,
  restoreOverlapPostById,
  restoreOverlapRequestById,
  searchOpponentFindings,
  type CreateRequestingOpponentFindingRequest,
  type OpponentFindingRequestPagination,
  type OpponentFindingWithBookingCreateRequest,
  type OpponentFindingWithoutBookingCreateRequest,
  type OverlapCheckRequest,
  type SearchOpponentFindingRequest
} from '@/api/opponentFindingApi'
import { USER_ID } from '@/constants/storage'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { OpponentFinding, RequestOfOpponentFindingResponse } from '@/types/OpponentFinding'
import { changeTimeToSeconds } from '@/utils/timeUtil'
import Cookies from 'js-cookie'
import { defineStore } from 'pinia'

interface State {
  search: SearchOpponentFindingRequest
  opponentFindings: OpponentFinding[]
  opponentFinding: OpponentFinding | undefined
  opponentFindingRequests: RequestOfOpponentFindingResponse[]
  loading: boolean
  currentMode: string
  abortController: AbortController
  hasOverlapRequest: boolean
  sendRequestLoading: boolean
  hasOverlapOpponentFinding: boolean
  hasOverlapAcceptedRequest: boolean
  hasOverlapAcceptedPost: boolean
  requestPagination: OpponentFindingRequestPagination
  totalOpponentFindings: number
  getSpecificOpponentFindingLoading: boolean
  dialogActionLoading: boolean
  OverlapCanRestoreOpponentFinding: OpponentFinding[]
  OverlapCanRestoreRequest: RequestOfOpponentFindingResponse[]
}

const initialRequestPagination = {
  IsSortDescByCreatedAt: false,
  limit: LIMIT_PER_PAGE,
  offset: 0
}

export const useOpponentFindingStore = defineStore('opponentFindingStore', {
  state: (): State => {
    return {
      search: {
        status: '',
        province: '',
        district: '',
        commune: '',
        fromDate: '',
        toDate: '',
        fromTime: '',
        toTime: '',
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      opponentFindings: [],
      opponentFinding: undefined,
      loading: false,
      opponentFindingRequests: [],
      currentMode: 'Tìm đối',
      abortController: new AbortController(),
      hasOverlapRequest: false,
      sendRequestLoading: false,
      getSpecificOpponentFindingLoading: false,
      hasOverlapOpponentFinding: false,
      hasOverlapAcceptedRequest: false,
      hasOverlapAcceptedPost: false,
      requestPagination: {
        ...initialRequestPagination
      },
      totalOpponentFindings: 0,
      dialogActionLoading: false,
      OverlapCanRestoreOpponentFinding: [],
      OverlapCanRestoreRequest: []
    }
  },
  actions: {
    cancelCurrentRequest() {
      this.abortController.abort()
      this.abortController = new AbortController()
      this.loading = false
    },
    resetSearch() {
      this.search = {
        ...this.search,
        province: '',
        district: '',
        commune: '',
        fromDate: '',
        toDate: '',
        fromTime: '',
        toTime: ''
      }
    },
    resetOpponentFindings() {
      this.opponentFindings = []
      this.search.offset = 0
      this.totalOpponentFindings = 0
    },
    resetRequestPagination() {
      this.requestPagination = { ...initialRequestPagination }
    },
    updateOpponentFinding(updatedItem: OpponentFinding) {
      const index = this.opponentFindings.findIndex((x) => x.id === updatedItem.id)
      if (index !== -1) {
        this.opponentFindings.splice(index, 1, updatedItem)
      }
    },
    updateRequest(updatedItem: RequestOfOpponentFindingResponse) {
      const index = this.opponentFindingRequests.findIndex((x) => x.id === updatedItem.id)
      if (index !== -1) {
        this.opponentFindingRequests.splice(index, 1, updatedItem)
      }
    },
    getOpponentFindingByID(opponentFindingID: Number) {
      return this.opponentFindings.find((finding) => finding.id === opponentFindingID)
    },
    async getOpponentFindingList(mode: string) {
      if (this.loading) {
        this.cancelCurrentRequest()
      }

      this.currentMode = mode
      this.loading = true
      try {
        switch (mode) {
          case 'Tìm đối': {
            const response = await searchOpponentFindings(this.search, {
              signal: this.abortController.signal
            })

            if (!response?.data) {
              return
            }
            const { data } = response
            this.opponentFindings = data.data

            const userId = Number(Cookies.get(USER_ID))
            const userOpponentFindings = this.opponentFindings.filter(
              (x) => x.userFindingId === userId
            )

            userOpponentFindings.forEach((x) => {
              x.isMine = true
            })
            this.totalOpponentFindings = data.meta?.total ?? 0
            break
          }

          case 'Bài của tôi': {
            const response = await getMyOpponentFindings(this.search, {
              signal: this.abortController.signal
            })

            if (!response?.data) {
              return
            }

            const { data } = response
            this.opponentFindings = data.data
            this.totalOpponentFindings = data.meta?.total ?? 0
            this.opponentFindings.forEach((x) => {
              x.isMine = true
            })
            break
          }

          case 'Yêu cầu đã gửi': {
            try {
              const response = await getMyOpponentFindingRequests(this.search, {
                signal: this.abortController.signal
              })
              if (!response?.data) {
                return
              }
              const { data: responseData } = response

              if (responseData.data) {
                this.opponentFindings = []
                for (const request of responseData.data) {
                  const opponentFinding = await getOpponentFindingById(request.opponentFindingId, {
                    signal: this.abortController.signal
                  })
                  if (!opponentFinding?.data) {
                    return
                  }
                  const { data: opponentData } = opponentFinding
                  opponentData.data.myRequest = request

                  this.opponentFindings.push(opponentData.data)
                }
              }
              this.totalOpponentFindings = responseData.meta?.total ?? 0
            } catch (error) {
              console.error('Error processing opponent finding requests:', error)
            }

            break
          }

          default:
            this.opponentFindings = []
        }
      } catch (err) {
        console.error('Error: ', err)
      }

      this.loading = false
    },
    async changePageOpponentFinding(page: number) {
      this.search.offset = (page - 1) * this.search.limit
      await this.getOpponentFindingList(this.currentMode)
      window.scrollTo(0, 0)
    },
    async getOpponentFinding(id: number) {
      this.getSpecificOpponentFindingLoading = true
      this.OverlapCanRestoreOpponentFinding = []
      this.OverlapCanRestoreRequest = []
      const { data } = await getOpponentFindingById(id)
      this.opponentFinding = data.data

      const userId = Number(Cookies.get(USER_ID))
      if (this.opponentFinding.userFindingId === userId) {
        this.opponentFinding.isMine = true
        await this.getRequestsOfOpponentFinding()
      } else {
        await this.getMyRequestByOpponentFindingId()
        if (!this.opponentFinding.myRequest) {
          await this.checkAcceptedRequest()
          if (!this.hasOverlapAcceptedRequest) {
            await this.checkAcceptedPost()
            if (!this.hasOverlapAcceptedPost) {
              await this.checkOverlapRequest()
            }
          }
        }
      }

      this.updateOpponentFinding(this.opponentFinding)
      this.getSpecificOpponentFindingLoading = false
      return data
    },
    async createRequestOfOpponentFinding(message: string) {
      if (this.opponentFinding) {
        this.sendRequestLoading = true
        const request: CreateRequestingOpponentFindingRequest = {
          opponentFindingId: this.opponentFinding.id,
          message: message
        }
        const { data } = await createRequestingOpponentFinding(request)
        if (data && data.success && data.data) {
          this.opponentFindingRequests.push(data.data)
          this.opponentFinding.myRequest = data.data
        } else {
          await this.checkAcceptedRequest()
        }
        this.sendRequestLoading = false
        return data
      }
    },
    async getRequestsOfOpponentFinding() {
      if (this.opponentFinding) {
        const { data } = await getRequestingOpponentFindingList(
          this.opponentFinding.id,
          this.requestPagination
        )
        this.opponentFindingRequests = data.data
        return data
      }
    },
    async changePageRequestsOfOpponentFinding(page: number) {
      this.requestPagination.offset = (page - 1) * this.requestPagination.limit
      await this.getRequestsOfOpponentFinding()
    },
    async acceptRequestOfOpponentFinding(requestId: number) {
      if (this.opponentFinding) {
        this.dialogActionLoading = true
        const { data } = await acceptRequestOpponentFinding(requestId)
        if (data.success) {
          this.opponentFinding.status = 'ACCEPTED'
          const updateOpponentFinding = this.opponentFindings.find(
            (x) => x.id === this.opponentFinding?.id
          )
          if (updateOpponentFinding) {
            updateOpponentFinding.status = 'ACCEPTED'
          }
          const request = this.opponentFindingRequests.find((x) => x.id === requestId)
          if (request) {
            request.isAccepted = true
          }
        } else {
          const { data } = await getRequestById(requestId)
          if (data.success) {
            this.updateRequest(data.data)
          }
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async getMyRequestByOpponentFindingId() {
      if (this.opponentFinding) {
        const { data } = await getMyRequestByOpponentFindingId(this.opponentFinding.id)
        this.opponentFindingRequests = []
        this.opponentFinding.myRequest = undefined
        if (data.success && data.data) {
          this.opponentFindingRequests = [data.data]
          this.opponentFinding.myRequest = data.data
          if (!this.opponentFinding.isOverdue && this.opponentFinding.myRequest.status) {
            if (
              this.opponentFinding.myRequest.status === 'CANCELLED' ||
              (this.opponentFinding.myRequest.status === 'SELF_CANCELLED' &&
                this.opponentFinding.myRequest.isAccepted)
            ) {
              await this.checkOverlapRequestCanRestore()
              await this.checkOverlapOpponentFindingCanRestore()
            }
          }
        }
        return data
      }
    },
    async cancelOpponentFinding() {
      if (this.opponentFinding) {
        this.dialogActionLoading = true
        const { data } = await cancelOpponentFinding(this.opponentFinding.id)
        if (data.success) {
          this.opponentFinding = data.data
          this.opponentFinding.isMine = true
          this.updateOpponentFinding(this.opponentFinding)
          this.opponentFindingRequests = this.opponentFindingRequests.map((request) => {
            if (request.isAccepted === true && request.status === undefined) {
              return { ...request, isAccepted: false }
            }
            return request
          })
        } else {
          this.getOpponentFinding(this.opponentFinding.id)
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async createOpponentFindingWithoutBooking(request: OpponentFindingWithoutBookingCreateRequest) {
      this.dialogActionLoading = true
      const { data } = await createOpponentFindingWithoutBooking(request)
      this.dialogActionLoading = false
      return data
    },
    async createOpponentFindingWithBooking(request: OpponentFindingWithBookingCreateRequest) {
      this.dialogActionLoading = true
      const { data } = await createOpponentFindingWithBooking(request)
      this.dialogActionLoading = false
      return data
    },
    async checkOverlapRequest() {
      if (this.opponentFinding) {
        const { data } = await checkOverlapRequest({
          date: this.opponentFinding.date,
          startTime: changeTimeToSeconds(this.opponentFinding.startTime).toString(),
          endTime: changeTimeToSeconds(this.opponentFinding.endTime).toString()
        })
        this.hasOverlapRequest = data.data
        return data
      }
    },
    async cancelMyRequest() {
      if (this.opponentFinding?.myRequest) {
        this.dialogActionLoading = true
        const { data } = await cancelRequestOpponentFinding(this.opponentFinding.myRequest.id)
        if (data.success) {
          this.opponentFinding.myRequest = data.data
          const opponentFinding = this.opponentFindings.find(
            (x) => x.id === this.opponentFinding?.id
          )
          if (opponentFinding) {
            opponentFinding.myRequest = data.data
          }
          this.updateRequest(data.data)
          await this.checkOverlapRequestCanRestore()
          await this.checkOverlapOpponentFindingCanRestore()
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async cancelAcceptedRequest() {
      if (this.opponentFinding) {
        this.dialogActionLoading = true
        const { data } = await cancelAcceptedRequest(this.opponentFinding.id)
        if (data.success) {
          this.opponentFinding = data.data
          this.opponentFinding.isMine = true
          this.updateOpponentFinding(this.opponentFinding)
          await this.getRequestsOfOpponentFinding()
        } else {
          this.getOpponentFinding(this.opponentFinding.id)
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async restoreFindingState() {
      if (this.opponentFinding) {
        this.dialogActionLoading = true
        const { data } = await restoreFindingState(this.opponentFinding.id)
        if (data.success) {
          this.opponentFinding = data.data
          this.opponentFinding.isMine = true
          this.updateOpponentFinding(this.opponentFinding)
        } else {
          this.getOpponentFinding(this.opponentFinding.id)
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async restoreCancelledOverlapRequest() {
      if (this.opponentFinding) {
        this.dialogActionLoading = true
        const { data } = await restoreCancelledOverlapRequest(this.opponentFinding.id)
        if (data.success) {
          data.data.forEach((item) => {
            const opponentFinding = this.getOpponentFindingByID(item.opponentFindingId)
            if (opponentFinding) {
              opponentFinding.myRequest = item
            }
          })
          this.OverlapCanRestoreRequest = []
        } else {
          this.getOpponentFinding(this.opponentFinding.id)
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async restoreCancelledOverlapOpponentFinding() {
      if (this.opponentFinding) {
        this.dialogActionLoading = true
        const { data } = await restoreCancelledOverlapOpponentFinding(this.opponentFinding.id)
        if (data.success) {
          this.OverlapCanRestoreOpponentFinding = []
        } else {
          this.getOpponentFinding(this.opponentFinding.id)
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async checkOverlapOpponentFinding(params: OverlapCheckRequest) {
      const { data } = await checkOverlapOpponentFinding({
        date: params.date,
        startTime: changeTimeToSeconds(params.startTime).toString(),
        endTime: changeTimeToSeconds(params.endTime).toString()
      })
      this.hasOverlapOpponentFinding = data.data.length > 0
      return data
    },
    async checkAcceptedRequest() {
      if (this.opponentFinding) {
        const { data } = await checkAcceptedRequest({
          date: this.opponentFinding.date,
          startTime: changeTimeToSeconds(this.opponentFinding.startTime).toString(),
          endTime: changeTimeToSeconds(this.opponentFinding.endTime).toString()
        })
        this.hasOverlapAcceptedRequest = !!data.data
        return data
      }
    },
    async checkAcceptedRequestWithParams(params: OverlapCheckRequest) {
      const { data } = await checkAcceptedRequest({
        date: params.date,
        startTime: changeTimeToSeconds(params.startTime).toString(),
        endTime: changeTimeToSeconds(params.endTime).toString()
      })
      this.hasOverlapAcceptedRequest = !!data.data

      return data
    },
    async checkAcceptedPost() {
      if (this.opponentFinding) {
        const { data } = await checkAcceptedPost({
          date: this.opponentFinding.date,
          startTime: changeTimeToSeconds(this.opponentFinding.startTime).toString(),
          endTime: changeTimeToSeconds(this.opponentFinding.endTime).toString()
        })
        this.hasOverlapAcceptedPost = data.data.length > 0
        return data
      }
    },
    async checkOverlapOpponentFindingCanRestore() {
      if (this.opponentFinding) {
        const { data } = await checkOverlapOpponentFindingCanRestore(this.opponentFinding.id)
        if (data.success) {
          this.OverlapCanRestoreOpponentFinding = data.data
        }
        return data
      }
    },
    async checkOverlapRequestCanRestore() {
      if (this.opponentFinding) {
        const { data } = await checkOverlapRequestCanRestore(this.opponentFinding.id)
        if (data.success) {
          this.OverlapCanRestoreRequest = data.data
        }
        return data
      }
    },
    async restoreOverlapPostById() {
      if (this.opponentFinding) {
        const { data } = await restoreOverlapPostById(this.opponentFinding.id)
        if (data.success) {
          this.opponentFinding = data.data
          this.opponentFinding.isMine = true
          this.updateOpponentFinding(this.opponentFinding)
        } else {
          this.getOpponentFinding(this.opponentFinding.id)
        }
        this.dialogActionLoading = false
        return data
      }
    },
    async restoreOverlapRequestById() {
      if (this.opponentFinding?.myRequest) {
        const { data } = await restoreOverlapRequestById(this.opponentFinding.myRequest.id)
        if (data.success) {
          this.opponentFinding.myRequest = data.data
          this.updateOpponentFinding(this.opponentFinding)
        } else {
          this.getOpponentFinding(this.opponentFinding.id)
        }
        this.dialogActionLoading = false
        return data
      }
    }
  }
})
