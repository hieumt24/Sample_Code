<template>
  <main class="flex-1">
    <section class="w-full h-full mx-auto space-x-4 relative">
      <section class="w-full h-auto background-image"></section>
      <section class="w-full card-group">
        <form class="space-y-4 card">
          <section>
            <InputField
              :label="$t('form.username')"
              v-model="email"
              :error="errors.email"
              class="w-full"
              id="email"
              name="email"
            />
          </section>
          <ActionButton class="w-full">Submit</ActionButton>
          <section>
            <InputField
              :label="$t('form.entercode')"
              v-model="password"
              :error="errors.password"
              class="w-full"
              id="password"
              name="password"
            />
          </section>
          <ActionButton class="w-full">Submit</ActionButton>
        </form>
      </section>
    </section>
  </main>
</template>

<script setup lang="ts">
import ActionButton from '@/components/buttons/ActionButton.vue'
import InputField from '@/components/inputs/InputField.vue'
import { useRouter } from 'vue-router'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const router = useRouter()

const schema = yup.object({
  email: yup.string().required().email(),
  password: yup.string().required(t('validation.required')).min(8)
})

const { errors, defineField } = useForm({
  initialValues: {
    email: '',
    password: ''
  },
  validationSchema: schema
})

const [email] = defineField('email')
const [password] = defineField('password')

const navigateHome = () => {
  router.push('/user/home')
}
</script>
