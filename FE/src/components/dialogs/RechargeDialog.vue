<template>
  <Dialog
    :visible="open"
    :header="'Nạp tiền'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
  >
    <div class="first-letter:flex justify-center items-center bg-gray-100">
      <div class="relative w-full bg-white p-6">
        <div
          v-if="paymentStoreUser.requestingAddMoney"
          class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
        >
          <ProgressSpinner strokeWidth="4" />
        </div>
        <form @submit.prevent="submitForm">
          <div class="mb-4">
            <label class="block text-sm font-medium text-gray-700 mb-2"
              >Nhập số tiền cần nạp (VNĐ):</label
            >
            <div class="grid grid-cols-2 gap-2 mb-2">
              <Button
                severity="secondary"
                v-for="amount in presetAmounts"
                :key="amount"
                :label="formatAmountWithVND(amount)"
                :class="{
                  'p-button-outlined': selectedAmount !== amount,
                  'p-button-primary': selectedAmount === amount
                }"
                @click="selectAmount(amount)"
                type="button"
                class="w-full"
              />
            </div>
            <div class="flex items-center gap-2">
              <InputNumber
                v-model="selectedAmount"
                placeholder="Nhập số tiền"
                :min="0"
                :step="1000"
                suffix=" VNĐ"
                class="w-full"
                @input="onInputChange"
              />
            </div>
          </div>
          <Message v-if="errorMessage" :closable="false" severity="warn">
            {{ errorMessage }}
          </Message>
          <Button
            type="submit"
            label="Chuyển đến trang thanh toán"
            class="w-full"
            :disabled="!selectedAmount || !!errorMessage"
          />
        </form>
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import Button from 'primevue/button'
import InputNumber, { type InputNumberInputEvent } from 'primevue/inputnumber'
import { usePaymentStoreUser } from '@/stores/paymentStoreUser'
import ProgressSpinner from 'primevue/progressspinner'
import Dialog from 'primevue/dialog'
import { useToast } from 'primevue/usetoast'
import Message from 'primevue/message'

const props = defineProps<{
  open: boolean
}>()

const emit = defineEmits<(e: 'close') => void>()

const paymentStoreUser = usePaymentStoreUser()

const toast = useToast()

const selectedAmount = ref(0)
const presetAmounts = [25000, 200000, 500000, 1000000]
const errorMessage = ref('')

const formatAmountWithVND = (amount: number) => {
  return (
    new Intl.NumberFormat('vi-VN', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(amount) + ' VNĐ'
  )
}

const selectAmount = (amount: number) => {
  selectedAmount.value = amount
  validateAmount()
}

const onInputChange = (event: InputNumberInputEvent) => {
  selectedAmount.value = Number(event.value)
  validateAmount()
}
const validateAmount = () => {
  if (selectedAmount.value < 10000) {
    errorMessage.value = 'Số tiền phải lớn hơn 10,000VNĐ'
  } else if (selectedAmount.value > 50000000)
    errorMessage.value = 'Số tiền phải nhỏ hơn 50,000,000VNĐ'
  else {
    errorMessage.value = ''
  }
}
const submitForm = async () => {
  console.log('Recharge amount:', selectedAmount.value)
  const response = await paymentStoreUser.addMoney({
    amount: selectedAmount.value
  })
  if (response.success) {
    window.location.href = response.data
  } else {
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: 'Có lỗi xảy ra, vui lòng thử lại',
      life: 3000
    })
  }
}
</script>
