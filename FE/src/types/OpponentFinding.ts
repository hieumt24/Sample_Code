export type OpponentFinding = {
  id: number
  content: string
  userFindingId: number
  userFindingName: string
  userFindingPhoneNumber: string
  userFindingAvatar: string
  fieldId: number
  fieldName: string
  address: string
  province: string
  district: string
  commune: string
  bookingId: number
  date: string
  startTime: string
  endTime: string
  status: string
  totalRequest: number
  myRequest: RequestOfOpponentFindingResponse | undefined
  isMine: boolean
  isOverdue: boolean
  createdAt: string
  lastUpdatedAt?: string
  isCanRestore?: boolean
}

export type RequestOfOpponentFindingResponse = {
  id: number
  userRequestingId: number
  userRequestingName: string
  userRequestingPhoneNumber: string
  opponentFindingId: number
  userRequestingAvatar: string
  message: string
  isAccepted: boolean
  status?: string
  createdAt: string
  lastUpdatedAt?: string
  isCanRestore?: boolean
}
