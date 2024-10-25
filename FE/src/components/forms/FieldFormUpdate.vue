<template>
  <div class="flex p-10">
    <div class="w-1/2 p-5">
      <div class="flex w-full">
        <InputField name="name" :label="'Tên sân'" v-model="name" :error="errors.name" />
        <InputField
          name="phoneNumber"
          :label="'Số điện thoại'"
          v-model="phoneNumber"
          :error="errors.phoneNumber"
        />
      </div>

      <InputField
        textarea
        name="description"
        :label="'Mô tả'"
        v-model="description"
        :error="errors.description"
      />

      <div class="w-full grid grid-cols-2 gap-4 my-2">
        <div class="flex">
          <label for="openTime" class="font-semibold w-10">Open time</label>
          <Calendar
            :label="'Open time'"
            id="openTime"
            v-model="openTime"
            :error="errors.openTime"
            class="w-9"
            timeOnly
          />
        </div>
        <div class="flex">
          <label for="closeTime" class="font-semibold">Close time</label>
          <Calendar
            :label="'Close time'"
            id="closeTime"
            v-model="closeTime"
            :error="errors.closeTime"
            class="w-full"
            timeOnly
          />
        </div>
      </div>
      <div class="flex flex-col">
        <label for="openTime" class="font-semibold">Slot time</label>
        <Calendar id="slotTime" v-model="slotTime" class="w-full" timeOnly />
      </div>
      <TimeInput :init-value="'01:00'" v-model="slotTime" label="Thời gian một trận" class="mr-5" />
      <label class="font-semibold block" for="cover-image">Cover: </label>
      <FileUpload
        accept="image/*"
        :maxFileSize="1000000"
        :showUploadButton="false"
        @select="onCoverImageFileSelect"
        @clear="onCoverImageClear"
        :pt:content:class="'relative bg-surface-0 dark:bg-surface-900 text-surface-700 dark:text-white/80 p-4 border border-surface-200 dark:border-surface-700 rounded-b-lg'"
      >
        <template #header="{ chooseCallback, clearCallback, files }">
          <div class="flex flex-wrap items-center justify-between flex-1 gap-2">
            <div class="flex gap-2">
              <Button @click="chooseCallback()" icon="pi pi-images" rounded outlined> </Button>
              <Button
                @click="clearCallback()"
                icon="pi pi-times"
                rounded
                outlined
                severity="danger"
                :disabled="!files || files.length === 0"
              ></Button>
            </div>
          </div>
        </template>
        <template #content="{ files }">
          <div v-if="files.length > 0">
            <div
              class="flex items-center flex-wrap p-4 mb-2 last:mb-0 border border-surface-200 dark:border-surface-700 gap-2 rounded"
            >
              <img
                role="presentation"
                :alt="files[files.length - 1].name"
                :src="files[files.length - 1].name"
                class="rounded-lg"
                width="50px"
              />
              <span class="font-semibold mb-2 break-all">{{ files[files.length - 1].name }}</span>
            </div>
            <div
              class="p-4 mb-2 last:mb-0 border border-surface-200 dark:border-surface-700 gap-2 rounded"
            >
              <Image
                :src="cover ? cover.objectURL : fieldStore.field?.cover"
                alt="Image"
                width="1000"
                preview
              />
            </div>
          </div>
        </template>
        <template #empty>
          <div class="flex items-center justify-center flex-col">
            <i
              class="pi pi-cloud-upload border-2 rounded-full p-5 text-5xl text-gray-400 border-gray-400"
            ></i>
            <p class="mt-4 mb-0">Drag and drop files to here to upload.</p>
            <div
              v-if="!isCreate"
              class="p-4 mb-2 last:mb-0 border border-surface-200 dark:border-surface-700 gap-2 rounded"
            >
              <Image
                :src="cover ? cover.objectURL : fieldStore.field?.cover"
                alt="Image"
                width="1000"
                preview
              />
            </div>
          </div>
        </template>
      </FileUpload>
      <label class="font-semibold block" for="cover-image">Avatar: </label>
      <FileUpload
        accept="image/*"
        :maxFileSize="1000000"
        :showUploadButton="false"
        @select="onAvatarImageFileSelect"
        @clear="onAvatarImageClear"
        :pt:content:class="'relative bg-surface-0 dark:bg-surface-900 text-surface-700 dark:text-white/80 p-4 border border-surface-200 dark:border-surface-700 rounded-b-lg'"
      >
        <template #header="{ chooseCallback, clearCallback, files }">
          <div class="flex flex-wrap items-center justify-between flex-1 gap-2">
            <div class="flex gap-2">
              <Button @click="chooseCallback()" icon="pi pi-images" rounded outlined> </Button>
              <Button
                @click="clearCallback()"
                icon="pi pi-times"
                rounded
                outlined
                severity="danger"
                :disabled="!files || files.length === 0"
              ></Button>
            </div>
          </div>
        </template>
        <template #content="{ files }">
          <div v-if="files.length > 0">
            <div
              class="flex items-center flex-wrap p-4 mb-2 last:mb-0 border border-surface-200 dark:border-surface-700 gap-2 rounded"
            >
              <img
                role="presentation"
                :alt="files[files.length - 1].name"
                :src="files[files.length - 1].name"
                class="rounded-lg"
                width="50px"
              />
              <span class="font-semibold mb-2 break-all">{{ files[files.length - 1].name }}</span>
            </div>
            <div
              class="p-4 mb-2 last:mb-0 border border-surface-200 dark:border-surface-700 gap-2 rounded"
            >
              <Image
                :src="avatar ? avatar.objectURL : fieldStore.field?.avatar"
                alt="Image"
                width="1000"
                preview
              />
            </div>
          </div>
        </template>
        <template #empty>
          <div class="flex items-center justify-center flex-col">
            <i
              class="pi pi-cloud-upload border-2 rounded-full p-5 text-5xl text-gray-400 border-gray-400"
            ></i>
            <p class="mt-4 mb-0">Drag and drop files to here to upload.</p>
            <div
              v-if="!isCreate"
              class="w-full p-4 mb-2 last:mb-0 border border-surface-200 dark:border-surface-700 gap-2 rounded"
            >
              <Image
                :src="avatar ? avatar.objectURL : fieldStore.field?.avatar"
                alt="Image"
                class="w-full"
                preview
              />
            </div>
          </div>
        </template>
      </FileUpload>
    </div>

    <!-- MAP HERE -->
    <div class="w-1/2">
      <label class="font-semibold" for="address">Address</label>
      <AutoComplete
        :id="'address'"
        class="w-full"
        v-model="address"
        input-class="w-full"
        :suggestions="predictions.map((x) => x.description)"
        @update:modelValue="onSearch"
        @item-select="selectSuggestion"
        emptySearchMessage=""
      />
      <small v-if="errors.address" class="text-red-500">{{ errors.address }}</small>
      <div class="flex space-x-4">
        <InputField
          name="province"
          :label="'Province'"
          v-model="province"
          :error="errors.province"
          class="flex-1"
          disabled
        />
        <InputField
          name="district"
          :label="'District'"
          v-model="district"
          :error="errors.district"
          class="flex-1"
          disabled
        />
        <InputField
          name="commune"
          :label="'Commune'"
          v-model="commune"
          :error="errors.commune"
          class="flex-1"
          disabled
        />
      </div>
      <div class="flex space-x-4 mb-2">
        <InputField
          name="latitude"
          :label="'Latitude'"
          v-model="latitude"
          :error="errors.latitude"
          class="flex-1"
          disabled
        />
        <InputField
          name="longitude"
          :label="'Longitude'"
          v-model="longitude"
          :error="errors.longitude"
          class="flex-1"
          disabled
        />
      </div>
      <div class="w-full p-5 h-[30rem] border border-orange-500">
        <MarkerMap v-model:coordinate="coordinate" />
      </div>
      <div class="flex justify-end space-x-4 py-4">
        <Button
          label="Cancel"
          icon="pi pi-times"
          outlined
          class="p-button-danger"
          @click="cancel"
        />
        <Button label="Submit" icon="pi pi-check" class="p-button-success" @click="submit" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import InputField from '@/components/inputs/InputField.vue'
import CustomForm from '@/components/forms/CustomForm.vue'
import MarkerMap from '@/components/maps/MarkerMap.vue'
import Image from 'primevue/image'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { useI18n } from 'vue-i18n'
import { useFieldStore } from '@/stores/fieldStore'
import { onMounted, ref, watch } from 'vue'
import type { FieldCreateRequest, FieldUpdateRequest } from '@/api/fieldApi'
import type { AutoCompletePlace, Prediction, Coordinate } from '@/types/Map'
import AutoComplete, { type AutoCompleteItemSelectEvent } from 'primevue/autocomplete'
import { useToast } from 'primevue/usetoast'
import FileUpload, { type FileUploadSelectEvent } from 'primevue/fileupload'
import Button from 'primevue/button'
import router from '@/router'
import Calendar from 'primevue/calendar'
import { getTimeFromDate } from '@/utils/timeUtil'
import TimeInput from '../calendar/TimeInput.vue'
import Stepper from 'primevue/stepper'
import StepperPanel from 'primevue/stepperpanel'
import { VITE_APP_GOONG_API_KEY } from '@/constants/env'

const GOONG_API_KEY = VITE_APP_GOONG_API_KEY

const predictions = ref<Prediction[]>([])
const fieldStore = useFieldStore()
const { t } = useI18n()
const toast = useToast()

const coordinate = ref<Coordinate>({ lat: 0, lng: 0 })

const props = defineProps<{
  isCreate: boolean
  id?: number
  noCancel?: boolean
  noSubmit?: boolean
  noApprove?: boolean
  noReject?: boolean
}>()

const emit = defineEmits<{
  (e: 'back'): void
  (e: 'submit', values: FieldCreateRequest): void
  (e: 'approve', id: number): void
  (e: 'reject', id: number): void
}>()

const schema = yup.object({
  name: yup.string().required(t('validation.required')),
  address: yup.string().required(),
  province: yup.string().required(),
  district: yup.string().required(),
  phoneNumber: yup.string().matches(/^\d*$/).required(),
  latitude: yup.number().required(),
  longitude: yup.number().required(),
  description: yup.string().required(),
  cover: props.isCreate ? yup.mixed().required() : yup.mixed()
})

const { meta, errors, defineField, handleSubmit, resetForm, values } = useForm<any>({
  validationSchema: schema
})

const [name] = defineField('name')
const [address] = defineField('address')
const [longitude] = defineField('longitude')
const [latitude] = defineField('latitude')
const [description] = defineField('description')
const [commune] = defineField('commune')
const [district] = defineField('district')
const [province] = defineField('province')
const [phoneNumber] = defineField('phoneNumber')
const [cover] = defineField('cover')
const [avatar] = defineField('avatar')
const [openTime] = defineField('openTime')
const [closeTime] = defineField('closeTime')
const [isFixedSlot] = defineField('isFixedSlot')
const [slotTime] = defineField('slotTime')

// Sync form latitude and longitude with coordinates object
watch(
  () => [values.latitude, values.longitude],
  ([newLat, newLng]) => {
    coordinate.value.lat = newLat
    coordinate.value.lng = newLng
  }
)

// Sync coordinates object with form latitude and longitude
watch(
  () => coordinate.value,
  (newCoordinate) => {
    longitude.value = newCoordinate.lng
    latitude.value = newCoordinate.lat
  }
)

onMounted(async () => {
  if (!props.isCreate && props.id) {
    await fieldStore.getFieldById(props.id)
    if (!fieldStore.field) return
    resetForm({ values: { ...fieldStore.field, cover: undefined } })
  }
})

const onCoverImageFileSelect = (event: FileUploadSelectEvent) => {
  cover.value = event.files[event.files.length - 1]
}
const onCoverImageClear = () => {
  cover.value = undefined
}
const onAvatarImageFileSelect = (event: FileUploadSelectEvent) => {
  avatar.value = event.files[event.files.length - 1]
}
const onAvatarImageClear = () => {
  avatar.value = undefined
}

const cancel = () => {
  emit('back')
}

const submit = handleSubmit(async (values) => {
  if (props.isCreate) {
    console.log(values.openTime)

    values.openTime = getTimeFromDate(values.openTime)
    values.closeTime = getTimeFromDate(values.closeTime)
    values.slotTime = getTimeFromDate(values.slotTime)

    console.log(values.openTime)

    let response = await fieldStore.createField(values)
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Create Successfully',
        detail: response.message,
        life: 3000
      })
      router.push('/field/list')
    } else {
      toast.add({
        severity: 'error',
        summary: 'Create Failed',
        detail: response.message,
        life: 3000
      })
    }
  } else {
    if (typeof values.openTime != 'number') values.openTime = getTimeFromDate(values.openTime)

    if (typeof values.closeTime != 'number') values.closeTime = getTimeFromDate(values.closeTime)

    if (typeof values.slotTime != 'number') values.slotTime = getTimeFromDate(values.slotTime)

    let response = await fieldStore.updateField(values)
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Update Successfully',
        life: 3000
      })
    } else {
      toast.add({
        severity: 'error',
        summary: 'Update Failed',
        life: 3000
      })
    }
  }
})

const onSearch = () => {
  if (address.value) {
    const url = `https://rsapi.goong.io/place/autocomplete?limit=5&more_compound=false&api_key=${GOONG_API_KEY}&input=${encodeURIComponent(
      address.value
    )}`

    fetch(url)
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`)
        }
        return response.json()
      })
      .then((data: AutoCompletePlace) => {
        predictions.value = data.predictions || []
      })
      .catch((error) => {
        console.error('Fetch error:', error)
      })
  }
}

const fetchPlaceDetails = (placeId: string) => {
  const url = `https://rsapi.goong.io/Place/Detail?place_id=${encodeURIComponent(
    placeId
  )}&api_key=${GOONG_API_KEY}`

  fetch(url)
    .then((response) => {
      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`)
      }
      return response.json()
    })
    .then((data) => {
      if (data.status === 'OK' && data.result) {
        const lat = data.result.geometry.location.lat
        const lng = data.result.geometry.location.lng

        coordinate.value = { lat, lng }
        // updateMap(lat, lng)
      } else {
        throw new Error('Place details not found')
      }
    })
    .catch((error) => {
      console.error('Fetch error:', error)
    })
}

const selectSuggestion = (event: AutoCompleteItemSelectEvent) => {
  let prediction = predictions.value.filter((x) => (x.description = event.value))[0]
  if (prediction) {
    const compound = prediction.compound
    district.value = compound.district || 'N/A'
    commune.value = compound.commune || 'N/A'
    province.value = compound.province || 'N/A'

    fetchPlaceDetails(prediction.place_id)
  }
}
</script>
