<template>
  <div class="bg-cover pt-32 bg-guest flex items-center">
    <div v-if="mode === 'login'" class="w-96 py-10 mx-auto h-full">
      <LoginForm @change-mode="changeMode" />
    </div>
    <div v-if="mode === 'forgot'" class="w-96 py-10 mx-auto h-full">
      <ForgotPasswordForm @change-mode="changeMode" />
    </div>
    <div v-if="mode === 'register'" class="w-96 py-10 mx-auto h-full">
      <RegisterForm @change-mode="changeMode" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import LoginForm from '@/components/forms/LoginForm.vue'
import RegisterForm from '@/components/forms/RegisterForm.vue'
import ForgotPasswordForm from '@/components/forms/ForgotPasswordForm.vue'

const mode = ref<'login' | 'forgot' | 'register'>('login')

const changeMode = (to: 'login' | 'forgot' | 'register') => {
  mode.value = to
}
</script>

<style scoped>
.bg-guest {
  background-image: url(/bg-guest.jpg);
  height: 100vh;
}
</style>
