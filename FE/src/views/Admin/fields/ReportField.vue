<template>
  <div class="flex flex-wrap w-full">
    <div class="">
      <SearchTab @search="onSearch" @reset="onReset" class="mb-4">
        <hr />
      </SearchTab>
    </div>
    <div class="flex-1 lg:px-10">
      <CustomTable
        :headers="headers"
        :total="reportStore.total"
        :loading="reportStore.loading"
        :items="reportStore.reports"
        @detail="onDetail"
        @change-page="reportStore.changePageReport"
      />
    </div>
  </div>

  <UpdateReportDialog
    :field-id="selectedReportId"
    :open="openDialog"
    @close="openDialog = false"
    @after-success="afterSuccess"
  />
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import CustomTable from '@/components/tables/CustomTable.vue'
import SearchTab from '@/components/search/SearchTab.vue'
import Dropdown from 'primevue/dropdown'
import type { DropdownType } from '@/constants/types'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import { useToast } from 'primevue/usetoast'
import { useReportStore } from '@/stores/reportStore'
import UpdateReportDialog from '@/components/dialogs/UpdateReportDialog.vue'
const toast = useToast()

const reportStore = useReportStore()

const openDialog = ref(false)
const openEditDialog = ref(false)
const selectedReportId = ref(0)

const onDetail = async (id: number) => {
  selectedReportId.value = id
  openDialog.value = true
}

const onSearch = async () => {
  await reportStore.getReportsAdmin()
}

const onReset = async () => {
  reportStore.search = {
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  onSearch()
}

const afterSuccess = () => {
  onSearch()
  openDialog.value = false
  openEditDialog.value = false
}

const headers = [
  { field: 'startTime', header: 'Ngày bắt đầu' },
  { field: 'endTime', header: 'Ngày kết thúc' },
  { field: 'reason', header: 'Lý do' }
]

onMounted(async () => {
  await reportStore.getReportsAdmin()
})
</script>
