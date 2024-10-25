<template>
  <div class="card flex justify-center">
    <Button severity="success" text aria-label="Notification" @click="toggle">
      <i class="pi pi-bell relative" style="font-size: 1.5rem" />
      <Badge
        v-if="notificationStore.unreadCount > 0"
        class="absolute top-0 right-0 inline-flex items-center justify-center p-1 text-xs font-bold leading-none text-red-100 bg-red-600 rounded-full"
        :value="notificationStore.unreadCount.toString()"
      ></Badge>
    </Button>

    <OverlayPanel ref="op" class="notification-panel">
      <div class="flex flex-col w-[25rem]">
        <div class="flex justify-between items-center">
          <span class="font-medium text-surface-700 dark:text-surface-0/80 text-xl">
            Thông báo
          </span>
          <Button
            label="Đánh dấu tất cả đã đọc"
            @click="markAllAsRead"
            class="p-button-text p-button-sm"
            :disabled="notificationStore.unreadCount === 0"
            text
          />
        </div>
        <div class="relative">
          <div
            v-if="loading"
            class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
          >
            <ProgressSpinner strokeWidth="4" />
          </div>
          <TabView v-model:activeIndex="activeTabIndex">
            <TabPanel header="Tất cả">
              <div class="max-h-[70vh] overflow-y-auto">
                <NotificationList
                  :notifications="notificationStore.notifications"
                  @mark-as-read="markAsRead"
                />
              </div>
            </TabPanel>
            <TabPanel header="Chưa đọc">
              <div class="max-h-[70vh] overflow-y-auto">
                <NotificationList :notifications="unreadNotifications" @mark-as-read="markAsRead" />
              </div>
            </TabPanel>
          </TabView>
          <div>
            <Button
              v-if="notificationStore.hasMoreNotifications"
              label="Xem các thông báo trước"
              @click="loadMoreNotifications"
              class="mt-3 w-full"
            />
          </div>
        </div>
      </div>
    </OverlayPanel>
  </div>
</template>

<script setup lang="ts">
import { useNotificationStore } from '@/stores/notificationStore'
import Badge from 'primevue/badge'
import Button from 'primevue/button'
import OverlayPanel from 'primevue/overlaypanel'
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import { onMounted, ref, computed } from 'vue'
import NotificationList from '../information/NotificationList.vue'
import IconButton from './IconButton.vue'
import ProgressSpinner from 'primevue/progressspinner'

const notificationStore = useNotificationStore()
const op = ref()
const activeTabIndex = ref(0)
const loading = ref(false)
const unreadNotifications = computed(() =>
  notificationStore.notifications.filter((notification) => !notification.isRead)
)

onMounted(async () => {
  await notificationStore.getUnreadNotificationsCount()
  await notificationStore.getNotificationList()
})

const toggle = async (event: Event) => {
  op.value.toggle(event)
}

const loadMoreNotifications = async () => {
  loading.value = true
  await notificationStore.getNotificationList()
  loading.value = false
}

const markAsRead = async (id: number) => {
  await notificationStore.markAsRead(id)
}

const markAllAsRead = async () => {
  loading.value = true
  await notificationStore.markAllAsRead()
  loading.value = false
}
</script>
