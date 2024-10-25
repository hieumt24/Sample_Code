import { defineStore } from 'pinia'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import {
  cratePostApi,
  deletePostApi,
  getBlogPostByIdApi,
  getBlogPostsApi,
  type BlogPostCreateRequest,
  type BlogPostSearchRequest
} from '@/api/postApi'
import type { BlogPost } from '@/types/Post'

interface State {
  search: BlogPostSearchRequest
  blogPosts: BlogPost[]
  blogPost: BlogPost | undefined
  loading: boolean
  loadingDetail: boolean
  creating: boolean
  total: number
}

export const useBlogPostStore = defineStore('blogPostStore', {
  state: (): State => {
    return {
      search: {
        limit: LIMIT_PER_PAGE,
        offset: 0
      },
      blogPosts: [],
      blogPost: undefined,
      loadingDetail: false,
      loading: false,
      creating: false,
      total: 0
    }
  },
  actions: {
    async getBlogPosts() {
      if (this.loading) return
      this.loading = true
      const { data } = await getBlogPostsApi(this.search)
      this.blogPosts = data.data ?? []
      this.loading = false
      this.total = data.meta?.total ?? 0
      return data
    },

    async createPost(rate: BlogPostCreateRequest) {
      this.creating = true
      const { data } = await cratePostApi(rate)
      this.creating = false
      return data
    },

    async getBlogPostById(id: number) {
      if (this.loadingDetail) return
      this.loadingDetail = true
      this.blogPost = undefined
      const { data } = await getBlogPostByIdApi(id)
      this.blogPost = data.data
      this.loadingDetail = false
      return data
    },

    async changePageBlogPost(page: number) {
      this.search.offset = (page - 1) * this.search.limit
      await this.getBlogPosts()
    },

    async deletePost(id: number) {
      const { data } = await deletePostApi(id)
      return data
    }
  }
})
