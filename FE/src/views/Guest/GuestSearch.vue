<template>
  <div class="h-full w-full bg-white">
    <div
      v-if="fieldStoreUser.superSearchFields?.length === 0"
      class="flex w-full items-center h-96 bg-white"
    >
      <span
        v-if="fieldStoreUser.superSearchFields?.length === 0 && fieldStoreUser.loadedFreeWord"
        class="mx-auto"
      >
        <i class="pi pi-file-excel ml-2" style="font-size: 2rem" />
        <span class="text-center text-base font-semibold ml-5">
          Không tìm thấy kêt quả nào phù hợp
        </span>
      </span>
      <span v-else>
        <span class="text-center text-base font-semibold ml-5"> Kết quả </span>
      </span>

      <span v-if="fieldStoreUser.loadingFreeword" class="mx-auto">
        <i class="pi pi-hourglass ml-2" style="font-size: 2rem" />
        <h1 class="text-center">Đang tải ...</h1>
      </span>
    </div>
    <div class="w-full grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-2 py-4">
      <div v-for="field in fieldStoreUser.superSearchFields" :key="field.id" class="col-span-1">
        <div
          class="bg-gray-200 h-56 w-72 rounded-md mx-auto relative shadow-md overflow-hidden cursor-pointer hover:bg-slate-100 hover:shadow-xl transition duration-200 ease-in-out"
          @click="$router.push('/user/field/' + field.id)"
        >
          <div
            class="z-20 absolute top-0 left-0 bg-blue-500 text-white rounded-tl-md rounded-br-md px-2 border-r border-b border-white border-t-blue-500"
          >
            {{ field.province }}
          </div>
          <div
            class="absolute z-10 h-24 w-full opacity-50 bg-gradient-to-b from-transparent to-black"
          ></div>
          <img class="absolute object-cover h-24 w-full z-0" :src="field.cover" alt="" />
          <div class="absolute bottom-0 justify-between h-32 flex flex-col w-full">
            <div class="h-10 w-full">
              <span class="px-2 line-clamp-2 w-full text-sm">{{ field.address }}</span>
            </div>
            <div class="flex justify-between px-2">
              <span>{{ field.price }} vnd</span>
              <Badge :value="field.numberOfBookings + ' lượt'" />
            </div>
            <span class="px-2">{{ field.openTime }} - {{ field.closeTime }}</span>
            <div class="p-2 flex w-full justify-between">
              <span class="font-semibold text-xl">{{ field.name }}</span>
              <span class="font-semibold">
                {{ field.rating }}
                <i class="pi pi-star-fill text-green-400" style="font-size: 1rem"></i>
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useFieldStoreUser } from '@/stores/fieldStoreUser'

const { t } = useI18n()

const fieldStoreUser = useFieldStoreUser()
</script>
