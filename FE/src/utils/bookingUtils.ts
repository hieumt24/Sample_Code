import type { Schedule } from '@/types/Schedule'
import dayjs from 'dayjs'

const WAITING = '#38bdf8'
const REJECTED = '#f43f5e'
const ACCEPTED = '#4EFF25'
const CANCELED = '#ccc'
export const INACTIVE_TIME = '#d1d5db'

export const setEventColor = (status: string): string => {
  switch (status) {
    case 'WAITING':
      return WAITING
    case 'REJECTED':
      return REJECTED
    case 'ACCEPTED':
      return ACCEPTED
    case 'CANCELED':
      return CANCELED
    default:
      return '#000'
  }
}

export const divideSchedule = (schedule: Schedule): Schedule[] => {
  const dividedSchedules: Schedule[] = []
  const start = dayjs(schedule.startTime)
  const end = dayjs(schedule.endTime)

  let currentDate = start.clone()

  let i = 0
  while (currentDate.isBefore(end) || currentDate.isSame(end)) {
    const nextDate = currentDate.add(1, 'day').startOf('day')

    const scheduleEnd =
      nextDate.subtract(1, 'millisecond') < end ? nextDate.subtract(1, 'millisecond') : end

    dividedSchedules.push({
      ...schedule,
      id: schedule.id + i * 100,
      startTime: currentDate.format('YYYY-MM-DDTHH:mm'),
      endTime: scheduleEnd.format('YYYY-MM-DDTHH:mm')
    })

    currentDate = nextDate
    i++
  }

  return dividedSchedules
}
