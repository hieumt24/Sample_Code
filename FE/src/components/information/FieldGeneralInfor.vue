<template>
  <div class="flex p-10">
    <div class="w-1/2 p-5">
      <h2 class="text-2xl font-bold mb-4">{{ name }}</h2>

      <div class="mb-4"><strong>Số điện thoại:</strong> {{ phoneNumber }}</div>

      <div class="mb-4">
        <strong>Mô tả:</strong>
        <p>{{ description }}</p>
      </div>

      <div class="mb-4"><strong>Giờ mở cửa:</strong> {{ openTime }}</div>
      <div class="mb-4"><strong>Giờ đóng cửa:</strong> {{ closeTime }}</div>
      <div class="mb-4"><strong>Thời gian một trận:</strong> {{ slotTime }}</div>

      <div class="mb-4">
        <strong>Cover:</strong>
        <Image :src="fieldStore.field?.cover" alt="Cover Image" width="500" preview />
      </div>

      <div class="mb-4">
        <strong>Avatar:</strong>
        <Image :src="fieldStore.field?.avatar" alt="Avatar Image" width="200" preview />
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
          <div class="w-[600px] h-[350px] flex items-center !bg-gray-200">
            <div class="mx-auto w-[600px] h-[350px]">
              <img
                :src="slotProps.item.url"
                class="object-contain"
                :alt="'anh san'"
                style="width: 100%"
              />
            </div>
          </div>
        </template>
        <template #thumbnail="slotProps">
          <div class="h-[200px] flex items-end">
            <img :src="slotProps.item.url" :alt="'anh san'" />
          </div>
        </template>
      </Galleria>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useFieldStore } from '@/stores/fieldStore'
import { useImageStore } from '@/stores/imageStore'
import Image from 'primevue/image'
import Galleria from 'primevue/galleria'

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
