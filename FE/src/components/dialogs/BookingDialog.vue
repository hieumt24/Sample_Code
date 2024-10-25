<template>
  <Dialog
    :visible="open"
    :header="'Chi tiết đặt lịch ngày ' + formatDateVietnamese(date ?? bookingStore.booking?.date)"
    :modal="true"
    @update:visible="onClose"
    :draggable="false"
    class="w-[700px] mx-20"
  >
    <div class="w-full flex">
      <div class="flex flex-col mr-5 flex-1">
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
          :disabled="disabled"
          :error="endTimeError"
        />
      </div>
      <div class="flex flex-col items-end justify-between flex-1" v-if="!id">
        <div class="flex flex-col items-end">
          <div class="font-medium">
            Số tiền phải đặt cọc:
            <span class="text-red-500">
              {{ Math.floor(Number(fieldStore.field?.deposit ?? 0)).toLocaleString() }}
            </span>
            VND
          </div>
          <div class="font-medium">
            Số dư hiện tại:
            <span class="text-red-500">
              {{ Math.floor(Number(balance ?? 0)).toLocaleString() }}
            </span>
            VND
          </div>
        </div>
        <div v-if="errorMessage" class="flex">
          <span class="font-medium text-red-700 mr-2 text-balance text-end">
            {{ errorMessage }}
          </span>
          <ActionButton @click.stop="emit('errorClose')" class="bg-green-400" value="Tải lại" />
        </div>
      </div>
    </div>

    <div class="my-2 mt-5 border-t border-gray-300 pt-4">
      <ActionButton
        @click.stop="emit('close')"
        class="bg-green-400 mr-2"
        is-outlined
        :disable="errorMessage"
        value="Đóng"
      />
      <ActionButton
        @click.stop="submit"
        v-if="!id"
        :disable="!meta.valid || bookingStore.isBooking || errorMessage"
        icon="check"
        class="bg-green-400 mx-auto"
        value="Đặt lịch"
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
import { changeTimeToSeconds } from '@/utils/timeUtil'
import { computed, watch } from 'vue'
import { ref } from 'vue'
import type { Booking } from '@/types/Booking'
import TimeInput from '../calendar/TimeInput.vue'
import { useFieldStore } from '@/stores/fieldStore'

const { t } = useI18n()
const balance = computed(() => localStorage.getItem('balance'))

const bookingStore = useBookingStore()
const fieldStore = useFieldStore()

const props = defineProps<{
  open: boolean
  id?: number
  partialId?: number
  disabled?: boolean
  date?: string
  fieldName?: string
  initStartTime?: string
  initEndTime?: string
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', booking: Booking): void
  (e: 'errorClose'): void
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
      errorMessage.value = undefined
      if (!props.id) {
        const initStartTimeArr = props.initStartTime?.split(':')
        const initEndTimeArr = props.initEndTime?.split(':')
        if (initStartTimeArr?.length === 2 && initEndTimeArr?.length === 2) {
          startTimeRaw.value = initStartTimeArr[0] + ':' + initStartTimeArr[1]
          endTimeRaw.value = initEndTimeArr[0] + ':' + initEndTimeArr[1]
        }

        startTime.value = changeTimeToSeconds(startTimeRaw.value)
        endTime.value = changeTimeToSeconds(endTimeRaw.value)
      } else {
        await bookingStore.getBookingById(props.id)
      }
    }
  }
)
const onClose = () => {
  if (errorMessage.value) {
    emit('errorClose')
  } else {
    emit('close')
  }
}
</script>
