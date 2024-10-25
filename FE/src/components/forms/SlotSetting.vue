<template>
  <div class="max-w-4xl mx-auto p-6">
    <div class="bg-white shadow-md rounded-lg overflow-hidden">
      <div class="bg-green-600 p-4 flex justify-between items-center">
        <h2 class="text-xl font-bold text-white">Lịch đặt sân</h2>
        <button
          @click="isInformationDialog = true"
          class="text-white hover:text-green-200 flex items-center"
        >
          Đây là gì?
          <i class="pi pi-question-circle text-xl ml-2"></i>
        </button>
      </div>

      <div class="p-6">
        <div class="flex space-x-6 mb-6">
          <label class="inline-flex items-center">
            <input
              type="radio"
              v-model="isFixedSlot"
              :value="true"
              @change="onChangeSlotMode"
              class="form-radio text-green-600"
            />
            <span class="ml-2">Cố định</span>
          </label>
          <label class="inline-flex items-center">
            <input
              type="radio"
              v-model="isFixedSlot"
              :value="false"
              @change="onChangeSlotMode"
              class="form-radio text-green-600"
            />
            <span class="ml-2">Không cố định</span>
          </label>
        </div>

        <button
          @click="(isCreateSlot = true), (isOpenSlotDialog = true)"
          :disabled="!isFixedSlot"
          class="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed mb-6"
        >
          Thêm lịch
        </button>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          <div
            v-for="slot in slotStore.slots"
            :key="slot.id"
            @click="onSlotDetail(slot.id)"
            class="bg-green-100 p-4 rounded-lg hover:bg-green-200 transition-colors"
            :class="isFixedSlot ? 'cursor-pointer' : 'bg-opacity-80 cursor-default'"
          >
            <div class="flex justify-between items-center">
              <span class="font-semibold text-green-800"
                >{{ slot.startTime }} - {{ slot.endTime }}</span
              >
              <i class="pi pi-cog text-lg text-green-600"></i>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <SlotDialog
    :field-id="currentFieldId"
    :open="isOpenSlotDialog"
    :id="selectedSlotId"
    :is-create="isCreateSlot"
    :start-time-to-create="startTimeSlot"
    :end-time-to-create="endTimeSlot"
    @afterSuccess="afterSuccess"
    @close="isOpenSlotDialog = false"
  />

  <ConfirmDialog
    :open="isOpenConfirmDialog"
    :message="confirmMessage"
    @close="onCancel"
    @submit="onSubmitSlotMode"
  />

  <InformationDialog :open="isInformationDialog" @close="isInformationDialog = false" />
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useSlotStore } from '@/stores/slotStore'
import { useRoute } from 'vue-router'
import { ref } from 'vue'
import { watch } from 'vue'
import { useFieldStore } from '@/stores/fieldStore'
import SlotDialog from '../dialogs/SlotDialog.vue'
import ConfirmDialog from '../dialogs/ConfirmDialog.vue'
import InformationDialog from '../dialogs/InformationDialog.vue'
import { useToast } from 'primevue/usetoast'
import '@toast-ui/calendar/dist/toastui-calendar.min.css'

const { t } = useI18n()
const route = useRoute()
const toast = useToast()

const slotStore = useSlotStore()
const fieldStore = useFieldStore()

const isFixedSlot = ref(false)
const isOpenSlotDialog = ref(false)
const isOpenConfirmDialog = ref(false)
const isInformationDialog = ref(false)
const confirmMessage = ref('')
const currentFieldId = ref(Number(route.params.id))
const startTimeSlot = ref()
const endTimeSlot = ref()
const selectedSlotId = ref()
const isCreateSlot = ref(false)

const props = defineProps<{
  isOpen: boolean
  fieldId: number
}>()

const afterSuccess = () => {
  isOpenSlotDialog.value = false
}

const onChangeSlotMode = () => {
  confirmMessage.value = isFixedSlot.value
    ? 'Bạn có chắc chắn muốn chuyển sang chế độ cố định không?'
    : 'Bạn có chắc chắn muốn chuyển sang chế độ không cố định không?'
  isOpenConfirmDialog.value = true
}

const onSlotDetail = async (id: number) => {
  selectedSlotId.value = id
  isCreateSlot.value = false
  isOpenSlotDialog.value = true
}

const onSubmitSlotMode = async () => {
  if (isFixedSlot.value !== undefined) {
    await slotStore
      .setFixedSlot({ id: props.fieldId, isFixedSlot: isFixedSlot.value })
      .then((response) => {
        if (response) {
          toast.add({
            severity: 'success',
            summary: 'Đã cập nhật cài đặt',
            detail:
              'sân của bạn bây giờ đang ở chế độ ' +
              (isFixedSlot.value ? 'cố định' : 'không cố định'),
            life: 3000
          })

          isOpenConfirmDialog.value = false

          if (isFixedSlot.value) {
            slotStore.getSlotByField(currentFieldId.value)
          }
        } else {
          toast.add({
            severity: 'error',
            summary: 'Lỗi',
            detail: 'Có lỗi xảy ra, vui lòng thử lại',
            life: 3000
          })
        }
      })
  }
}

const onCancel = () => {
  isOpenConfirmDialog.value = false
  isFixedSlot.value = fieldStore.field?.isFixedSlot ?? false
}
watch(
  () => props.isOpen,
  async (value) => {
    if (value) {
      isFixedSlot.value = fieldStore.field?.isFixedSlot ?? false
      slotStore.getSlotByField(currentFieldId.value)
    }
  }
)
</script>
