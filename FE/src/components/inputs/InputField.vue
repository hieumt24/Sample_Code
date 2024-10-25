<template>
  <div class="mt-2 mb-2 flex flex-col">
    <label class="font-semibold" :for="name"
      >{{ label }}<span v-if="props.require" class="text-red-500"> *</span></label
    >
    <InputText
      v-if="!textarea"
      :id="name"
      size="small"
      class="w-full"
      type="text"
      v-model="model"
      :disabled="disabled"
    />
    <Textarea v-if="textarea" v-model="model" rows="5" cols="30" class="h-full" />
    <small v-if="error" class="text-red-500">{{ error }}</small>
  </div>
</template>

<script setup lang="ts">
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import type { Nullable } from 'primevue/ts-helpers'

const props = defineProps<{
  title?: string
  modelValue: string
  error?: string
  label?: string
  textarea?: boolean
  disabled?: boolean
  name: string
  require?: boolean
}>()
const emit = defineEmits(['update:modelValue'])

const model = defineModel<Nullable<string>>()
</script>
