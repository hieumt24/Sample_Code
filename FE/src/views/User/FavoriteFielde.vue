<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-2xl font-bold mb-6">Danh sách yêu thích</h1>

    <div v-if="favoriteFieldStore.loadingFavorite" class="flex justify-center items-center py-12">
      <i class="pi pi-spin pi-spinner text-4xl text-blue-500"></i>
    </div>

    <div
      v-else-if="!favoriteFieldStore.totalFavotite"
      class="bg-gray-100 rounded-lg p-6 text-center"
    >
      <i class="pi pi-heart-fill text-4xl text-gray-400 mb-4"></i>
      <p class="text-lg font-semibold text-gray-600">Không có sân nào trong danh sách yêu thích</p>
      <p class="text-gray-500 mt-2">
        Hãy thêm vào danh sách yêu thích để việc tìm kiếm dễ dàng hơn!
      </p>
    </div>

    <div v-else>
      <p class="text-lg mb-6">
        Bạn đã thêm
        <span class="font-bold text-green-500">{{ favoriteFieldStore.totalFavotite }}</span>
        sân vào danh sách yêu thích
      </p>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <div
          v-for="field in favoriteFieldStore.favoriteFields"
          :key="field.id"
          class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition duration-300 cursor-pointer"
        >
          <div class="relative h-48">
            <img :src="field.cover" :alt="field.name" class="w-full h-full object-cover" />
            <div class="absolute top-0 left-0 bg-blue-500 text-white px-2 py-1 rounded-br-lg">
              {{ field.province }}
            </div>
            <div
              class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-4"
            >
              <h2 class="text-white font-bold text-xl">{{ field.name }}</h2>
            </div>
          </div>
          <div class="p-4">
            <p class="text-gray-600 text-sm mb-2 line-clamp-2">
              {{ field.address || '&nbsp;' }}
            </p>
            <div class="flex justify-between items-center mb-2">
              <span>{{ field?.price }} VND</span>
              <Badge :value="`${field.numberOfBookings} lượt`" severity="success" />
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-500"
                >{{ field.openTime }} - {{ field.closeTime }}</span
              >
              <Rating :modelValue="4" readonly :cancel="false" />
            </div>
          </div>
          <div class="px-4 pb-4 flex">
            <Button
              label="Chi tiết"
              icon="pi pi-external-link"
              class="p-button-outlined w-full mr-1"
              @click="$router.push('/user/field/' + field.id)"
            />
            <Button
              label="Bỏ thích"
              icon="pi pi-heart"
              class="p-button-outlined w-full ml-1"
              outlined
              @click="onRemoveList(field.id)"
            />
          </div>
        </div>
      </div>

      <!-- <div class="mt-8">
        <PagingElement
          :limit="4"
          :total="favoriteFieldStore.totalFavotite"
          @change-page="favoriteFieldStore.changePageRecentlyBooking"
          v-model="favoriteFieldStore.currentPage"
        />
      </div> -->
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import PagingElement from '@/components/pagination/PagingElement.vue'
import { onMounted } from 'vue'
import Rating from 'primevue/rating'
import Badge from 'primevue/badge'
import Button from 'primevue/button'
import { useFavoriteFieldStore } from '@/stores/favoriteFieldStore'

const { t } = useI18n()

const favoriteFieldStore = useFavoriteFieldStore()

const onRemoveList = async (fieldId: number) => {
  await favoriteFieldStore.deleteFromFavoriteFields(fieldId)
  await favoriteFieldStore.getMyFavoriteFields()
}

onMounted(async () => {
  await favoriteFieldStore.getMyFavoriteFields()
})
</script>
