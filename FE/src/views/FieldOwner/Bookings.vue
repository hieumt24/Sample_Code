<template>
  <div class="flex flex-wrap w-full">
    <div class="">
      <SearchTab @search="onSearch" @reset="onReset" class="mb-4">
        <div class="flex flex-col my-2">
          <h1>Chọn sân</h1>
          <Dropdown
            v-model="selectedField"
            :options="listFields"
            optionLabel="name"
            placeholder="Chọn sân"
            class="w-full md:w-[14rem]"
            @change="onChangeField"
          />
        </div>
        <hr class="mt-2" />
        <div>
          <h3 class="font-medium mb-2">Trạng thái</h3>
          <Dropdown
            v-model="selectedStatus"
            :options="bookingStatusOptionsTest"
            optionLabel="name"
            placeholder="Chọn trạng thái"
            class="w-56"
          />
        </div>
        <CustomCalendar
          name="fromDate"
          class="col-span-1"
          label="Ngày đặt"
          v-model="bookingStoreOwner.searchBookByFieldByOwner.date"
        />
      </SearchTab>
    </div>
    <div class="flex-1 lg:px-10">
      <CustomTable
        :headers="headers"
        :total="bookingStoreOwner.totalTable"
        :loading="bookingStoreOwner.tableLoading"
        :items="bookingShows"
        @edit="onEdit"
        @detail="onDetail"
        has-status
        has-date
        no-delete
        no-detail
        @change-page="bookingStoreOwner.changePageTableOwner"
      />
    </div>
  </div>

  <BookingAcceptDialog
    :open="openAceptBookingDialog"
    :id="selectedBookingId"
    @close="openAceptBookingDialog = false"
    @after-submit="onAfterAcceptBookingUser"
  ></BookingAcceptDialog>

  <BookingDialog
    :open="openDetailBooking"
    :id="selectedDetailBooking"
    :disabled="true"
    @close="openDetailBooking = false"
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
import { useBookingStore } from '@/stores/bookingStoreOwner'
import { useI18n } from 'vue-i18n'
import BookingAcceptDialog from '@/components/dialogs/BookingAcceptDialog.vue'
import BookingDialog from '@/components/dialogs/BookingDialog.vue'
import { FieldStatus } from '@/constants/field'
import { bookingStatusOptionsTest } from '@/constants/options'
const { t } = useI18n()

const fieldStore = useFieldStore()
const bookingStoreOwner = useBookingStore()

const listFields = ref<DropdownType[]>([])
const selectedField = ref<DropdownType>()
const openAceptBookingDialog = ref(false)
const openDetailBooking = ref(false)
const selectedBookingId = ref(0)
const selectedDetailBooking = ref(0)
const selectedStatus = ref<DropdownType>()

const bookingShows = computed(() =>
  bookingStoreOwner.bookingsByField.map((booking) => ({
    ...booking,
    time: `${booking.startTime} - ${booking.endTime}`,
    date: booking.date
  }))
)

const onDetail = async (id: number) => {
  selectedDetailBooking.value = id
}

const onEdit = async (id: number) => {
  selectedBookingId.value = id
  openAceptBookingDialog.value = true
}

const onSearch = async () => {
  bookingStoreOwner.searchBookByFieldByOwner.status =
    selectedStatus.value?.code?.toString() ?? undefined
  await bookingStoreOwner.getBookingByFieldId()
}

const onReset = async () => {
  bookingStoreOwner.searchBookByFieldByOwner = {
    date: undefined,
    startTime: undefined,
    endTime: undefined,
    status: undefined,
    limit: LIMIT_PER_PAGE,
    offset: 0,
    fieldId: bookingStoreOwner.searchBookByFieldByOwner.fieldId
  }
  selectedStatus.value = undefined
  onSearch()
}

const onAfterAcceptBookingUser = async () => {
  openAceptBookingDialog.value = false
  onSearch()
}

const onChangeField = async () => {
  bookingStoreOwner.searchBookByFieldByOwner.fieldId = Number(selectedField.value?.code)
  await onReset()
}

const headers = [{ field: 'time', header: 'Thời gian' }]

onMounted(async () => {
  fieldStore.ownerSearch.status = FieldStatus.ACCEPTED
  await fieldStore.getOwnerFieldList()
  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))

  if (listFields.value.length > 0) {
    selectedField.value = listFields.value[0]
    bookingStoreOwner.searchBookByFieldByOwner.fieldId = Number(selectedField.value.code)
  }

  selectedStatus.value = bookingStatusOptionsTest[0]
  bookingStoreOwner.searchBookByFieldByOwner.status =
    selectedStatus.value?.code?.toString() ?? undefined
  await bookingStoreOwner.getBookingByFieldId()
})
</script>
