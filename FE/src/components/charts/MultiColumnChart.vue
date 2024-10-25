<template>
  <div class="w-full">
    <h1>{{ title }}</h1>
    <div class="relative">
      <Chart type="bar" :data="chartData" :options="chartOptions" />
      <div v-if="!props.data" class="absolute top-0 w-full h-full bg-gray-200 bg-opacity-55 z-20">
        <h1 class="text-center mt-20">Không có dữ liệu</h1>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import Chart from 'primevue/chart'
import type { chartDataType } from './chartType'
import type { StatisticByBookingStatus } from '@/types/Statistics'

const props = defineProps<{
  data: StatisticByBookingStatus | undefined
  title?: string
  description: string
  toolTip: string
}>()

const chartData = ref()
const chartOptions = ref()

const setChartOptions = () => {
  const documentStyle = getComputedStyle(document.documentElement)
  const textColor = documentStyle.getPropertyValue('--text-color')
  const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary')
  const surfaceBorder = documentStyle.getPropertyValue('--surface-border')

  return {
    plugins: {
      legend: {
        labels: {
          color: textColor
        }
      }
    },
    scales: {
      x: {
        ticks: {
          color: textColorSecondary
        },
        grid: {
          color: surfaceBorder
        }
      },
      y: {
        beginAtZero: true,
        ticks: {
          color: textColorSecondary
        },
        grid: {
          color: surfaceBorder
        }
      }
    }
  }
}
const setChartData = (labels: string[], accept: number[], cencel: number[], reject: number[]) => {
  const documentStyle = getComputedStyle(document.documentElement)

  return {
    labels: labels,
    datasets: [
      {
        label: 'Thành công',
        backgroundColor: documentStyle.getPropertyValue('--cyan-500'),
        borderColor: documentStyle.getPropertyValue('--cyan-500'),
        data: accept
      },
      {
        label: 'Đã huỷ',
        backgroundColor: documentStyle.getPropertyValue('--gray-500'),
        borderColor: documentStyle.getPropertyValue('--gray-500'),
        data: cencel
      },
      {
        label: 'Từ chối',
        backgroundColor: documentStyle.getPropertyValue('--gray-500'),
        borderColor: documentStyle.getPropertyValue('--gray-500'),
        data: reject
      }
    ]
  }
}

watch(
  () => props.data,
  (newDate) => {
    if (!newDate) return

    chartData.value = setChartData(
      newDate.monthly.map((x) => x.month + '/' + x.year),
      newDate.monthly.map((x) => x.accepted),
      newDate.monthly.map((x) => x.canceled),
      newDate.monthly.map((x) => x.rejected)
    )
    chartOptions.value = setChartOptions()
  }
)
</script>
