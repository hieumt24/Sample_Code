<template>
  <div class="p-4 bg-white rounded-[10px]">
    <h2 class="text-2xl font-bold text-center">{{ props.title }}</h2>
    <form @submit.prevent>
      <slot />
      <div class="py-4 flex justify-evenly">
        <ActionButton
          v-if="!noCancel"
          class="mr-2"
          :value="$t('form.cancel')"
          @click="emit('cancel')"
          :is-outlined="true"
        />
        <ActionButton
          v-if="!noSubmit"
          :disable="!isSubmit"
          :value="'Lưu'"
          @click="emit('submit')"
        />
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import ActionButton from '@/components/buttons/ActionButton.vue'

const props = defineProps<{
  title?: string
  error?: string
  label?: string
  noCancel?: boolean
  noSubmit?: boolean
  // noApprove?: boolean
  // noReject?: boolean
  isSubmit?: boolean
}>()
const emit = defineEmits(['cancel', 'submit'])
</script>
