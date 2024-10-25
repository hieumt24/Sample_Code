<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    :modal="true"
    header="Chi tiết báo cáo"
    :draggable="false"
    :style="{ width: '32rem' }"
  >
    <CustomForm :is-submit="true" @cancel="onClose" @submit="submit">
      <div>
        <h1>Chọn loại</h1>
        <Dropdown
          v-model="selectedType"
          :options="reportTypeOptions"
          class="w-[14rem] col-span-1"
          optionLabel="name"
          placeholder="Chọn thể loại"
        />
      </div>
      <div class="flex flex-col mb-4 mt-2">
        <span class="font-medium">Mô tả</span>
        <Textarea v-model="reason" rows="5" cols="30" />
      </div>
    </CustomForm>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import CustomForm from '@/components/forms/CustomForm.vue'
import { useI18n } from 'vue-i18n'
import { onMounted, ref } from 'vue'
import { useToast } from 'primevue/usetoast'

import Textarea from 'primevue/textarea'
import { useReportStore } from '@/stores/reportStore'
import { reportTypeOptions } from '@/constants/options'
import Dropdown from 'primevue/dropdown'

const { t } = useI18n()
const reportStore = useReportStore()
const toast = useToast()

const reason = ref<string>('')
const selectedType = ref()

const props = defineProps<{
  open: boolean
  fieldId: number
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'afterSuccess'): void
}>()

const onClose = () => {
  emit('close')
}

const submit = async () => {
  if (!props.fieldId) {
    return
  }
  const response = await reportStore.createReport({
    fieldId: props.fieldId,
    reportType: selectedType.value?.code,
    reason: reason.value
  })
  if (response?.success) {
    toast.add({
      severity: 'success',
      summary: 'Thành công',
      detail: 'Báo cáo đã được gửi',
      life: 3000
    })
    emit('afterSuccess')
  } else {
    if (!selectedType.value) {
      toast.add({
        severity: 'warn',
        summary: 'Không thể gửi báo cáo',
        detail: 'Vui lòng chọn loại báo cáo',
        life: 3000
      })
      return
    } else
      toast.add({
        severity: 'warn',
        summary: 'Không thể gửi báo cáo',
        detail: 'Vui lòng thử lại',
        life: 3000
      })
  }
}

onMounted(async () => {})
</script>
