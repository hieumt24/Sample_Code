<template>
  <Dialog
    :visible="open"
    :header="'Bản đồ đến sân'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[800px] mx-20"
  >
    <div class="font-semibold text-blue-400">
      <span> <UserMainMap ref="userMainMapRef" /></span>
    </div>
    <div class="my-2 flex justify-between">
      <ActionButton
        @click.stop="emit('close')"
        icon="times"
        class="bg-green-400 mx-2"
        is-outlined
        value="Đóng"
      />
      <div>
        <ActionButton
          @click.stop="onSearchPath"
          class="bg-green-400 mx-auto ml-4"
          value="Tìm đường"
          :is-loading="isSearchLoading"
        />
      </div>
    </div>
  </Dialog>
  <Toast />
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useBookingStore } from '@/stores/bookingStoreOwner'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { ref, watch } from 'vue'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'
import UserMainMap from '../maps/UserMainMap.vue'
import { useFieldStore } from '@/stores/fieldStore'
import { useLocationStore } from '@/stores/locationStore'

const toast = useToast()
const { t } = useI18n()

const bookingStore = useBookingStore()
const fieldStore = useFieldStore()
const locationStore = useLocationStore()

const props = defineProps<{
  open: boolean
}>()

const isSearchLoading = ref(false)
const userMainMapRef = ref<InstanceType<typeof UserMainMap> | null>(null)

const emit = defineEmits<(e: 'close') => void>()

const onSearchPath = async () => {
  try {
    isSearchLoading.value = true
    locationStore.getCurrentLocation()
    if (locationStore.enableLocation && locationStore.latitude && locationStore.longitude) {
      const destination = fieldStore.field
      if (destination) {
        const routePoints = {
          origin: { lat: locationStore.latitude, lng: locationStore.longitude },
          destination: { lat: destination.latitude, lng: destination.longitude }
        }
        if (userMainMapRef.value) {
          userMainMapRef.value.direction(routePoints)
        }
      }
    } else {
      toast.add({
        severity: 'error',
        detail: 'Không thể lấy vị trí của bạn.\n Hãy bật thủ công!',
        life: 8000
      })
    }
  } catch (error) {
    toast.add({
      severity: 'error',
      detail: 'Không thể lấy vị trí của bạn.\n Hãy bật thủ công!',
      life: 8000
    })
  }
  isSearchLoading.value = false
}
</script>
