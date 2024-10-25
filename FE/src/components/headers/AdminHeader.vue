<template>
  <div
    class="fixed z-30 top-0 justify-between flex w-full h-20 border-b border-orange-400 bg-red-50"
  >
    <div class="flex">
      <div class="h-full w-20 bg-cover bg-guest"></div>
      <Span class="text-3xl flex items-center">MatchFinder</Span>
    </div>
    <div class="h-full flex items-center px-10">
      <span class="text-center flex h-full items-center font-semibold pr-2">{{
        $t('common.language')
      }}</span>
      <select class="px-4 h-10 font-bold border border-gray-200" v-model="$i18n.locale">
        <option v-for="(lang, i) in langs" :key="`Lang${i}`" :value="lang">
          {{ lang }}
        </option>
      </select>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'

const langs = ['vn', 'en']

const authStore = useAuthStore()
</script>

<style scoped>
.bg-guest {
  background-image: url(../../assets/imgs/logo.png);
}
</style>
