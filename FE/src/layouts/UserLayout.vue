<template>
  <UserHeader v-if="!authStore.identity" />
  <UserSideBar />

  <div
    class="min-w-full mb-20"
    :class="{
      'mt-24': authStore.identity && $router.currentRoute.value.name !== 'user-home',
      'mt-32': !authStore.identity
    }"
  >
    <div
      class="w-full h-full min-h-screen"
      :class="{
        'lg:px-36': $router.currentRoute.value.name !== 'user-home',
        'xl:px-0': $router.currentRoute.value.name === 'user-home'
      }"
    >
      <RouterView />
    </div>
  </div>
  <ScrollTop />
  <UserFooter />
</template>

<script setup lang="ts">
import UserFooter from '@/components/foodters/UserFooter.vue'
import UserHeader from '@/components/headers/UserHeader.vue'
import UserSideBar from '@/components/sidebars/UserSideBar.vue'
import { useAuthStore } from '@/stores/authStore'
import ScrollTop from 'primevue/scrolltop'

const authStore = useAuthStore()
</script>
