<template>
  <div class="container mx-auto px-4 pb-8">
    <h1 class="text-3xl font-bold mb-4">Lịch sử giao dịch</h1>

    <div class="flex flex-col lg:flex-row gap-8">
      <!-- Left Column: Balance and Search -->
      <div class="lg:w-1/3">
        <!-- Balance Card -->
        <div class="relative bg-white p-6 rounded-lg shadow-md mb-6">
          <div
            v-if="paymentStore.requestingGetBalance"
            class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
          >
            <ProgressSpinner strokeWidth="4" />
          </div>
          <h2 class="text-xl font-semibold mb-2">Số dư của bạn</h2>
          <div class="text-3xl font-bold text-green-600 mb-2">
            {{ Math.floor(paymentStore.balance).toLocaleString() }} VNĐ
          </div>
          <div class="text-gray-600">{{ balanceText }}</div>
          <Button
            class="w-full mt-4"
            label="Nạp thêm"
            severity="success"
            @click="isOpenRechargeDialog = true"
          />
        </div>

        <!-- Search Card -->
        <div class="bg-white p-6 rounded-lg shadow-md">
          <h2 class="text-xl font-semibold mb-4">Tìm kiếm</h2>
          <div class="flex flex-col">
            <div class="w-56 p-2">
              <CustomCalendar label="Ngày" v-model="paymentStore.searchPayment.date" maxDateToday />
            </div>
            <div class="flex gap-4 w-full p-2">
              <Button
                icon="pi pi-undo"
                label="Đặt lại"
                class="bg-green-400 w-56"
                outlined
                @click="onReset"
              />
              <Button icon="pi pi-search" label="Tìm" class="bg-green-400 w-56" @click="onSearch" />
            </div>
          </div>
        </div>
      </div>

      <!-- Right Column: Payment History -->
      <div class="lg:w-2/3">
        <div class="bg-white rounded-lg shadow-md">
          <div class="relative p-4 border-b flex justify-between items-center">
            <h2 class="text-xl font-semibold">Danh sách</h2>

            <div>
              <PagingElement
                :limit="paymentStore.searchPayment.limit"
                :total="paymentStore.total"
                @change-page="paymentStore.changePagePaymentHistory"
              />
            </div>
            <div
              v-if="paymentStore.loading"
              class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
            >
              <ProgressSpinner strokeWidth="4" />
            </div>
          </div>

          <div
            v-if="paymentStore.myPayments.length === 0 && !paymentStore.loading"
            class="flex justify-center items-center p-8"
          >
            <p>Không có dữ liệu</p>
          </div>

          <ul class="divide-y divide-gray-200">
            <li
              v-for="payment in paymentStore.myPayments"
              :key="payment.id"
              @click="onPaymentDetail(payment.id)"
              class="p-4 hover:bg-gray-100 cursor-pointer transition duration-150 ease-in-out"
            >
              <div class="flex justify-between items-center">
                <div>
                  <p class="font-medium">{{ t(`paymentType.${payment.type}`) }}</p>
                  <p class="text-sm text-gray-500">
                    {{
                      payment.description.length > 60
                        ? payment.description.slice(0, 60) + '...'
                        : payment.description
                    }}
                  </p>
                </div>
                <div class="text-right">
                  <p
                    class="font-medium"
                    :class="{
                      'text-gray-500':
                        payment?.status === PaymentStatus.PENDING ||
                        payment?.status === PaymentStatus.CANCELLED,
                      ' text-green-500':
                        payment?.status === PaymentStatus.SUCCESSFUL &&
                        payment.type !== PaymentType.BOOKING,
                      ' text-red-500': payment?.status === PaymentStatus.FAILED
                    }"
                  >
                    {{ payment.type === PaymentType.BOOKING ? '-' : '+'
                    }}{{ payment.amount.toLocaleString() }} VNĐ
                    <span v-if="payment?.status !== PaymentStatus.SUCCESSFUL" class="font-medium">
                      | {{ t(`paymentStatus.${payment.status}`) }}
                    </span>
                  </p>
                  <p class="text-sm text-gray-500">{{ formatDate(payment.createdAt ?? '') }}</p>
                </div>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>

  <PaymentDetailDialog
    :open="isOpenPaymentDetail"
    :payment-id="selectedPaymentId"
    @close="isOpenPaymentDetail = false"
  />
  <RechargeDialog
    :open="isOpenRechargeDialog"
    @close="isOpenRechargeDialog = false"
  ></RechargeDialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import PagingElement from '@/components/pagination/PagingElement.vue'
import { onMounted } from 'vue'
import { usePaymentStoreUser } from '@/stores/paymentStoreUser'
import PaymentDetailDialog from '@/components/dialogs/PaymentDetailDialog.vue'
import Button from 'primevue/button'
import {
  InvalidFormatError,
  InvalidNumberError,
  NotEnoughUnitError,
  ReadingConfig,
  doReadNumber
} from 'read-vietnamese-number'
import { formatDate, formatDateVietnamese } from '@/utils/dateUtil'
import { PaymentStatus, PaymentType } from '@/constants/payment'
import RechargeDialog from '@/components/dialogs/RechargeDialog.vue'
import ProgressSpinner from 'primevue/progressspinner'

const { t } = useI18n()

const paymentStore = usePaymentStoreUser()

const isOpenPaymentDetail = ref(false)
const isOpenRechargeDialog = ref(false)
const selectedPaymentId = ref(0)
const balanceText = ref('')

const onPaymentDetail = (PaymentId: number) => {
  selectedPaymentId.value = PaymentId
  isOpenPaymentDetail.value = true
}

const onReset = () => {
  paymentStore.searchPayment.date = undefined
  onSearch()
}

const onSearch = () => {
  paymentStore.getMyPayments()
}

onMounted(async () => {
  paymentStore.getMyPayments()
  await paymentStore.getCurrentBalance()
  const config = new ReadingConfig()
  config.unit = ['đồng']
  try {
    const result = doReadNumber(config, paymentStore.balance + '')
    balanceText.value = result.charAt(0).toUpperCase() + result.slice(1)
  } catch (err) {
    if (err instanceof InvalidFormatError) {
      console.error('Định dạng input không hợp lệ')
    } else if (err instanceof InvalidNumberError) {
      console.error('Số không hợp lệ')
    } else if (err instanceof NotEnoughUnitError) {
      console.error('Không đủ đơn vị đọc số')
    }
  }
})
</script>
