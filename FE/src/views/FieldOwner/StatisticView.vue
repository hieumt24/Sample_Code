<template>
  <div
    class="bg-white w-full px-5 h-60 md:h-20 mb-10 p-2 flex items-end flex-col md:flex-row gap-4"
  >
    <Dropdown
      v-model="selectedField"
      :options="listFields"
      optionLabel="name"
      placeholder="Chọn sân"
      class="w-[14rem]"
      @change="onChangeField"
    />
    <CustomCalendar
      name="fromDate"
      label="Ngày bắt đầu"
      v-model="statisticOwnerStore.searchStatistic.fromDate"
      maxDateToday
    />
    <CustomCalendar
      name="fromDate"
      label="Ngày bắt đầu"
      v-model="statisticOwnerStore.searchStatistic.toDate"
      maxDateToday
    />
    <Button @click="onSearch" label="Thống kê" />
  </div>

  <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-10">
    <div class="col-span-1 p-5 bg-white">
      <ColumnChart
        :data="bookingMonthlyBySlotStatistic"
        :title="'Lượng đặt sân theo khung giờ'"
        :description="'Chart'"
        :toolTip="'Số lượt'"
      />
    </div>
    <div class="col-span-2 p-5 bg-white">
      <ColumnChart
        :data="bookingMonthlyStatistic"
        :title="'Lượng đặt sân theo tháng'"
        :description="'Chart'"
        :toolTip="'Số lượt'"
      />
    </div>
    <div class="col-span-2 bg-white p-5">
      <h1 class="text-center font-semibold">Thống kê đặt sân theo trạng thái</h1>
      <div class="grid grid-cols-1 lg:grid-cols-2 p-5 h-50">
        <div class="col-span-1 w-72">
          <DonutChart
            :data="bookingMonthlyByStatusStatistic"
            :description="'Chart'"
            :toolTip="'Số lượt'"
          />
        </div>
        <div class="col-span-1">
          <MultiColumnChart
            :data="statisticOwnerStore.statisticBookingByStatus"
            :description="'Chart'"
            :toolTip="'Số lượt'"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { addDaysToDate, getCurrentDate } from '@/utils/dateUtil'
import dayjs from 'dayjs'
import ColumnChart from '@/components/charts/ColumnChart.vue'
import type { chartDataType } from '@/components/charts/chartType'
import DonutChart from '@/components/charts/DonutChart.vue'
import MultiColumnChart from '@/components/charts/MultiColumnChart.vue'
import { useStatisticOwnerStore } from '@/stores/ownerStatisticStore'
import Dropdown from 'primevue/dropdown'
import type { DropdownType } from '@/constants/types'
import { useFieldStore } from '@/stores/fieldStore'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import Button from 'primevue/button'

const statisticOwnerStore = useStatisticOwnerStore()
const bookingMonthlyStatistic = ref<chartDataType[]>([])
const bookingMonthlyByStatusStatistic = ref<chartDataType[]>([])
const bookingMonthlyBySlotStatistic = ref<chartDataType[]>([])
const selectedField = ref<DropdownType>()
const listFields = ref<DropdownType[]>([])
const fieldStore = useFieldStore()

const onChangeField = async () => {
  statisticOwnerStore.searchStatistic = {
    fromDate: addDaysToDate(dayjs(), -365),
    toDate: getCurrentDate()
  }
  statisticOwnerStore.searchStatistic.fieldId = Number(selectedField.value?.code ?? 0)
  await onSearch()
}

const onSearch = async () => {
  await statisticOwnerStore.getBookingByMonth()
  bookingMonthlyStatistic.value = statisticOwnerStore.statisticBookingByMonth.map((item) => ({
    label: item.month + '/' + item.year,
    value: item.total
  }))

  await statisticOwnerStore.getBookingSlotsByMonth()
  if (statisticOwnerStore.statisticBookingSlotByMonth?.bookingBySlot) {
    bookingMonthlyBySlotStatistic.value =
      statisticOwnerStore.statisticBookingSlotByMonth?.bookingBySlot.map((item) => ({
        label: item.slotName,
        value: item.total
      }))
  }

  await statisticOwnerStore.getBookingByStatus()
  const totalCancel = {
    label: 'Hủy',
    value: statisticOwnerStore.statisticBookingByStatus?.canceledTotal ?? 0
  }
  const totalAccept = {
    label: 'Thành công',
    value: statisticOwnerStore.statisticBookingByStatus?.acceptedTotal ?? 0
  }
  const totalReject = {
    label: 'Bị từ chối',
    value: statisticOwnerStore.statisticBookingByStatus?.rejectedTotal ?? 0
  }
  bookingMonthlyByStatusStatistic.value = [totalCancel, totalAccept, totalReject]
}

onMounted(async () => {
  await fieldStore.getOwnerFieldList()
  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))

  if (listFields.value.length > 0) {
    selectedField.value = listFields.value[0]
  }

  if (listFields.value.length > 0) {
    onChangeField()
  }
})
</script>
