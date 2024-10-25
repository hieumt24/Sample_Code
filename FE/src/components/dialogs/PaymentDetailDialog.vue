<template>
  <Dialog
    :visible="open"
    :header="'Chi tiết thanh toán'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[800px] mx-20"
  >
    <div class="relative w-full flex items-center">
      <div
        v-if="paymentStoreUser.loadingDetail"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <div class="flex flex-col mr-5">
        <div class="flex flex-col justify-center">
          <p v-if="paymentStoreUser.myPaymentDetail" class="text-sm text-gray-600 mb-2">
            <strong>Số tiền: </strong>
            <span
              :class="{
                'text-xl': true,
                ' text-green-500':
                  paymentStoreUser.myPaymentDetail?.status === PaymentStatus.SUCCESSFUL &&
                  paymentStoreUser.myPaymentDetail.amount > 0,
                ' text-red-500': paymentStoreUser.myPaymentDetail?.status === PaymentStatus.FAILED
              }"
              >{{ paymentStoreUser.myPaymentDetail.amount > 0 ? '+' : ''
              }}{{ paymentStoreUser.myPaymentDetail.amount.toLocaleString() }} VNĐ</span
            >
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Loại thanh toán: </strong>

            {{ t(`paymentType.${paymentStoreUser.myPaymentDetail?.type}`) }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Trạng thái: </strong>
            {{ t(`paymentStatus.${paymentStoreUser.myPaymentDetail?.status}`) }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Người gửi: </strong> {{ paymentStoreUser.myPaymentDetail?.userName }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Người nhận: </strong>{{ paymentStoreUser.myPaymentDetail?.reciverName }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Nội dung: </strong>{{ paymentStoreUser.myPaymentDetail?.description }}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Ngày tạo: </strong
            >{{ formatDate(paymentStoreUser.myPaymentDetail?.createdAt ?? '') }}
          </p>
          <p
            v-if="paymentStoreUser.myPaymentDetail?.lastUpdatedAt"
            class="text-sm text-gray-600 mb-2"
          >
            <strong>Cập nhật: </strong
            >{{ formatDate(paymentStoreUser.myPaymentDetail?.lastUpdatedAt ?? '') }}
          </p>
        </div>
      </div>
    </div>
    <div
      v-if="
        paymentStoreUser.myPaymentDetail?.status === PaymentStatus.PENDING &&
        paymentStoreUser.myPaymentDetail.paymentLink
      "
      class="my-2 mt-5 border-t border-gray-300 pt-4"
    >
      <div class="grid grid-cols-2 gap-2">
        <Button @click.stop="cancelPaymentLink" severity="danger" outlined label="Huỷ thanh toán" />
        <ActionButton
          @click.stop="redirectToPaymentLink"
          class="bg-green-400"
          is-outlined
          value="Thanh toán"
        />
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { watch } from 'vue'
import { useToast } from 'primevue/usetoast'
import { usePaymentStoreUser } from '@/stores/paymentStoreUser'
import { formatDate } from '@/utils/dateUtil'
import { PaymentStatus } from '@/constants/payment'
import Button from 'primevue/button'
import ProgressSpinner from 'primevue/progressspinner'

const { t } = useI18n()
const toast = useToast()
const paymentStoreUser = usePaymentStoreUser()

const props = defineProps<{
  open: boolean
  paymentId: number
}>()

const emit = defineEmits<(e: 'close') => void>()

watch(
  () => props.open,
  async (value) => {
    if (value) {
      await paymentStoreUser.getDetailPayment(props.paymentId)
    }
  }
)

const redirectToPaymentLink = () => {
  if (paymentStoreUser.myPaymentDetail?.paymentLink) {
    window.location.href = paymentStoreUser.myPaymentDetail?.paymentLink
  } else {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Không tìm thấy đường dẫn thanh toán',
      life: 3000
    })
  }
}

const cancelPaymentLink = async () => {
  if (
    paymentStoreUser.myPaymentDetail?.paymentLink &&
    paymentStoreUser.myPaymentDetail?.status === PaymentStatus.PENDING
  ) {
    const response = await paymentStoreUser.cancelPaymentLink(paymentStoreUser.myPaymentDetail.id)
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Success',
        detail: 'Hủy thanh toán thành công',
        life: 3000
      })
      emit('close')
    } else {
      toast.add({
        severity: 'error',
        summary: 'Error',
        detail: 'Hủy thanh toán thất bại',
        life: 300
      })
    }
  } else {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Không tìm thấy đường dẫn thanh toán',
      life: 3000
    })
  }
}
</script>
