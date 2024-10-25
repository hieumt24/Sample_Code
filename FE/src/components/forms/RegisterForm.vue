<template>
  <div class="bg-white p-10 rounded-md">
    <div>
      <h1 class="font-semibold text-center text-xl mb-2">Đăng ký tài khoản</h1>
      <div class="flex w-full pb-2">
        <SelectButton
          v-model="userType"
          :options="options"
          aria-labelledby="basic"
          :allowEmpty="false"
          class="mx-auto"
        />
      </div>
      <div class="h-14">
        <InputGroup>
          <InputGroupAddon>
            <i class="pi pi-user"></i>
          </InputGroupAddon>
          <InputText v-model="userName" placeholder="Tên của bạn" />
        </InputGroup>
        <small class="text-red-400 flex items-start">{{ errors.userName }}</small>
      </div>
      <div class="h-14">
        <InputGroup>
          <InputGroupAddon>
            <i class="pi pi-envelope"></i>
          </InputGroupAddon>
          <InputText v-model="email" placeholder="Email của bạn" />
        </InputGroup>
        <small class="text-red-400 flex items-start">{{ errors.email }}</small>
      </div>
      <div class="h-14">
        <InputGroup>
          <InputGroupAddon>
            <i class="pi pi-phone"></i>
          </InputGroupAddon>
          <InputText
            v-model="phoneNumber"
            inputId="withoutgrouping"
            placeholder="Số điện thoại"
            :useGrouping="false"
            class="rounded-none"
          />
        </InputGroup>
        <small class="text-red-400 flex items-start">{{ errors.phoneNumber }}</small>
      </div>
      <div class="h-14 flex flex-col w-full">
        <Password
          class="w-full"
          v-model="password"
          toggleMask
          :feedback="false"
          placeholder="Mật khẩu"
        />
        <small class="text-red-400 flex items-start">{{ errors.password }}</small>
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
      <div class="h-5">
        <h1 v-if="errorMessage" class="text-red-500 text-center">{{ errorMessage }}</h1>
        <h1 v-if="registerSuccess && !errorMessage" class="text-green-500 text-center">
          Đã đăng ký thành công, vui lòng kiểm tra email để xác nhận tài khoản!
        </h1>
      </div>
      <div class="w-full flex mt-6">
        <ActionButton
          class="mx-auto"
          :value="'Đăng nhập'"
          isOutlined
          @click.prevent="emit('changeMode', 'login')"
        />
        <ActionButton
          class="mx-auto"
          :disable="!meta.valid || authStore.isLoggingIn"
          :value="'Đăng ký'"
          @click.prevent="submit"
        />
      </div>
    </div>
    <ProgressBar
      v-if="authStore.isRegistering"
      class="mt-2"
      mode="indeterminate"
      style="height: 6px"
    ></ProgressBar>
  </div>
</template>

<script setup lang="ts">
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import type { RegisterRequest } from '@/api/authApi'
import { ref } from 'vue'
import ActionButton from '../buttons/ActionButton.vue'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import SelectButton from 'primevue/selectbutton'
import ProgressBar from 'primevue/progressbar'

const authStore = useAuthStore()

const { t } = useI18n()

const emit = defineEmits<(e: 'changeMode', id: 'login' | 'register') => void>()

const userType = ref('Người dùng')
const options = ref(['Người dùng', 'Chủ sân'])

const registerSuccess = ref(false)
const errorMessage = ref('')

const schema = yup.object({
  userName: yup.string().required(t('validation.required')).max(50, 'Tên tối đa 50 ký tự'),
  email: yup.string().required(t('validation.required')).email('Email không hợp lệ'),
  password: yup
    .string()
    .required(t('validation.required'))
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})/,
      'Mật khẩu chứa ít nhất 1 chữ hoa, 1 số và 1 ký tự đặc biệt'
    ),
  phoneNumber: yup
    .string()
    .required(t('validation.required'))
    .matches(/^[0-9]*$/, t('validation.phoneNumber'))
    .max(11, 'Số điện thoại tối đa 11 số'),
  confirmPassword: yup
    .string()
    .required(t('validation.required'))
    .min(8, t('validation.passwordLength'))
    .oneOf([yup.ref('password')], t('validation.passwordNotMatch'))
})

const { meta, errors, defineField, handleSubmit } = useForm<RegisterRequest>({
  initialValues: {
    userName: '',
    role: 3,
    email: '',
    password: '',
    confirmPassword: '',
    phoneNumber: ''
  },
  validationSchema: schema
})

const [userName] = defineField('userName')
const [email] = defineField('email')
const [password] = defineField('password')
const [phoneNumber] = defineField('phoneNumber')
const [confirmPassword] = defineField('confirmPassword')

const submit = handleSubmit((values) => {
  authStore
    .register({ ...values, role: userType.value === 'Người dùng' ? 3 : 2 })
    .then((response) => {
      if (response.success) {
        registerSuccess.value = true
        errorMessage.value = ''
      } else {
        errorMessage.value = response.message
      }
    })
})
</script>
