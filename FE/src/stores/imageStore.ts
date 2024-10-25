import { defineStore } from 'pinia'
import {
  deleteImageApi,
  getImageByIdApi,
  getImagesByFieldApi,
  postImagesByFieldApi,
  type ImageFieldCreate
} from '@/api/imageApi'

export type ImageFiled = {
  id: number
  url: string
}

interface State {
  images: ImageFiled[]
  image: ImageFiled | undefined
  loading: boolean
  total: number
}

export const useImageStore = defineStore('imageStore', {
  state: (): State => {
    return {
      images: [],
      image: undefined,
      loading: false,
      total: 0
    }
  },
  actions: {
    async getImagesByField(fieldId: number) {
      if (this.loading) return
      this.loading = true
      const { data } = await getImagesByFieldApi(fieldId)
      this.images = data.data
      this.loading = false
      return data
    },

    async postImages(values: ImageFieldCreate) {
      const { data } = await postImagesByFieldApi(values)
      return data
    },

    async getImageById(id: number) {
      if (this.loading) return
      this.loading = true
      const { data } = await getImageByIdApi(id)
      this.image = data.data
      this.loading = false
      return data
    },

    async deleteImage(id: number) {
      if (this.loading) return
      this.loading = true
      const { data } = await deleteImageApi(id)
      this.loading = false
      return data
    }
  }
})
