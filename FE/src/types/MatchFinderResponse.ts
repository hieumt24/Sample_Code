export type MfResponse<T> = {
  meta?: {
    total: number
    limit: number
    offset: number
  }
  success: boolean
  message: string
  data: T
}
