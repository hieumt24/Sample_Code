<template>
  <div class="flex flex-wrap w-full">
    <div class="">
      <SearchTab @search="onSearch" @reset="onReset" class="mb-4">
        <ActionButton
          :value="$t('form.create')"
          class="col-span-1"
          @click="$router.push('/admin/post-create')"
          :is-outlined="true"
        />
      </SearchTab>
    </div>
    <div class="flex-1 lg:px-10">
      <CustomTable
        :headers="headers"
        :total="blogPostStore.total"
        :loading="blogPostStore.loading"
        :items="
          blogPostStore.blogPosts.map((post) => ({
            ...post,
            date: post.createdAt,
            category: $t(`blogPostType.${post.category}`)
          }))
        "
        hasThumbnail
        hasDate
        @edit="onEdit"
        @detail="onDetail"
        @change-page="blogPostStore.changePageBlogPost"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import CustomTable from '@/components/tables/CustomTable.vue'
import ActionButton from '@/components/buttons/ActionButton.vue'
import SearchTab from '@/components/search/SearchTab.vue'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import { useBlogPostStore } from '@/stores/postStore'
import { useRouter } from 'vue-router'
const router = useRouter()

const blogPostStore = useBlogPostStore()

const openDialog = ref(false)

onMounted(async () => {
  blogPostStore.search.isAdmin = true
  await blogPostStore.getBlogPosts()
})

const onDetail = async (id: number) => {
  router.push(`/admin/post/${id}`)
}

const onEdit = async (id: number) => {
  onDetail(id)
  openDialog.value = true
}

const onSearch = async () => {
  await blogPostStore.getBlogPosts()
}

const onReset = async () => {
  blogPostStore.search = {
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  onSearch()
}

const headers = [
  { field: 'title', header: 'Tiêu đề' },
  { field: 'category', header: 'Loại' }
]
</script>
