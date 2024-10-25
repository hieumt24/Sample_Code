<template>
  <div class="w-full">
    <h1 class="text-center font-semibold mb-5">{{ title }}</h1>
    <div class="relative">
      <Chart type="bar" :data="chartData" :options="chartOptions" />
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
  title: string
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
const setChartData = (labels: string[], values: number[]) => {
  return {
    labels: labels,
    datasets: [
      {
        label: props.toolTip,
        data: values,
        backgroundColor: [
          'rgba(249, 115, 22, 0.2)',
          'rgba(6, 182, 212, 0.2)',
          'rgb(107, 114, 128, 0.2)',
          'rgba(139, 92, 246 0.2)'
        ],
        borderColor: [
          'rgb(249, 115, 22)',
          'rgb(6, 182, 212)',
          'rgb(107, 114, 128)',
          'rgb(139, 92, 246)'
        ],
        borderWidth: 1
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
