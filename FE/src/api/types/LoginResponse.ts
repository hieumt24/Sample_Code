export type LoginResponse = {
  token: string
  refreshToken: string
  user: {
    id: number
    userName: string
    email: string
    roleName: string
    status: string
    phoneNumber: string
    avatar: string
  }
}
