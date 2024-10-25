<template>
  <div class="fixed top-0 w-full bg-white shadow-md z-30">
    <Menubar class="w-full" :model="items" :aria-modal="true">
      <template #end>
        <div class="flex items-center space-x-2">
          <NotificationButton class="mr-2" />
          <button
            @click="authStore.logout()"
            class="flex items-center px-3 py-2 rounded-md text-sm font-medium text-red-600 hover:bg-red-100 hover:text-red-800 transition duration-150 ease-in-out"
          >
            <i class="pi pi-sign-out mr-2"></i>
            Đăng xuất
          </button>
        </div>
      </template>
    </Menubar>
  </div>
</template>

<script setup lang="ts">
import router from '@/router'
import { useAuthStore } from '@/stores/authStore'
import Menubar from 'primevue/menubar'
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import NotificationButton from '../buttons/NotificationButton.vue'
const { t } = useI18n()

const authStore = useAuthStore()

const items = ref([
  {
    label: t('sidebar.owner.home'),
    icon: 'pi pi-calendar',
    command: () => navigate('/home')
  },

  {
    label: t('sidebar.owner.bookings'),
    icon: 'pi pi-ticket',
    command: () => navigate('/bookings')
  },
  {
    label: t('sidebar.owner.field'),
    icon: 'pi pi-money-bill',
    items: [
      {
        label: t('sidebar.owner.schedules'),
        icon: 'pi pi-calendar-clock',
        command: () => navigate('/schedules')
      },
      {
        label: t('sidebar.owner.posts'),
        icon: 'pi pi-align-center',
        command: () => navigate('/posts')
      }
    ]
  },

  {
    label: t('sidebar.owner.me'),
    icon: 'pi pi-user',
    command: () => navigate('/info')
  },
  {
    label: t('sidebar.owner.find_booking'),
    icon: 'pi pi-search',
    command: () => navigate('/find-booking')
  }
])

const navigate = (to: string) => {
  router.push('/staff' + to)
}
</script>
