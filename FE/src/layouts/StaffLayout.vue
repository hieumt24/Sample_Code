<template>
  <div class="min-w-full flex relative mt-12 mb-10 pb-20">
    <StaffSideBar />
    <div class="overflow-auto p-10 lg:px-32 w-full">
      <div class="px-2">
        <RouterView />
      </div>
    </div>
  </div>
  <ScrollTop />
</template>

<script setup lang="ts">
import StaffSideBar from '@/components/sidebars/StaffSideBar.vue'
import ScrollTop from 'primevue/scrolltop'
</script>
