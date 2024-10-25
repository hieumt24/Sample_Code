<template>
  <div class="">
    <div class="flex w-full mb-12 bg-white p-5 shadow-md">
      <div class="mr-2">
        <div>Sân bóng</div>
        <Dropdown
          v-model="selectedField"
          :options="listFields"
          optionLabel="name"
          placeholder="Chọn sân"
          class="w-full md:w-[14rem]"
        />
      </div>
      <CustomCalendar
        class="mr-2"
        label="Ngày"
        v-model="findBookingStore.searchAvailableRequest.date"
      />
      <div class="mr-2">
        <TimeInput
          :init-value="startTime"
          v-model="startTime"
          label="Giờ"
          :error="startTimeError"
        />
      </div>
      <div class="mr-5">
        <div>Thời lượng</div>
        <Dropdown
          v-model="selectedDuration"
          :options="durationOptions"
          optionLabel="label"
          optionValue="value"
          placeholder="Select Duration"
        />
      </div>
      <div class="flex flex-col justify-end px-2">
        <Button
          :disabled="!findBookingStore.searchAvailableRequest.date || findBookingStore.loading"
          icon="pi pi-search"
          label="Tìm kiếm"
          class="bg-green-400"
          @click.stop="onSearch"
        />
      </div>
    </div>

    <div class="min-h-96 bg-gray-50 p-8 rounded-xl shadow-lg">
      <div
        v-if="findBookingStore.partialFields?.length === 0"
        class="flex items-center justify-center h-full"
      >
        <div v-if="isInit" class="text-center">
          <i class="pi pi-face-smile text-5xl text-green-400 mb-6"></i>
          <h2 class="text-2xl font-semibold text-gray-800">
            Các sân nhỏ đủ điều kiện sẽ hiển thị tại đây
          </h2>
        </div>

        <div v-else-if="notFoundPartial" class="text-center">
          <i class="pi pi-file-excel text-5xl text-yellow-400 mb-6"></i>
          <h2 class="text-2xl font-semibold text-gray-800">
            Không tìm thấy sân nào trống trong khoảng thời gian này
          </h2>
        </div>

        <div v-else-if="findBookingStore.loading" class="text-center">
          <i class="pi pi-spin pi-spinner text-5xl text-blue-400 mb-6"></i>
          <h2 class="text-2xl font-semibold text-gray-800">Đang tải ...</h2>
        </div>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
        <div
          v-for="partialField in findBookingStore.partialFields"
          :key="partialField.id"
          @click.stop="onSelectPartialField(partialField.id)"
          class="bg-white border border-gray-300 rounded-xl p-6 transition-transform duration-300 ease-in-out transform hover:scale-105 hover:shadow-2xl cursor-pointer"
        >
          <h3 class="text-xl font-semibold text-gray-900 mb-3">{{ partialField.name }}</h3>
          <p class="text-gray-700">{{ partialField.description }}</p>
        </div>
      </div>
    </div>
  </div>

  <PartialBookingDialog
    :open="isShowBookingTable"
    :is-fixed-slot="fieldStore.field?.isFixedSlot ?? false"
    :partial-id="selectedPartialFieldId"
    :field-id="selectedFieldId"
    :field-name="partialFieldStore.partialField?.fieldName"
    @close="isShowBookingTable = false"
  />
</template>

<script setup lang="ts">
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import { useFindBookingOwnerStore } from '@/stores/findBookingOwnerStore'
import Dropdown from 'primevue/dropdown'
import { onUnmounted, ref } from 'vue'
import type { DropdownType } from '@/constants/types'
import { computed } from 'vue'
import { onMounted } from 'vue'
import { useFieldStore } from '@/stores/fieldStore'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import Button from 'primevue/button'
import PartialBookingDialog from '@/components/dialogs/PartialBookingDialog.vue'
import TimeInput from '@/components/calendar/TimeInput.vue'
import { FieldStatus } from '@/constants/field'

const partialFieldStore = usePartialFieldStore()
const findBookingStore = useFindBookingOwnerStore()
const fieldStore = useFieldStore()

const listFields = ref<DropdownType[]>([])
const selectedField = ref<DropdownType>()
const selectedFieldId = computed(() => Number(selectedField.value?.code) ?? 0)

const startTimeError = ref('')
const startTime = ref()
const notFoundPartial = ref(false)
const isShowBookingTable = ref(false)
const selectedPartialFieldId = ref(0)
const isInit = ref(true)
const selectedDuration = ref(5400)
const durationOptions = [
  { label: '60 phút', value: 3600 },
  { label: '90 phút', value: 5400 },
  { label: '120 phút', value: 7200 }
]

const onSearch = async () => {
  isInit.value = false
  await findBookingStore.getAvailableByField({
    fieldId: Number(selectedFieldId.value),
    date: findBookingStore.searchAvailableRequest.date,
    startTime: startTime.value,
    duration: selectedDuration.value
  })

  if (findBookingStore.partialFields.length === 0) {
    notFoundPartial.value = true
  } else {
    notFoundPartial.value = false
  }
}

const onSelectPartialField = (id: number) => {
  selectedPartialFieldId.value = id
  isShowBookingTable.value = true
}

onMounted(async () => {
  fieldStore.ownerSearch.status = FieldStatus.ACCEPTED
  await fieldStore.getOwnerFieldList()
  listFields.value = fieldStore.fields.map((field) => ({
    name: field.name,
    code: field.id
  }))
  if (listFields.value.length > 0) {
    selectedField.value = listFields.value[0]
    fieldStore.getFieldById(Number(selectedFieldId.value))
  }
})

onUnmounted(() => {
  isInit.value = false
})
</script>
