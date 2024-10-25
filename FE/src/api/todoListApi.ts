import type { AxiosResponse } from 'axios'
import api from './api'

export type SearchTodoListRequest = {
  userId: number
}

export const searchTodoList = async (params: SearchTodoListRequest): Promise<AxiosResponse> => {
  return await api.get('/todos', { params })
}
