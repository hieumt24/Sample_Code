<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    :modal="true"
    header="Thêm thời gian không hoạt động"
    :draggable="false"
    :style="{ width: '32rem' }"
  >
    <CustomForm :is-submit="true" @cancel="onClose" @submit="submit">
      <div class="flex justify-between">
        <CustomCalendar
          name="fromDate"
          class="col-span-1"
          label="Ngày bắt đầu"
          v-model="fromDate"
          :min-date="getCurrentDate()"
        />
        <TimeInput v-model="startTime" label="Giờ bắt đầu" class="ml-5" />
      </div>
      <div class="flex mt-2 justify-between">
        <CustomCalendar
          name="fromDate"
          class="col-span-1"
          label="Ngày kết thúc"
          v-model="toDate"
          :min-date="fromDate"
        />
        <TimeInput v-model="endTime" label="Giờ kết thúc" class="ml-5" />
      </div>
      <div class="flex flex-col mb-4 mt-2">
        <span class="font-medium">Lý do</span>
        <Textarea v-model="reason" rows="5" cols="30" />
        <h1 class="text-green-500 text-justify">
          Chú ý: Khi tạo lịch nghỉ, tất cả các lịch đặt trong khoảng thời gian này sẽ bị huỷ và
          không thể hoàn tác khi lịch nghỉ được xoá.
        </h1>
      </div>
    </CustomForm>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import CustomForm from '@/components/forms/CustomForm.vue'
import { useI18n } from 'vue-i18n'
import { ref, watch } from 'vue'
import { useScheduleStore } from '@/stores/scheduleStore'
import { useToast } from 'primevue/usetoast'
import CustomCalendar from '../calendar/CustomCalendar.vue'
import TimeInput from '../calendar/TimeInput.vue'
import { addDaysToDate, getCurrentDate } from '@/utils/dateUtil'
import { changeSecondToHour, changeTimeToSeconds } from '@/utils/timeUtil'
import Textarea from 'primevue/textarea'
import { useFieldStore } from '@/stores/fieldStore'

const { t } = useI18n()
const scheduleStore = useScheduleStore()
const toast = useToast()
const fieldStore = useFieldStore()

const fromDate = ref<string>()
const startTime = ref(0)
const toDate = ref<string>(addDaysToDate(getCurrentDate(), 1))
const endTime = ref(0)
const reason = ref<string>('')

const props = defineProps<{
  isCreate: boolean
  open: boolean
  fieldId: number
  id?: number
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'afterSuccess'): void
}>()

const onClose = () => {
  emit('close')
}

const submit = async () => {
  if (!props.isCreate && props.id) {
    await scheduleStore
      .updateSchedule({
        id: props.id ?? 0,
        startTime: fromDate.value + 'T' + changeSecondToHour(startTime.value + '') + ':00.118Z',
        endTime: toDate.value + 'T' + changeSecondToHour(endTime.value + '') + ':00.118Z',
        reason: reason.value
      })
      .then((reponse) => {
        if (reponse.success) {
          toast.add({
            severity: 'success',
            summary: 'Thành công',
            detail: 'Đã cập nhật lịch nghỉ thành công',
            life: 3000
          })
          emit('afterSuccess')
        } else {
          toast.add({
            severity: 'warn',
            summary: 'Không thể cập nhật lịch nghỉ',
            detail: 'Lịch nghỉ đã trùng với lịch nghỉ khác hoặc đã qua thời gian',
            life: 3000
          })
        }
      })
  } else {
    const response = await scheduleStore.createSchedule({
      startTime: fromDate.value + 'T' + changeSecondToHour(startTime.value + '') + ':00.118Z',
      endTime: toDate.value + 'T' + changeSecondToHour(endTime.value + '') + ':00.118Z',
      reason: reason.value,
      fieldId: props.fieldId
    })
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Thành công',
        detail: 'Đã thêm lịch nghỉ thành công',
        life: 3000
      })
      emit('afterSuccess')
    } else {
      toast.add({
        severity: 'warn',
        summary: 'Không thể thêm lịch nghỉ',
        detail: 'Lịch nghỉ đã trùng với lịch nghỉ khác hoặc đã qua thời gian',
        life: 3000
      })
    }
  }
}

watch(
  () => props.open,
  () => {
    if (props.open) {
      if (props.isCreate) {
        fromDate.value = addDaysToDate(getCurrentDate(), 1)
        startTime.value = 0
        toDate.value = addDaysToDate(getCurrentDate(), 2)
        endTime.value = 0
        reason.value = ''
      } else if (props.id) {
        scheduleStore.getScheduleById(props.id).then((response) => {
          if (!response.data) return
          fromDate.value = response.data.startTime.toString().split('T')[0]
          startTime.value = changeTimeToSeconds(
            response.data.startTime.toString().substring(11, 16)
          )
          toDate.value = response.data.endTime.toString().split('T')[0]
          endTime.value = changeTimeToSeconds(response.data.endTime.toString().substring(11, 16))
          reason.value = response.data.reason
        })
      }
    }
  }
)
</script>
