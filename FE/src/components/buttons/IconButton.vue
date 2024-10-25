<template>
  <Button
    :icon="`pi pi-${icon}`"
    :class="{
      'h-10 w-10': size === 'large',
      'h-7 w-7': size === undefined,
      'h-6 w-8': size === 'small'
    }"
    :size="size"
    rounded
    outlined
  >
  </Button>
</template>

<script setup lang="ts">
import Button from 'primevue/button'
import { type Size, type ButtonIcons } from './buttons'
import type { PropType } from 'vue'

defineProps({
  size: {
    type: String as PropType<Size>,
    required: false,
    default: undefined
  },
  icon: {
    type: String as PropType<ButtonIcons>,
    required: false
  }
})
</script>
