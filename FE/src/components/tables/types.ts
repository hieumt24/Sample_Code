export type TableColumn = {
  field: string
  header: string
  style?: string
  width?: string
}

export interface EventObject {
  id?: string
  calendarId?: string
  title?: string
  body?: string
  isAllday?: boolean
  start?: Date | string | number
  end?: Date | string | number
  goingDuration?: number
  comingDuration?: number
  location?: string
  attendees?: string[]
  category?: 'milestone' | 'task' | 'allday' | 'time'
  recurrenceRule?: string
  state?: 'Busy' | 'Free'
  isVisible?: boolean
  isPending?: boolean
  isFocused?: boolean
  isReadOnly?: boolean
  isPrivate?: boolean
  color?: string
  backgroundColor?: string
  dragBackgroundColor?: string
  borderColor?: string
  raw?: any
}

export interface EventInfo {
  event: EventObject
  nativeEvent: MouseEvent
}

export interface CommonTheme {
  backgroundColor: string
  border: string
  gridSelection: {
    backgroundColor: string
    border: string
  }
  dayName: { color: string }
  holiday: { color: string }
  saturday: { color: string }
  today: { color: string }
}

export interface SelectDateTimeInfo {
  start: Date
  end: Date
  isAllday: boolean
  nativeEvent?: MouseEvent // Native mouse event on mouse release
  gridSelectionElements: Element[] // List of elements corresponding to the selection area
}

export const BOOKING_TABLE_OPTIONS = {
  defaultView: 'week',
  // useFormPopup: true,
  // useDetailPopup: true,
  isReadOnly: false,
  usageStatistics: false,
  timezone: {
    zones: [
      {
        timezoneName: 'Asia/Ho_Chi_Minh',
        displayLabel: 'TIme'
      }
    ]
  },
  calendars: [
    {
      id: 'cal1',
      name: 'Personal',
      backgroundColor: '#03bd9e'
    }
  ],
  week: {
    dayNames: ['Chủ Nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'],
    hourStart: 7,
    startDayOfWeek: 1,
    hourEnd: 23,
    taskView: false,
    eventView: ['time']
  },
  gridSelection: true
}
