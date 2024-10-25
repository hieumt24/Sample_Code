<template>
  <div class="bg-gray-100 py-12 px-4 sm:px-6 lg:px-8 mb-10 relative">
    <h2 class="text-3xl font-bold text-gray-900 mb-8">Bài viết nổi bật</h2>
    <div class="relative">
      <button
        @click="scrollLeft"
        class="absolute -left-14 top-1/2 h-12 w-12 transform -translate-y-1/2 bg-white p-2 rounded-full shadow-md z-10"
        :class="{ 'opacity-40': !canScrollLeft }"
        :disabled="!canScrollLeft"
      >
        <i class="pi pi-chevron-left"></i>
      </button>
      <div ref="scrollContainer" class="flex overflow-x-auto scrollbar-hide scroll-smooth">
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
                <div class="flex flex-col">
                  <p class="text-sm text-gray-600 mb-1">
                    {{ formatDateVietnamese(post.createdAt) }}
                  </p>
                  <div class="font-normal text-gray-500 mb-1">
                    <span>Người tạo: </span>
                    <span>{{ post.isAdmin ? 'Quản trị viên' : 'Sân bóng' }}</span>
                  </div>
                  <Tag
                    :icon="getIcon(post.category)"
                    severity="info"
                    :value="$t(`blogPostType.${post.category}`)"
                  />
                </div>
                <div
                  class="flex items-center text-blue-500"
                  @click="$router.push(`/post/${post.id}`)"
                >
                  <span>Xem thêm</span>
                  <i class="pi pi-angle-double-right mt-1 ml-1" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <button
        @click="scrollRight"
        class="absolute -right-14 top-1/2 h-12 w-12 transform -translate-y-1/2 bg-white p-2 rounded-full shadow-md z-10"
        :class="{ 'opacity-40': !canScrollRight }"
        :disabled="!canScrollRight"
      >
        <i class="pi pi-chevron-right"></i>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBlogPostStore } from '@/stores/postStore'
import { useRoute } from 'vue-router'
import { formatDate, formatDateVietnamese } from '@/utils/dateUtil'
import Tag from 'primevue/tag'
import { CategoryPost } from '@/constants/blogPost'

const route = useRoute()
const blogPostStore = useBlogPostStore()
const scrollContainer = ref<HTMLElement | null>(null)

const canScrollLeft = ref(false)
const canScrollRight = ref(false)

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

const checkScroll = () => {
  if (scrollContainer.value) {
    canScrollLeft.value = scrollContainer.value.scrollLeft > 0
    canScrollRight.value =
      scrollContainer.value.scrollLeft <
      scrollContainer.value.scrollWidth - scrollContainer.value.clientWidth
  }
}

const scrollLeft = () => {
  if (scrollContainer.value) {
    scrollContainer.value.scrollBy({ left: -500, behavior: 'smooth' })
  }
}

const scrollRight = () => {
  if (scrollContainer.value) {
    scrollContainer.value.scrollBy({ left: 500, behavior: 'smooth' })
  }
}

onMounted(async () => {
  blogPostStore.search.isAdmin = true
  blogPostStore.search.isPinned = true
  await blogPostStore.getBlogPosts()

  if (scrollContainer.value) {
    scrollContainer.value.addEventListener('scroll', checkScroll)
    checkScroll() // Initial check
  }
})
</script>

<style>
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}
.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
