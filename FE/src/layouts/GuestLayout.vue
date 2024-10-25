<template>
  <UserHeader />
  <div class="min-w-full mb-20 flex flex-col min-h-screen">
    <div
      class="w-full flex-1 mb-20"
      :class="{
        'lg:px-36 mt-32':
          $router.currentRoute.value.name !== 'guest-home' &&
          $router.currentRoute.value.name !== 'login-screen',
        'xl:px-0 ': $router.currentRoute.value.name === 'guest-home'
      }"
    >
      <RouterView class="h-full" />
    </div>
    <UserFooter class="" />
  </div>
  <ScrollTop />
</template>

<script setup lang="ts">
import UserFooter from '@/components/foodters/UserFooter.vue'
import UserHeader from '@/components/headers/UserHeader.vue'
import { useAuthStore } from '@/stores/authStore'
import ScrollTop from 'primevue/scrolltop'

const authStore = useAuthStore()
</script>
