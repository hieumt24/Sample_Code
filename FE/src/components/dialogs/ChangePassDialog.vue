<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    modal
    :draggable="false"
    header="Đổi mật khẩu"
    :style="{ width: '25rem' }"
  >
    <div class="flex flex-col w-fit mx-auto p-2 pb-5">
      <PasswordField
        name="oldPassword"
        :label="'Mật khẩu cũ '"
        v-model="oldPassword"
        :error="errors.oldPassword"
      />
      <PasswordField
        name="newPassword"
        :label="'Mật khẩu mới'"
        v-model="newPassword"
        :feedback="true"
        :error="errors.newPassword"
      />
      <PasswordField
        name="reNewPassword"
        :label="'Mật khẩu mới'"
        v-model="confirmPassword"
        :error="errors.newPassword"
      />
    </div>
    <div class="flex justify-between px-10 py-2 border-t pt-5">
      <Button label="Đóng" class="w-32" @click="onClose" outlined></Button>
      <Button label="Xác nhận" class="w-32" @click.prevent="submit"></Button>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import CustomForm from '@/components/forms/CustomForm.vue'
import InputField from '@/components/inputs/InputField.vue'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import type { ChangePassRequest } from '@/api/authApi'
import { onMounted } from 'vue'
import PasswordField from '../inputs/PasswordField.vue'
import Password from 'primevue/password'
import Button from 'primevue/button'

const authStore = useAuthStore()

const { t } = useI18n()

const props = defineProps<{
  open: boolean
  data: { [key: string]: string | number } | undefined
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

const schema = yup.object({
  email: yup.string().required(t('validation.required')),
  oldPassword: yup.string().required(t('validation.required')),
  newPassword: yup.string().required(t('validation.required')),
  confirmPassword: yup
    .string()
    .required(t('validation.required'))
    .oneOf([yup.ref('newPassword')], t('validation.confirmPassword'))
})

const { meta, errors, defineField, handleSubmit, resetForm, values } = useForm<ChangePassRequest>({
  validationSchema: schema
})

const [email] = defineField('email')
const [oldPassword] = defineField('oldPassword')
const [newPassword] = defineField('newPassword')
const [confirmPassword] = defineField('confirmPassword')

const onClose = () => {
  emit('close')
}

const submit = handleSubmit((values) => {
  authStore.changePass(values)
})

onMounted(async () => {
  resetForm({
    values: { email: props.data?.email.toString() || '' }
  })
})
</script>
