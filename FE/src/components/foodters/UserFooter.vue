<script setup lang="ts">
import { getCurrentDate } from '@/utils/dateUtil'
</script>

<template>
  <div class="bg-[#1C2A30]">
    <div
      class="justify-between flex flex-col lg:flex-row w-full h-52 lg:h-28 border-t border-t-green-400 p-4"
    >
      <div class="flex items-center text-white">
        <div class="flex items-center mr-10">
          <div class="bg-white rounded-lg">
            <img src="/logo.png" alt="" class="h-20 w-20 fill-white" />
          </div>
          <div class="ml-2">
            <span class="te xt-3xl flex items-center text-xl font-bold">MatchFinder</span>
            <h1>{{ getCurrentDate().substring(0, 4) }}</h1>
          </div>
        </div>
        <div>
          <img src="/payos.svg" alt="MatchFinder Logo" class="h-10 object-contain" />
        </div>
      </div>
      <div class="flex">
        <div class="text-white flex flex-col items-end justify-center">
          <span class="my-1 flex items-center">
            <i class="pi pi-envelope mr-2" style="font-size: 1rem" />
            matchfinder.center@gmail.com</span
          >

          <span class="my-1"
            ><i class="pi pi-phone mr-2" style="font-size: 1rem" /> 0335 633 402</span
          >
        </div>
        <div class="h-full flex flex-col">
          <div class="h-12 w-32 flex items-center">
            <i
              class="pi pi-facebook mx-auto ml-4 text-blue-600 rounded-full border-2 border-white"
              style="font-size: 1.7rem"
            ></i>
            <span class="font-semibold text-white">Facebook</span>
          </div>
          <div
            class="h-14 w-32 flex items-center cursor-pointer"
            to="https://www.facebook.com/ji.nguyenhong"
          >
            <i
              class="pi pi-instagram mx-auto ml-4 bg-gradient-to-r from-blue-600 via-green-500 to-indigo-400 inline-block text-transparent bg-clip-text rounded-xl border-2 border-white"
              style="font-size: 1.7rem"
            ></i>
            <span class="font-semibold text-white">Instagram</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.bg-guest {
  background-image: url(../../assets/imgs/logo.png);
}
</style>
