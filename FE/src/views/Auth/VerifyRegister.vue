<template>
  <div class="bg-gradient-to-r to-green-200 flex h-screen items-center justify-center">
    <div v-if="!errorMessage && !isSuccessful">
      <h1>Đang xác thực mã</h1>
      <ProgressBar mode="indeterminate" style="height: 6px"></ProgressBar>
    </div>
    <div v-if="isSuccessful">
      <div class="bg-white rounded-lg shadow-xl p-8 max-w-md w-full">
        <div class="text-center">
          <svg
            class="mx-auto h-12 w-12 text-green-500"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M5 13l4 4L19 7"
            />
          </svg>
          <h2 class="mt-4 text-xl font-bold text-gray-900">MatchFinder thông báo</h2>
          <p class="mt-2 text-gray-600">Tài khoản của bạn đã được kích hoạt thành công</p>
        </div>
        <div class="mt-8">
          <ActionButton
            class="w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 px-4 rounded-lg transition duration-300 ease-in-out transform hover:scale-105"
            value="Đăng nhập ngay"
            @click.prevent="$router.push('/auth')"
          />
        </div>
      </div>
    </div>
    <div v-if="errorMessage" class="bg-white rounded-lg shadow-xl p-8 max-w-md w-full">
      <div class="text-center">
        <svg
          class="mx-auto h-12 w-12 text-red-500"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
          />
        </svg>
        <h2 class="mt-4 text-xl font-bold text-gray-900">MatchFinder thông báo</h2>
        <p class="mt-2 text-gray-600">{{ errorMessage }}</p>
      </div>
      <div class="mt-8">
        <ActionButton
          class="w-full bg-blue-500 hover:bg-blue-600 text-white font-semibold py-2 px-4 rounded-lg transition duration-300 ease-in-out transform hover:scale-105"
          value="Quay lại trang chủ"
          @click.prevent="$router.push('/')"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import ActionButton from '@/components/buttons/ActionButton.vue'
import ProgressBar from 'primevue/progressbar'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const isSuccessful = ref(false)

const errorMessage = ref('')

if (typeof route.params.token === 'string' && typeof route.params.userId === 'string') {
  authStore.activeAccount(route.params.token, Number(route.params.userId)).then((response) => {
    if (response.success) {
      isSuccessful.value = true
    } else {
      errorMessage.value = response.message
    }
  })
}
</script>
