<template>
  <Dialog
    :visible="false"
    @update:visible="console.log('update')"
    :modal="true"
    header="Partial field"
    :draggable="false"
    :style="{ width: '25rem' }"
  >
    <!-- <CustomForm :is-submit="meta.valid" @cancel="onClose" @submit="submit">
      <InputField
        :disabled="true"
        name="name"
        :label="'Name'"
        v-model="name"
        :error="errors.name"
      />
      <InputField
        textarea
        name="description"
        :label="'Description'"
        v-model="description"
        :error="errors.description"
      />
      <InputField name="reason" :label="'Reason'" v-model="reason" :error="errors.reason" />
    </CustomForm> -->
  </Dialog>
</template>
<!-- 
<script setup lang="ts">
import Dialog from 'primevue/dialog'
import CustomForm from '@/components/forms/CustomForm.vue'
import InputField from '@/components/inputs/InputField.vue'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import { useI18n } from 'vue-i18n'
import { onMounted } from 'vue'
import type { ScheduleCreateRequest } from '@/api/fieldApi'
import Calendar from 'primevue/calendar'
import { useScheduleStore } from '@/stores/scheduleStore'
import { useToast } from 'primevue/usetoast'

const { t } = useI18n()
const scheduleStore = useScheduleStore()
const toast = useToast()

const props = defineProps<{
  isCreate: boolean
  open: boolean
  data: { [key: string]: string | number } | undefined
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

const schema = yup.object({
  startTime: yup.string().required(t('validation.required')),
  endTime: yup.string().required(t('validation.required')),
  reason: yup.string().required(t('validation.required'))
})

const { meta, errors, defineField, handleSubmit, resetForm, values } =
  useForm<ScheduleCreateRequest>({
    validationSchema: schema
  })

const [startTime] = defineField('startTime')
const [endTime] = defineField('endTime')
const [reason] = defineField('reason')

const onClose = () => {
  emit('close')
}

const submit = handleSubmit(async (values) => {
  if (props.data) {
    console.log('update')
    let response = await scheduleStore.updateSchedule(values, 2)
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Update Successfully',
        detail: response.message,
        life: 3000
      })
    }
    emit('close')
  } else {
    console.log('create')
    let response = await scheduleStore.createSchedule(values)
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Create Successfully',
        detail: response.message,
        life: 3000
      })
    }
    emit('close')
  }
})
onMounted(async () => {
  resetForm({
    values: {}
  })
})
</script> -->
