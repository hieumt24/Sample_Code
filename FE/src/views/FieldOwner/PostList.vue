<template>
  <div class="flex flex-wrap w-full">
    <div class="">
      <SearchTab @search="onSearch" @reset="onReset" class="mb-4">
        <ActionButton
          :value="$t('form.create')"
          class="col-span-1"
          @click="$router.push('/field/post-create')"
          :is-outlined="true"
        />
        <Dropdown
          v-model="selectedField"
          :options="listFields"
          optionLabel="name"
          class="col-span-1"
          placeholder="Chọn sân bóng"
          @change="onChangeField"
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
        @delete="openConfirm"
        @change-page="blogPostStore.changePageBlogPost"
      />
    </div>
  </div>

  <ConfirmDialog
    :open="isConfirmCancelDiablog"
    :message="'Bạn có chắc chắn muốn huỷ lịch này?'"
    @close="isConfirmCancelDiablog = false"
    @submit="onConfirmDelete"
  />
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import CustomTable from '@/components/tables/CustomTable.vue'
import ActionButton from '@/components/buttons/ActionButton.vue'
import SearchTab from '@/components/search/SearchTab.vue'
import Dropdown from 'primevue/dropdown'
import type { DropdownType } from '@/constants/types'
import { useFieldStore } from '@/stores/fieldStore'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import { useBlogPostStore } from '@/stores/postStore'
import { useRouter } from 'vue-router'
import { FieldStatus } from '@/constants/field'
const router = useRouter()

const layoutStore = useLayoutStore()
const blogPostStore = useBlogPostStore()
const fieldStore = useFieldStore()

const openDialog = ref(false)
const listFields = ref<DropdownType[]>([])
const selectedField = ref<DropdownType>()
const isConfirmCancelDiablog = ref(false)
const selectedDeleteFieldId = ref(0)

const onDetail = async (id: number) => {
  router.push(`/field/post/${id}`)
}

const onEdit = async (id: number) => {
  onDetail(id)
  openDialog.value = true
}

const onSearch = async () => {
  await blogPostStore.getBlogPosts()
}

const openConfirm = (id: number) => {
  selectedDeleteFieldId.value = id
  isConfirmCancelDiablog.value = true
}

const onConfirmDelete = async (id: number) => {
  await blogPostStore.deletePost(id)
  await onSearch()
}

const onReset = async () => {
  blogPostStore.search = {
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  onSearch()
}

const onChangeField = async () => {
  blogPostStore.search.fieldId = Number(selectedField.value?.code)
  await onSearch()
}

const headers = [
  { field: 'title', header: 'Tiêu đề' },
  { field: 'category', header: 'Loại' }
]

onMounted(async () => {
  fieldStore.ownerSearch.status = FieldStatus.ACCEPTED
  await fieldStore.getOwnerFieldList()
  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))

  if (listFields.value.length > 0) {
    selectedField.value = listFields.value[0]
    blogPostStore.search.fieldId = Number(selectedField.value.code)
  }

  layoutStore.breadcrumb = [{ label: 'My schedule' }]

  await blogPostStore.getBlogPosts()
})
</script>
