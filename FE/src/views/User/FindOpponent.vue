<template>
  <div class="max-w-7xl mx-auto my-6 flex gap-6">
    <div
      class="sticky top-16 left-0 w-96 bg-white border shadow-md rounded-lg p-4 h-fit group max-h-[calc(100vh-8rem)] overflow-y-auto"
    >
      <h3 class="text-lg font-bold mb-4"><span class="pi pi-filter"></span> Bộ lọc tìm kiếm</h3>
      <div class="flex flex-col gap-4">
        <Accordion :multiple="true" :activeIndex="[0, 1]">
          <AccordionTab
            :pt="{
              headerTitle: { class: 'font-bold block' },
              headerAction: { class: 'py-3' }
            }"
            header="Địa điểm"
          >
            <div>
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
            </div>
          </AccordionTab>
          <AccordionTab
            :pt="{
              headerTitle: { class: 'font-bold block' },
              headerAction: { class: 'py-3' }
            }"
            header="Thời gian"
          >
            <div>
              <div class="card flex justify-center pt-4">
                <Calendar
                  v-model="dates"
                  inline
                  selectionMode="range"
                  :manualInput="false"
                  class="w-full"
                />
              </div>
              <div class="card flex gap-3 p-fluid pt-4">
                <div class="w-1/2">
                  <label for="from-time" class="font-semibold block mb-2"> Bắt đầu </label>
                  <input
                    class="p-2 border rounded-md"
                    type="time"
                    id="from-time"
                    v-model="fromTime"
                    @change="validateTimes"
                  />
                </div>
                <div class="w-1/2">
                  <label for="to-time" class="font-semibold block mb-2"> Kết thúc </label>
                  <input
                    class="p-2 border rounded-md"
                    type="time"
                    id="to-time"
                    v-model="toTime"
                    @change="validateTimes"
                  />
                </div>
              </div>
              <Message v-if="errorMessage" :closable="false" severity="error">{{
                errorMessage
              }}</Message>
            </div>
          </AccordionTab>
          <AccordionTab
            :pt="{
              headerTitle: { class: 'font-bold block' },
              headerAction: { class: 'py-3' }
            }"
            header="Trạng thái"
          >
            <div class="card flex justify-content-center">
              <div class="flex flex-col gap-3">
                <div class="flex align-items-center">
                  <RadioButton v-model="filterStatus" inputId="allStatus" name="all" value="" />
                  <label for="allStatus" class="ml-2">Tất cả</label>
                </div>
                <div class="flex align-items-center">
                  <RadioButton
                    v-model="filterStatus"
                    inputId="findingStatus"
                    name="finding"
                    :value="OpponentFindingStatus.FINDING"
                  />
                  <label for="findingStatus" class="ml-2">
                    {{ getVNStatus(OpponentFindingStatus.FINDING) }}
                  </label>
                </div>
                <div class="flex align-items-center">
                  <RadioButton
                    v-model="filterStatus"
                    inputId="acceptedStatus"
                    name="accepted"
                    :value="OpponentFindingStatus.ACCEPTED"
                  />
                  <label for="acceptedStatus" class="ml-2">
                    {{ getVNStatus(OpponentFindingStatus.ACCEPTED) }}
                  </label>
                </div>
                <div class="flex align-items-center">
                  <RadioButton
                    v-model="filterStatus"
                    inputId="cancelledStatus"
                    name="cancelled"
                    :value="OpponentFindingStatus.CANCELLED"
                  />
                  <label for="cancelledStatus" class="ml-2">
                    {{ getVNStatus(OpponentFindingStatus.CANCELLED) }}
                  </label>
                </div>
                <div class="flex align-items-center">
                  <RadioButton
                    v-model="filterStatus"
                    inputId="opponentCancelledStatus"
                    name="opponentCancelled"
                    :value="OpponentFindingStatus.OPPONENT_CANCELLED"
                  />
                  <label for="opponentCancelledStatus" class="ml-2">
                    {{ getVNStatus(OpponentFindingStatus.OPPONENT_CANCELLED) }}
                  </label>
                </div>
                <div class="flex align-items-center">
                  <RadioButton
                    v-model="filterStatus"
                    inputId="overlappedCancelledStatus"
                    name="overlappedCancelled"
                    :value="OpponentFindingStatus.OVERLAPPED_CANCELLED"
                  />
                  <label for="overlappedCancelledStatus" class="ml-2">
                    {{ getVNStatus(OpponentFindingStatus.OVERLAPPED_CANCELLED) }}
                  </label>
                </div>
              </div>
            </div>
          </AccordionTab>
        </Accordion>

        <!-- Add more filter options here -->
      </div>
    </div>
    <div class="flex-1 overflow-auto space-y-3 h-full pl-6" ref="postContainer">
      <!-- Post -->
      <div class="card">
        <TabMenu :model="modes" @tab-change="onTabChange" v-model:activeIndex="activeTabIndex" />
        <div class="flex flex-wrap gap-2 mt-2">
          <Chip
            v-for="(value, key) in activeFilters"
            :key="key"
            :label="formatChipLabel(key, value)"
            @remove="removeFilter(key as OpponentFindingFilterKey)"
            removable
            :pt="{
              root: {
                class: [
                  'inline-flex items-center',
                  'px-3 py-1 gap-2',
                  'rounded-[16px]',
                  'text-surface-700 dark:text-white',
                  'bg-surface-300 dark:bg-surface-700'
                ]
              }
            }"
          />
          <Button
            v-if="hasManyFilters"
            text
            label="Xoá tất cả"
            @click="removeAllFilters"
            severity="danger"
          />
        </div>
      </div>

      <div v-if="selectedMode.name === 'Bài của tôi'">
        <Button @click="openConfirmDialog" icon="pi pi-plus" label="Tạo mới"></Button>
        <OpponentFindingTypeConfirmDialog
          :open="isConfirmDialogOpen"
          @close="isConfirmDialogOpen = false"
          @open-create-without-booking-dialog="openWithoutBookingCreateDialog"
        />
        <OpponentFindingCreateDialog
          :open="isWithoutBookingCreateDialogOpen"
          @close="isWithoutBookingCreateDialogOpen = false"
          @on-create-successfully="goToFirstPage"
        />
      </div>

      <!-- Main Content (Post List) -->
      <div v-if="opponentFindingStore.loading" class="rounded-lg space-y-3">
        <Skeleton height="15rem"></Skeleton>
        <Skeleton height="15rem"></Skeleton>
        <Skeleton height="15rem"></Skeleton>
        <Skeleton height="15rem"></Skeleton>
      </div>

      <Message
        :closable="false"
        v-else-if="opponentFindingStore.opponentFindings.length === 0"
        severity="warn"
      >
        Không tìm thấy bài đăng nào!
      </Message>

      <div
        v-for="of in opponentFindingStore.opponentFindings"
        class="card bg-white rounded-lg cursor-pointer"
        :key="of.id"
      >
        <PostPanel
          :opponent-finding="of"
          @show-detail-dialog="showDetailDialog"
          :selected-mode="selectedMode.name"
        ></PostPanel>
      </div>
      <div class="flex justify-center items-center card bg-white rounded-lg w-full border">
        <PagingElement
          ref="PagingElementRef"
          :limit="opponentFindingStore.search.limit"
          :total="opponentFindingStore.totalOpponentFindings"
          @change-page="opponentFindingStore.changePageOpponentFinding"
        />
      </div>
    </div>

    <Dialog
      v-model:visible="detailDialogVisible"
      modal
      :draggable="false"
      header="Tìm đối"
      :style="{ width: '60rem' }"
    >
      <PostDetailPanel></PostDetailPanel>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import Button from 'primevue/button'
import { useOpponentFindingStore } from '@/stores/opponentFindingStore'
import { computed, onMounted, watch, ref } from 'vue'
import Calendar from 'primevue/calendar'
import { changeSecondToHour, changeTimeToSeconds } from '@/utils/timeUtil'
import Dialog from 'primevue/dialog'
import { storeToRefs } from 'pinia'
import Accordion from 'primevue/accordion'
import AccordionTab from 'primevue/accordiontab'
import TabMenu, { type TabMenuChangeEvent } from 'primevue/tabmenu'
import Chip from 'primevue/chip'
import type { OpponentFindingFilterKey } from '@/api/opponentFindingApi'
import Message from 'primevue/message'
import OpponentFindingCreateDialog from '@/components/dialogs/OpponentFindingCreateDialog.vue'
import OpponentFindingTypeConfirmDialog from '@/components/dialogs/OpponentFindingTypeConfirmDialog.vue'
import { useRoute } from 'vue-router'
import router from '@/router'
import PostPanel from '@/components/panels/PostPanel.vue'
import PagingElement from '@/components/pagination/PagingElement.vue'
import Skeleton from 'primevue/skeleton'
import AutoComplete, { type AutoCompleteItemSelectEvent } from 'primevue/autocomplete'
import { useLocationStore } from '@/stores/locationStore'
import PostDetailPanel from '@/components/panels/PostDetailPanel.vue'
import { getDateFormattedShow } from '@/utils/dateUtil'
import RadioButton from 'primevue/radiobutton'
import { OpponentFindingStatus } from '@/constants/opponentFinding'
import { getVNStatus } from '@/utils/opponentFindingTagUtils'

const route = useRoute()

const selectedMode = ref({ name: 'Tìm đối' })
// const modes = ref([{ name: 'Tìm đối' }, { name: 'Bài của tôi' }, { name: 'Yêu cầu đã gửi' }])
const showRequesting = ref(false)
const detailDialogVisible = ref(false)
const dates = ref<Date[]>()
const fromTime = ref()
const toTime = ref()
const isConfirmDialogOpen = ref(false)
const isWithoutBookingCreateDialogOpen = ref(false)
const activeTabIndex = ref(0)
const selectedProvince = ref()
const selectedDistrict = ref()
const selectedCommune = ref()
const filteredProvince = ref<any[]>([])
const filteredDistrict = ref<any[]>([])
const filteredCommune = ref<any[]>([])
const filterStatus = ref('')
const PagingElementRef = ref<InstanceType<typeof PagingElement> | null>(null)

const opponentFindingStore = useOpponentFindingStore()
const locationStore = useLocationStore()

const postContainer = ref<HTMLElement | null>(null)
const modes = ref([
  { label: 'Tìm đối', icon: 'pi pi-fw pi-search', to: '', page: '' },
  {
    label: 'Yêu cầu đã gửi',
    icon: 'pi pi-fw pi-send',
    to: '/mine/my-requests',
    page: 'my-requests'
  },
  { label: 'Bài của tôi', icon: 'pi pi-fw pi-users', to: '/mine/my-posts', page: 'my-posts' }
])

const errorMessage = ref('')
const validateTimes = () => {
  if (fromTime.value && toTime.value) {
    if (fromTime.value >= toTime.value) {
      errorMessage.value = 'Thời gian bắt đầu phải diễn ra trước thời gian kết thúc'
    } else {
      errorMessage.value = ''
    }
  }
}
const { search } = storeToRefs(opponentFindingStore)

const activeFilters = computed(() => {
  const { limit, offset, ...filters } = search.value
  return Object.entries(filters)
    .filter(([_, value]) => value !== '')
    .reduce(
      (acc, [key, value]) => ({
        ...acc,
        [key]: formatValue(key as OpponentFindingFilterKey, value)
      }),
      {} as Record<string, string>
    )
})

const hasManyFilters = computed(() => Object.keys(activeFilters.value).length > 1)

const formatValue = (key: OpponentFindingFilterKey, value: string): string => {
  if (['fromDate', 'toDate'].includes(key)) {
    return new Date(value).toLocaleDateString()
  }
  if (['fromTime', 'toTime'].includes(key)) {
    return changeSecondToHour(value)
  }
  return value
}

const formatChipLabel = (key: string, value: string): string => {
  switch (key) {
    case 'fromDate':
      return `từ ${getDateFormattedShow(value)}`
    case 'toDate':
      return `đến ${getDateFormattedShow(value)}`
    case 'fromTime':
      return `từ ${value}`
    case 'toTime':
      return `đến ${value}`
    case 'status':
      return getVNStatus(value) ?? ''
    default:
      return value
  }
}

const removeFilter = async (key: OpponentFindingFilterKey) => {
  search.value[key] = ''
  switch (key) {
    case 'fromDate': {
      dates.value = undefined
      search.value['toDate'] = ''
      return
    }
    case 'toDate': {
      if (dates.value) {
        dates.value.pop()
        await searchOpponentFindings()
      }
      return
    }
    case 'fromTime': {
      fromTime.value = undefined
      return
    }
    case 'toTime': {
      toTime.value = undefined
      return
    }
    case 'province': {
      await removeProvinceChip()
      return
    }
    case 'district': {
      await removeDistrictChip()
      return
    }
    case 'commune': {
      await removeCommuneChip()
      return
    }
    case 'status': {
      filterStatus.value = ''
      return
    }
  }
}

const removeCommuneChip = async () => {
  selectedCommune.value = undefined
  search.value['commune'] = ''
  await searchOpponentFindings()
}

const removeDistrictChip = async () => {
  selectedDistrict.value = undefined
  search.value['district'] = ''
  locationStore.allCommune = []
  await removeCommuneChip()
}

const removeProvinceChip = async () => {
  selectedProvince.value = undefined
  locationStore.allDistrict = []
  search.value['province'] = ''
  await removeDistrictChip()
}

const removeAllFilters = async () => {
  dates.value = undefined
  toTime.value = undefined
  fromTime.value = undefined
  removeProvinceChip()
  opponentFindingStore.resetSearch()
  await searchOpponentFindings()
}

onMounted(async () => {
  await locationStore.fetchCurrentLocationDetails()
  if (locationStore.enableLocation) {
    filteredProvince.value = locationStore.allProvince
    filteredDistrict.value = locationStore.allDistrict
    filteredCommune.value = locationStore.allCommune
    selectedProvince.value = locationStore.currentProvince
    selectedDistrict.value = locationStore.currentDistrict
    opponentFindingStore.search.province = selectedProvince.value.full_name
    opponentFindingStore.search.district = selectedDistrict.value.full_name
  }
  await opponentFindingStore.getOpponentFindingList(selectedMode.value.name)
})

watch(dates, async (newDates) => {
  if (newDates) {
    if (newDates[0]) {
      opponentFindingStore.search.fromDate = newDates[0].toDateString()
    }
    if (newDates[1]) {
      opponentFindingStore.search.toDate = newDates[1].toDateString()
    } else {
      opponentFindingStore.search.toDate = ''
    }
  }
  await searchOpponentFindings()
})

watch([fromTime, toTime], async () => {
  validateTimes()
  if (errorMessage.value) return
  if (fromTime.value) {
    opponentFindingStore.search.fromTime = changeTimeToSeconds(fromTime.value).toString()
  }
  if (toTime.value) {
    opponentFindingStore.search.toTime = changeTimeToSeconds(toTime.value).toString()
  }
  await searchOpponentFindings()
})

watch(filterStatus, async () => {
  opponentFindingStore.search.status = filterStatus.value
  await searchOpponentFindings()
})
const getOpponentFindingList = async () => {
  await opponentFindingStore.getOpponentFindingList(selectedMode.value.name)
}

const searchOpponentFindings = async () => {
  window.scrollTo(0, 0)
  opponentFindingStore.resetOpponentFindings()
  await getOpponentFindingList()
  resetPageNumberDisplay()
}

watch(
  route,
  async (newRoute) => {
    const page = newRoute.params.page
    const index = modes.value.findIndex((m) => m.page === page)
    if (index !== -1) {
      activeTabIndex.value = index
      selectedMode.value.name = modes.value[activeTabIndex.value].label
      filterStatus.value = ''
    } else {
      activeTabIndex.value = 0
      selectedMode.value.name = modes.value[activeTabIndex.value].label
      filterStatus.value = OpponentFindingStatus.FINDING
    }
    await searchOpponentFindings()
  },
  { immediate: true }
)

const onTabChange = async (event: TabMenuChangeEvent) => {
  if (selectedMode.value.name != modes.value[event.index].label) {
    router.push('/user/find-opponent' + modes.value[event.index].to)
  }
}

const showDetailDialog = async (id: number) => {
  detailDialogVisible.value = true
  await opponentFindingStore.getOpponentFinding(id)
  showRequesting.value = true
}

const openConfirmDialog = () => {
  isConfirmDialogOpen.value = true
}

const openWithoutBookingCreateDialog = () => {
  isConfirmDialogOpen.value = false
  isWithoutBookingCreateDialogOpen.value = true
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

const onProvinceChange = async (event: AutoCompleteItemSelectEvent) => {
  selectedProvince.value = event.value
  selectedDistrict.value = null
  locationStore.allDistrict = []
  filteredDistrict.value = []
  selectedCommune.value = null
  locationStore.allCommune = []
  filteredCommune.value = []
  searchDistrict(null)
  opponentFindingStore.search.province = selectedProvince.value.full_name
  opponentFindingStore.search.district = ''
  await searchOpponentFindings()
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
  opponentFindingStore.search.district = selectedDistrict.value.full_name
  opponentFindingStore.search.commune = ''
  await searchOpponentFindings()
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
  opponentFindingStore.search.commune = selectedCommune.value.full_name
  await searchOpponentFindings()
}
const goToFirstPage = () => {
  if (PagingElementRef.value) {
    PagingElementRef.value.goToFirstPage()
  }
}
const resetPageNumberDisplay = () => {
  if (PagingElementRef.value) {
    PagingElementRef.value.resetPageNumberDisplay()
  }
}
</script>
