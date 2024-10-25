<template>
  <div class="flex flex-wrap w-full">
    <div class="">
      <SearchTab @search="onSearch" @reset="onReset" class="mb-4">
        <Dropdown
          v-model="selectedField"
          :options="listFields"
          optionLabel="name"
          placeholder="Chọn sân"
          class="w-full md:w-[14rem] my-2"
          @change="onChangeField"
        />
        <hr class="mt-2" />
        <CustomCalendar
          name="fromDate"
          class="col-span-1 w-full"
          label="Ngày"
          v-model="paymentStoreOwner.searchPayment.date"
        />
      </SearchTab>
    </div>

    <div class="flex-1 lg:px-10">
      <div class="bg-white p-5 flex flex-col shadow-md rounded-md">
        <div>
          <span class="font-semibold text-green-500">Số dư của bạn: </span>
          <span class="text-base"
            >{{ Math.floor(paymentStoreOwner.balance).toLocaleString() }} VNĐ</span
          >
          <div class="text-gray-600 mb-2">{{ balanceText }}</div>
        </div>
        <Button class="w-60" label="Yêu cầu thanh toán" />
      </div>
      <h1 class="font-semibold text-lg mt-5">Lịch sử thanh toán</h1>
      <div class="">
        <CustomTable
          :headers="headers"
          :total="paymentStoreOwner.total"
          :loading="paymentStoreOwner.loading"
          :items="
            paymentStoreOwner.myPayments.map((payment) => ({
              ...payment,
              paymentType: payment.type,
              createdAt: formatDateVietnamese(payment.createdAt),
              paymentStatus: payment.status
            }))
          "
          @detail="onDetail"
          is-payment
          no-delete
          no-edit
          @change-page="paymentStoreOwner.changePagePaymentHistory"
        />
      </div>
    </div>
  </div>

  <PaymentDetailDialog
    :open="isOpenPaymentDetail"
    :payment-id="selectedPaymentId ?? 0"
    @close="isOpenPaymentDetail = false"
  />
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import CustomTable from '@/components/tables/CustomTable.vue'
import SearchTab from '@/components/search/SearchTab.vue'
import Dropdown from 'primevue/dropdown'
import type { DropdownType } from '@/constants/types'
import { useFieldStore } from '@/stores/fieldStore'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import { useI18n } from 'vue-i18n'
import { usePaymentStoreOwner } from '@/stores/paymentStoreOwner'
import Button from 'primevue/button'
import PaymentDetailDialog from '@/components/dialogs/PaymentDetailDialog.vue'
import { formatDateVietnamese } from '@/utils/dateUtil'
const { t } = useI18n()
import {
  InvalidFormatError,
  InvalidNumberError,
  NotEnoughUnitError,
  ReadingConfig,
  doReadNumber
} from 'read-vietnamese-number'

const layoutStore = useLayoutStore()
const fieldStore = useFieldStore()
const paymentStoreOwner = usePaymentStoreOwner()

const isOpenPaymentDetail = ref(false)
const listFields = ref<DropdownType[]>([])
const selectedField = ref<DropdownType>()
const selectedPaymentId = ref<number>()
const balanceText = ref('')

const onDetail = async (id: number) => {
  selectedPaymentId.value = id
  isOpenPaymentDetail.value = true
}

const onSearch = async () => {
  await paymentStoreOwner.getMyPayments()
}

const onReset = async () => {
  paymentStoreOwner.searchPayment = {
    date: undefined,
    fieldId: undefined,
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  onSearch()
}

const onChangeField = async () => {
  paymentStoreOwner.searchPayment = {
    date: undefined,
    fieldId: Number(selectedField.value?.code),
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  await onSearch()
}

const headers = [
  { field: 'createdAt', header: 'Thời gian' },
  { field: 'userName', header: 'Người gửi' },
  { field: 'reciverName', header: 'Người nhận' }
]

onMounted(async () => {
  await fieldStore.getOwnerFieldList()
  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))

  if (listFields.value.length > 0) {
    selectedField.value = listFields.value[0]
    paymentStoreOwner.searchPayment.fieldId = Number(selectedField.value.code)
  }

  layoutStore.breadcrumb = [{ label: 'My schedule' }]

  await paymentStoreOwner.getMyPayments()
  const balance = await paymentStoreOwner.getCurrentBalance()
  const config = new ReadingConfig()
  config.unit = ['đồng']
  try {
    const result = doReadNumber(config, balance.data + '')
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
