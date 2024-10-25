<template>
  <CustomForm :is-submit="meta.valid" no-cancel no-submit :title="$t('form.title.forgot')">
    <div class="h-14 mt-8">
      <InputGroup>
        <InputGroupAddon>
          <i class="pi pi-envelope"></i>
        </InputGroupAddon>
        <InputText v-model="email" placeholder="Email của bạn" />
      </InputGroup>
      <small class="text-red-400 flex items-start">{{ errors.email }}</small>
    </div>
    <div>
      <span class="cursor-pointer hover:underline" @click="$emit('changeMode', 'login')">{{
        $t('form.back')
      }}</span>
    </div>

    <div class="flex items-center justify-center mt-5">
      <ActionButton :value="$t('form.sendCode')" @click.prevent="submit" />
    </div>
  </CustomForm>
</template>

<script setup lang="ts">
import CustomForm from '@/components/forms/CustomForm.vue'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import { useToast } from 'primevue/usetoast'
import ActionButton from '../buttons/ActionButton.vue'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'
import InputText from 'primevue/inputtext'

const authStore = useAuthStore()

const { t } = useI18n()
const toast = useToast()

const emit = defineEmits<(e: 'changeMode', id: 'login' | 'register') => void>()

const schema = yup.object({
  email: yup.string().required(t('validation.required')).email('Email không hợp lệ')
})

const { meta, errors, defineField, handleSubmit } = useForm<{ email: string }>({
  initialValues: {
    email: ''
  },
  validationSchema: schema
})

const [email] = defineField('email')

const cancel = () => {
  emit('changeMode', 'login')
}

const submit = handleSubmit((values) => {
  authStore.forgetPassword(values.email).then(
    (reponse) => {
      if (reponse.success) {
        toast.add({
          severity: 'success',
          summary: 'Đã gửi mã xác nhận',
          detail: 'Vui lòng kiểm tra email của bạn',
          life: 3000
        })
      }
    },
    (error) => {
      console.log(error)
      toast.add({
        severity: 'error',
        summary: 'Đã có lỗi xảy ra',
        detail: 'Có lỗi xảy ra, vui lòng thử lại',
        life: 3000
      })
    }
  )
})
</script>
