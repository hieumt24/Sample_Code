import { format, formatDistanceToNow } from 'date-fns'
import dayjs from 'dayjs'
import type { Dayjs } from 'dayjs'
import { vi } from 'date-fns/locale'

export const DATE_FORMAT_DD_MM_YYYY = 'YYYY-MM-DD'
export const DATE_FORMAT_SLASH_SHOW = 'DD/MM/YYYY'
export const DATE_TIME_FORMAT_SLASH_SHOW = 'HH:mm DD/MM/YYYY'

export const getFirstDayOfWeek = (): string => {
  let now = dayjs()
  if (now.day() === 0) {
    now = now.subtract(1, 'day')
  }
  return getDateFormatted(now.startOf('week').add(1, 'day'))
}

export const getLastDayOfWeek = (): string => {
  let now = dayjs()
  if (now.day() === 0) {
    now = now.subtract(1, 'day')
  }
  return getDateFormatted(now.endOf('week').add(1, 'day'))
}

export const getCurrentDate = (): string => {
  const now = dayjs()
  return getDateFormatted(now)
}

export const getDateFormatted = (date: Dayjs | string): string => {
  if (typeof date === 'string') {
    date = dayjs(date)
  }
  return date.format(DATE_FORMAT_DD_MM_YYYY)
}

export const getDateFromDate = (date: Date, format: string): string => {
  const newDate = dayjs(date)
  return newDate.format(format)
}

export const addDaysToDate = (date: Dayjs | string, days: number): string => {
  if (typeof date === 'string') {
    date = dayjs(date)
  }
  return getDateFormatted(date.add(days, 'day'))
}

export const isAfterDate = (date: Dayjs | string, compareDate: Dayjs | string): boolean => {
  if (typeof date === 'string') {
    date = dayjs(date)
  }
  if (typeof compareDate === 'string') {
    compareDate = dayjs(compareDate)
  }
  return date.isAfter(compareDate)
}

// ===================SHOW TO UI===============

export const getDateFormattedShow = (date: Dayjs | string): string => {
  if (!date) return ''
  if (typeof date === 'string') {
    date = dayjs(date)
  }
  return date.format(DATE_FORMAT_SLASH_SHOW)
}

export const formatDateYYYYHHHHH = (date: Dayjs | string | undefined | null) => {
  if (!date) return ''
  if (typeof date === 'string') {
    date = dayjs(date)
  }
  return date.format(DATE_TIME_FORMAT_SLASH_SHOW)
}

export const getFirstDayOfWeekBySpecificDate = (date: Dayjs | string): string => {
  if (!date) return ''
  if (typeof date === 'string') {
    date = dayjs(date)
  }
  if (date.day() === 0) {
    date = date.subtract(1, 'day')
  }
  return getDateFormatted(date.startOf('week').add(1, 'day'))
}

export const formatDateVietnamese = (date: string | undefined | null) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

export const formatTimeAgo = (date: string) => {
  if (!date) return ''
  return formatDistanceToNow(new Date(date), { addSuffix: true, locale: vi })
}

export const formatDate = (date: string) => {
  if (!date) return ''
  return format(new Date(date), "EEEE, dd 'Tháng' MM, yyyy 'lúc' HH:mm", { locale: vi })
}

export const formatDateWithoutHHmm = (date: string) => {
  if (!date) return ''
  return format(new Date(date), 'EEEE, dd/MM/yyyy', { locale: vi })
}

export function getDateTimeFormatted(date: Date): string {
  const year = date.getFullYear()
  const month = (date.getMonth() + 1).toString().padStart(2, '0')
  const day = date.getDate().toString().padStart(2, '0')
  const hours = date.getHours().toString().padStart(2, '0')
  const minutes = date.getMinutes().toString().padStart(2, '0')
  return `${year}-${month}-${day}T${hours}:${minutes}`
}
