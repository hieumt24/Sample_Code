<template>
  <div class="flex justify-center items-center bg-gray-100">
    <div class="relative bg-white p-8 rounded-lg shadow-md max-w-md w-full">
      <h1 class="text-2xl font-bold text-center mb-6">Nạp tiền</h1>
      <div
        v-if="paymentStoreUser.loadingDetail"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <div v-else>
        <div v-if="paymentStoreUser.myPaymentDetail?.status === PaymentStatus.SUCCESSFUL">
          <div class="mb-4">
            <Message severity="success" :closable="false"> Đã nạp tiền thành công! </Message>
          </div>
          <div class="w-full flex items-center">
            <div class="flex flex-col mr-5">
              <div class="flex flex-col justify-center">
                <p v-if="paymentStoreUser.myPaymentDetail" class="text-sm text-gray-600 mb-2">
                  <strong>Số tiền: </strong>
                  <span class="text-xl text-green-500"
                    >{{ paymentStoreUser.myPaymentDetail.amount.toLocaleString() }} VNĐ</span
                  >
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
        </div>
        <div v-else>
          <div class="mb-4">
            <Message severity="info" :closable="false">
              Giao dịch đang xử lý. Vui lòng thử lại sau vài giây!</Message
            >
          </div>
        </div>
      </div>
      <div
        class="mt-6"
        v-if="paymentStoreUser.myPaymentDetail?.status !== PaymentStatus.SUCCESSFUL"
      >
        <Button
          label="Tải lại"
          severity="secondary"
          outlined
          class="w-full"
          @click="getDetailPayment"
        />
      </div>
      <div class="mt-6">
        <Button label="Về trang chủ" class="w-full" @click="goToHome" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { PaymentStatus } from '@/constants/payment'
import { usePaymentStoreUser } from '@/stores/paymentStoreUser'
import { formatDate } from '@/utils/dateUtil'
import Button from 'primevue/button'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'
import { useToast } from 'primevue/usetoast'
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import type { LocationQueryValue } from 'vue-router'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()

const paymentStoreUser = usePaymentStoreUser()

const orderCode = ref('')

onMounted(async () => {
  orderCode.value = getQueryParamAsString(route.query.orderCode)
  await getDetailPayment()
})

const getQueryParamAsString = (param: LocationQueryValue | LocationQueryValue[]): string => {
  if (Array.isArray(param)) {
    return param[0] || ''
  }
  return param || ''
}
const getDetailPayment = async () => {
  const transactionId = Number(orderCode.value)
  if (!transactionId) return
  await paymentStoreUser.getDetailPayment(transactionId)
}

const goToHome = () => {
  router.push('/user/home')
}
</script>
