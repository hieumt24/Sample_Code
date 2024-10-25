<template>
  <div class="absolute top-4 left-4 z-10 w-fit flex items-start space-x-2">
    <div class="relative bg-white shadow-lg rounded-md w-1/2 min-w-[400px]">
      <div class="p-2">
        <div class="flex items-center space-x-2">
          <div v-if="isInputMode" class="flex items-center w-full space-x-2">
            <InputText
              type="text"
              class="bg-transparent !rounded-full w-full border-2"
              v-model="freeSearchKeyword"
              placeholder="Tìm bằng tên, địa chỉ, số điện thoại"
            />
          </div>
          <div v-else class="grid grid-cols-2 gap-2 w-full">
            <AutoComplete
              placeholder="Chọn Tỉnh Thành"
              forceSelection
              v-model="selectedProvince"
              :suggestions="filteredProvince"
              @complete="searchProvince"
              @item-select="onProvinceChange"
              optionLabel="full_name"
              dropdown
              :disabled="locationStore.isProvinceLoading"
              :dropdownIcon="
                locationStore.isProvinceLoading
                  ? 'pi pi-spinner pi-spin animate-spin'
                  : 'pi pi-chevron-down'
              "
              :pt:input:class="'text-base leading-none appearance-none rounded-md rounded-tr-none rounded-br-none m-0 py-2 px-3 text-surface-700 dark:text-white/80 border bg-surface-0 dark:bg-surface-950 border-surface-300 dark:border-surface-700 focus:outline-none focus:outline-offset-0 focus:ring-1 focus:ring-primary-500 dark:focus:ring-primary-400 focus:z-10 transition-colors duration-200 w-full'"
            />
            <AutoComplete
              placeholder="Chọn Quận Huyện"
              forceSelection
              v-model="selectedDistrict"
              :suggestions="filteredDistrict"
              @complete="searchDistrict"
              @item-select="onDistrictChange"
              optionLabel="full_name"
              dropdown
              :disabled="locationStore.isDistrictLoading || !selectedProvince"
              :dropdownIcon="
                locationStore.isDistrictLoading
                  ? 'pi pi-spinner pi-spin animate-spin'
                  : 'pi pi-chevron-down'
              "
              :pt:input:class="'text-base leading-none appearance-none rounded-md rounded-tr-none rounded-br-none m-0 py-2 px-3 text-surface-700 dark:text-white/80 border bg-surface-0 dark:bg-surface-950 border-surface-300 dark:border-surface-700 focus:outline-none focus:outline-offset-0 focus:ring-1 focus:ring-primary-500 dark:focus:ring-primary-400 focus:z-10 transition-colors duration-200 w-full'"
            />
            <input
              type="datetime-local"
              v-model="startTime"
              class="border border-gray-300 rounded-md w-full"
            />
            <Dropdown
              v-model="selectedDuration"
              :options="durationOptions"
              optionLabel="label"
              optionValue="value"
              placeholder="Select Duration"
              class="w-full"
            />
            <InputNumber
              v-model="fromPrice"
              placeholder="Từ giá"
              :min="0"
              :step="10000"
              suffix=" VNĐ"
              class="w-full"
              :pt:input:root="'leading-none flex flex-auto py-2 px-3 m-0 rounded-md text-surface-800 dark:text-white/80 placeholder:text-surface-400 dark:placeholder:text-surface-500 bg-surface-0 dark:bg-surface-950 border border-surface-300 dark:border-surface-700 invalid:focus:ring-red-200 invalid:hover:border-red-500 hover:border-primary focus:outline-none focus:outline-offset-0 focus:ring-1 focus:ring-primary-500 dark:focus:ring-primary-400 focus:z-10 w-full'"
            />
            <InputNumber
              v-model="toPrice"
              placeholder="Đến giá"
              :min="0"
              :step="10000"
              suffix=" VNĐ"
              class="w-full"
              :pt:input:root="'leading-none flex flex-auto py-2 px-3 m-0 rounded-md text-surface-800 dark:text-white/80 placeholder:text-surface-400 dark:placeholder:text-surface-500 bg-surface-0 dark:bg-surface-950 border border-surface-300 dark:border-surface-700 invalid:focus:ring-red-200 invalid:hover:border-red-500 hover:border-primary focus:outline-none focus:outline-offset-0 focus:ring-1 focus:ring-primary-500 dark:focus:ring-primary-400 focus:z-10 w-full'"
            />
          </div>
        </div>
      </div>
      <div
        v-if="fieldsToDisplay.length > 0"
        class="bg-white shadow-lg rounded-b-md overflow-auto"
        :style="{ maxHeight: `calc(100vh - ${topOffset}px)` }"
        ref="listFieldDiv"
      >
        <div class="h-full overflow-y-auto">
          <ul class="list-none p-0 m-0">
            <li
              v-for="field in fieldsToDisplay"
              :key="field.id"
              class="p-4 border-b border-gray-200 hover:bg-gray-100 cursor-pointer"
              @click="selectResult(field)"
              @mouseover="hoverResult(field)"
              @mouseleave="unhoverResult"
            >
              <h3 class="text-base font-semibold mb-1">{{ field.name }}</h3>
              <p class="text-sm text-gray-600 mb-1">{{ field.address }}</p>
              <div class="flex items-center text-sm">
                <span v-if="field.rating && field.rating != 'N/A'" class="text-gray-600 mr-1">
                  {{ field.rating }}
                  <i class="pi pi-star-fill text-green-400 ml-2" style="font-size: 1rem" />
                  <span class="text-gray-500 mx-1">·</span>
                </span>

                <span :class="isOpen(field) ? 'text-green-600' : 'text-red-600'">
                  {{ isOpen(field) ? 'Đang mở cửa' : 'Đã đóng cửa' }}
                </span>
                <span class="text-gray-500 mx-1">·</span>
                <span class="text-gray-600">{{ field.openTime }} - {{ field.closeTime }}</span>
              </div>
            </li>
          </ul>
        </div>
      </div>

      <Message :closable="false" v-else-if="!isSearchLoading && hasSearched" severity="secondary">
        Không tìm thấy!
      </Message>
      <ProgressSpinner
        v-if="isSearchLoading"
        class="absolute top-full left-1/2 transform -translate-x-1/2 mt-2"
      />
    </div>
    <div class="space-x-2 p-2">
      <Button icon="pi pi-search" aria-label="Search" @click="onSearch" />
      <Button
        v-tooltip.right="'Chuyển loại tìm kiếm'"
        icon="pi pi-arrow-right-arrow-left"
        @click="toggleMode"
        class="mr-2"
      >
      </Button>

      <Dropdown
        v-model="selectedRating"
        :options="ratingOptions"
        optionLabel="label"
        placeholder="Chọn đánh giá"
        v-if="!isInputMode"
      >
        <template #value="slotProps">
          <div v-if="slotProps.value" class="flex items-center">
            <span>{{ slotProps.value.label }}</span>
            <span class="ml-2">
              <i class="pi pi-star-fill text-green-400 mr-1"></i>
            </span>
          </div>
          <span v-else>{{ slotProps.placeholder }}</span>
        </template>
        <template #option="slotProps">
          <div class="flex items-center">
            <span>{{ slotProps.option.label }}</span>
            <span class="ml-2">
              <i class="pi pi-star-fill text-green-400 mr-1"></i>
            </span>
          </div>
        </template>
      </Dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { nextTick, onMounted, ref, watch } from 'vue'
import InputText from 'primevue/inputtext'
import ProgressSpinner from 'primevue/progressspinner'
import { useFieldStoreUser } from '@/stores/fieldStoreUser'
import type { Field } from '@/types/Field'
import AutoComplete, { type AutoCompleteItemSelectEvent } from 'primevue/autocomplete'
import { useLocationStore } from '@/stores/locationStore'
import Button from 'primevue/button'
import Message from 'primevue/message'
import Dropdown from 'primevue/dropdown'
import { useFieldStore } from '@/stores/fieldStore'
import InputNumber from 'primevue/inputnumber'
import { onBeforeUnmount } from 'vue'

interface SearchResult {
  name: string
  address: string
}
const emit = defineEmits<{
  (e: 'addMarker', fields: Field[]): void
  (e: 'setMapCenter', field: Field): void
  (e: 'focusMarker', field: Field): void
  (e: 'showDetailDialog', fieldId: number): void
  (e: 'getFieldByMapBound'): Promise<Field[] | undefined>
}>()

const fieldStoreUser = useFieldStoreUser()
const locationStore = useLocationStore()
const fieldStore = useFieldStore()

const freeSearchKeyword = ref('')

const topOffset = ref(0)
const selectedProvince = ref()
const selectedDistrict = ref()
const startTime = ref()
const filteredProvince = ref<any[]>([])
const filteredDistrict = ref<any[]>([])
const isInputMode = ref(true)
const hoverFlag = ref(false)
const hoverField = ref<Field | undefined>()
const isSearchLoading = ref(false)
const fieldsToDisplay = ref<Field[]>([])
const hasSearched = ref(false)
const selectedDuration = ref(5400)
const durationOptions = [
  { label: '60 phút', value: 3600 },
  { label: '90 phút', value: 5400 },
  { label: '120 phút', value: 7200 }
]
const fromPrice = ref<number>()
const toPrice = ref<number>()
const selectedRating = ref<any>()
const ratingOptions = [
  { label: '5', value: 5 },
  { label: '4+', value: 4 },
  { label: '3+', value: 3 },
  { label: '2+', value: 2 },
  { label: 'Tất cả', value: undefined }
]
const listFieldDiv = ref<HTMLElement | null>(null)

const toggleMode = () => {
  isInputMode.value = !isInputMode.value
}
const selectResult = (field: Field) => {
  emit('showDetailDialog', field.id)
}

onMounted(() => {
  window.addEventListener('resize', calculateTopOffset)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', calculateTopOffset)
})
const calculateTopOffset = () => {
  if (listFieldDiv.value) {
    const rect = listFieldDiv.value.getBoundingClientRect()
    topOffset.value = rect.top
  }
}
watch(fieldsToDisplay, async (newVal) => {
  if (newVal.length > 0) {
    await nextTick()
    calculateTopOffset()
  }
})
const onSearch = async () => {
  isSearchLoading.value = true
  hasSearched.value = true

  if (isInputMode.value) {
    if (freeSearchKeyword.value) {
      fieldsToDisplay.value = []
      selectedProvince.value = undefined
      selectedDistrict.value = undefined
      startTime.value = undefined
      const response = await fieldStoreUser.searchFieldListFreeWord(freeSearchKeyword.value)
      if (response?.success) {
        fieldsToDisplay.value = fieldStoreUser.freewordFields
      }
    }
  } else {
    fieldsToDisplay.value = []
    freeSearchKeyword.value = ''
    if (startTime.value) {
      fieldStoreUser.superSearch.province = selectedProvince.value?.full_name
      fieldStoreUser.superSearch.district = selectedDistrict.value?.full_name
      fieldStoreUser.superSearch.startDateTime = startTime.value
      fieldStoreUser.superSearch.duration = selectedDuration.value
      fieldStoreUser.superSearch.fromPrice = fromPrice.value
      fieldStoreUser.superSearch.toPrice = toPrice.value
      if (selectedRating.value) {
        if (selectedRating.value.value) {
          fieldStoreUser.superSearch.toStar = Number(selectedRating.value.value)
        } else {
          fieldStoreUser.superSearch.fromStar = undefined
        }
      }
      const response = await fieldStoreUser.getSuperSearchField()
      if (response?.success) {
        fieldsToDisplay.value = fieldStoreUser.superSearchFields
      }
    } else {
      fieldStore.search.limit = 20
      fieldStore.search.province = selectedProvince.value?.full_name
      fieldStore.search.district = selectedDistrict.value?.full_name
      fieldStore.search.fromPrice = fromPrice.value
      fieldStore.search.toPrice = toPrice.value
      if (selectedRating.value) {
        if (selectedRating.value.value) {
          fieldStore.search.fromStar = Number(selectedRating.value.value)
        } else {
          fieldStore.search.fromStar = undefined
        }
      }
      const response = await fieldStore.getFieldList()
      if (response?.success) {
        fieldsToDisplay.value = fieldStore.fields
      }
    }
  }
  isSearchLoading.value = false
  emit('addMarker', fieldsToDisplay.value)
}

const isOpen = (field: Field) => {
  const now = new Date()
  const currentTime = now.getHours() * 60 + now.getMinutes()
  const [openHour, openMinute] = field.openTime.split(':').map(Number)
  const [closeHour, closeMinute] = field.closeTime.split(':').map(Number)
  const openTime = openHour * 60 + openMinute
  const closeTime = closeHour * 60 + closeMinute

  return currentTime >= openTime && currentTime < closeTime
}
const searchProvince = async (event: any) => {
  if (locationStore.allProvince.length > 0) {
    filteredProvince.value = locationStore.allProvince.filter((item) =>
      item.full_name.toLowerCase().includes(event.query.toLowerCase())
    )
  } else {
    await locationStore.getAllProvinces()
  }
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

const onProvinceChange = (event: AutoCompleteItemSelectEvent) => {
  selectedProvince.value = event.value
  selectedDistrict.value = null
  locationStore.allDistrict = []
  filteredDistrict.value = []
  // setMapCenter(event.value)
  searchDistrict(null)
}

const onDistrictChange = (event: AutoCompleteItemSelectEvent) => {
  // setMapCenter(event.value)
}
const hoverResult = (field: Field) => {
  if (!hoverFlag.value) {
    emit('setMapCenter', field)
    emit('focusMarker', field)
    hoverFlag.value = true
  }
}
const unhoverResult = () => {
  resetHover()
}
const resetHover = () => {
  hoverFlag.value = false
}
</script>
