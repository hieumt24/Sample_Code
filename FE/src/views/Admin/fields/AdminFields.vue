<template>
  <div class="flex">
    <div>
      <SearchTab @search="onSearch" @reset="onReset">
        <InputField
          name="Name"
          :label="$t('searchTab.name')"
          v-model="fieldStoreAdmin.search.name"
        />
        <InputField
          name="address"
          :label="$t('searchTab.address')"
          v-model="fieldStoreAdmin.search.address"
        />
        <div class="mt-2 mb-2 flex flex-col">
          <label class="font-semibold" for="province">Tỉnh Thành</label>
          <AutoComplete
            input-class="w-full"
            inputId="province"
            v-model="selectedProvince"
            :suggestions="filteredProvince"
            @complete="searchProvince"
            @item-select="onProvinceChange"
            optionLabel="full_name"
            dropdown
            forceSelection
            :disabled="locationStore.isProvinceLoading"
            :dropdownIcon="
              locationStore.isProvinceLoading
                ? 'pi pi-spinner pi-spin animate-spin'
                : 'pi pi-chevron-down'
            "
          />
        </div>
        <div class="mt-2 mb-2 flex flex-col">
          <label class="font-semibold" for="district">Quận Huyện</label>
          <AutoComplete
            input-class="w-full"
            inputId="district"
            v-model="selectedDistrict"
            :suggestions="filteredDistrict"
            @complete="searchDistrict"
            @item-select="onDistrictChange"
            optionLabel="full_name"
            dropdown
            forceSelection
            :disabled="locationStore.isDistrictLoading || !selectedProvince"
            :dropdownIcon="
              locationStore.isDistrictLoading
                ? 'pi pi-spinner pi-spin animate-spin'
                : 'pi pi-chevron-down'
            "
          />
        </div>
        <div class="mt-2 mb-2 flex flex-col">
          <label class="font-semibold" for="district">Phường Xã</label>
          <AutoComplete
            input-class="w-full"
            inputId="commune"
            v-model="selectedCommune"
            :suggestions="filteredCommune"
            @complete="searchCommune"
            @item-select="onCommuneChange"
            optionLabel="full_name"
            dropdown
            forceSelection
            :disabled="locationStore.isCommuneLoading || !selectedDistrict"
            :dropdownIcon="
              locationStore.isCommuneLoading
                ? 'pi pi-spinner pi-spin animate-spin'
                : 'pi pi-chevron-down'
            "
          />
        </div>
        <div class="mt-2 mb-2 flex flex-col">
          <label class="font-semibold" for="status">Trạng thái</label>
          <Dropdown
            inputId="status"
            v-model="selectedStatus"
            :options="FieldStatusOptions"
            optionLabel="name"
            placeholder="Chọn trạng thái"
            class="w-full"
          />
        </div>
      </SearchTab>
    </div>
    <div class="flex-1 mx-10">
      <CustomTable
        :headers="headers"
        :items="fieldStoreAdmin.fields"
        @detail="onShowReport"
        @edit="onFieldDetail"
        hasFieldStatus
        :no-delete="true"
        :total="fieldStoreAdmin.total"
        :loading="fieldStoreAdmin.loading"
        @change-page="fieldStoreAdmin.changePageAdmin"
      />
    </div>
    <AdminFieldDialog
      :open="isOpenDetailDialog"
      @close="isOpenDetailDialog = false"
      :fieldId="selectedFieldId"
    />
    <AdminFieldReportDialog
      :open="isOpenReportDialog"
      @close="isOpenReportDialog = false"
      :fieldId="selectedFieldId"
    />
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import CustomTable from '@/components/tables/CustomTable.vue'
import { useI18n } from 'vue-i18n'
import router from '@/router'
import { useToast } from 'primevue/usetoast'
import SearchTab from '@/components/search/SearchTab.vue'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import InputField from '@/components/inputs/InputField.vue'
import { useFieldStoreAdmin } from '@/stores/fieldStoreAdmin'
import AdminFieldDialog from '@/components/dialogs/AdminFieldDialog.vue'
import AutoComplete, { type AutoCompleteItemSelectEvent } from 'primevue/autocomplete'
import { useLocationStore } from '@/stores/locationStore'
import Dropdown from 'primevue/dropdown'
import { FieldStatusOptions } from '@/constants/options'
import type { DropdownType } from '@/constants/types'
import AdminFieldReportDialog from '@/components/dialogs/AdminFieldReportDialog.vue'
const { t } = useI18n()

const layoutStore = useLayoutStore()
const fieldStoreAdmin = useFieldStoreAdmin()
const locationStore = useLocationStore()
const isOpenDetailDialog = ref(false)
const isOpenReportDialog = ref(false)
const selectedFieldId = ref(0)
const selectedProvince = ref()
const selectedDistrict = ref()
const selectedCommune = ref()
const filteredProvince = ref<any[]>([])
const filteredDistrict = ref<any[]>([])
const filteredCommune = ref<any[]>([])
const selectedStatus = ref<DropdownType>(FieldStatusOptions[0])

const onFieldDetail = (fieldId: number) => {
  selectedFieldId.value = fieldId
  isOpenDetailDialog.value = true
}

const onShowReport = async (fieldId: number) => {
  selectedFieldId.value = fieldId
  isOpenReportDialog.value = true
}

const onSearch = async () => {
  fieldStoreAdmin.search.status = selectedStatus.value.code?.toString() ?? ''
  await fieldStoreAdmin.getFieldList()
}

const onReset = async () => {
  selectedStatus.value = FieldStatusOptions[0]
  selectedProvince.value = null
  selectedDistrict.value = null
  selectedCommune.value = null
  fieldStoreAdmin.search = {
    name: '',
    address: '',
    province: '',
    district: '',
    commune: '',
    status: '',
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  await fieldStoreAdmin.getFieldList()
}

onMounted(async () => {
  layoutStore.breadcrumb = [{ label: t('sidebar.admin.fields') }]
  await fieldStoreAdmin.getFieldList()
})

const headers = [
  { field: 'name', header: 'Tên sân', width: '20%' },
  { field: 'address', header: 'Địa chỉ', width: '50%' },
  { field: 'ownerName', header: 'Chủ sân', width: '15%' },
  { field: 'phoneNumber', header: 'SĐT', width: '15%' }
]
const searchProvince = async (event: any) => {
  if (locationStore.allProvince.length > 0) {
    filteredProvince.value = locationStore.allProvince.filter((item) =>
      item.full_name.toLowerCase().includes(event.query.toLowerCase())
    )
  } else {
    await locationStore.getAllProvinces()
  }
}

const onProvinceChange = async (event: AutoCompleteItemSelectEvent) => {
  selectedProvince.value = event.value
  selectedDistrict.value = null
  locationStore.allDistrict = []
  filteredDistrict.value = []
  selectedCommune.value = null
  locationStore.allCommune = []
  filteredCommune.value = []
  searchDistrict(null)
  fieldStoreAdmin.search.province = selectedProvince.value.full_name
  fieldStoreAdmin.search.district = ''
}
const searchDistrict = async (event: any) => {
  if (selectedProvince.value) {
    if (locationStore.allDistrict.length > 0) {
      filteredDistrict.value = locationStore.allDistrict.filter((item) =>
        item.full_name.toLowerCase().includes(event.query.toLowerCase())
      )
    } else {
      await locationStore.getDistrictsByProvinceId(selectedProvince.value.id)
    }
  }
}

const onDistrictChange = async (event: AutoCompleteItemSelectEvent) => {
  selectedDistrict.value = event.value
  selectedCommune.value = null
  locationStore.allCommune = []
  filteredCommune.value = []
  searchCommune(null)
  fieldStoreAdmin.search.district = selectedDistrict.value.full_name
  fieldStoreAdmin.search.commune = ''
}

const searchCommune = async (event: any) => {
  if (selectedDistrict.value) {
    if (locationStore.allCommune.length > 0) {
      filteredCommune.value = locationStore.allCommune.filter((item) =>
        item.full_name.toLowerCase().includes(event.query.toLowerCase())
      )
    } else {
      await locationStore.getCommunesByDistrictId(selectedDistrict.value.id)
    }
  }
}

const onCommuneChange = async (event: AutoCompleteItemSelectEvent) => {
  selectedCommune.value = event.value
  fieldStoreAdmin.search.commune = selectedCommune.value.full_name
}
</script>
