export type Identity = {
  id: number
  name?: string
  status: string
  roleName: 'Player' | 'Field' | 'Admin' | 'Staff'
}
