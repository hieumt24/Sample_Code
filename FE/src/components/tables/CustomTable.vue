<template>
  <div class="card rounded-lg overflow-hidden w-full border border-gray-200 shadow-md bg-white">
    <DataTable
      lazy
      :loading="loading"
      :totalRecords="total"
      :value="items"
      paginator
      :rows="LIMIT_PER_PAGE"
      @page="onPreviousPage"
      :rowsPerPageOptions="[5, 10, 20, 50]"
      tableStyle="min-width: 50rem"
      paginatorTemplate="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
      currentPageReportTemplate="{first} đến {last} / {totalRecords}"
      class="p-datatable-sm"
      :class="{ 'opacity-50': loading }"
      pt:thead:class="custom-header"
    >
      <template #empty>
        <div class="text-center py-8">
          <i class="pi pi-inbox text-gray-400 text-5xl mb-4"></i>
          <h3 class="text-lg font-medium text-gray-600">
            {{ loading ? 'Đang tải dữ liệu...' : 'Không có dữ liệu' }}
          </h3>
        </div>
      </template>

      <Column
        v-for="column in headers"
        :key="column.field"
        :field="column.field"
        :header="column.header"
        :style="`width: ${column.width ?? '30%'}`"
        class="p-4"
      />

      <Column v-if="hasDate" field="date" header="Ngày">
        <template #body="slotProps">
          {{ getDateFormattedShow(slotProps.data?.date) }}
        </template>
      </Column>

      <Column v-if="hasThumbnail" field="thumbnail" header="Ảnh bìa">
        <template #body="slotProps">
          <img :src="slotProps.data?.thumbnailUrl" alt="anh" class="h-20 w-32 object-cover" />
        </template>
      </Column>

      <Column v-if="hasStatus" field="status" header="Trạng thái">
        <template #body="{ data }">
          <div
            class="flex items-center text-sm space-x-2 w-32 p-1.5 rounded-md font-semibold"
            :class="{
              'bg-lime-200 text-lime-600': data?.status === BookingStatus.ACCEPTED,
              'bg-red-200 text-red-600': data?.status === BookingStatus.REJECTED,
              'bg-blue-200 text-blue-600': data?.status === BookingStatus.WAITING,
              'bg-gray-200 text-gray-600': data?.status === BookingStatus.CANCELED
            }"
          >
            {{ $t(`bookingStatus.${data?.status}`) as string }}
          </div>
        </template>
      </Column>

      <Column v-if="hasFieldStatus" field="fieldStatus" header="Trạng thái sân">
        <template #body="{ data }">
          <div
            class="flex items-center text-sm space-x-2 w-32 p-1.5 rounded-md font-semibold"
            :class="{
              'bg-lime-200 text-lime-600': data?.status === FieldStatus.ACCEPTED,
              'bg-red-200 text-red-600': data?.status === FieldStatus.REJECTED,
              'bg-blue-200 text-blue-600': data?.status === FieldStatus.WAITING,
              'bg-gray-200 text-gray-600': data?.status === FieldStatus.INACTIVE
            }"
          >
            {{ $t(`fieldStatus.${data?.status}`) as string }}
          </div>
        </template>
      </Column>

      <Column v-if="isPayment" field="paymentType" header="Loại thanh toán">
        <template #body="{ data }">
          <div
            class="flex items-center text-sm space-x-2 w-32 p-1.5 rounded-md font-semibold justify-between pr-4"
            :class="{
              'bg-lime-200 text-lime-600': data?.paymentType === PaymentType.BOOKING,
              'bg-red-200 text-red-600': data?.paymentType === PaymentType.DEBTPAYMENT,
              'bg-blue-200 text-blue-600': data?.paymentType === PaymentType.RECHARGE,
              'bg-gray-200 text-gray-600': data?.paymentType === PaymentType.REFUND
            }"
          >
            <span>
              {{ $t(`paymentType.${data?.paymentType}`) as string }}
            </span>
            <i
              v-if="data?.paymentType === PaymentType.BOOKING"
              class="pi pi-ticket mr-1"
              style="font-size: 1rem"
            />
            <i
              v-if="data?.paymentType === PaymentType.DEBTPAYMENT"
              class="pi pi-receipt mr-1"
              style="font-size: 1rem"
            />
            <i
              v-if="data?.paymentType === PaymentType.RECHARGE"
              class="pi pi-check mr-1"
              style="font-size: 1rem"
            />
            <i
              v-if="data?.paymentType === PaymentType.REFUND"
              class="pi pi-replay mr-1"
              style="font-size: 1rem"
            />
          </div>
        </template>
      </Column>

      <Column v-if="isPayment" field="paymentStatus" header="Trạng thái">
        <template #body="{ data }">
          <div
            class="flex items-center text-sm space-x-2 w-36 p-1.5 rounded-md font-semibold justify-between pr-4"
            :class="{
              'bg-blue-200 text-blue-600': data?.paymentStatus === PaymentStatus.PENDING,
              'bg-green-200 text-green-600': data?.paymentStatus === PaymentStatus.SUCCESSFUL,
              'bg-red-200 text-red-600': data?.paymentStatus === PaymentStatus.FAILED
            }"
          >
            <span>
              {{ $t(`paymentStatus.${data?.paymentStatus}`) as string }}
            </span>
            <i
              v-if="data?.paymentStatus === PaymentStatus.PENDING"
              class="pi pi-hourglass mr-1"
              style="font-size: 0.8rem"
            />
            <i
              v-if="data?.paymentStatus === PaymentStatus.SUCCESSFUL"
              class="pi pi-check-circle mr-1"
              style="font-size: 0rem"
            />
            <i
              v-if="data?.paymentStatus === PaymentStatus.FAILED"
              class="pi pi-minus-circle mr-1"
              style="font-size: 0rem"
            />
          </div>
        </template>
      </Column>
      <Column v-if="isStaffList" field="isActive" header="Trạng thái">
        <template #body="{ data }">
          <div
            class="flex items-center text-sm space-x-2 w-fit p-1.5 rounded-md font-semibold"
            :class="{
              'bg-lime-200 text-lime-600': data?.isActive,
              'bg-red-200 text-red-600': !data?.isActive
            }"
          >
            {{ data?.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động' }}
          </div>
        </template>
      </Column>

      <Column v-if="!noDetail || !noEdit || !noDelete || ban" header="" :style="{ width: '120px' }">
        <template #body="{ data }">
          <div class="flex justify-center space-x-2">
            <Button
              v-if="!noDetail"
              icon="pi pi-info-circle"
              class="p-button-text p-button-rounded"
              @click="onDetail(Number(data.id))"
            />
            <Button
              v-if="!noEdit"
              icon="pi pi-pencil"
              class="p-button-text p-button-rounded"
              @click="onEdit(Number(data.id))"
            />
            <Button
              v-if="!noDelete"
              icon="pi pi-trash"
              class="p-button-text p-button-rounded bg-red-400 hover:bg-red-600"
              @click="onDelete(Number(data.id))"
            />
            <Button
              v-if="ban"
              icon="pi pi-ban"
              class="p-button-text p-button-rounded p-button-warning bg-red-400 hover:bg-red-600"
              @click="onBan(Number(data.id))"
            />
          </div>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script setup lang="ts">
import DataTable, { type DataTablePageEvent } from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import type { TableColumn } from './types'
import { getDateFormattedShow } from '@/utils/dateUtil'
import { BookingStatus } from '@/constants/booking'
import { PaymentStatus, PaymentType } from '@/constants/payment'
import { FieldStatus } from '@/constants/field'

defineProps<{
  title?: string
  noDetail?: boolean
  noEdit?: boolean
  loading?: boolean
  noDelete?: boolean
  total: number
  ban?: boolean
  hasDate?: boolean
  hasStatus?: boolean
  hasThumbnail?: boolean
  hasFieldStatus?: boolean
  isPayment?: boolean
  headers: TableColumn[]
  items: { [key: string]: any }[]
  isStaffList?: boolean
}>()

const emit = defineEmits<{
  (e: 'detail', id: number): void
  (e: 'edit', id: number): void
  (e: 'delete', id: number): void
  (e: 'changePage', offset: number, limit: number): void
  (e: 'ban', id: number): void
}>()

const onDetail = (id: number) => {
  emit('detail', id)
}

const onEdit = (id: number) => {
  emit('edit', id)
}

const onDelete = (id: number) => {
  emit('delete', id)
}

const onPreviousPage = (page: DataTablePageEvent) => {
  emit('changePage', page.rows, page.page * page.rows)
}

const onBan = (id: number) => {
  emit('ban', id)
}
</script>

<style scoped>
.p-datatable .p-datatable-thead > tr > th {
  background-color: #f3f4f6;
  color: #1f2937;
  font-weight: 600;
}

.p-datatable .p-datatable-tbody > tr:nth-child(even) {
  background-color: #f9fafb;
}

.p-datatable .p-datatable-tbody > tr:hover {
  background-color: #e5e7eb;
}

.p-paginator {
  justify-content: flex-end;
  padding: 1rem;
}

.custom-header {
  min-height: 1000px !important;
  background-color: #ccc;
}
</style>
