import { OpponentFindingRequestStatus, OpponentFindingStatus } from '@/constants/opponentFinding'

export const getSeverity = (status: string, isOverdue?: boolean) => {
  switch (status) {
    case OpponentFindingStatus.FINDING:
      if (isOverdue) return 'warning'
      return 'info'

    case OpponentFindingStatus.ACCEPTED:
      return 'success'

    case OpponentFindingStatus.CANCELLED:
      return 'danger'

    case OpponentFindingStatus.OPPONENT_CANCELLED:
      return 'warning'

    case OpponentFindingStatus.OVERLAPPED_CANCELLED:
      return 'warning'

    default:
      return undefined
  }
}

export const getVNStatus = (status: string, isOverdue?: boolean) => {
  switch (status) {
    case OpponentFindingStatus.FINDING:
      if (isOverdue) return 'Đã hết hạn'
      return 'Đang tìm'

    case OpponentFindingStatus.ACCEPTED:
      return 'Đã tìm được'

    case OpponentFindingStatus.CANCELLED:
      return 'Không tìm nữa'

    case OpponentFindingStatus.OPPONENT_CANCELLED:
      return 'Bị đối hủy kèo'

    case OpponentFindingStatus.OVERLAPPED_CANCELLED:
      return 'Huỷ bài đăng do trùng lịch'
    default:
      return undefined
  }
}

export const getMyRequestSeverity = (
  opponentFindingStatus: string,
  isAccepted: boolean,
  requestStatus?: string,
  isOverdue?: boolean
) => {
  if (requestStatus) {
    if (opponentFindingStatus === OpponentFindingStatus.OVERLAPPED_CANCELLED) {
      return 'danger'
    }
    if (requestStatus === OpponentFindingRequestStatus.OVERLAPPED_CANCELLED) {
      return 'warning'
    }
    return 'danger'
  }

  switch (opponentFindingStatus) {
    case OpponentFindingStatus.FINDING:
      if (isOverdue) return 'warning'
      return 'info'

    case OpponentFindingStatus.ACCEPTED:
      if (isAccepted) return 'success'
      else return 'danger'

    case OpponentFindingStatus.CANCELLED:
      return 'danger'

    case OpponentFindingStatus.OPPONENT_CANCELLED:
      return 'danger'

    case OpponentFindingStatus.OVERLAPPED_CANCELLED:
      return 'danger'

    default:
      return undefined
  }
}

export const getMyRequestVNStatus = (
  opponentFindingStatus: string,
  isAccepted: boolean,
  requestStatus?: string,
  isOverdue?: boolean
) => {
  switch (requestStatus) {
    case OpponentFindingRequestStatus.SELF_CANCELLED:
      if (isAccepted) return 'Bạn đã huỷ kèo'
      else return 'Bạn đã huỷ yêu cầu'
    case OpponentFindingRequestStatus.CANCELLED:
      return 'Bị huỷ kèo'
    case OpponentFindingRequestStatus.OVERLAPPED_CANCELLED:
      if (opponentFindingStatus === OpponentFindingStatus.OVERLAPPED_CANCELLED) {
        return 'Huỷ bài đăng do trùng lịch'
      }
      return 'Huỷ yêu cầu do trùng lịch'
  }

  switch (opponentFindingStatus) {
    case OpponentFindingStatus.FINDING:
      if (isOverdue) return 'Đã hết hạn'
      return 'Đang chờ duyệt'

    case OpponentFindingStatus.ACCEPTED:
      if (isAccepted) return 'Chấp nhận'
      else return 'Từ chối'

    case OpponentFindingStatus.CANCELLED:
      return 'Không tìm nữa'

    case OpponentFindingStatus.OPPONENT_CANCELLED:
      return 'Bị đối hủy kèo'

    case OpponentFindingStatus.OVERLAPPED_CANCELLED:
      return 'Huỷ bài đăng do trùng lịch'
    default:
      return undefined
  }
}
