<template>
  <div class="flex flex-col md:flex-row gap-4">
    <div class="w-1/2">
      <Dropdown
        v-model="selectedField"
        :options="listFields"
        optionLabel="name"
        placeholder="Chọn sân"
        class="w-full md:w-[14rem] my-2"
        @change="onChangeField"
      />
      <InputField name="province" label="Tiêu đề" v-model="title" />
      <div class="flex items-center my-2 h-36">
        <div class="w-1/2 flex flex-col gap-2">
          <div>
            <Checkbox v-model="isPinned" :binary="true" inputId="inPinned" />
            <label for="inPinned" class="ml-2 cursor-pointer">
              Ghim lên trang chủ của sân bóng
            </label>
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
        <small class="text-red-500">{{ errors.content }}</small>
      </div>

      <div class="flex justify-center p-10">
        <Button @click="submit" :disabled="!meta.valid" label="Tạo bài viết" class="bg-green-400" />
      </div>
    </div>
    <div class="w-1/2 bg-white flex flex-col p-4">
      <h1 class="font-semibold text-center text-lg">Bản xem trước</h1>
      <div class="">
        <div class="flex w-full">
          <div>
            {{ formatDateVietnamese(dayjs().toString()) }}
          </div>
        </div>
        <div class="flex">
          <h1 class="text-2xl font-bold mr-2">{{ title }}</h1>
          <Chip v-if="isPinned" label="Bài viết đã ghim" icon="pi pi-paperclip" />
        </div>
        <div class="flex items-center mt-5"></div>
      </div>
      <span class="p-4 bg-white shadow-sm rounded-sm" v-html="content"></span>
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
import { useFieldStore } from '@/stores/fieldStore'
import Checkbox from 'primevue/checkbox'
import InputField from '@/components/inputs/InputField.vue'
import { categoryOptions } from '@/constants/blogPost'
import { useToast } from 'primevue/usetoast'
import { useRouter } from 'vue-router'
import { formatDateVietnamese, formatDateYYYYHHHHH } from '@/utils/dateUtil'
import dayjs from 'dayjs'
import Chip from 'primevue/chip'

const { t } = useI18n()
const toast = useToast()
const router = useRouter()

defineEmits<(e: 'changeMode', id: 'forgot' | 'register') => void>()

const blogPostStore = useBlogPostStore()
const fieldStore = useFieldStore()

const thumnailTemp = ref()
const selectedField = ref<DropdownType>()
const selectedCategory = ref<DropdownType>()
const listFields = ref<DropdownType[]>([])
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

const [fieldId] = defineField('fieldId')
const [category] = defineField('category')
const [title] = defineField('title')
const [content] = defineField('content')
const [isPinned] = defineField('isPinned')
const [thumbnail] = defineField('thumbnail')

const onChangeField = async () => {
  fieldId.value = Number(selectedField.value?.code ?? 0)
}

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
      router.push(`/field/post/${response.data.id}`)
    }
  })
})

onMounted(async () => {
  await fieldStore.getOwnerFieldList()
  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))
})
</script>
