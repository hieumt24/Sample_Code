<template>
  <Dialog
    :visible="open"
    :header="'Thông tin nhân viên'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[600px] mx-20"
  >
    <div class="relative w-full flex items-center">
      <div
        v-if="detailLoading"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <div class="flex flex-col mr-5">
        <div class="flex flex-col justify-center">
          <p class="text-sm text-gray-600 mb-2">
            <strong>Tên nhân viên: </strong>

            {{ staffStore.staff?.name }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Số điện thoại: </strong>
            {{ staffStore.staff?.phoneNumer }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Tài khoản: </strong>

            {{ staffStore.staff?.userName }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Email: </strong> {{ staffStore.staff?.email }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Sân: </strong> {{ staffStore.staff?.fieldName }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Trạng thái: </strong>
            <span
              class="items-center text-sm space-x-2 w-fit p-1.5 rounded-md font-semibold"
              :class="{
                'bg-lime-200 text-lime-600': staffStore.staff?.isActive,
                'bg-red-200 text-red-600': !staffStore.staff?.isActive
              }"
            >
              {{ staffStore.staff?.isActive ? StaffStatus.Active : StaffStatus.NotActive }}</span
            >
          </p>
        </div>
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useI18n } from 'vue-i18n'
import { ref, watch } from 'vue'
import ProgressSpinner from 'primevue/progressspinner'
import { useStaffStore } from '@/stores/staffStore'
import { StaffStatus } from '@/constants/staff'

const { t } = useI18n()
const staffStore = useStaffStore()
const detailLoading = ref(false)
const props = defineProps<{
  open: boolean
  staffId: number
  fieldId: number
}>()

const emit = defineEmits<(e: 'close') => void>()

watch(
  () => props.open,
  async (value) => {
    if (value) {
      detailLoading.value = true
      await staffStore.getStaffById(props.staffId, props.fieldId)
      detailLoading.value = false
    }
  }
)
</script>
