<template>
  <Dialog
    :visible="open"
    :header="'Báo cáo'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[800px] mx-20"
  >
    <div class="relative w-full flex items-center">
      <div
        v-if="loading"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <CustomTable
        :headers="headers"
        :total="reportStore.total"
        :loading="reportStore.loading"
        :items="reports"
        no-ban
        no-delete
        no-edit
        no-detail
        @change-page="onChangePage"
      />
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import CustomTable from '@/components/tables/CustomTable.vue'
import { useToast } from 'primevue/usetoast'
import { useReportStore } from '@/stores/reportStore'
import ProgressSpinner from 'primevue/progressspinner'
import Dialog from 'primevue/dialog'
import { reportTypeOptions } from '@/constants/options'
import type { ReportType } from '@/constants/report'
const toast = useToast()
const props = defineProps<{
  open: boolean
  fieldId: number
}>()
const emit = defineEmits<(e: 'close') => void>()

const reportStore = useReportStore()

const reports = computed(() => {
  return reportStore.reports.map((report) => ({
    ...report,
    reportType: getNameByCode(report.reportType)
  }))
})
const getNameByCode = (code: string): string | undefined => {
  const option = reportTypeOptions.find((option) => option.code === code)
  return option ? option.name : undefined
}
const openDialog = ref(false)
const openEditDialog = ref(false)
const loading = ref(false)
watch(
  () => props.open,
  async (value) => {
    if (value) {
      loading.value = true
      reportStore.search.fieldId = props.fieldId
      await reportStore.getReportsAdmin()
      loading.value = false
    }
  }
)

const onChangePage = async (page: number) => {
  loading.value = true
  await reportStore.changePageReport(page)
  loading.value = false
}

const headers = [
  { field: 'reportType', header: 'Loại báo cáo', width: '30%' },
  { field: 'reason', header: 'Lý do', width: '70%' }
]

onUnmounted(() => {
  reportStore.search.fieldId = undefined
})
</script>
