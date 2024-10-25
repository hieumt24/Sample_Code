<template>
  <div class="mt-2 mb-2 flex flex-col">
    <label class="font-semibold" :for="name">{{ label }}</label>
    <Password
      :id="name"
      v-model="model"
      :disabled="disabled"
      toggleMask
      promptLabel="Nhập mật khẩu"
      weakLabel="Yếu"
      mediumLabel="Trung bình"
      strongLabel="Mạnh"
      :feedback="feedBack"
    />
    <small v-if="error" class="text-red-500">{{ error }}</small>
  </div>
</template>

<script setup lang="ts">
import Password from 'primevue/password'
import type { Nullable } from 'primevue/ts-helpers'

const props = defineProps<{
  title?: string
  modelValue: string
  error?: string
  label?: string
  textarea?: boolean
  disabled?: boolean
  feedBack?: boolean
  name: string
}>()
const emit = defineEmits(['update:modelValue'])

const model = defineModel<Nullable<string>>()
</script>
