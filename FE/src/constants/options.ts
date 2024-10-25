import { useI18n } from 'vue-i18n'
import { BookingStatus } from './booking'
import { ReportStatus, ReportType } from './report'
import { FieldStatus } from './field'

export const bookingStatusOptions = () => {
  const { t } = useI18n()
  return [
    { name: t('bookingStatus.ACCEPTED'), code: BookingStatus.ACCEPTED },
    { name: t('bookingStatus.CANCELED'), code: BookingStatus.CANCELED },
    { name: t('bookingStatus.REJECTED'), code: BookingStatus.REJECTED },
    { name: t('bookingStatus.WAITING'), code: BookingStatus.WAITING }
  ]
}

export const bookingStatusOptionsTest = [
  { name: 'Chờ xác nhận', code: BookingStatus.WAITING },
  { name: 'Thành công', code: BookingStatus.ACCEPTED },
  { name: 'Đã huỷ', code: BookingStatus.CANCELED },
  { name: 'Bị từ chối', code: BookingStatus.REJECTED }
]

export const reportTypeOptions = [
  { name: 'Sân không có thực', code: ReportType.UNREALFIELD },
  { name: 'Sân không giống ảnh', code: ReportType.VIOLATEDPHOTO },
  { name: 'Sân không phản hồi', code: ReportType.NOTRESPONDING }
]

export const reportStatusOptions = [
  { name: 'Chờ xử lý', code: ReportStatus.PENDING },
  { name: 'Đã bỏ qua', code: ReportStatus.DISMISS },
  { name: 'Đã xử lý', code: ReportStatus.RESOLVED }
]
export const FieldStatusOptions = [
  { name: 'Tất cả', code: '' },
  { name: 'Chờ duyệt', code: FieldStatus.WAITING },
  { name: 'Hoạt động', code: FieldStatus.ACCEPTED },
  { name: 'Bị từ chối', code: FieldStatus.REJECTED }
]
