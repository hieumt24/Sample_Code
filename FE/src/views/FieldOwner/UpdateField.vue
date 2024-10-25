<template>
  <TabView @tab-change="onChangeTab">
    <TabPanel header="Thông tin chung">
      <FieldGeneralInfor :id="fieldId" />
    </TabPanel>
    <TabPanel header="Lịch sân">
      <SlotSetting :is-open="currentTab === 1" :field-id="fieldId" />
    </TabPanel>
    <TabPanel header="Các sân nhỏ">
      <PartialFieldList :is-open="currentTab === 2" />
    </TabPanel>
  </TabView>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import FieldForm from '@/components/forms/FieldForm.vue'
import router from '@/router'
import { useRoute } from 'vue-router'
import TabView, { type TabViewChangeEvent } from 'primevue/tabview'
import TabPanel from 'primevue/tabpanel'
import SlotSetting from '@/components/forms/SlotSetting.vue'
import { ref } from 'vue'
import PartialFieldList from '@/components/forms/PartialFieldList.vue'
import FieldGeneralInfor from '@/components/information/FieldGeneralInfor.vue'

const layoutStore = useLayoutStore()
const route = useRoute()
const fieldId = Number(route.params.id)
const currentTab = ref(0)

onMounted(() => {
  layoutStore.breadcrumb = [{ label: 'My Field' }, { label: 'Update Field' }]
})

const onChangeTab = (event: TabViewChangeEvent) => {
  currentTab.value = event.index
}

const back = () => {
  router.push('/field/list')
}
</script>
