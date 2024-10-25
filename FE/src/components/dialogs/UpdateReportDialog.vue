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
        <h1>Chọn trạng thái</h1>
        <Dropdown
          v-model="selectedStatus"
          :options="reportStatusOptions"
          class="w-[14rem] col-span-1"
          optionLabel="name"
          placeholder="Chọn trạng thái"
        />
      </div>
      <div class="flex flex-col mb-4 mt-2">
        <span class="font-medium">Ghi chú</span>
        <Textarea v-model="adminNotes" rows="5" cols="30" />
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
import { reportStatusOptions } from '@/constants/options'
import Dropdown from 'primevue/dropdown'

const { t } = useI18n()
const reportStore = useReportStore()
const toast = useToast()

const adminNotes = ref<string>('')
const selectedStatus = ref('')

const props = defineProps<{
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
  if (!props.id) {
    return
  }
  const response = await reportStore.updateReport({
    id: props.id,
    status: selectedStatus.value,
    adminNotes: adminNotes.value
  })
  if (response?.success) {
    toast.add({
      severity: 'success',
      summary: 'Thành công',
      detail: 'Cập nhật báo cáo thành công',
      life: 3000
    })
    emit('afterSuccess')
  } else {
    toast.add({
      severity: 'warn',
      summary: 'Không thể cập nhật',
      detail: 'Vui lòng thử lại',
      life: 3000
    })
  }
}

onMounted(async () => {})
</script>
