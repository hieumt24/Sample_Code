import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { BlogPost } from '@/types/Post'

export type BlogPostSearchRequest = {
  fieldId?: number
  category?: number
  isAdmin?: boolean
  isPinned?: boolean
  limit: number
  offset: number
}

export type BlogPostCreateRequest = {
  fieldId: number
  title: string
  content: string
  isPinned: boolean
  category: string
  thumbnail: File | undefined
}

export type BlogPostUpdateRequest = {
  id: number
  fieldId: number
  title: string
  content: string
  isPinned: boolean
  thumbnail: boolean
}

export const getBlogPostByIdApi = async (
  id: number
): Promise<AxiosResponse<MfResponse<BlogPost>>> => {
  return await api.get('/blog-posts/' + id)
}

export const getBlogPostsApi = async (
  params: BlogPostSearchRequest
): Promise<AxiosResponse<MfResponse<BlogPost[]>>> => {
  return await api.get('/blog-posts', { params })
}

export const cratePostApi = async (
  value: BlogPostCreateRequest
): Promise<AxiosResponse<MfResponse<BlogPost>>> => {
  return await api.post('/blog-posts', value, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}

export const updatePostApi = async (
  value: BlogPostUpdateRequest
): Promise<AxiosResponse<MfResponse<BlogPost>>> => {
  return await api.put('/blog-posts', value)
}

export const deletePostApi = async (id: number): Promise<AxiosResponse<MfResponse<BlogPost>>> => {
  return await api.delete('/blog-posts/' + id)
}
