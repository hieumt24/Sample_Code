<template>
  <div class="bg-white rounded-lg shadow-md p-6 w-fit">
    <h2 class="text-xl font-semibold text-gray-800 mb-4">Tìm kiếm</h2>
    <div class="space-y-4 mb-6 grid grid-cols-1">
      <slot />
    </div>
    <div class="flex flex-col space-y-3">
      <div class="flex space-x-3">
        <Button
          label="Đặt lại"
          icon="pi pi-undo"
          class="p-button-outlined p-button-secondary flex-1"
          outlined
          @click="emit('reset')"
        />
        <Button
          label="Tìm"
          icon="pi pi-search"
          class="p-button-primary flex-1"
          @click="emit('search')"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import Button from 'primevue/button'

const emit = defineEmits<{
  (e: 'reset'): void
  (e: 'search'): void
}>()
</script>

<style scoped>
.p-button {
  transition: all 0.3s ease;
}

.p-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.p-button-outlined:hover {
  background-color: #f3f4f6;
}

.p-button-primary {
  background-color: #4f46e5;
  border-color: #4f46e5;
}

.p-button-primary:hover {
  background-color: #4338ca;
  border-color: #4338ca;
}
</style>
