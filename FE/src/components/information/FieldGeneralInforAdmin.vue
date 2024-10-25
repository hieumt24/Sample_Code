<template>
  <div class="h-56 w-full relative mb-20">
    <img
      class="object-cover h-full w-full rounded-md shadow-lg"
      :src="fieldStore?.field?.cover"
      alt="Hinh san 1"
    />
    <div class="absolute h-20 w-full left-20 -bottom-10 flex">
      <div class="rounded-full w-20 h-20 overflow-hidden ring-2 ring-gray-500">
        <img
          class="object-cover h-full w-full"
          :src="
            fieldStore?.field?.avatar || fieldStore?.field?.avatar === ''
              ? '/default-field-avt.png'
              : fieldStore?.field?.avatar
          "
          alt="Hinh san 1"
        />
      </div>
      <div class="h-full flex items-end pl-5 w-full justify-start">
        <div class="flex justify-start items-end">
          <span class="text-2xl font-semibold mr-2">{{ fieldStore?.field?.name }}</span>
        </div>
        <Button
          label="Xem bản đồ"
          class="!bg-orange-300 w-52 ml-5"
          @click="isShowMapDialog = true"
        ></Button>
      </div>
    </div>
  </div>
  <div class="flex p-10">
    <div class="w-1/2 p-5">
      <div class="mb-4"><strong>Số điện thoại:</strong> {{ phoneNumber }}</div>
      <div class="mb-4"><strong>Địa chỉ:</strong> {{ fieldStore.field?.address }}</div>
      <div class="mb-4"><strong>Phường Xã:</strong> {{ fieldStore.field?.commune }}</div>
      <div class="mb-4"><strong>Quận Huyện:</strong> {{ fieldStore.field?.district }}</div>
      <div class="mb-4"><strong>Tỉnh Thành:</strong> {{ fieldStore.field?.province }}</div>
      <div class="mb-4">
        <strong>Mô tả:</strong>
        <p>{{ description }}</p>
      </div>

      <div class="mb-4"><strong>Giờ mở cửa:</strong> {{ openTime }}</div>
      <div class="mb-4"><strong>Giờ đóng cửa:</strong> {{ closeTime }}</div>
      <div class="mb-4">
        <strong>Giá sân:</strong>
        {{ Math.floor(fieldStore.field?.price ?? 0).toLocaleString() }} VNĐ
      </div>
      <div class="mb-4">
        <strong>Tiền cọc:</strong>
        {{ Math.floor(fieldStore.field?.deposit ?? 0).toLocaleString() }} VNĐ
      </div>
    </div>

    <div>
      <h3 class="text-xl font-bold mb-2">Hình ảnh sân</h3>
      <Galleria
        :value="imageStore.images"
        :responsiveOptions="responsiveOptions"
        :numVisible="5"
        containerStyle="max-width: 600px"
        :pt:itemcontainer:class="'bg-surface-0 before:bg-none'"
      >
        <template #item="slotProps">
          <img
            :src="slotProps.item.url"
            class="object-contain"
            :alt="'anh san'"
            style="width: 100%"
          />
        </template>
        <template #thumbnail="slotProps">
          <img :src="slotProps.item.url" :alt="'anh san'" />
        </template>
      </Galleria>
    </div>
    <MapFieldDetailDialog :open="isShowMapDialog" @close="isShowMapDialog = false" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useFieldStore } from '@/stores/fieldStore'
import { useImageStore } from '@/stores/imageStore'
import Image from 'primevue/image'
import Galleria from 'primevue/galleria'
import MapFieldDetailDialog from '../dialogs/MapFieldDetailDialog.vue'
import Button from 'primevue/button'

const fieldStore = useFieldStore()
const imageStore = useImageStore()

const props = defineProps<{
  id: number
}>()

const name = ref('')
const phoneNumber = ref('')
const description = ref('')
const openTime = ref()
const closeTime = ref()
const slotTime = ref()
const isShowMapDialog = ref(false)
const responsiveOptions = ref([
  {
    breakpoint: '1300px',
    numVisible: 4
  },
  {
    breakpoint: '575px',
    numVisible: 1
  }
])

onMounted(async () => {
  await fieldStore.getFieldById(props.id)
  if (fieldStore.field) {
    name.value = fieldStore.field.name
    phoneNumber.value = fieldStore.field.phoneNumber
    description.value = fieldStore.field.description
    openTime.value = fieldStore.field.openTime
    closeTime.value = fieldStore.field.closeTime
    slotTime.value = fieldStore.field.slotTime
  }
  imageStore.getImagesByField(props.id)
})
</script>
