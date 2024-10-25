<template>
  <div class="flex flex-row">
    <BlogPostDetail class="w-4/5" :id="Number(route.params.id)" />
    <div>
      <div
        ref="scrollContainer"
        class="flex flex-row lg:flex-col overflow-auto scrollbar-hide scroll-smooth px-14"
      >
        <div>Các thông báo khác</div>
        <div
          v-for="post in blogPostStore.blogPosts"
          :key="post.id"
          class="px-4 flex-none w-[28rem] hover:shadow-lg hover:bg-white transition cursor-pointer duration-300 ease-in-out p-5 hover:scale-105 mx-5"
        >
          <div class="">
            <h3 class="text-xl font-semibold text-gray-900 mb-2 truncate">{{ post.title }}</h3>
            <div class="flex">
              <img :src="post.thumbnailUrl" :alt="post.title" class="w-52 h-32 object-cover" />
              <div class="flex flex-col justify-between px-4">
                <div>
                  <p class="text-sm text-gray-600 mb-2">
                    {{ formatDateVietnamese(post.createdAt) }}
                  </p>
                  <Tag
                    :icon="getIcon(post.category)"
                    severity="info"
                    :value="$t(`blogPostType.${post.category}`)"
                  />
                </div>
                <div
                  class="flex items-center text-blue-500"
                  @click="$router.push(`/user/post/${post.id}`)"
                >
                  <span>Xem thêm</span>
                  <i class="pi pi-angle-double-right mt-1 ml-1" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useBlogPostStore } from '@/stores/postStore'
import { useRoute } from 'vue-router'
import BlogPostDetail from '@/components/information/BlogPostDetail.vue'
import { formatDateVietnamese } from '@/utils/dateUtil'
import { CategoryPost } from '@/constants/blogPost'

const route = useRoute()
const blogPostStore = useBlogPostStore()

const getIcon = (category: string) => {
  switch (category) {
    case CategoryPost.ANNOUCMENT:
      return 'pi pi-bell'
    case CategoryPost.PROMOTION:
      return 'pi pi-gift'
    case CategoryPost.TOURNAMENT:
      return 'pi pi-trophy'
    default:
      return 'pi pi-question-circle'
  }
}

onMounted(async () => {
  blogPostStore.search.isPinned = true
  await blogPostStore.getBlogPosts()
})
</script>
