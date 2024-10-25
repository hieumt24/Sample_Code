<template>
  <Dialog
    :visible="open"
    :header="'Thêm lịch đặt sân ngày ' + formatDateVietnamese(date)"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[800px] mx-20"
  >
    <div class="w-full flex items-center">
      <div class="flex flex-col mr-5">
        <TimeInput
          :init-value="startTimeRaw"
          v-model="startTime"
          label="Giờ bắt đầu"
          class="mr-5"
          :error="startTimeError"
          :disabled="disabled"
        />

        <TimeInput
          :init-value="endTimeRaw"
          v-model="endTime"
          label="Giờ kết thúc"
          class="mr-5"
          :error="endTimeError"
          :disabled="disabled"
        />
      </div>
    </div>

    <div class="my-2 mt-5 border-t border-gray-300 pt-4">
      <ActionButton
        @click.stop="emit('close')"
        class="bg-green-400 mr-2"
        is-outlined
        value="Đóng"
      />
      <ActionButton
        @click.stop="submit"
        :disable="!meta.valid || bookingStore.isBooking"
        class="bg-green-400 mx-auto"
        value="Thêm"
      />
      <i
        v-if="bookingStore.isBooking"
        class="pi pi-spin pi-spinner mx-auto ml-4"
        style="font-size: 2rem"
      ></i>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useBookingStore } from '@/stores/bookingStoreOwner'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { formatDateVietnamese, getCurrentDate } from '@/utils/dateUtil'
import { watch } from 'vue'
import { ref } from 'vue'
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import type { Booking } from '@/types/Booking'
import TimeInput from '../calendar/TimeInput.vue'

const { t } = useI18n()

const bookingStore = useBookingStore()
const partialFieldStore = usePartialFieldStore()

const props = defineProps<{
  open: boolean
  id?: number
  partialId?: number
  date?: string
  fieldName?: string
  initStartTime?: string
  initEndTime?: string
  disabled?: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', booking: Booking): void
}>()

const startTimeRaw = ref()
const endTimeRaw = ref()
const errorMessage = ref()
const startTimeError = ref('')
const endTimeError = ref('')

const schema = yup.object({
  startTime: yup.string().required(t('validation.required')),

  endTime: yup.string().required(t('validation.required'))
})

const { meta, errors, defineField, handleSubmit, resetForm, values } = useForm<any>({
  validationSchema: schema
})

const [startTime] = defineField('startTime')
const [endTime] = defineField('endTime')

const submit = handleSubmit(async (values) => {
  if (!props.partialId) return

  const data = await bookingStore.createABooking({
    startTime: values.startTime,
    endTime: values.endTime,
    partialFieldId: props.partialId,
    date: props.date ?? getCurrentDate()
  })
  if (data.success) {
    emit('submit', data.data)
    emit('close')
  } else {
    console.log(data.message)
    errorMessage.value = data.message
  }
})

watch(
  () => props.open,
  async (value) => {
    if (value) {
      if (!props.id) {
        if (props.initStartTime && props.initEndTime) {
          startTimeRaw.value =
            props.initStartTime?.substring(0, 2) + ':' + props.initStartTime?.substring(3)
          endTimeRaw.value =
            props.initEndTime?.substring(0, 2) + ':' + props.initEndTime?.substring(3)
        }
      } else {
        await bookingStore.getBookingById(props.id)
      }
    }
  }
)
</script>
