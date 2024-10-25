<template>
  <div
    class="bg-white w-full px-5 h-60 md:h-20 mb-10 p-2 flex items-end flex-col md:flex-row gap-4"
  >
    <CustomCalendar
      name="fromDate"
      label="Ngày bắt đầu"
      v-model="statisticAdminStore.searchStatistic.fromDate"
      maxDateToday
    />
    <CustomCalendar
      name="fromDate"
      label="Ngày bắt đầu"
      v-model="statisticAdminStore.searchStatistic.toDate"
      maxDateToday
    />
    <Button @click="onSearch" label="Thống kê" />
  </div>

  <div class="grid grid-cols-1 lg:grid-cols-2 gap-10">
    <div class="col-span- p-5 bg-white">
      <ColumnChart
        :data="bookingMonthlyStatistic"
        :title="'Lượng đặt sân theo tháng'"
        :description="'Chart'"
        :toolTip="'Số lượt'"
      />
    </div>
    <div class="col-span-1 p-5 bg-white">
      <ColumnChart
        :data="userMonthlyStatistic"
        :title="'Lượng người dùng theo tháng'"
        :description="'Chart'"
        :toolTip="'Số lượng'"
      />
    </div>
    <div class="col-span-2 grid grid-cols-1 lg:grid-cols-2 gap-5 bg-white">
      <div class="col-span-1 p-5 h-50">
        <DonutChart
          :data="bookingMonthlyByStatusStatistic"
          :title="'Thống kê đặt sân theo trạng thái'"
          :description="'Chart'"
          :toolTip="'Số lượt'"
        />
      </div>
      <div>
        <MultiColumnChart
          :data="statisticAdminStore.statisticBookingByStatus"
          :title="'Thống kê đặt sân theo trạng thái'"
          :description="'Chart'"
          :toolTip="'Số lượt'"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useStatisticAdminStore } from '@/stores/adminStatisticStore'
import { onMounted, ref } from 'vue'
import { addDaysToDate, getCurrentDate } from '@/utils/dateUtil'
import dayjs from 'dayjs'
import ColumnChart from '@/components/charts/ColumnChart.vue'
import type { chartDataType } from '@/components/charts/chartType'
import DonutChart from '@/components/charts/DonutChart.vue'
import type { StatisticByBookingStatus } from '@/types/Statistics'
import MultiColumnChart from '@/components/charts/MultiColumnChart.vue'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import Button from 'primevue/button'

const statisticAdminStore = useStatisticAdminStore()

const bookingMonthlyStatistic = ref<chartDataType[]>([])
const userMonthlyStatistic = ref<chartDataType[]>([])
const bookingMonthlyByStatusStatistic = ref<chartDataType[]>([])

onMounted(async () => {
  statisticAdminStore.searchStatistic = {
    fromDate: addDaysToDate(dayjs(), -365),
    toDate: getCurrentDate()
  }

  await onSearch()
})

const onSearch = async () => {
  await statisticAdminStore.getBookingByMonth()
  bookingMonthlyStatistic.value = statisticAdminStore.statisticBookingByMonth.map((item) => ({
    label: item.month + '/' + item.year,
    value: item.total
  }))

  await statisticAdminStore.getUserByMonth()
  userMonthlyStatistic.value = statisticAdminStore.statisticUserByMonth.map((item) => ({
    label: item.month + '/' + item.year,
    value: item.total
  }))

  await statisticAdminStore.getBookingByStatus()
  const totalCancel = {
    label: 'Hủy',
    value: statisticAdminStore.statisticBookingByStatus?.canceledTotal ?? 0
  }
  const totalAccept = {
    label: 'Thành công',
    value: statisticAdminStore.statisticBookingByStatus?.acceptedTotal ?? 0
  }
  const totalReject = {
    label: 'Bị từ chối',
    value: statisticAdminStore.statisticBookingByStatus?.rejectedTotal ?? 0
  }
  bookingMonthlyByStatusStatistic.value = [totalCancel, totalAccept, totalReject]
}
</script>
