import type { AxiosResponse } from 'axios'
import api from './api'
import type { MfResponse } from '@/types/MatchFinderResponse'
import type { ImageFiled } from '@/stores/imageStore'

export type ListImageRequest = {
  fieldId: number
}

export type ImageFieldCreate = {
  fieldId: number
  images?: File[]
}

export const getImagesByFieldApi = async (
  fieldId: number
): Promise<AxiosResponse<MfResponse<ImageFiled[]>>> => {
  return await api.get('/images-field?fieldId=' + fieldId)
}

export const getImageByIdApi = async (
  id: number
): Promise<AxiosResponse<MfResponse<ImageFiled>>> => {
  return await api.get('/images-field/' + id)
}

export const postImagesByFieldApi = async (
  value: ImageFieldCreate
): Promise<AxiosResponse<MfResponse<object>>> => {
  const formData = new FormData()

  formData.append('fieldId', value.fieldId.toString())

  if (value.images) {
    value.images.forEach((image, index) => {
      formData.append(`images`, image)
    })
  }

  return await api.post('/images-field', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}

export const deleteImageApi = async (id: number): Promise<AxiosResponse<MfResponse<object>>> => {
  return await api.delete('/images-field/' + id)
}
