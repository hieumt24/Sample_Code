<template>
  <div class="text-blue-500 mb-2 font-semibold">
    <i class="pi pi-arrow-left"></i>
    <router-link to="/admin/fields">Danh sách sân</router-link>
  </div>
  <div class="p-4 bg-white rounded-[10px]">
    <FieldGeneralInforAdmin :id="fieldId" />

    <div class="py-4 flex justify-evenly">
      <ActionButton
        v-if="fieldStoreAdmin.field?.status == 'WAITING'"
        :value="$t('form.approve')"
        @click="onApprove"
      />

      <ActionButton
        v-if="fieldStoreAdmin.field?.status == 'WAITING'"
        :value="$t('form.reject')"
        @click="onReject"
        severity="danger"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import router from '@/router'
import { useRoute } from 'vue-router'
import { useFieldStore } from '@/stores/fieldStore'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { useToast } from 'primevue/usetoast'
import FieldGeneralInforAdmin from '@/components/information/FieldGeneralInforAdmin.vue'
import { FieldStatus } from '@/constants/field'
import { useFieldStoreAdmin } from '@/stores/fieldStoreAdmin'
const layoutStore = useLayoutStore()
const route = useRoute()
const fieldId = Number(route.params.id)
const fieldStoreAdmin = useFieldStoreAdmin()
const toast = useToast()
onMounted(async () => {
  await fieldStoreAdmin.getFieldById(fieldId)
  layoutStore.breadcrumb = [{ label: 'Edit Field' }]
})

const back = () => {
  router.push('/admin/fields')
}

const onReject = async () => {
  let response = await fieldStoreAdmin.changeFieldStatus(FieldStatus.REJECTED)
  if (response?.success) {
    toast.add({
      severity: 'success',
      summary: 'Từ chối thành công',
      detail: response.message,
      life: 3000
    })
  } else {
    toast.add({
      severity: 'error',
      summary: 'Thao tác không thành công!',
      detail: response?.message,
      life: 3000
    })
  }
}

const onApprove = async () => {
  let response = await fieldStoreAdmin.changeFieldStatus(FieldStatus.ACCEPTED)
  if (response?.success) {
    toast.add({
      severity: 'success',
      summary: 'Chấp nhận thành công',
      life: 3000
    })
  } else {
    toast.add({
      severity: 'error',
      summary: 'Thao tác không thành công!',
      life: 3000
    })
  }
}
</script>
