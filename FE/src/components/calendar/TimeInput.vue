<template>
  <div class="flex flex-col">
    <span class="font-medium">{{ label }}</span>
    <InputMask
      :id="Math.random().toString"
      v-model="time"
      mask="99:99"
      placeholder="HH:mm"
      slotChar="__"
      :disabled="disabled"
      :class="['w-full', { 'rounded-lg border-[3px] border-blue-300': isBlueBorder }]"
      @complete="onChange"
      @focus="onFocus"
      @blur="onBlur"
    />
    <small v-if="error" class="text-red-600">{{ error }}</small>
  </div>
</template>

<script setup lang="ts">
import { changeTimeToSeconds } from '@/utils/timeUtil'
import InputMask from 'primevue/inputmask'
import { onMounted, watch } from 'vue'
import { ref } from 'vue'

const props = defineProps<{
  modelValue: number | string
  error?: string
  label?: string
  initValue?: string
  disabled?: boolean
  isBlueBorder?: boolean
}>()

const emit = defineEmits(['update:modelValue'])

const time = ref('00:00')

const onChange = () => {
  if (time.value.includes('_')) {
    return
  }

  const [hours, minutes] = time.value.split(':').map(Number)

  // Validate hours and minutes
  if (hours > 24 || (hours === 24 && minutes > 0) || minutes > 59) {
    // Invalid time, reset to last valid value
    time.value = formatTime(props.modelValue as number)
    return
  }

  emit('update:modelValue', changeTimeToSeconds(time.value))
}

const onFocus = (event: Event) => {
  // Optional: You can add focus behavior here if needed
}

const onBlur = (event: Event) => {
  onChange() // Validate on blur
}

const formatTime = (seconds: number): string => {
  const hours = Math.floor(seconds / 3600)
  const minutes = Math.floor((seconds % 3600) / 60)
  return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`
}

watch(
  () => props.modelValue,
  (newValue) => {
    if (typeof newValue === 'number') {
      time.value = formatTime(newValue)
    }
  }
)

onMounted(() => {
  if (props.initValue !== undefined) {
    if (props.initValue === '') {
      emit('update:modelValue', 0)
      return
    }
    emit('update:modelValue', changeTimeToSeconds(props.initValue))
    const arr = props.initValue.split(':')
    time.value = `${arr[0].padStart(2, '0')}:${arr[1].padStart(2, '0')}`
  }
})
</script>
