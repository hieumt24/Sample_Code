<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    modal
    :draggable="false"
    header="Edit Profile"
    :style="{ width: '25rem' }"
  >
    <div class="flex items-center gap-4 mb-8">
      {{ data }}
    </div>
    <div class="py-4 flex justify-evenly">
      <ActionButton
        class="mr-2"
        icon="times"
        :value="$t('form.cancel')"
        @click="onClose"
        :is-outlined="true"
      />
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import ActionButton from '@/components/buttons/ActionButton.vue'

defineProps<{
  open: boolean
  data: { [key: string]: string | number } | undefined
}>()

const emit = defineEmits<(e: 'close') => void>()

const onClose = () => {
  emit('close')
}
</script>
