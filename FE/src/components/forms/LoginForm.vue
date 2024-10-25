<template>
  <div class="bg-white p-10 rounded-md mb-2">
    <h1 class="text-center text-xl font-semibold mb-8">Đăng nhập</h1>
    <div class="h-20">
      <h1 class="font-medium">Email của bạn (Tên đăng nhập với nhân viên)</h1>
      <InputGroup>
        <InputText v-model="email" name="email" placeholder="Nhập email" />
      </InputGroup>
      <small class="text-red-400 flex items-start">{{ errors.email }}</small>
    </div>
    <div class="h-20 w-full flex flex-col">
      <h1 class="font-medium">Mật khẩu</h1>
      <Password
        class="w-full"
        v-model="password"
        toggleMask
        name="password"
        :feedback="false"
        placeholder="Nhập mật khẩu"
      />
      <small class="text-red-400 flex items-start">{{ errors.password }}</small>
    </div>
    <div class="w-full flex justify-between relative">
      <span class="cursor-pointer hover:underline" @click="$emit('changeMode', 'forgot')">{{
        $t('form.forgotPassword')
      }}</span>
      <span class="cursor-pointer hover:underline" @click="$emit('changeMode', 'register')">{{
        $t('form.title.register')
      }}</span>
    </div>
    <ProgressBar
      v-if="authStore.isLoggingIn"
      mode="indeterminate"
      style="height: 6px"
    ></ProgressBar>
    <div>
      <span class="text-red-400" v-if="authStore.loginError">
        Tên đăng nhập hoặc mật khâu không đúng!
      </span>
    </div>
    <div class="w-full flex mt-10">
      <ActionButton
        class="mx-auto"
        :disable="!meta.valid || authStore.isLoggingIn"
        :value="'Đăng nhập'"
        @click.prevent="submit"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import ActionButton from '../buttons/ActionButton.vue'
import ProgressBar from 'primevue/progressbar'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'

const authStore = useAuthStore()

const { t } = useI18n()

defineEmits<(e: 'changeMode', id: 'forgot' | 'register') => void>()

const schema = yup.object({
  email: yup.string().required(t('validation.required')),
  password: yup.string().required(t('validation.required')).min(8, t('validation.passwordLength'))
})

const { meta, errors, defineField, handleSubmit } = useForm({
  initialValues: {
    email: '',
    password: ''
  },
  validationSchema: schema
})

const [email] = defineField('email')
const [password] = defineField('password')

const cancel = () => {
  console.log('cancel')
}

const submit = handleSubmit((values) => {
  authStore.login(values)
})
</script>
