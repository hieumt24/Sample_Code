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
              class="items-center text-sm space-x-2 w-32 p-1.5 rounded-md font-semibold"
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
    <div v-if="staffStore.staff" class="my-2 mt-5 border-t border-gray-300 pt-4">
      <div class="grid gap-2">
        <Button
          v-if="staffStore.staff?.isActive"
          @click.stop="deactivateAccountConfirm"
          severity="danger"
          outlined
          label="Tắt hoạt động"
        />
        <Button v-else @click.stop="activateAccountConfirm" label="Bật hoạt động" />
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
import Button from 'primevue/button'
import { StaffStatus } from '@/constants/staff'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'

const { t } = useI18n()
const toast = useToast()
const confirm = useConfirm()
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

const activateAccount = async () => {
  if (staffStore.staff) {
    detailLoading.value = true
    const response = await staffStore.changeStaffStatus(true)
    if (response?.success) {
      toast.add({ severity: 'success', detail: 'Kích hoạt tài khoản thành công!', life: 3000 })
    } else {
      toast.add({ severity: 'error', detail: 'Kích hoạt tài khoản không thành công!', life: 3000 })
    }
    detailLoading.value = false
  }
  emit('close')
}
const deactivateAccount = async () => {
  if (staffStore.staff) {
    detailLoading.value = true
    const response = await staffStore.changeStaffStatus(false)
    if (response?.success) {
      toast.add({
        severity: 'success',
        detail: 'Ngừng hoạt động của tài khoản thành công!',
        life: 3000
      })
    } else {
      toast.add({
        severity: 'error',
        detail: 'Ngừng hoạt động của tài khoản không thành công!',
        life: 3000
      })
    }
    detailLoading.value = false
  }
  emit('close')
}
const activateAccountConfirm = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn muốn kích hoạt tài khoản này không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await activateAccount()
    }
  })
}
const deactivateAccountConfirm = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn muốn ngừng hoạt động của tài khoản này không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await deactivateAccount()
    }
  })
}
</script>
