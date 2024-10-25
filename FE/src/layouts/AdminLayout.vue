<template>
  <div class="min-w-full flex">
    <AdminSideBar />
    <div class="overflow-auto w-full p-10 md:px-32 md:pt-24">
      <!-- <BreadScrums /> -->
      <RouterView />
    </div>
  </div>
  <!-- <AdminFooter /> -->
</template>

<script setup lang="ts">
import AdminFooter from '@/components/foodters/AdminFooter.vue'
import AdminHeader from '@/components/headers/AdminHeader.vue'
import AdminSideBar from '@/components/sidebars/AdminSideBar.vue'
import BreadScrums from '@/components/buttons/BreadScrums.vue'
</script>
