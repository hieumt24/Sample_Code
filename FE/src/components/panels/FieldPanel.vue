<template>
  <div class="relative h-48">
    <img :src="field.cover" :alt="field.name" class="w-full h-full object-cover" />
    <div class="absolute top-0 left-0 bg-blue-500 text-white px-2 py-1 rounded-br-lg text-sm">
      {{ field.province }}
    </div>
    <div class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-4">
      <h2 class="text-white font-bold text-xl line-clamp-2">{{ field.name }}</h2>
    </div>
  </div>
  <div class="p-4 flex-grow flex flex-col justify-between">
    <div>
      <p class="text-gray-600 text-sm mb-3 line-clamp-3">{{ field.address }}</p>
      <div class="flex justify-between items-center mb-3">
        <span> {{ Math.floor(field.price).toLocaleString() }} VNĐ</span>
        <Badge :value="`${field.numberOfBookings} lượt`" severity="success" />
      </div>
    </div>
    <div>
      <div class="flex justify-between items-center mb-3">
        <span class="text-sm text-gray-500">{{ field.openTime }} - {{ field.closeTime }}</span>
        <span v-if="field.rating" class="flex items-center">
          {{ field.rating }}
          <i class="pi pi-star-fill text-green-400 ml-2" style="font-size: 1rem" />
        </span>
        <span v-else>Không có đánh giá</span>
      </div>
      <Button
        label="Chi tiết"
        icon="pi pi-external-link"
        class="p-button-outlined w-full"
        @click="$router.push('/user/field/' + field.id)"
      />
    </div>
  </div>
</template>
<script setup lang="ts">
import type { Field } from '@/types/Field'
import Badge from 'primevue/badge'
import Button from 'primevue/button'
defineProps<{
  field: Field
}>()
</script>
