<template>
  <Toast />
  <Toast position="bottom-left" group="bl" />
  <ConfirmDialog class="w-fit" group="headless">
    <template #container="{ message, acceptCallback, rejectCallback }">
      <div class="flex flex-col items-center p-5 bg-surface-0 dark:bg-surface-900 rounded-md">
        <div
          class="rounded-full bg-primary text-white dark:text-surface-950 inline-flex justify-center items-center h-[6rem] w-[6rem] -mt-8"
        >
          <i class="pi pi-question text-5xl"></i>
        </div>
        <span class="font-bold text-2xl block mb-2 mt-4">{{ message.header }}</span>
        <p class="mb-0">{{ message.message }}</p>
        <div class="flex items-center gap-2 mt-4">
          <Button label="Xác nhận" @click="acceptCallback"></Button>
          <Button label="Huỷ" outlined @click="rejectCallback"></Button>
        </div>
      </div>
    </template>
  </ConfirmDialog>
  <RouterView />
</template>

<script setup lang="ts">
import Toast from 'primevue/toast'
import { useNotificationStore } from '@/stores/notificationStore'
import { onMounted, watchEffect } from 'vue'
import { useToast } from 'primevue/usetoast'
import { ACCESS_TOKEN_KEY } from './constants/storage'
import Cookies from 'js-cookie'
import ConfirmDialog from 'primevue/confirmdialog'
import Button from 'primevue/button'
import { useLocationStore } from './stores/locationStore'
import { NOTIFICATION_HUB } from './constants/env'

const toast = useToast()

const notificationStore = useNotificationStore()
const locationStore = useLocationStore()
watchEffect(() => {
  const newNotification = notificationStore.latestNotification
  if (newNotification) {
    toast.add({
      severity: 'secondary',
      summary: newNotification.title,
      detail: newNotification.content,
      group: 'bl'
    })
  }
})

onMounted(async () => {
  const accessToken = Cookies.get(ACCESS_TOKEN_KEY)
  if (accessToken) {
    notificationStore.startConnection(NOTIFICATION_HUB)
  }
  if (!locationStore.enableLocation) {
    await locationStore.getCurrentLocation()
  }
  if (locationStore.allProvince.length === 0) {
    await locationStore.getAllProvinces()
  }
})
</script>
