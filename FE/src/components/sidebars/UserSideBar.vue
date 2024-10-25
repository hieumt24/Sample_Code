<template>
  <div
    class="w-full fixed top-0 z-30 pt-2 px-5 pb-5"
    :class="{ 'bg-[#f5f5f5] shadow-lg': isScrolled }"
  >
    <Menubar class="w-full" :model="items">
      <template #end>
        <div class="flex">
          <Button
            icon="pi pi-map"
            label="Tìm bằng bản đồ"
            class="bg-green-400"
            @click.stop="navigate('/map-search')"
          />
          <div class="flex items-center cursor-pointer" @click="openLocationInfo = true">
            <i
              class="pi pi-info-circle ml-3"
              :class="locationStore.enableLocation ? 'text-green-400' : 'text-orange-400'"
              style="font-size: 1.2rem"
            />
          </div>
          <NotificationButton class="mr-2" />
          <button ref="toggleProfile" class="relative">
            <div @click="openProfile = !openProfile" class="flex items-center pr-6">
              <Avatar :image="avatarImg ?? '/noavatar.png'" class="mr-2" shape="circle" />
              <span class="font-bold mr-2">{{ authStore.name }}</span>
              <i v-if="!openProfile" class="pi pi-angle-down pt-1" style="color: green"></i>
              <i v-if="openProfile" class="pi pi-angle-up pt-1" style="color: green"></i>
            </div>
            <div
              v-if="openProfile"
              class="absolute -bottom-24 flex flex-col h-20 w-full bg-white text-start rounded-md shadow-md border border-gray-200"
            >
              <div class="p-2" @click="navigate('/info')">
                <i class="pi pi-user"></i>
                <span class="font-bold ml-2">Hồ sơ</span>
              </div>
              <div class="p-2 flex items-center" @click="authStore.logout()">
                <i class="pi pi-sign-out"></i>
                <span class="font-bold ml-2">Đăng xuất</span>
              </div>
            </div>
          </button>
        </div>
      </template>
    </Menubar>
  </div>

  <LocationInfoDialog :open="openLocationInfo" @close="openLocationInfo = false" />
</template>

<script setup lang="ts">
import router from '@/router'
import Menubar from 'primevue/menubar'
import Avatar from 'primevue/avatar'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import { onClickOutside } from '@vueuse/core'
import LocationInfoDialog from '../dialogs/LocationInfoDialog.vue'
import { useLocationStore } from '@/stores/locationStore'
import { useFieldStoreUser } from '@/stores/fieldStoreUser'
import NotificationButton from '../buttons/NotificationButton.vue'
import Button from 'primevue/button'
const { t } = useI18n()

const fieldStoreUser = useFieldStoreUser()
const authStore = useAuthStore()
const locationStore = useLocationStore()
const avatarImg = computed(() => localStorage.getItem('avatar'))

const isScrolled = ref(false)
const openProfile = ref(false)
const openLocationInfo = ref(false)
const freeSearchKeyword = ref('')
const toggleProfile = ref(null)
onClickOutside(toggleProfile, () => (openProfile.value = false))

const items = ref([
  {
    label: t('sidebar.user.home'),
    icon: 'pi pi-home',
    command: () => navigate('/home')
  },
  {
    label: t('sidebar.user.bookingHistory'),
    icon: 'pi pi-history',
    items: [
      {
        label: t('sidebar.user.payment-history'),
        icon: 'pi pi-dollar',
        command: () => navigate('/payment-history')
      },
      {
        label: t('sidebar.user.booking-history'),
        icon: 'pi pi-list-check',
        command: () => navigate('/booking-history')
      }
    ]
  },
  {
    label: t('sidebar.user.favorite-field'),
    icon: 'pi pi-heart',
    command: () => navigate('/favorite-fields')
  },
  {
    label: t('sidebar.user.find-opponent'),
    icon: 'pi pi-users',
    command: () => navigate('/find-opponent')
  }
])

const navigate = (to: string) => {
  router.push('/user' + to)
  openProfile.value = false
}

const checkLocationStatus = () => {
  if ('geolocation' in navigator) {
    navigator.geolocation.getCurrentPosition(
      () => {
        locationStore.enableLocation = true
      },
      (error) => {
        locationStore.enableLocation = false
      },
      { timeout: 5000 }
    )
  } else {
    locationStore.enableLocation = false
  }
}

onMounted(() => {
  checkLocationStatus()
})

const handleScroll = () => {
  isScrolled.value = window.scrollY > 0
}

onMounted(() => {
  window.addEventListener('scroll', handleScroll)
  checkLocationStatus()
})

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll)
})
</script>

<style scoped>
.bg-\[\#f5f5f5\] {
  transition: background-color 0.3s ease;
}
</style>
