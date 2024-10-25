<template>
  <div class="flex flex-col px-10">
    <div class="flex-1">
      <div class="flex w-full justify-between">
        <div class="text-blue-500">
          <i class="pi pi-arrow-left"></i>
          <router-link to="/admin/posts">Danh sách bài viết</router-link>
        </div>
        <div>
          {{ formatDateVietnamese(blogPostStore.blogPost?.createdAt) }}
        </div>
      </div>
      <div class="flex">
        <h1 class="text-2xl font-bold">{{ blogPostStore.blogPost?.title }}</h1>
        <Chip
          v-if="blogPostStore.blogPost?.isPinned"
          label="Bài viết đã ghim"
          icon="pi pi-paperclip"
        />
      </div>
      <div class="flex items-center mt-5"></div>
    </div>
    <div class="p-10 bg-white" v-html="blogPostStore.blogPost?.content"></div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useBlogPostStore } from '@/stores/postStore'
import { useRoute } from 'vue-router'
import Chip from 'primevue/chip'
import { formatDateVietnamese } from '@/utils/dateUtil'

const route = useRoute()
const blogPostStore = useBlogPostStore()

onMounted(async () => {
  blogPostStore.getBlogPostById(Number(route.params.id))
})
</script>
