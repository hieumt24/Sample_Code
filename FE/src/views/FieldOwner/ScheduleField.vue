<template>
  <div class="flex flex-wrap w-full">
    <div class="">
      <SearchTab @search="onSearch" @reset="onReset" class="mb-4">
        <ActionButton
          :value="$t('form.create')"
          class="col-span-1"
          @click="onCreate"
          :is-outlined="true"
        />
        <Dropdown
          v-model="selectedField"
          :options="listFields"
          optionLabel="name"
          class="col-span-1"
          placeholder="Chọn sân"
          @change="onChangeField"
        />
        <hr />
        <h1 class="font-medium">Tìm trong khoảng ngày</h1>
        <CustomCalendar
          name="fromDate"
          class="col-span-1"
          label="Từ ngày"
          v-model="scheduleStore.search.startDate"
        />
        <CustomCalendar
          name="fromDate"
          class="col-span-1"
          label="Đến ngày"
          v-model="scheduleStore.search.endDate"
        />
      </SearchTab>
    </div>
    <div class="flex-1 lg:px-10">
      <CustomTable
        :headers="headers"
        :total="scheduleStore.total"
        :loading="scheduleStore.loading"
        :items="
          scheduleStore.schedules.map((schedule) => ({
            ...schedule,
            startTime: formatDateYYYYHHHHH(schedule.startTime),
            endTime: formatDateYYYYHHHHH(schedule.endTime)
          }))
        "
        no-detail
        @edit="onEdit"
        @delete="onConfirmDelete"
        @change-page="scheduleStore.changePage"
      />
    </div>
  </div>

  <CreateInactiveTimeDialog
    :isCreate="true"
    :field-id="scheduleStore.selectedFieldId"
    :open="openDialog"
    @close="openDialog = false"
    @after-success="afterSuccess"
  />

  <CreateInactiveTimeDialog
    :isCreate="false"
    :field-id="scheduleStore.selectedFieldId"
    :open="openEditDialog"
    :id="selectedSchedultId"
    @close="openEditDialog = false"
    @after-success="afterSuccess"
  />

  <DetailDialog :open="openDetail" :data="scheduleStore.schedule" @close="openDetail = false" />
  <ConfirmDialog
    :open="isConfirmDeleteDiablog"
    :message="'Bạn có chắc chắn muốn xoá lịch nghỉ này không?'"
    @close="isConfirmDeleteDiablog = false"
    @submit="onDelete"
  />
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import CustomTable from '@/components/tables/CustomTable.vue'
import DetailDialog from '@/components/dialogs/DetailDialog.vue'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { useScheduleStore } from '@/stores/scheduleStore'
import CreateInactiveTimeDialog from '@/components/dialogs/CreateInactiveTimeDialog.vue'
import SearchTab from '@/components/search/SearchTab.vue'
import Dropdown from 'primevue/dropdown'
import type { DropdownType } from '@/constants/types'
import { useFieldStore } from '@/stores/fieldStore'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import ConfirmDialog from '@/components/dialogs/ConfirmDialog.vue'
import { useToast } from 'primevue/usetoast'
import { formatDateYYYYHHHHH } from '@/utils/dateUtil'
import { FieldStatus } from '@/constants/field'
const toast = useToast()

const layoutStore = useLayoutStore()
const scheduleStore = useScheduleStore()
const fieldStore = useFieldStore()

const openDetail = ref(false)
const openDialog = ref(false)
const openEditDialog = ref(false)
const isConfirmDeleteDiablog = ref(false)
const listFields = ref<DropdownType[]>([])
const selectedField = ref<DropdownType>()
const onSelectedSchedule = ref(0)
const selectedSchedultId = ref(0)

const onCreate = () => {
  scheduleStore.schedule = undefined
  openDialog.value = true
}

const onEdit = async (id: number) => {
  selectedSchedultId.value = id
  openEditDialog.value = true
}

const onDelete = async () => {
  await scheduleStore.deleteSchedule(onSelectedSchedule.value).then((response) => {
    if (response.success) {
      onSearch()
      isConfirmDeleteDiablog.value = false
      toast.add({
        severity: 'info',
        summary: 'Xoá lịch nghỉ',
        detail: 'Đã xoá lịch nghỉ thành công',
        life: 3000
      })
    } else {
      toast.add({
        severity: 'error',
        summary: 'Không thể xoá lịch nghỉ',
        detail: 'Có lỗi xảy ra, vui lòng thử lại',
        life: 3000
      })
    }
  })
}

const onConfirmDelete = (id: number) => {
  isConfirmDeleteDiablog.value = true
  onSelectedSchedule.value = id
}

const onSearch = async () => {
  await scheduleStore.getScheduleList()
}

const onReset = async () => {
  scheduleStore.search = {
    limit: LIMIT_PER_PAGE,
    startDate: '',
    endDate: '',
    offset: 0,
    fieldId: scheduleStore.selectedFieldId
  }
  onSearch()
}

const onChangeField = async () => {
  scheduleStore.selectedFieldId = Number(selectedField.value?.code)
  await onReset()
}

const afterSuccess = () => {
  onSearch()
  openDialog.value = false
  openEditDialog.value = false
}

const headers = [
  { field: 'startTime', header: 'Bắt đầu' },
  { field: 'endTime', header: 'Kết thúc' },
  { field: 'reason', header: 'Lý do' }
]

onMounted(async () => {
  fieldStore.ownerSearch.status = FieldStatus.ACCEPTED
  await fieldStore.getOwnerFieldList()
  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))

  if (listFields.value.length > 0) {
    selectedField.value = listFields.value[0]
    scheduleStore.selectedFieldId = Number(selectedField.value.code)
  }

  layoutStore.breadcrumb = [{ label: 'My schedule' }]

  await scheduleStore.getScheduleList()
})
</script>
