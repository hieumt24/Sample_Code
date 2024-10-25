<template>
  <!-- <FieldOwnerHeader /> -->
  <div class="min-w-full flex relative mt-12 mb-10 pb-20">
    <FieldOwnerSideBar />
    <div class="overflow-auto p-10 lg:px-32 w-full">
      <!-- <BreadScrums /> -->
      <div class="px-2">
        <RouterView />
      </div>
    </div>
  </div>
  <ScrollTop />

  <!-- <FieldOwnerFooter /> -->
</template>

<script setup lang="ts">
import FieldOwnerHeader from '@/components/headers/FieldOwnerHeader.vue'
import FieldOwnerFooter from '@/components/foodters/FieldOwnerFooter.vue'
import FieldOwnerSideBar from '@/components/sidebars/FieldOwnerSideBar.vue'
import BreadScrums from '@/components/buttons/BreadScrums.vue'
import ScrollTop from 'primevue/scrolltop'
</script>
