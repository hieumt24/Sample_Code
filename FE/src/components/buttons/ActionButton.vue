<template>
  <Button
    :icon="icon ? `pi pi-${icon}` : ''"
    :class="[
      {
        'min-w-20': !isIcon && size === 'small',
        'min-w-24': !isIcon && size === undefined,
        'min-w-28': !isIcon && size === 'large',
        'hover:text-white hover:bg-primary-hover !duration-0': isOutlined
      },
      '!p-2 text-base'
    ]"
    :severity="severity"
    :outlined="isOutlined"
    :label="value"
    :disabled="disable"
    :size="size"
    :loading="isLoading"
  />
</template>

<script setup lang="ts">
import Button from 'primevue/button'
import { type Severity, type Size } from './buttons'
import type { PropType } from 'vue'

defineProps({
  isIcon: {
    type: Boolean,
    required: false,
    default: false
  },
  size: {
    type: String as PropType<Size>,
    required: false,
    default: undefined
  },
  severity: {
    type: String as PropType<Severity>,
    required: false,
    default: 'success'
  },
  icon: {
    type: String,
    required: false
  },
  value: String,
  isOutlined: Boolean,
  disable: Boolean,
  isLoading: Boolean
})
</script>
