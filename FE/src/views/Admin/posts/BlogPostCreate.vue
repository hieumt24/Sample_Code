<template>
  <div class="flex flex-col md:flex-row gap-4">
    <div class="w-1/2">
      <InputField name="province" label="Tiêu đề" v-model="title" />
      <div class="flex items-center my-2 h-36">
        <div class="w-1/2 flex flex-col gap-2">
          <div>
            <Checkbox v-model="isPinned" :binary="true" inputId="inPinned" />
            <label for="inPinned" class="ml-2 cursor-pointer"> Ghim lên trang chủ </label>
          </div>
          <Dropdown
            v-model="selectedCategory"
            :options="listCategory"
            optionLabel="name"
            placeholder="Chọn loại bài viết"
            class="w-full md:w-[14rem]"
            @change="onChangeCategory"
          />
          <FileUpload
            mode="basic"
            name="demo[]"
            class="w-56"
            url="/api/upload"
            chooseLabel="Chọn ảnh đại diện"
            accept="image/*"
            :maxFileSize="1000000"
            @select="onUploadThumnail"
          />
        </div>
        <div class="flex justify-between w-1/2">
          <div class="h-32 bg-cover">
            <img v-if="thumnailTemp" :src="thumnailTemp" alt="..." class="h-32" />
          </div>
        </div>
      </div>
      <div class="card">
        <Editor v-model="content" editorStyle="height: 320px" />
      </div>

      <div class="flex justify-center p-10">
        <Button
          @click="submit"
          label="Tạo bài viết"
          class="bg-green-400"
          :loading="blogPostStore.creating"
        />
      </div>
    </div>
    <div class="w-1/2 bg-white">
      <h1 class="font-semibold text-center text-lg">Bản xem trước</h1>
      <div class="">
        <h2 class="font-semibold">{{ title }}</h2>
        <div class="p-10" v-html="content"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useBlogPostStore } from '@/stores/postStore'
import Editor from 'primevue/editor'
import { onMounted, ref } from 'vue'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { useI18n } from 'vue-i18n'
import type { BlogPostCreateRequest } from '@/api/postApi'
import FileUpload, { type FileUploadSelectEvent } from 'primevue/fileupload'
import Button from 'primevue/button'
import Dropdown from 'primevue/dropdown'
import type { DropdownType } from '@/constants/types'
import Checkbox from 'primevue/checkbox'
import InputField from '@/components/inputs/InputField.vue'
import { categoryOptions } from '@/constants/blogPost'
import { useToast } from 'primevue/usetoast'
import { useRouter } from 'vue-router'

const { t } = useI18n()
const toast = useToast()
const router = useRouter()

defineEmits<(e: 'changeMode', id: 'forgot' | 'register') => void>()

const blogPostStore = useBlogPostStore()

const thumnailTemp = ref()
const selectedCategory = ref<DropdownType>()
const listCategory = ref<DropdownType[]>(categoryOptions)

const schema = yup.object({
  title: yup.string().required(t('validation.required')),
  content: yup.string().required(t('validation.required'))
})

const { meta, errors, defineField, handleSubmit } = useForm<BlogPostCreateRequest>({
  initialValues: {
    fieldId: undefined,
    title: '',
    content: '',
    isPinned: false,
    thumbnail: undefined
  },
  validationSchema: schema
})

const [category] = defineField('category')
const [title] = defineField('title')
const [content] = defineField('content')
const [isPinned] = defineField('isPinned')
const [thumbnail] = defineField('thumbnail')

const onChangeCategory = async () => {
  category.value = selectedCategory.value?.code + ''
}

const onUploadThumnail = (event: FileUploadSelectEvent) => {
  thumnailTemp.value = URL.createObjectURL(event.files[event.files.length - 1])
  thumbnail.value = event.files[event.files.length - 1]
}

const submit = handleSubmit((values) => {
  blogPostStore.createPost(values).then((response) => {
    if (response.success) {
      toast.add({
        severity: 'info',
        summary: 'Tạo bài viết',
        detail: 'Đã tạo bài viết thành công',
        life: 3000
      })
      router.push(`/admin/post/${response.data.id}`)
    }
  })
})

onMounted(async () => {})
</script>
