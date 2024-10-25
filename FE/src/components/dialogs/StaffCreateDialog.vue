<template>
  <Dialog
    :visible="open"
    :header="'Thêm nhân viên'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[600px] mx-20"
  >
    <div class="relative">
      <div
        v-if="formLoading"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <form @submit.prevent="createStaffConfirm">
        <div class="my-4">
          <div class="flex flex-col mb-4">
            <label for="fieldName" class="mb-2 font-semibold">Sân</label>
            <InputText id="fieldName" disabled :value="field?.name" class="p-2 border rounded-md" />
          </div>
          <div class="flex flex-col mb-4">
            <label for="name" class="mb-2 font-semibold">Tên nhân viên</label>
            <InputText id="name" v-model="name" class="p-2 border rounded-md" />
            <ErrorMessage name="name" class="text-red-500" />
          </div>
          <div class="flex flex-col mb-4">
            <label for="phoneNumber" class="mb-2 font-semibold">Số điện thoại</label>
            <InputText id="phoneNumber" v-model="phoneNumber" class="p-2 border rounded-md" />
            <ErrorMessage name="phoneNumber" class="text-red-500" />
          </div>
          <div class="flex flex-col mb-4">
            <label for="userName" class="mb-2 font-semibold">Tài khoản</label>

            <InputText id="userName" v-model="userName" class="p-2 border rounded-md" />
            <ErrorMessage name="userName" class="text-red-500" />
          </div>
          <div class="flex flex-col mb-4">
            <label for="password" class="mb-2 font-semibold">Mật khẩu</label>
            <Password v-model="password" toggleMask :feedback="false" />
            <ErrorMessage name="password" class="text-red-500" />
          </div>
        </div>
        <Message v-if="errorMessage" :closable="false" severity="error">{{ errorMessage }}</Message>
        <Button type="submit" label="Tạo" class="w-full mt-4 font-bold py-2 px-4 rounded" />
      </form>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import type { CreateStaffRequest } from '@/api/staffApi'
import type { DropdownType } from '@/constants/types'
import { useStaffStore } from '@/stores/staffStore'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Message from 'primevue/message'
import Password from 'primevue/password'
import ProgressSpinner from 'primevue/progressspinner'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'
import { ErrorMessage, useForm } from 'vee-validate'
import { onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import * as yup from 'yup'
const props = defineProps<{
  open: boolean
  field?: DropdownType
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'onCreateSuccessfully'): void
}>()
const { t } = useI18n()
const toast = useToast()
const confirm = useConfirm()
const staffStore = useStaffStore()

const formLoading = ref(false)
const errorMessage = ref()

const schema = yup.object({
  fieldId: yup.number().required(),
  name: yup.string().required(t('validation.required')),
  phoneNumber: yup
    .string()
    .matches(/^\(?(\d{3})\)?[-. ]?(\d{3})[-. ]?(\d{4})$/, t('validation.phoneNumber'))
    .required(t('validation.required')),
  userName: yup.string().required(t('validation.required')),
  password: yup
    .string()
    .required(t('validation.required'))
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})/,
      'Mật khẩu chứa 8 ký tự trở lên và ít nhất 1 chữ hoa, 1 số và 1 ký tự đặc biệt'
    )
})

const { meta, errors, defineField, handleSubmit, resetForm, values } = useForm<CreateStaffRequest>({
  validationSchema: schema
})

const [name] = defineField('name')
const [phoneNumber] = defineField('phoneNumber')
const [userName] = defineField('userName')
const [password] = defineField('password')
const [fieldId] = defineField('fieldId')
watch(
  () => props.field,
  (newValue) => {
    fieldId.value = Number(newValue?.code)
  }
)

const submitForm = handleSubmit(async (values) => {
  try {
    formLoading.value = true
    const response = await staffStore.createStaff(values)
    if (response.success) {
      toast.add({ severity: 'success', detail: 'Tài khoản đã được tạo thành công!', life: 3000 })
      emit('onCreateSuccessfully')
      emit('close')
    } else {
      toast.add({
        severity: 'error',
        detail: 'Thao tác không thành công. Vui lòng thử lại!',
        life: 3000
      })
      errorMessage.value = response.message
    }
  } catch (error) {
    toast.add({ severity: 'error', detail: 'Đã xảy ra lỗi. Vui lòng thử lại!', life: 3000 })
  } finally {
    formLoading.value = false
  }
})

const createStaffConfirm = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn muốn tạo nhân viên mới không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await submitForm()
    }
  })
}
</script>
