<template>
  <div class="flex flex-col">
    <span v-if="!noLabel" class="font-medium">{{ label }}</span>
    <Calendar
      v-model="rawDate"
      showIcon
      dateFormat="dd/mm/yy"
      iconDisplay="input"
      class="!max-w-full"
      @update:modelValue="onChange"
      :minDate="minDate ? new Date(minDate) : undefined"
      :maxDate="maxDateToday ? new Date() : undefined"
      :class="{ 'rounded-lg border-[3px] border-blue-300 ': isBlueBorder }"
      :pt:input:class="'flex flex-auto leading-none text-surface-600 dark:text-surface-200 placeholder:text-surface-400 dark:placeholder:text-surface-500 bg-surface-0 dark:bg-surface-950 border border-surface-300 dark:border-surface-600 invalid:focus:ring-red-200 invalid:hover:border-red-500 m-0 py-2 px-3 appearance-none rounded-md rounded-md flex-1 pr-0 transition-colors duration-200 hover:border-surface-400 dark:hover:border-surface-600 focus:outline-none focus:outline-offset-0 focus:ring-1 focus:ring-primary-500 dark:focus:ring-primary-400 focus:z-1'"
    />
    <small v-if="error" class="text-red-600">{{ error }}</small>
  </div>
</template>

<script setup lang="ts">
import { DATE_FORMAT_DD_MM_YYYY, getDateFromDate } from '@/utils/dateUtil'
import Calendar from 'primevue/calendar'
import { usePrimeVue } from 'primevue/config'
import { onMounted, ref, watch } from 'vue'

const props = defineProps<{
  modelValue: number | string | null | undefined
  error?: string
  label?: string
  noLabel?: boolean
  minDate?: string
  maxDateToday?: boolean
  isBlueBorder?: boolean
}>()

const emit = defineEmits(['update:modelValue'])

const rawDate = ref()

const onChange = () => {
  emit('update:modelValue', getDateFromDate(rawDate.value, DATE_FORMAT_DD_MM_YYYY))
}

const changeToSpanish = () => {
  const primevue = usePrimeVue()
  if (!primevue.config.locale) return
  primevue.config.locale.dayNamesShort = ['CN', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7']
  primevue.config.locale.dayNamesMin = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']
  primevue.config.locale.dayNames = [
    'Thứ 2',
    'Thứ 3',
    'Thứ 4',
    'Thứ 5',
    'Thứ 6',
    'Thứ 7',
    'Chủ nhật'
  ]
  primevue.config.locale.monthNamesShort = [
    'Th 1',
    'Th 2',
    'Th 3',
    'Th 4',
    'Th 5',
    'Th 6',
    'Th 7',
    'Th 8',
    'Th 9',
    'Th 10',
    'Th 11',
    'Th 12'
  ]
  primevue.config.locale.firstDayOfWeek = 1
  primevue.config.locale.monthNames = [
    'Tháng 1',
    'Tháng 2',
    'Tháng 3',
    'Tháng 4',
    'Tháng 5',
    'Tháng 6',
    'Tháng 7',
    'Tháng 8',
    'Tháng 9',
    'Tháng 10',
    'Tháng 11',
    'Tháng 12'
  ]
}

onMounted(() => {
  if (props.modelValue) rawDate.value = new Date(props.modelValue)

  changeToSpanish()
})

watch(
  () => props.modelValue,
  (newValue) => {
    if (newValue) rawDate.value = new Date(newValue)
    else rawDate.value = null
  }
)
</script>
