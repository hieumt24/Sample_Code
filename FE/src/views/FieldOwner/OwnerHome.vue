<template>
  <div class="bg-gray-100 min-h-screen px-8">
    <div class="max-w-7xl mx-auto">
      <div class="flex flex-col lg:flex-row space-y-8 lg:space-y-0 lg:space-x-8">
        <div class="w-full lg:w-1/3 bg-white rounded-lg mr-4 py-2 max-h-[700px] shadow-md">
          <div class="w-full px-10 mb-4">
            <label class="block text-sm font-medium text-gray-700 mb-1" for="name">Chọn sân</label>
            <Dropdown
              v-model="selectedField"
              :options="listFields"
              optionLabel="name"
              placeholder="Chọn sân bóng"
              class="w-full"
              @change="onChangeField"
            />
          </div>
          <h2 class="text-xl font-semibold text-gray-800 mb-2 text-center">Danh sách sân</h2>
          <div class="px-6 max-h-[500px] overflow-auto border-t border-b border-gray-200">
            <div
              v-for="partialField in partialFieldStore.partialFields"
              :key="partialField.id"
              @click="onSelectPartialField(partialField.id)"
              class="border border-gray-200 px-4 py-1 rounded-md cursor-pointer transition duration-300 hover:bg-gray-100 my-2 hover:border-gray-300 hover:shadow-md"
              :class="{
                'bg-blue-50 border-blue-300': selectedPartialFieldId === partialField.id
              }"
            >
              <div class="text-lg font-semibold text-gray-800">{{ partialField.name }}</div>
              <div class="text-sm text-gray-600 mt-1">
                <span class="font-medium text-blue-500"
                  >Lịch đợi duyệt: {{ partialField.numberWaiting }}</span
                >
              </div>
            </div>
          </div>
        </div>

        <div class="w-full h-fit lg:w-2/3">
          <div class="bg-white rounded-lg h-fit shadow-md p-6">
            <div class="flex flex-wrap items-center justify-between mb-6">
              <button
                class="px-4 py-2 text-sm font-medium text-green-600 bg-green-100 rounded-md hover:bg-green-200 transition duration-300"
                @click.stop="goToToday"
              >
                Ngày hôm nay
              </button>

              <CustomCalendar
                name="date"
                :label="$t('searchTab.date')"
                no-label
                v-model="selectedDate"
                @update:modelValue="onChangeDate"
              />

              <div class="flex items-center space-x-4">
                <button class="text-gray-600 hover:text-gray-800" @click.stop="onPreviousWeek">
                  <i class="pi pi-arrow-circle-left text-2xl"></i>
                </button>
                <div class="text-lg font-semibold text-gray-800">
                  {{ getDateFormattedShow(fromDateWeek) }} ~ {{ getDateFormattedShow(toDateWeek) }}
                </div>
                <button class="text-gray-600 hover:text-gray-800" @click.stop="onNextWeek">
                  <i class="pi pi-arrow-circle-right text-2xl"></i>
                </button>
              </div>
            </div>
            <div
              v-if="selectedPartialFieldId == 0"
              class="min-h-[600px] max-h-[1200px] flex items-center"
            >
              <h1 class="text-lg text-center w-full">
                <i class="pi pi-directions-alt text-5xl" style="font-size: 1.5rem" /> Vui lòng chọn
                sân bóng nhỏ
              </h1>
            </div>
            <div class="h-[1200px] w-full">
              <OwnerBookingTable
                v-if="selectedPartialFieldId !== 0"
                :partial-field-id="selectedPartialFieldId"
                :from-date="fromDateWeek"
                :to-date="toDateWeek"
                :is-fixed-slot="isFixedSlot ?? false"
                :field-id="selectedFieldId"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import { useFieldStore } from '@/stores/fieldStore'
import Dropdown from 'primevue/dropdown'
import { ref } from 'vue'
import type { DropdownType } from '@/constants/types'
import { computed } from 'vue'
import OwnerBookingTable from '@/components/tables/OwnerBookingTable.vue'
import {
  addDaysToDate,
  getCurrentDate,
  getDateFormattedShow,
  getFirstDayOfWeek,
  getFirstDayOfWeekBySpecificDate,
  getLastDayOfWeek
} from '@/utils/dateUtil'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'

const layoutStore = useLayoutStore()
const partialFieldStore = usePartialFieldStore()
const fieldStore = useFieldStore()

const listFields = ref<DropdownType[]>([])
const selectedField = ref<DropdownType>()
const fromDateWeek = ref(getFirstDayOfWeek())
const selectedDate = ref(getCurrentDate())
const toDateWeek = ref(getLastDayOfWeek())
const selectedFieldId = computed(() => Number(selectedField.value?.code ?? 0))
const selectedPartialFieldId = ref<number>(0)
const isFixedSlot = computed(
  () => fieldStore.fields.find((field) => field.id == selectedFieldId.value)?.isFixedSlot
)

const onChangeField = async () => {
  await partialFieldStore.getPartialFieldByFieldId(selectedFieldId.value)
  selectedPartialFieldId.value = 0
}

const goToToday = () => {
  fromDateWeek.value = getFirstDayOfWeek()
  toDateWeek.value = getLastDayOfWeek()
}

const onPreviousWeek = () => {
  fromDateWeek.value = addDaysToDate(fromDateWeek.value, -7)
  toDateWeek.value = addDaysToDate(toDateWeek.value, -7)
}

const onNextWeek = () => {
  fromDateWeek.value = addDaysToDate(fromDateWeek.value, 7)
  toDateWeek.value = addDaysToDate(toDateWeek.value, 7)
}

const onSelectPartialField = (partialFieldId: number) => {
  selectedPartialFieldId.value = partialFieldId
}

const onChangeDate = () => {
  fromDateWeek.value = getFirstDayOfWeekBySpecificDate(selectedDate.value)
  toDateWeek.value = addDaysToDate(fromDateWeek.value, 6)
}

onMounted(async () => {
  layoutStore.breadcrumb = []
  fieldStore.ownerSearch.status = 'ACCEPTED'
  await fieldStore.getOwnerFieldList()

  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))
  if (listFields.value.length > 0) {
    selectedField.value = listFields.value[0]
  }

  if (!selectedFieldId.value) return

  await partialFieldStore.getPartialFieldByFieldId(selectedFieldId.value)
  if (partialFieldStore.partialFields.length > 0) {
    selectedPartialFieldId.value = partialFieldStore.partialFields[0].id
  }
})
</script>
