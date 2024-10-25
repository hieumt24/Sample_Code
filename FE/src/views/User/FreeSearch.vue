<template>
  <div class="w-full bg-white p-5">
    <div v-if="fieldStoreUser.freewordFields?.length === 0" class="text-center py-20">
      <i class="pi pi-search text-4xl text-gray-400 mb-4"></i>
      <p class="text-xl text-gray-600">Không tìm thấy kết quả nào phù hợp</p>
    </div>
    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <div
        v-for="field in fieldStoreUser.freewordFields"
        :key="field.id"
        class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition duration-300 cursor-pointer flex flex-col"
      >
        <div class="relative h-48">
          <img :src="field.cover" :alt="field.name" class="w-full h-full object-cover" />
          <div class="absolute top-0 left-0 bg-blue-500 text-white px-2 py-1 rounded-br-lg text-sm">
            {{ field.province }}
          </div>
          <div
            class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-4"
          >
            <h2 class="text-white font-bold text-xl line-clamp-2">{{ field.name }}</h2>
          </div>
        </div>
        <div class="p-4 flex-grow flex flex-col justify-between">
          <div>
            <p class="text-gray-600 text-sm mb-3 line-clamp-3">{{ field.address }}</p>
            <div class="flex justify-between items-center mb-3">
              <span>{{ field.price }} vnd</span>
              <Badge :value="`${field.numberOfBookings} lượt`" severity="success" />
            </div>
          </div>
          <div>
            <div class="flex justify-between items-center mb-3">
              <span class="text-sm text-gray-500"
                >{{ field.openTime }} - {{ field.closeTime }}</span
              >
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
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useFieldStoreUser } from '@/stores/fieldStoreUser'
import Badge from 'primevue/badge'
import Button from 'primevue/button'

const fieldStoreUser = useFieldStoreUser()
</script>
