<template>
  <Dialog
    :visible="openDialog"
    :header="isCreate ? 'Thêm lịch' : 'Sửa lịch'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[800px] mx-20"
  >
    <div class="w-full flex justify-start">
      <TimeInput
        v-model="startTime"
        :init-value="startTimeInit"
        label="Giờ bắt đầu"
        class="mr-5"
        :error="errors.startTime"
      />
      <TimeInput
        v-model="endTime"
        :init-value="endTimeInit"
        label="Giờ kết thúc"
        class="mr-5"
        :error="errors.endTime"
      />
    </div>
    <div class="mt-3 font-medium">
      Ghi chú: thời gian mở cửa:
      <span class="font-semibold">{{ fieldStore.field?.openTime }} </span> / thời gian đóng cửa:
      <span class="font-semibold">
        {{ fieldStore.field?.closeTime }}
      </span>
    </div>
    <div class="my-2 mt-3 border-t border-gray-300 pt-4">
      <ActionButton @click.stop="emit('close')" class="bg-green-400 mr-2" is-outlined value="Hủy" />
      <ActionButton
        @click.stop="submit"
        :disable="slotStore.loading || typeof startTime !== 'number' || typeof endTime !== 'number'"
        class="bg-green-400 mx-auto"
        value="Lưu"
      />
      <i
        v-if="slotStore.loading"
        class="pi pi-spin pi-spinner mx-auto ml-4"
        style="font-size: 2rem"
      />
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { watch } from 'vue'
import type { Booking } from '@/types/Booking'
import { useSlotStore } from '@/stores/slotStore'
import { ref } from 'vue'
import { useToast } from 'primevue/usetoast'
import TimeInput from '../calendar/TimeInput.vue'
import { useFieldStore } from '@/stores/fieldStore'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import type { SlotUpdateRequest } from '@/api/slotApi'
import { changeTimeToSeconds } from '@/utils/timeUtil'

const { t } = useI18n()
const toast = useToast()

const slotStore = useSlotStore()
const fieldStore = useFieldStore()
const startTimeInit = ref()
const endTimeInit = ref()
const openDialog = ref(false)
const loading = ref(false)
const fieldOpenTime = ref(0)
const fieldCloseTime = ref(0)

const props = defineProps<{
  open: boolean
  id?: number
  fieldId?: number
  isCreate?: boolean
  startTimeToCreate?: string
  endTimeToCreate?: string
}>()

const schema = yup.object({})

const { meta, errors, defineField, handleSubmit } = useForm<SlotUpdateRequest>({
  initialValues: {
    startTime: 0,
    endTime: 0
  },
  validationSchema: schema
})

const [startTime] = defineField('startTime')
const [endTime] = defineField('endTime')

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'afterSuccess'): void
  (e: 'submit', booking: Booking): void
}>()

const submit = () => {
  if (!props.isCreate) {
    loading.value = true
    if (props.id) {
      slotStore
        .updateSlot({
          id: props.id,
          fieldId: props.fieldId ?? 0,
          startTime: startTime.value,
          endTime: endTime.value
        })
        .then(
          () => {
            toast.add({
              severity: 'success',
              summary: 'Sửa lịch thành công',
              detail: 'Bạn đã sửa lịch thành công',
              life: 3000
            })
            loading.value = false
            emit('afterSuccess')
          },
          () => {
            toast.add({
              severity: 'error',
              summary: 'Sửa lịch thất bại',
              detail: 'Lịch trùng hoặc nằm ngoài thời gian mở cửa',
              life: 3000
            })
          }
        )
    }
    loading.value = false
  } else {
    slotStore
      .createSlot({
        fieldId: props.fieldId ?? 0,
        startTime: startTime.value,
        endTime: endTime.value
      })
      .then((response) => {
        if (response.success) {
          toast.add({
            severity: 'success',
            summary: 'Tạo slot thành công',
            detail: 'bạn đã tạo slot mới thành công',
            life: 3000
          })
          loading.value = false

          emit('afterSuccess')
        } else {
          toast.add({
            severity: 'error',
            summary: 'Tạo slot thất bại',
            detail: 'Slot trùng lịch hoặc ngoài thời gian mở cửa',
            life: 3000
          })
        }
        loading.value = false
      })
    loading.value = false
  }
}

const initValidation = () => {
  fieldOpenTime.value = changeTimeToSeconds(fieldStore.field?.openTime) ?? 0
  fieldCloseTime.value = changeTimeToSeconds(fieldStore.field?.closeTime) ?? 0
  schema.concat(
    yup.object({
      startTime: yup
        .number()
        .required(t('validation.required'))
        .min(fieldOpenTime.value, 'Thời gian bắt đầu phải sau thời gian mở cửa'),
      endTime: yup
        .number()
        .required(t('validation.required'))
        .test('is-greater-than-start-plus-hour', t('validation.endTimeTooEarly'), function (value) {
          const { startTime } = this.parent
          return value >= startTime + 3600
        })
        .max(fieldCloseTime.value, 'Thời gian kết thúc phải trước thời gian đóng cửa')
    })
  )
}

watch(
  () => props.open,
  async (value) => {
    if (!value) {
      openDialog.value = false
    } else if (!props.isCreate && props.id) {
      initValidation()
      await slotStore.getSlotById(props.id)
      startTimeInit.value = slotStore.slot?.startTime
      endTimeInit.value = slotStore.slot?.endTime
      openDialog.value = true
    } else if (props.isCreate && props.startTimeToCreate && props.endTimeToCreate) {
      startTimeInit.value = props.startTimeToCreate
      endTimeInit.value = props.endTimeToCreate
      openDialog.value = true
    } else {
      initValidation()

      openDialog.value = true
    }
  }
)
</script>
