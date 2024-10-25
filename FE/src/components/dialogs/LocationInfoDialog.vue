<template>
  <Dialog
    :visible="open"
    @update:visible="emit('close')"
    modal
    :draggable="false"
    header="Thông tin vị trí"
    :style="{ width: '25rem' }"
  >
    <div v-if="isEnableLocation()">Bạn đã cho phép truy cập vị trí của mình</div>
    <div v-else class="p-2 flex flex-col">
      <h1 class="text-red-400 text-base mb-2">Bạn chưa cho phép truy cập vị trí của mình</h1>
      <p class="">
        Để có trải nghiệm tốt nhất, hãy cho phép chúng tôi truy cập vào vị trí của bạn, nhiều tính
        năng của chúng tôi sẽ cho ra kết quả tốt hơn dựa trên dữ liệu vị trí của người dùng. Xin cảm
        ơn!
      </p>
      <Button
        @click.prevent="enableLocation"
        class="mt-2 mx-auto"
        label="Cho phép truy cập vị trí"
      />
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useI18n } from 'vue-i18n'
import Button from 'primevue/button'
import { useLocationStore } from '@/stores/locationStore'
import { useToast } from 'primevue/usetoast'
const toast = useToast()

const { t } = useI18n()

defineProps<{
  open: boolean
}>()

const locationStore = useLocationStore()

const isEnableLocation = () => {
  return locationStore.enableLocation
}

const enableLocation = async () => {
  console.log(1234)

  await locationStore.getCurrentLocation()
  if (locationStore.enableLocation) {
    toast.add({
      severity: 'success',
      detail: 'Lấy vị trí thành công!',
      life: 8000
    })
  } else {
    console.log(13)
    toast.add({
      severity: 'error',
      detail: 'Không thể lấy vị trí của bạn.\n Hãy bật thủ công!',
      life: 8000
    })
  }
}

const emit = defineEmits<(e: 'close') => void>()
</script>
