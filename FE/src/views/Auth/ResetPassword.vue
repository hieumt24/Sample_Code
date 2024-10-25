<template>
  <div class="bg-cover bg-guest flex items-center">
    <div class="h-80 w-96 mx-auto bg-white p-5 rounded-md">
      <h1 class="font-semibold text-center text-xl my-3">Cài lại mật khẩu</h1>
      <div v-if="mode === 'forgot'">
        <div class="h-14">
          <InputGroup>
            <InputGroupAddon>
              <i class="pi pi-envelope"></i>
            </InputGroupAddon>
            <InputText v-model="email" placeholder="Email của bạn" />
          </InputGroup>
          <small class="text-red-400 flex items-start">{{ errors.email }}</small>
        </div>
        <div class="h-14 flex flex-col w-full">
          <Password
            class="w-full"
            v-model="newPassword"
            toggleMask
            :feedback="false"
            placeholder="Mật khẩu"
          />
          <small class="text-red-400 flex items-start">{{ errors.newPassword }}</small>
        </div>
        <div class="h-14 flex flex-col w-full">
          <Password
            v-model="confirmPassword"
            toggleMask
            :feedback="false"
            placeholder="Xác nhận mật khẩu"
          />
          <small class="text-red-400 flex items-start">{{ errors.confirmPassword }}</small>
        </div>
        <div>
          <h1 class="hover:underline cursor-pointer" @click.prevent="$router.push('/')">
            Trang chủ
          </h1>
          <h1 class="text-red-500">{{ errorMessage }}</h1>
        </div>
        <div class="flex w-full">
          <ActionButton
            :disabled="!meta.valid"
            class="mx-auto"
            value="Xác nhận"
            @click.prevent="submit"
          />
        </div>
      </div>
    </div>
  </div>
  <UserFooter />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { useAuthStore } from '@/stores/authStore'
import { useI18n } from 'vue-i18n'
import { useToast } from 'primevue/usetoast'
import { useRouter } from 'vue-router'
import type { ResetPasswordRequest } from '@/api/authApi'
import Password from 'primevue/password'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'
import InputText from 'primevue/inputtext'
import UserFooter from '@/components/foodters/UserFooter.vue'

const { t } = useI18n()
const toast = useToast()
const router = useRouter()

const authStore = useAuthStore()

const mode = ref<'login' | 'forgot' | 'register'>('forgot')
const errorMessage = ref<string>('')

const schema = yup.object({
  email: yup.string().required(t('validation.required')).email('Email không hợp lệ'),
  newPassword: yup
    .string()
    .required(t('validation.required'))
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})/,
      'Mật khẩu chứa ít nhất 1 chữ hoa, 1 chữ thường và 1 số'
    ),
  confirmPassword: yup
    .string()
    .required(t('validation.required'))
    .min(8, t('validation.passwordLength'))
    .oneOf([yup.ref('newPassword')], t('validation.passwordNotMatch'))
})

const { meta, errors, defineField, handleSubmit } = useForm<ResetPasswordRequest>({
  initialValues: {
    email: '',
    newPassword: ''
  },
  validationSchema: schema
})

const [email] = defineField('email')
const [newPassword] = defineField('newPassword')
const [confirmPassword] = defineField('confirmPassword')

const submit = handleSubmit((values) => {
  authStore
    .resetPassword({
      email: values.email,
      newPassword: values.newPassword,
      confirmPassword: values.newPassword
    })
    .then((reponse) => {
      if (reponse.success) {
        toast.add({
          severity: 'success',
          summary: 'Đã đổi mật khẩu',
          detail: 'Mật khẩu của bạn đã được thay đổi',
          life: 3000
        })
        router.push({
          name: 'login-screen'
        })
      } else {
        toast.add({
          severity: 'warn',
          summary: 'Vui lòng thử lại',
          detail: 'Email không đúng hoặc mã đã hết hạn, vui lòng thử lại',
          life: 3000
        })
      }
    })
})
</script>

<style scoped>
.bg-guest {
  background-image: url(/bg-guest.jpg);
  height: calc(100vh - 7rem);
}
</style>
