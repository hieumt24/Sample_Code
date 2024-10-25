<template>
  <div class="container mx-auto px-4">
    <div class="flex justify-between items-center my-8">
      <h1 class="text-2xl font-bold">
        Tổng số sân nhỏ: {{ partialFieldStore.partialFields?.length ?? 0 }}
      </h1>
      <Button
        label="Thêm mới"
        icon="pi pi-plus"
        outlined
        class="p-button-success"
        @click="onCreateModal"
      />
    </div>

    <div class="bg-green-50 rounded-lg p-6">
      <div v-if="partialFieldStore.loading" class="flex flex-col items-center py-12">
        <i class="pi pi-spin pi-spinner text-4xl mb-4"></i>
        <h2 class="text-xl">Đang tải...</h2>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <div
          v-for="field in partialFieldStore.partialFields"
          :key="field.id"
          class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300"
        >
          <div class="p-4">
            <h3 class="font-semibold text-lg mb-2">{{ field.name }}</h3>
            <p class="text-gray-600">{{ field.description }}</p>
            <p class="text-blue-600">Lịch đặt đang chờ: {{ field.numberWaiting }}</p>
            <p class="text-green-600">Lịch đặt sắp tới: {{ field.numberAccepted }}</p>
          </div>
          <div class="border-t px-4 py-3">
            <Button
              label="Cài đặt"
              icon="pi pi-cog"
              outlined
              class="p-button-success w-full"
              @click.stop="onUpdateModal(field.id)"
            />
          </div>
        </div>
      </div>
    </div>
  </div>

  <PartialFieldDialog
    @close="isShowDetailModal = false"
    @afterSuccess="afterSucess"
    :open="isShowDetailModal"
    :is-create="isCreatePartial"
    :field-id="currentFieldId"
    :partial-field-id="selectedPartialFieldId"
  />
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useSlotStore } from '@/stores/slotStore'
import { useRoute } from 'vue-router'
import { onMounted, ref } from 'vue'
import { watch } from 'vue'
import { useFieldStore } from '@/stores/fieldStore'
import Button from 'primevue/button'
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import PartialFieldDialog from '../dialogs/PartialFieldDialog.vue'
const { t } = useI18n()
const route = useRoute()

const partialFieldStore = usePartialFieldStore()

const emit = defineEmits<(e: 'changeMode', id: 'login' | 'register') => void>()
const currentFieldId = ref(Number(route.params.id))
const isShowDetailModal = ref(false)
const isCreatePartial = ref(false)
const selectedPartialFieldId = ref(0)

const props = defineProps<{
  isOpen: boolean
  fieldId?: number
}>()

const onUpdateModal = (partialId: number) => {
  selectedPartialFieldId.value = partialId
  isCreatePartial.value = false
  isShowDetailModal.value = true
}

const onCreateModal = () => {
  isCreatePartial.value = true
  isShowDetailModal.value = true
}

const afterSucess = async () => {
  isShowDetailModal.value = false
  await partialFieldStore.getPartialFieldByFieldId(currentFieldId.value)
}

watch(
  () => props.isOpen,
  async (value) => {
    if (value) {
      partialFieldStore.partialFields = []
      await partialFieldStore.getPartialFieldByFieldId(currentFieldId.value)
    }
  }
)
</script>
