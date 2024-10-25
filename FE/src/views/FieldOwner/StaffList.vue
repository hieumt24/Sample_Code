<template>
  <div class="flex flex-wrap w-full">
    <div class="">
      <SearchTab @search="onSearch" @reset="onReset" class="mb-4">
        <ActionButton value="Thêm nhân viên" @click="openCreateDialog = true" :is-outlined="true" />
        <div class="flex flex-col my-2">
          <h1>Chọn sân</h1>
          <Dropdown
            v-model="selectedField"
            :options="listFields"
            optionLabel="name"
            placeholder="Chọn sân"
            class="w-full md:w-[14rem]"
            @change="onChangeField"
          />
        </div>
        <hr class="mt-2" />
        <div>
          <h3 class="font-medium mb-2">Trạng thái</h3>
          <Dropdown
            v-model="selectedStatus"
            :options="listStatus"
            optionLabel="name"
            placeholder="Chọn trạng thái"
            class="w-56"
            @change="onChangeStatus"
          />
        </div>
        <div>
          <h3 class="font-medium mb-2">Tên nhân viên</h3>
          <InputText v-model="staffStore.search.name" />
        </div>
      </SearchTab>
    </div>
    <div class="flex-1 lg:px-10">
      <CustomTable
        :headers="headers"
        :total="staffStore.total"
        :loading="tableLoading"
        :items="staffStore.staffs"
        @edit="onEdit"
        @detail="onDetail"
        @change-page="staffStore.changePage"
        no-delete
        is-staff-list
      />
    </div>
  </div>

  <StaffCreateDialog
    :field="selectedField"
    :open="openCreateDialog"
    @close="openCreateDialog = false"
  />
  <StaffDetailDialog
    :open="openDetailDialog"
    :staff-id="selectedStaffId"
    :field-id="selectedFieldId"
    @close="openDetailDialog = false"
  />
  <StaffEditDialog
    :open="openEditDialog"
    :staff-id="selectedStaffId"
    :field-id="selectedFieldId"
    @close="openEditDialog = false"
  />
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import CustomTable from '@/components/tables/CustomTable.vue'
import SearchTab from '@/components/search/SearchTab.vue'
import Dropdown from 'primevue/dropdown'
import type { DropdownType } from '@/constants/types'
import { useFieldStore } from '@/stores/fieldStore'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import { useI18n } from 'vue-i18n'
import { FieldStatus } from '@/constants/field'
import { useStaffStore } from '@/stores/staffStore'
import StaffCreateDialog from '@/components/dialogs/StaffCreateDialog.vue'
import ActionButton from '@/components/buttons/ActionButton.vue'
import StaffDetailDialog from '@/components/dialogs/StaffDetailDialog.vue'
import StaffEditDialog from '@/components/dialogs/StaffEditDialog.vue'
import InputText from 'primevue/inputtext'
import { StaffStatus } from '@/constants/staff'

const { t } = useI18n()

const fieldStore = useFieldStore()
const staffStore = useStaffStore()

const tableLoading = ref(false)
const listFields = ref<DropdownType[]>([])
const selectedField = ref<DropdownType>()
const openCreateDialog = ref(false)
const openDetailDialog = ref(false)
const openEditDialog = ref(false)
const selectedStaffId = ref(0)
const listStatus = ref<DropdownType[]>([
  { name: 'Tất cả', code: undefined },
  { name: StaffStatus.Active, code: true },
  { name: StaffStatus.NotActive, code: false }
])
const selectedStatus = ref<DropdownType>(listStatus.value[0])

const selectedFieldId = computed(() => Number(selectedField.value?.code))
const onDetail = async (id: number) => {
  selectedStaffId.value = id
  openDetailDialog.value = true
}

const onEdit = async (id: number) => {
  selectedStaffId.value = id
  openEditDialog.value = true
}

const onSearch = async () => {
  tableLoading.value = true
  await staffStore.getStaffList()
  tableLoading.value = false
}

const onReset = async () => {
  selectedStatus.value = listStatus.value[0]
  staffStore.search = {
    fieldId: staffStore.search.fieldId,
    name: '',
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  await onSearch()
}

const onChangeField = async () => {
  staffStore.search.fieldId = Number(selectedField.value?.code)
  await onReset()
}
const onChangeStatus = async () => {
  staffStore.search.isActive =
    selectedStatus.value?.code === undefined ? undefined : !!selectedStatus.value?.code
}

const headers = [
  { field: 'name', header: 'Tên nhân viên', width: '25%' },
  { field: 'phoneNumer', header: 'Số điện thoại', width: '25%' },
  { field: 'userName', header: 'Tài khoản', width: '25%' }
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
    staffStore.search.fieldId = Number(selectedField.value.code)

    await onSearch()
  }
})
</script>
