<template>
  <div class="fixed z-30 top-0 justify-between flex w-full h-16 border-b bg-white">
    <div class="flex items-center cursor-pointer" @click="$router.push('/')">
      <div class="h-full w-16 bg-cover bg-guest"></div>
      <span class="te xt-3xl flex items-center font-extrabold text-xl">MatchFinder</span>
    </div>
    <!-- ball image -->
    <img src="/ball.png" alt="ball" class="w-5 h-5 absolute bottom-2 ball-animation" />

    <span
      v-if="!authStore.identity"
      class="text-center hidden lg:flex h-full items-center font-semibold"
      >{{ $t('common.hello') }}</span
    >

    <div class="flex items-center">
      <div class="flex items-center">
        <span class="mx-2 text-green-600 cursor-pointer hover:underline">Chính sách</span>
        <span class="mx-2 text-green-600 cursor-pointer hover:underline">Chúng tôi</span>
      </div>
      <ActionButton
        class="mr-2 h-8"
        value="Đăng nhập"
        @click="$router.push({ name: 'login-screen' })"
      />
      <img src="/goal.png" alt="ball" class="w-20" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import ActionButton from '../buttons/ActionButton.vue'
const authStore = useAuthStore()
</script>

<style scoped>
.bg-guest {
  background-image: url(../../assets/imgs/logo.png);
}
.ball-animation {
  animation: unsteady-roll-back-forth 10s infinite linear;
  transform-origin: center;
}

@keyframes unsteady-roll-back-forth {
  0%,
  100% {
    transform: translateX(-2.5rem) translateY(0) rotate(0deg);
  }
  10%,
  90% {
    transform: translateX(10vw) translateY(-10px) rotate(180deg);
  }
  25%,
  75% {
    transform: translateX(25vw) translateY(8px) rotate(450deg);
  }
  40%,
  60% {
    transform: translateX(40vw) translateY(-10px) rotate(720deg);
  }
  50% {
    transform: translateX(calc(100vw - 2.5rem)) translateY(0) rotate(900deg);
  }
}
</style>
