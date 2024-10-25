<template>
  <Dialog
    :visible="open"
    :header="fieldStoreAdmin.field?.name"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[800px] mx-20"
  >
    <div class="relative w-full flex items-center">
      <div
        v-if="loading"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <div class="flex flex-col mr-5">
        <div class="flex flex-col justify-center">
          <p class="text-sm text-gray-600 mb-2">
            <strong>Trạng thái: </strong>
            <span
              class="items-center text-sm space-x-2 w-32 p-1.5 rounded-md font-semibold"
              :class="{
                'bg-lime-200 text-lime-600': fieldStoreAdmin.field?.status === FieldStatus.ACCEPTED,
                'bg-red-200 text-red-600': fieldStoreAdmin.field?.status === FieldStatus.REJECTED,
                'bg-blue-200 text-blue-600': fieldStoreAdmin.field?.status === FieldStatus.WAITING,
                'bg-gray-200 text-gray-600': fieldStoreAdmin.field?.status === FieldStatus.INACTIVE
              }"
            >
              {{ $t(`fieldStatus.${fieldStoreAdmin.field?.status}`) as string }}
            </span>
          </p>

          <p class="text-sm text-gray-600 mb-2">
            <strong>Chủ sân: </strong>
            {{ fieldStoreAdmin.field?.ownerName }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>SĐT: </strong>
            {{ fieldStoreAdmin.field?.phoneNumber }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Địa chỉ: </strong>
            {{ fieldStoreAdmin.field?.address }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Phường Xã: </strong>
            {{ fieldStoreAdmin.field?.commune }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Quận Huyện: </strong>
            {{ fieldStoreAdmin.field?.district }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Tỉnh thành: </strong>
            {{ fieldStoreAdmin.field?.province }}
          </p>
          <router-link :to="`/admin/fields/${fieldStoreAdmin.field?.id}`" target="_blank">
            <Button severity="Secondary" link label="Xem thêm" />
          </router-link>
        </div>
      </div>
    </div>
    <div
      v-if="fieldStoreAdmin.field?.status === FieldStatus.WAITING"
      class="my-2 mt-5 border-t border-gray-300 pt-4"
    >
      <div class="grid grid-cols-2 gap-2">
        <Button @click.stop="rejectConfirm" severity="danger" outlined label="Từ chối" />
        <ActionButton @click.stop="approveConfirm" is-outlined value="Chấp nhận" />
      </div>
    </div>
    <div
      v-if="fieldStoreAdmin.field?.status === FieldStatus.ACCEPTED"
      class="my-2 mt-5 border-t border-gray-300 pt-4"
    >
      <div class="grid grid-cols-1 gap-2">
        <Button @click.stop="rejectConfirm" severity="danger" outlined label="Dừng hoạt động" />
      </div>
    </div>
    <div
      v-if="fieldStoreAdmin.field?.status === FieldStatus.REJECTED"
      class="my-2 mt-5 border-t border-gray-300 pt-4"
    >
      <div class="grid grid-cols-1 gap-2">
        <ActionButton @click.stop="approveConfirm" is-outlined value="Kích hoạt hoạt động" />
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { ref, watch } from 'vue'
import { useToast } from 'primevue/usetoast'
import { formatDate } from '@/utils/dateUtil'
import { PaymentStatus } from '@/constants/payment'
import Button from 'primevue/button'
import ProgressSpinner from 'primevue/progressspinner'
import { useFieldStoreAdmin } from '@/stores/fieldStoreAdmin'
import { FieldStatus } from '@/constants/field'
import { useConfirm } from 'primevue/useconfirm'

const { t } = useI18n()
const toast = useToast()
const confirm = useConfirm()
const fieldStoreAdmin = useFieldStoreAdmin()

const props = defineProps<{
  open: boolean
  fieldId: number
}>()

const emit = defineEmits<(e: 'close') => void>()
const loading = ref(false)
watch(
  () => props.open,
  async (value) => {
    if (value) {
      await fieldStoreAdmin.getFieldById(props.fieldId)
    }
  }
)
const rejectConfirm = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await onReject()
    }
  })
}
const approveConfirm = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await onApprove()
    }
  })
}
const onReject = async () => {
  loading.value = true
  let response = await fieldStoreAdmin.changeFieldStatus(FieldStatus.REJECTED)
  loading.value = false
  if (response?.success) {
    toast.add({
      severity: 'success',
      summary: 'Từ chối thành công',
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
const onApprove = async () => {
  loading.value = true
  let response = await fieldStoreAdmin.changeFieldStatus(FieldStatus.ACCEPTED)
  loading.value = false
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
