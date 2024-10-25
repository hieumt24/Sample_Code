<!-- NotificationList.vue -->
<template>
  <div class="notification-list">
    <div v-if="notifications.length === 0" class="text-surface-500 dark:text-surface-0/60">
      Không có thông báo
    </div>
    <ul v-else class="list-none p-0 m-0 flex flex-col gap-3">
      <li
        v-for="notification in notifications"
        :key="notification.id"
        class="flex items-center gap-2 p-2 border-b border-gray-200 hover:bg-gray-100 cursor-pointer relative"
        @click="$emit('mark-as-read', notification.id)"
      >
        <div class="flex-1">
          <span class="font-medium">{{ notification.content }}</span>
          <div>
            <span
              class="text-sm text-gray-500 mt-1"
              v-tooltip.bottom="{
                value: formatDate(notification.createdAt ?? ''),
                autoHide: false
              }"
            >
              {{ formatTimeAgo(notification.createdAt ?? '') }}
            </span>
          </div>
        </div>
        <span :class="{ 'opacity-0': notification.isRead }">
          <Badge value="" severity="info"></Badge>
        </span>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import type { Notification } from '@/types/Notification'
import { formatDate, formatTimeAgo } from '@/utils/dateUtil'
import Badge from 'primevue/badge'

defineProps({
  notifications: {
    type: Array as () => Notification[],
    required: true
  }
})

defineEmits(['mark-as-read'])
</script>
