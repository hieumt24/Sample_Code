<template>
  <div class="flex w-full items-start">
    <div class="flex flex-col m-5">
      <img
        class="cursor-pointer w-32 h-32 object-cover"
        src="https://th.bing.com/th/id/OIP.awAiMS1BCAQ2xS2lcdXGlwHaHH?rs=1&pid=ImgDetMain"
        alt="..."
      />
    </div>
    <div class="flex flex-col ml-10 m-5 flex-grow">
      <span class="font-medium text-4xl w-max mb-5">{{ userStore.user?.userName }}</span>
      <span class="mb-5">{{ userStore.user?.email }}</span>
      <div class="py-4 flex justify-evenly">
        <ActionButton
          class="mr-2 w-full"
          :value="$t('form.editProfile')"
          :severity="'info'"
          :is-outlined="false"
        />
      </div>
    </div>
  </div>
  <div class="w-full flex flex-col items-start gap-6 justify-center p-4">
    <span>Joing date: 12/12/2024</span>
    <span>Booking slots: 12 bookings</span>
    <span>Number of teams: 2 teams</span>
  </div>
  <div class="w-full flex flex-col items-start gap-6 justify-center p-4">
    <InputText
      id="name"
      size="small"
      class="w-full"
      type="text"
      v-model="name"
      :error="errors.name"
      :disabled="true"
    />
    <InputText
      id="email"
      size="small"
      class="w-full"
      type="text"
      v-model="email"
      :error="errors.email"
      :disabled="true"
    />
    <ActionButton
      class="mr-2 w-full"
      :value="$t('form.resetPass')"
      :severity="'info'"
      :is-outlined="false"
      @click="onChangePass"
    />
  </div>
  <div class="w-full flex flex-col items-start gap-6 justify-center p-4">
    <ActionButton
      class="mr-2 w-full"
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

const layoutStore = useLayoutStore()
const userStore = profileStore()
const userId = Cookies.get(USER_ID)
const openDialog = ref(false)

const { t } = useI18n()

const schema = yup.object({
  name: yup.string().required(t('validation.required')),
  email: yup.string().required(t('validation.required'))
})

const { meta, errors, defineField, handleSubmit, resetForm, values } = useForm<{
  name: string
  email: string
}>({
  validationSchema: schema
})

const [email] = defineField('email')
const [name] = defineField('name')

const onChangePass = async () => {
  openDialog.value = true
}

onMounted(async () => {
  await userStore.getData(Number(userId))
  layoutStore.breadcrumb = [{ label: 'Info' }]
  resetForm({
    values: { name: userStore.user?.userName ?? '', email: userStore.user?.email ?? '' }
  })
})
</script>
