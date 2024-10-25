<template>
  <div class="px-3 fixed w-full top-0 z-30">
    <Menubar :model="items">
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
    label: t('common.home'),
    icon: 'pi pi-th-large',
    command: () => navigate('/home')
  },
  {
    label: t('sidebar.admin.fields'),
    icon: 'pi pi-money-bill',
    command: () => navigate('/fields')
  },
  {
    label: t('sidebar.admin.users'),
    icon: 'pi pi-users',
    command: () => navigate('/users')
  },
  {
    label: t('sidebar.admin.posts'),
    icon: 'pi pi-align-center',
    command: () => navigate('/posts')
  },
  {
    label: t('sidebar.admin.system'),
    icon: '',
    command: () => navigate('/system')
  },
  {
    label: t('sidebar.admin.reports'),
    icon: 'pi pi-exclamation-circle',
    command: () => navigate('/reports')
  }
])

const navigate = (to: string) => {
  router.push('/admin' + to)
}
</script>
