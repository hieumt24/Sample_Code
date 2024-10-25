<template>
  <div>
    <div class="mx-auto rounded-lg overflow-hidden">
      <!-- Cover Image Section -->
      <div class="relative rounded-lg shadow-inner pb-3 mb-3 border">
        <img :src="fieldStore.field?.cover" alt="Cover" class="w-full h-80 object-cover" />
        <div class="px-4 pt-7">
          <h2 class="text-3xl font-semibold">{{ fieldStore.field?.name }}</h2>
        </div>
      </div>

      <!-- Profile and Details Section -->
      <div class="flex p-4 border rounded-lg">
        <div class="relative p-5 rounded-lg shadow-md w-full mr-2">
          <div class="space-y-6">
            <div class="flex">
              <div class="w-1/2 mr-3">
                <div class="flex items-baseline mb-4">
                  <h3 class="font-semibold text-lg text-gray-700 mr-2">Address:</h3>
                  <p class="text-gray-600">{{ fieldStore.field?.address }}</p>
                </div>
                <div class="flex items-baseline mb-4">
                  <h3 class="font-semibold text-lg text-gray-700 mr-2">Latitude:</h3>
                  <p class="text-gray-600">{{ fieldStore.field?.latitude }}</p>
                </div>
                <div class="flex items-baseline mb-4">
                  <h3 class="font-semibold text-lg text-gray-700 mr-2">Longitude:</h3>
                  <p class="text-gray-600">{{ fieldStore.field?.longitude }}</p>
                </div>
              </div>
              <div class="w-1/2">
                <div class="flex items-baseline mb-4">
                  <h3 class="font-semibold text-lg text-gray-700 mr-2">Commune:</h3>
                  <p class="text-gray-600">Tân Xã</p>
                </div>
                <div class="flex items-baseline mb-4">
                  <h3 class="font-semibold text-lg text-gray-700 mr-2">District:</h3>
                  <p class="text-gray-600">Thạch Thất</p>
                </div>
                <div class="flex items-baseline mb-4">
                  <h3 class="font-semibold text-lg text-gray-700 mr-2">Province:</h3>
                  <p class="text-gray-600">Hà Nôi</p>
                </div>
              </div>
            </div>
            <Divider />
            <!-- Description -->
            <div class="flex items-baseline">
              <h3 class="font-semibold text-lg text-gray-700 mr-2">Description:</h3>
              <p class="text-gray-600">{{ fieldStore.field?.description }}</p>
            </div>

            <div class="absolute bottom-0 pb-4">
              <Divider />
              <IconButton icon="pencil" @click="onFieldEdit(fieldId)" class="mr-5" />
              <IconButton icon="trash" @click="onFieldDelete(fieldId)" />
            </div>
          </div>
        </div>
        <!-- Map Section -->
        <div class="rounded-lg shadow-md w-full">
          <div class="bg-white rounded-lg shadow-lg h-full">
            <MarkerMap
              v-model:coordinate="coordinate"
              :is-draggable="false"
              class="rounded-lg shadow-md w-full h-64"
            />
          </div>
        </div>
      </div>
      <div class="rounded-lg shadow-inner my-3 border">
        <Panel header="Partial Field">
          <div class="px-4">
            <div class="w-full p-4">
              <ActionButton
                class="mb-2"
                :value="$t('form.create')"
                @click="onPartialFieldCreate"
                :is-outlined="true"
              />
              <CustomTable
                :headers="headers"
                :total="partialFieldStore.total"
                :loading="partialFieldStore.loading"
                :items="partialFieldStore.partialFields"
                @detail="onPartialFieldDetail"
                @edit="onPartialFieldEdit"
                @change-page="partialFieldStore.changePagePartialField"
              />
            </div>
          </div>
        </Panel>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import MarkerMap from '@/components/maps/MarkerMap.vue'
import { useI18n } from 'vue-i18n'
import { useFieldStore } from '@/stores/fieldStore'
import { onMounted, onUnmounted, reactive, ref } from 'vue'
import type { Coordinate } from '@/types/Map'
import { useRoute } from 'vue-router'
import Divider from 'primevue/divider'
import Button from 'primevue/button'
import ActionButton from '@/components/buttons/ActionButton.vue'
import CustomTable from '@/components/tables/CustomTable.vue'
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import PartialFieldDialog from '@/components/dialogs/PartialFieldDialog.vue'
import Panel from 'primevue/panel'
import router from '@/router'
import IconButton from '@/components/buttons/IconButton.vue'

const route = useRoute()
const fieldId = Number(route.params.id)
const fieldStore = useFieldStore()
const partialFieldStore = usePartialFieldStore()
const { t } = useI18n()

const openDetail = ref(false)
const openCreatePartialField = ref(false)
const coordinate = reactive<Coordinate>({ lat: 0, lng: 0 })
const isCreatePartialField = ref(false)

const headers = [
  { field: 'id', header: 'Id' },
  { field: 'name', header: 'Name' },
  { field: 'status', header: 'Status' }
]

onMounted(async () => {
  if (fieldId) {
    await fieldStore.getFieldById(fieldId)
    if (!fieldStore.field) return
    coordinate.lat = fieldStore.field.latitude
    coordinate.lng = fieldStore.field.longitude
    await partialFieldStore.getPartialFieldByFieldId(fieldId)
  }
})
onUnmounted(() => {
  partialFieldStore.partialFields = []
})

const onFieldEdit = async (id: number) => {
  router.push(`/field/update-field/${id}`)
}

const onFieldDelete = async (id: number) => {}

const onPartialFieldDetail = async (partialFieldId: number) => {}

const onPartialFieldEdit = async (partialFieldId: number) => {
  await partialFieldStore.getPartialFieldById(partialFieldId)
  partialFieldStore.partialField = partialFieldStore.partialField
  isCreatePartialField.value = false
  openCreatePartialField.value = true
}

const onPartialFieldCreate = () => {
  partialFieldStore.partialField = undefined
  isCreatePartialField.value = true
  openCreatePartialField.value = true
}
</script>
