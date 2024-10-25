export type StatisticBookingByMonth = {
  month: number
  year: number
  total: number
}

export type StatisticByWeekDay = {
  monday: number
  tuesday: number
  wednesday: number
  thursday: number
  friday: number
  saturday: number
  sunday: number
}

export type StatisticBySlot = {
  bookingBySlot: { slotName: string; total: number }[]
}

export type StatisticByBookingStatus = {
  canceledTotal: number
  acceptedTotal: number
  rejectedTotal: number
  monthly: { canceled: number; accepted: number; rejected: number; month: number; year: number }[]
}

export type StatisticUserByMonth = {
  month: number
  year: number
  total: number
}
