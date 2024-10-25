<template>
  <div class="max-w-96">
    <h1>{{ title }}</h1>
    <div class="relative">
      <Chart
        type="doughnut"
        :data="chartData"
        :options="chartOptions"
        class="w-full flex"
        :height="30"
      />
      <div v-if="!props.data" class="absolute top-0 w-full h-full bg-gray-200 bg-opacity-55 z-20">
        <h1 class="text-center mt-20">Không có dữ liệu</h1>
      </div>
      <div
        v-else-if="props.data.every((d) => d.value === 0)"
        class="absolute top-0 w-full h-full bg-gray-200 bg-opacity-55 z-20"
      >
        <h1 class="text-center mt-20">Không có dữ liệu</h1>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import Chart from 'primevue/chart'
import type { chartDataType } from './chartType'

const props = defineProps<{
  data: chartDataType[]
  title?: string
  description: string
  toolTip: string
}>()

const chartData = ref()
const chartOptions = ref()

const setChartOptions = () => {
  const documentStyle = getComputedStyle(document.documentElement)
  const textColor = documentStyle.getPropertyValue('--text-color')

  return {
    plugins: {
      legend: {
        labels: {
          cutout: '60%',
          color: textColor
        }
      }
    }
  }
}
const setChartData = (labels: string[], values: number[]) => {
  const documentStyle = getComputedStyle(document.body)

  return {
    labels: labels,
    datasets: [
      {
        data: values,
        backgroundColor: [
          'rgba(249, 115, 22, 0.2)',
          'rgba(6, 182, 212, 0.2)',
          'rgb(107, 114, 128, 0.2)'
        ],
        hoverBackgroundColor: ['rgb(249, 115, 22)', 'rgb(6, 182, 212)', 'rgb(107, 114, 128)']
      }
    ]
  }
}

watch(
  () => props.data,
  (newDate) => {
    if (!newDate) return

    chartData.value = setChartData(
      newDate.map((x) => x.label),
      newDate.map((x) => x.value)
    )
    chartOptions.value = setChartOptions()
  }
)
</script>
