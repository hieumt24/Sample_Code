<template>
  <div class="flex w-full items-start">
    <div class="mb-5 bg-white rounded-md shadow-md flex flex-col p-5">
      <div class="relative m-5 w-36 h-36 rounded-full overflow-hidden border border-gray-300">
        <Image
          :src="tempAvatarUrl || userStore.user?.avatar || '/noavatar.png'"
          alt="Avatar"
          preview
          :pt="{
            root: { class: 'w-full h-full' },
            image: { class: 'w-full h-full object-cover' }
          }"
        />
      </div>
      <FileUpload
        v-if="updatingProfile"
        ref="fileInput"
        mode="basic"
        name="avatar"
        accept="image/*"
        customUpload
        :auto="true"
        @select="onAvatarImageFileSelect"
        class="mt-2 ml-0.5 mx-auto"
        chooseLabel="Chọn ảnh đại diện"
      />
    </div>

    <div class="flex flex-col ml-10 m-5 flex-grow">
      <span class="font-medium text-4xl w-max mb-5">{{ userStore.user?.userName }}</span>
      <span class="mb-5">Email: {{ userStore.user?.email }}</span>
      <span>Ngày đăng ký: {{ formatDate(userStore.user?.createdAt ?? '') }}</span>
    </div>
  </div>

  <div class="w-full md:w-2/3 lg:w-1/2 xl:w-1/3 p-4 rounded-md mb-5 bg-white shadow-lg">
    <div class="w-full">
      <div class="flex m-2">
        <span class="w-20 flex items-end">Họ tên: </span>
        <InputText
          id="userName"
          size="small"
          type="text"
          v-model="userName"
          :error="errors.userName"
          :disabled="!updatingProfile"
        />
      </div>

      <div class="flex m-2">
        <span class="w-20 flex items-end">Email: </span>
        <InputText
          id="email"
          size="small"
          type="text"
          v-model="email"
          :error="errors.email"
          :disabled="true"
        />
      </div>
    </div>
    <div class="my-4 mx-2 flex mt-10">
      <ActionButton
        v-if="!updatingProfile"
        class="w-56 mx-2"
        :value="$t('form.editProfile')"
        :severity="'info'"
        is-outlined
        @click="onCanceled"
      />
      <ActionButton
        v-if="updatingProfile"
        class="w-56 mx-2"
        value="Hủy"
        :severity="'warning'"
        is-outlined
        @click="updatingProfile = false"
      />
      <ActionButton
        v-if="updatingProfile"
        class="w-56 mx-2"
        value="Lưu"
        :severity="'success'"
        @click="onChangeProfile"
      />
    </div>
  </div>

  <div
    class="w-full md:w-2/3 lg:w-1/2 xl:w-1/3 border border-gray-300 p-4 grid grid-cols-1 md:grid-cols-2 rounded-md"
  >
    <ActionButton
      class="col-span-1 mx-2"
      :value="$t('form.resetPass')"
      :severity="'info'"
      @click="onChangePass"
    />

    <ActionButton
      class="col-span-1 mx-2"
      :value="$t('form.inActiveAccount')"
      :severity="'danger'"
      :is-outlined="false"
    />
  </div>

  <ChangePassDialog :open="openDialog" :data="userStore.user" @close="openDialog = false" />
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import InputText from 'primevue/inputtext'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { profileStore } from '@/stores/userStore'
import Cookies from 'js-cookie'
import { USER_ID } from '@/constants/storage'
import ChangePassDialog from '@/components/dialogs/ChangePassDialog.vue'
import * as yup from 'yup'
import { useI18n } from 'vue-i18n'
import { useForm } from 'vee-validate'
import { useToast } from 'primevue/usetoast'
import Image from 'primevue/image'
import FileUpload, { type FileUploadSelectEvent } from 'primevue/fileupload'
import { formatDate } from '@/utils/dateUtil'

const layoutStore = useLayoutStore()
const userStore = profileStore()
const userId = Cookies.get(USER_ID)
const openDialog = ref(false)
const updatingProfile = ref(false)
const toast = useToast()
const tempAvatarUrl = ref<string | null>(null)
const avatarFile = ref<File | undefined>(undefined)

const { t } = useI18n()

const schema = yup.object({
  userName: yup.string().required(t('validation.required')),
  email: yup.string().required(t('validation.required'))
})

const { meta, errors, defineField, handleSubmit, resetForm, values, setFieldValue } = useForm<{
  userName: string
  email: string
  phoneNumber: string
  avatar: File | undefined
}>({
  validationSchema: schema
})

const [email] = defineField('email')
const [userName] = defineField('userName')

const onChangePass = async () => {
  openDialog.value = true
}

const onCanceled = () => {
  updatingProfile.value = true
  tempAvatarUrl.value = null
  setFieldValue('userName', userStore.user?.userName ?? '')
}

const onChangeProfile = async () => {
  updatingProfile.value = false
  let response = await userStore.changeProfileUser(
    { ...values, avatar: avatarFile.value },
    Number(userId)
  )
  if (response.success) {
    toast.add({
      severity: 'success',
      summary: 'Thành công',
      detail: 'Thay đổi thông tin thành công',
      life: 3000
    })
    userStore.getData(Number(userId))
    localStorage.setItem('avatar', response.data.avatar ?? '/noavatar.png')
  } else {
    toast.add({
      severity: 'error',
      summary: 'Thất bại',
      detail: 'Thay đổi thông tin thất bại, vui lòng thử lại',
      life: 3000
    })
  }
}

const onAvatarImageFileSelect = (event: FileUploadSelectEvent) => {
  const selectedFile = event.files[event.files.length - 1]
  avatarFile.value = selectedFile
  tempAvatarUrl.value = URL.createObjectURL(selectedFile)
}

onMounted(async () => {
  await userStore.getData(Number(userId))
  layoutStore.breadcrumb = [{ label: 'Info' }]
  resetForm({
    values: { userName: userStore.user?.userName ?? '', email: userStore.user?.email ?? '' }
  })
})
</script>
