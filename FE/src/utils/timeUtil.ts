import dayjs, { Dayjs } from 'dayjs'

export const TIME_FORMAT = 'HH:mm'
export const TIME_REGEX = /^([01]\d|2[0-3]):([0-5]\d)$/

export const getCurrentTime = (): string => {
  return dayjs().format(TIME_FORMAT)
}

export const isValidHour = (time: string): boolean => {
  return TIME_REGEX.test(time)
}

export const isBefore = (time1: string, time2: string): boolean => {
  return dayjs(time1, TIME_FORMAT).isBefore(dayjs(time2, TIME_FORMAT))
}

export const isAfter = (time1: string, time2: string): boolean => {
  return dayjs(time1, TIME_FORMAT).isAfter(dayjs(time2, TIME_FORMAT))
}

export const getTimeFromDate = (date: Date): string => {
  const newDate = dayjs(date)
  return newDate.format(TIME_FORMAT)
}

export const changeTimeToSeconds = (time: string | undefined): number => {
  if (typeof time !== 'string') return 0

  if (!time.includes(':')) return 0
  const [hour, minute] = time.split(':')
  return parseInt(hour) * 3600 + parseInt(minute) * 60
}

export const changeSecondToHour = (second: string): string => {
  const totalSeconds = parseInt(second, 10)
  const hours = Math.floor(totalSeconds / 3600)
  const minutes = Math.floor((totalSeconds % 3600) / 60)
  return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`
}

export const addDurationToDate = (date: Date, durationInSeconds: number) => {
  const newDate = new Date(date)
  newDate.setSeconds(newDate.getSeconds() + durationInSeconds)
  return newDate
}

export function getSecondsFromMidnightPlus2Hours(): number {
  const now = new Date()
  now.setHours(now.getHours() + 2) // Add 2 hours to the current time
  const hours = now.getHours()
  return hours * 3600
}
