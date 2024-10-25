<template>
  <TabView @update:active-index="onChangeTab">
    <TabPanel header="Người dùng"> </TabPanel>
    <TabPanel header="Đặt sân"> </TabPanel>
    <TabPanel header="Sân bóng"> </TabPanel>
  </TabView>

  <div class="px-2 bg-white h-screen">
    <RouterView />
  </div>
</template>
<script setup lang="ts">
import TabView from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import { useRouter } from 'vue-router'
const router = useRouter()

const onChangeTab = (index: number) => {
  if (index === 0) {
    router.push('/admin/statistic/users')
  } else if (index === 1) {
    router.push('/admin/statistic/bookings')
  }
}
</script>
