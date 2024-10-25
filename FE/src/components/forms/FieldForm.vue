<template>
  <div class="max-w-6xl mx-auto p-6 bg-white shadow-lg rounded-lg">
    <h1 class="text-3xl font-bold mb-8 text-center text-gray-800">Tạo Sân Bóng Mới</h1>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 md:p-6">
      <div>
        <InputField
          name="name"
          :label="'Tên sân'"
          class="mb-4"
          v-model="name"
          :error="errors.name"
          require
        />
        <InputField
          name="phoneNumber"
          :label="'Số điện thoại'"
          class="mb-4"
          v-model="phoneNumber"
          :error="errors.phoneNumber"
          require
        />

        <!-- Giá sân and Tiền cọc on the same line -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-4">
          <!-- Giá sân -->
          <div class="field">
            <label class="font-semibold" for="price">Giá sân</label>
            <InputNumber
              v-model="price"
              id="price"
              inputId="integeronly"
              suffix=" VNĐ"
              class="w-full"
              require
            />
            <small v-if="errors.price" class="text-red-500 ml-3">{{ errors.price }}</small>
            <span v-else class="block text-sm text-green-600 mt-1">
              {{ getVietnameseMoney(price) }}
            </span>
          </div>

          <!-- Tiền cọc -->
          <div class="field">
            <label class="font-semibold" for="deposit">Tiền cọc</label>
            <InputNumber
              v-model="deposit"
              id="deposit"
              inputId="integeronly"
              suffix=" VNĐ"
              class="w-full"
              require
            />
            <small v-if="errors.deposit" class="text-red-500 ml-3">{{ errors.deposit }}</small>
            <span v-else class="block text-sm text-green-600 mt-1">
              {{ getVietnameseMoney(deposit) }}
            </span>
          </div>
        </div>

        <!-- Giờ hoạt động -->
        <div>
          <div class="flex items-center mb-4">
            <ToggleButton
              v-model="isOpenAllDay"
              onLabel="Mở cả ngày"
              offLabel="Giờ cụ thể"
              onIcon="pi pi-clock"
              offIcon="pi pi-calendar"
              class="mr-4"
              @change="handleOpenAllDayToggle"
            />
          </div>

          <div v-if="!isOpenAllDay" class="flex flex-col sm:flex-row gap-4 mb-4">
            <div class="mb-4">
              <TimeInput
                init-value="08:00"
                v-model="openTime"
                label="Giờ mở cửa"
                :error="errors.openTime"
              />
            </div>
            <div class="mb-4">
              <TimeInput
                init-value="23:00"
                v-model="closeTime"
                label="Giờ đóng cửa"
                :error="errors.closeTime"
              />
            </div>
          </div>
          <p v-else class="text-green-600 mb-4">Mở cả ngày (24/7)</p>
        </div>
      </div>

      <InputField
        textarea
        name="description"
        :label="'Mô tả'"
        class="col-span-1 h-full"
        v-model="description"
        :error="errors.description"
        :inputClass="'h-full'"
      />

      <!-- Bản đồ sân -->
      <div class="col-span-full bg-gray-50 p-6 rounded-lg">
        <div class="md:p-6 grid grid-cols-1 md:grid-cols-2 gap-5">
          <div class="col-span-1 md:col-span-2 flex flex-col">
            <div>
              <label class="font-semibold" for="address"
                >Địa chỉ <span class="text-red-500">*</span></label
              >
            </div>
            <AutoComplete
              :id="'address'"
              v-model="address"
              :suggestions="predictions.map((x) => x.description)"
              @update:modelValue="onSearch"
              @item-select="selectSuggestion"
              emptySearchMessage="Không có gợi ý cho địa chỉ của bạn"
              :pt:input:class="'w-full h-8 border border-gray-300 focus:outline-1 focus:outline-gray-400 rounded-md p-2'"
            />
            <small class="text-green-600"
              >Tip: Nhập địa chỉ để được đề xuất, sau đó chọn vị trí chính xác trên bản đồ
            </small>
            <small v-if="errors.address" class="text-red-500 ml-3">{{ errors.address }}</small>
          </div>

          <div class="col-span-1 md:col-span-2 grid grid-cols-3 gap-4">
            <div class="mt-2 mb-2 flex flex-col">
              <label class="font-semibold" for="province"
                >Tỉnh Thành <span class="text-red-500">*</span></label
              >
              <AutoComplete
                input-class="w-full"
                inputId="province"
                v-model="selectedProvince"
                :suggestions="filteredProvince"
                @complete="searchProvince"
                @item-select="onProvinceChange"
                optionLabel="full_name"
                dropdown
                forceSelection
                :disabled="locationStore.isProvinceLoading"
                :dropdownIcon="
                  locationStore.isProvinceLoading
                    ? 'pi pi-spinner pi-spin animate-spin'
                    : 'pi pi-chevron-down'
                "
              />
              <small v-if="errors.province" class="text-red-500 ml-3">{{ errors.province }}</small>
            </div>

            <div class="mt-2 mb-2 flex flex-col">
              <label class="font-semibold" for="district"
                >Quận Huyện <span class="text-red-500">*</span></label
              >
              <AutoComplete
                input-class="w-full"
                inputId="district"
                v-model="selectedDistrict"
                :suggestions="filteredDistrict"
                @complete="searchDistrict"
                @item-select="onDistrictChange"
                optionLabel="full_name"
                dropdown
                forceSelection
                :disabled="locationStore.isDistrictLoading || !selectedProvince"
                :dropdownIcon="
                  locationStore.isDistrictLoading
                    ? 'pi pi-spinner pi-spin animate-spin'
                    : 'pi pi-chevron-down'
                "
              />
              <small v-if="errors.district" class="text-red-500 ml-3">{{ errors.district }}</small>
            </div>
            <div class="mt-2 mb-2 flex flex-col">
              <label class="font-semibold" for="district"
                >Phường Xã <span class="text-red-500">*</span></label
              >
              <AutoComplete
                input-class="w-full"
                inputId="commune"
                v-model="selectedCommune"
                :suggestions="filteredCommune"
                @complete="searchCommune"
                @item-select="onCommuneChange"
                optionLabel="full_name"
                dropdown
                forceSelection
                :disabled="locationStore.isCommuneLoading || !selectedDistrict"
                :dropdownIcon="
                  locationStore.isCommuneLoading
                    ? 'pi pi-spinner pi-spin animate-spin'
                    : 'pi pi-chevron-down'
                "
              />
              <small v-if="errors.commune" class="text-red-500 ml-3">{{ errors.commune }}</small>
            </div>
          </div>
          <div class="col-span-1 md:col-span-2 border h-96 border-orange-500 rounded-md p-4">
            <MarkerMap ref="markerMapRef" v-model:coordinate="coordinate" />
          </div>
          <small class="text-green-600"
            >Tip 1: Ấn chuột trái 2 lần để tạo điểm đánh dấu trên bản đồ.<br />
            Tip 2: Giữ chuột trái rồi kéo đến vị trí chính xác của sân
          </small>
        </div>
      </div>

      <div class="col-span-1 bg-gray-50 p-6 rounded-lg">
        <label for="cover-image" class="font-semibold block mb-2"
          >Hình nền sân bóng <span class="text-red-500">*</span></label
        >
        <FileUpload
          accept="image/*"
          :maxFileSize="1000000"
          :showUploadButton="false"
          @select="onCoverImageFileSelect"
          @clear="onCoverImageClear"
          :pt:content:class="'relative bg-surface-0 dark:bg-surface-900 text-surface-700 dark:text-white/80 p-4 border border-surface-200 dark:border-surface-700 rounded-b-lg'"
        >
          <template #header="{ chooseCallback, clearCallback, files }">
            <div class="flex gap-2">
              <Button @click="chooseCallback()" icon="pi pi-images"> </Button>
              <Button
                @click="clearCallback()"
                icon="pi pi-times"
                outlined
                severity="danger"
                :disabled="!files || files.length === 0"
              ></Button>
            </div>
          </template>
          <template #content="{ files }">
            <div v-if="files.length > 0">
              <span class="mb-2 break-all text-sm">
                {{ files[files.length - 1].name }}
              </span>
              <div class="h-56 w-full">
                <img
                  role="presentation"
                  class="object-contain"
                  :alt="files[files.length - 1].name"
                  :src="coverUrl"
                />
              </div>
            </div>
          </template>
          <template #empty>
            <div class="flex items-center justify-center flex-col">
              <i
                class="pi pi-cloud-upload border-2 rounded-full p-5 text-5xl text-gray-400 border-gray-400"
              ></i>
              <p class="mt-4 mb-0">Tải lên hoặc kéo và thả ảnh bìa vào đây.</p>
            </div>
          </template>
        </FileUpload>
        <small v-if="errors.cover" class="text-red-500 ml-3">{{ errors.cover }}</small>
      </div>

      <div class="col-span-1 bg-gray-50 p-6 rounded-lg">
        <label class="font-semibold block mb-2" for="cover-image"
          >Ảnh đại diện <span class="text-red-500">*</span></label
        >
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
                <Button @click="chooseCallback()" icon="pi pi-images"> </Button>
                <Button
                  @click="clearCallback()"
                  icon="pi pi-times"
                  outlined
                  severity="danger"
                  :disabled="!files || files.length === 0"
                ></Button>
              </div>
            </div>
          </template>
          <template #content="{ files }">
            <div v-if="files.length > 0">
              <span class="font-semibold mb-2 break-all">
                {{ files[files.length - 1].name }}
              </span>
              <div class="w-24 h-24 flex items-center rounded-full overflow-hidden">
                <img
                  role="presentation"
                  class="h-full w-full object-cover"
                  :alt="files[files.length - 1].name"
                  :src="avatarUrl"
                />
              </div>
            </div>
          </template>
          <template #empty>
            <div class="flex items-center justify-center flex-col">
              <i
                class="pi pi-cloud-upload border-2 rounded-full p-5 text-5xl text-gray-400 border-gray-400"
              ></i>
              <p class="mt-4 mb-0">Tải lên hoặc kéo và thả ảnh đại diện vào đây.</p>
            </div>
          </template>
        </FileUpload>

        <small v-if="errors.avatar" class="text-red-500 ml-3">{{ errors.avatar }}</small>
      </div>

      <div class="col-span-full bg-gray-50 p-6 rounded-lg">
        <label for="listImages" class="font-semibold block mb-2">
          Hình ảnh sân bóng (Tối đa 10 ảnh)
        </label>
        <Toast />
        <FileUpload
          name="demo[]"
          id="listImages"
          url="/api/upload"
          :multiple="true"
          @select="onListImageFieldUpload"
          accept="image/*"
          :maxFileSize="1000000"
          :fileLimit="10"
          chooseLabel="Chọn ảnh"
          :showUploadButton="false"
          :showCancelButton="false"
          class="p-4"
          :pt:removeButton:class="'!h-10 !w-10 !bg-gray-300'"
        >
          <template #content="{ files }">
            <div v-if="files.length > 0">
              <div class="flex flex-wrap p-0 sm:p-5 gap-5 justify-center">
                <div
                  v-for="(file, index) of listFieldImages"
                  :key="file + index"
                  class="border border-gray-300 rounded-md p-2"
                >
                  <div class="w-full flex justify-between">
                    <p>Ảnh {{ index + 1 }}</p>
                    <i class="pi pi-times cursor-pointer" @click="onRemoveFieldFile(file)"></i>
                  </div>

                  <img
                    class="object-contain"
                    role="presentation"
                    :alt="'new field '"
                    width="200"
                    height="120"
                    :src="file"
                  />
                </div>
              </div>
            </div>
          </template>
          <template #empty>
            <p>Tải lên hoặc kéo thả ảnh vào đây.</p>
          </template>
        </FileUpload>
      </div>
    </div>

    <div class="flex justify-center mt-8">
      <Button label="Tạo sân" class="px-8 py-3" @click.prevent="submit()" />
    </div>
  </div>

  <PartialFieldDialog
    @close="isShowDetailModal = false"
    :open="isShowDetailModal"
    :is-create="true"
    :field-id="createdFieldId"
  />
</template>

<script setup lang="ts">
import InputField from '@/components/inputs/InputField.vue'
import MarkerMap from '@/components/maps/MarkerMap.vue'
import * as yup from 'yup'
import { ErrorMessage, useForm } from 'vee-validate'
import { useI18n } from 'vue-i18n'
import { useFieldStore } from '@/stores/fieldStore'
import { computed, onMounted, ref, watch } from 'vue'
import type { FieldCreateRequest, FieldUpdateRequest } from '@/api/fieldApi'
import type { AutoCompletePlace, Prediction, Coordinate } from '@/types/Map'
import AutoComplete, { type AutoCompleteItemSelectEvent } from 'primevue/autocomplete'
import { useToast } from 'primevue/usetoast'
import FileUpload, { type FileUploadSelectEvent } from 'primevue/fileupload'
import Button from 'primevue/button'
import router from '@/router'
import TimeInput from '../calendar/TimeInput.vue'
import InputNumber from 'primevue/inputnumber'
import { getVietnameseMoney } from '@/utils/paymentUtils'
import PartialFieldDialog from '../dialogs/PartialFieldDialog.vue'
import { useImageStore } from '@/stores/imageStore'
import { useLocationStore } from '@/stores/locationStore'
import Toast from 'primevue/toast'
import ToggleButton from 'primevue/togglebutton'
import { changeSecondToHour } from '@/utils/timeUtil'
import { VITE_APP_GOONG_API_KEY } from '@/constants/env'

const GOONG_API_KEY = VITE_APP_GOONG_API_KEY

const predictions = ref<Prediction[]>([])
const fieldStore = useFieldStore()
const imageStore = useImageStore()
const locationStore = useLocationStore()
const { t } = useI18n()
const toast = useToast()

const coordinate = ref<Coordinate>({ lat: 0, lng: 0 })
const coverUrl = ref<string>('')
const avatarUrl = ref<string>('')
const listFieldImages = ref<string[]>([])
const listFieldImagesReal = ref<File[]>([])
const isShowDetailModal = ref(false)
const createdFieldId = ref(0)
const selectedProvince = ref()
const selectedDistrict = ref()
const selectedCommune = ref()
const filteredProvince = ref<any[]>()
const filteredDistrict = ref<any[]>()
const filteredCommune = ref<any[]>()
const markerMapRef = ref<InstanceType<typeof MarkerMap> | null>(null)
const isOpenAllDay = ref(false)

const disableNextMap = computed(
  () =>
    errors.value.name !== undefined ||
    errors.value.phoneNumber !== undefined ||
    errors.value.description !== undefined ||
    errors.value.openTime !== undefined ||
    errors.value.closeTime !== undefined ||
    errors.value.cover !== undefined ||
    errors.value.avatar !== undefined
)

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
  name: yup
    .string()
    .required(t('validation.required'))
    .max(255, t('validation.maxLength', { max: 255 })),
  phoneNumber: yup
    .string()
    .matches(/^\(?(\d{3})\)?[-. ]?(\d{3})[-. ]?(\d{4})$/, t('validation.phoneNumber'))
    .required(t('validation.required')),
  description: yup
    .string()
    .required(t('validation.required'))
    .max(1024, t('validation.maxLength', { max: 1024 })),
  openTime: yup
    .number()
    .min(0, t('validation.invalidTime'))
    .max(86_400, t('validation.invalidTime'))
    .required(t('validation.required')),
  closeTime: yup
    .number()
    .min(0, t('validation.invalidTime'))
    .max(86_400, t('validation.invalidTime'))
    .required(t('validation.required')),
  latitude: yup.number().required(),
  address: yup.string().required(t('validation.required')),
  province: yup.string().required(t('validation.required')),
  district: yup.string().required(t('validation.required')),
  commune: yup.string().required(t('validation.required')),
  longitude: yup.number().required(),
  cover: props.isCreate ? yup.mixed().required('Hãy tải ảnh nền lên!') : yup.mixed(),
  avatar: props.isCreate ? yup.mixed().required('Hãy tải đại diện lên!') : yup.mixed(),
  price: yup.number().min(0, 'Tiền sân phải lớn hơn 0').required(t('validation.required')),
  deposit: yup.number().min(0, 'Tiền cọc phải lớn hơn 0').required(t('validation.required'))
})

const { meta, errors, defineField, handleSubmit, resetForm, values, validate } =
  useForm<FieldUpdateRequest>({
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
const [price] = defineField('price')
const [deposit] = defineField('deposit')

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
  if (props.isCreate) {
    resetForm({ values: { cover: undefined, avatar: undefined } })
    isFixedSlot.value = true
    openTime.value = 8 * 60 * 60
    closeTime.value = 23 * 60 * 60
    longitude.value = 0
    latitude.value = 0
    price.value = 400_000
    deposit.value = 200_000
    return
  }
  if (!props.isCreate && props.id) {
    await fieldStore.getFieldById(props.id)
    if (!fieldStore.field) return
    resetForm({ values: { ...fieldStore.field, cover: undefined, avatar: undefined } })
  }
})

const onCoverImageFileSelect = (event: FileUploadSelectEvent) => {
  cover.value = event.files[event.files.length - 1]
  coverUrl.value = URL.createObjectURL(event.files[event.files.length - 1])
}
const onCoverImageClear = () => {
  cover.value = undefined
  coverUrl.value = ''
}

const onAvatarImageFileSelect = (event: FileUploadSelectEvent) => {
  avatar.value = event.files[event.files.length - 1]
  console.log(avatar.value)
  avatarUrl.value = URL.createObjectURL(event.files[event.files.length - 1])
}

const onAvatarImageClear = () => {
  avatar.value = undefined
  avatarUrl.value = ''
}

const onListImageFieldUpload = (event: FileUploadSelectEvent) => {
  if (event.files.length > 10) {
    toast.add({
      severity: 'info',
      summary: 'Thông báo',
      detail: 'Chỉ tối đa 10 ảnh có thể tải lên',
      life: 3000
    })
    return
  }

  listFieldImages.value = event.files.map((x: File) => URL.createObjectURL(x))
  for (const element of event.files) {
    listFieldImagesReal.value.push(element)
  }
}

const onRemoveFieldFile = (event: string) => {
  listFieldImages.value = listFieldImages.value.filter((x) => x !== event)
  listFieldImagesReal.value = listFieldImagesReal.value.filter((x) => x.name !== event)
}

const submit = async () => {
  const validateResult = await validate()

  if (validateResult.valid) {
    if (props.isCreate) {
      let response = await fieldStore.createField(values)
      if (response.success) {
        toast.add({
          severity: 'success',
          summary: 'Tạo sân thành công',
          detail: response.message,
          life: 3000
        })

        console.log(listFieldImagesReal.value)
        imageStore.postImages({
          fieldId: response.data.id,
          images: listFieldImagesReal.value.map((x) => x)
        })

        router.push('/field/list')
      } else {
        toast.add({
          severity: 'error',
          summary: 'Tạo sân thất bại',
          detail: response.message,
          life: 3000
        })
      }
    } else {
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
  } else {
    toast.add({
      severity: 'error',
      summary: 'Thông tin của bạn chưa hợp lệ!',
      life: 3000
    })
  }
}

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

const selectSuggestion = async (event: AutoCompleteItemSelectEvent) => {
  let prediction = predictions.value.find((x) => x.description === event.value)
  if (prediction) {
    const compound = prediction.compound
    const predictionProvince = locationStore.allProvince.find((item) =>
      item.full_name.toLowerCase().includes(compound.province.toLowerCase())
    )
    await onProvinceChange({ value: predictionProvince, originalEvent: new Event('prediction') })
    if (locationStore.allDistrict.length > 0) {
      const predictionDistrict = locationStore.allDistrict.find((item) =>
        item.full_name.toLowerCase().includes(compound.district.toLowerCase())
      )
      await onDistrictChange({ value: predictionDistrict, originalEvent: new Event('prediction') })
    }
    if (locationStore.allDistrict.length > 0) {
      const predictionCommune = locationStore.allCommune.find((item) =>
        item.full_name.toLowerCase().includes(compound.commune.toLowerCase())
      )
      await onCommuneChange({ value: predictionCommune, originalEvent: new Event('prediction') })
    }

    fetchPlaceDetails(prediction.place_id)
  }
}
const searchProvince = async (event: any) => {
  if (locationStore.allProvince.length > 0) {
    filteredProvince.value = locationStore.allProvince.filter((item) =>
      item.full_name.toLowerCase().includes(event.query.toLowerCase())
    )
  } else {
    await locationStore.getAllProvinces()
  }
}

const onProvinceChange = async (event: AutoCompleteItemSelectEvent) => {
  selectedProvince.value = event.value
  setMapCenter(selectedProvince.value.longitude, selectedProvince.value.latitude)
  selectedDistrict.value = null
  locationStore.allDistrict = []
  filteredDistrict.value = []
  selectedCommune.value = null
  locationStore.allCommune = []
  filteredCommune.value = []
  console.log('selectedProvince')
  await searchDistrict(null)
}

const searchDistrict = async (event: any) => {
  if (selectedProvince.value) {
    if (locationStore.allDistrict.length > 0) {
      filteredDistrict.value = locationStore.allDistrict.filter((item) =>
        item.full_name.toLowerCase().includes(event.query.toLowerCase())
      )
    } else {
      await locationStore.getDistrictsByProvinceId(selectedProvince.value.id)
    }
  }
}

const onDistrictChange = async (event: AutoCompleteItemSelectEvent) => {
  selectedDistrict.value = event.value
  setMapCenter(selectedDistrict.value.longitude, selectedDistrict.value.latitude)
  selectedCommune.value = null
  locationStore.allCommune = []
  filteredCommune.value = []
  await searchCommune(null)
}

const searchCommune = async (event: any) => {
  if (selectedDistrict.value) {
    if (locationStore.allCommune.length > 0) {
      filteredCommune.value = locationStore.allCommune.filter((item) =>
        item.full_name.toLowerCase().includes(event.query.toLowerCase())
      )
    } else {
      await locationStore.getCommunesByDistrictId(selectedDistrict.value.id)
    }
  }
}

const onCommuneChange = async (event: AutoCompleteItemSelectEvent) => {
  selectedCommune.value = event.value
}
const responsiveOptions = ref([
  {
    breakpoint: '1300px',
    numVisible: 4
  },
  {
    breakpoint: '575px',
    numVisible: 1
  }
])
watch(
  selectedProvince,
  (newVal) => {
    province.value = newVal?.full_name
  },
  { immediate: true }
)

watch(
  selectedDistrict,
  (newVal) => {
    district.value = newVal?.full_name
  },
  { immediate: true }
)

watch(
  selectedCommune,
  (newVal) => {
    commune.value = newVal?.full_name
  },
  { immediate: true }
)

onMounted(async () => {
  await locationStore.fetchCurrentLocationDetails()
  filteredProvince.value = locationStore.allProvince
  filteredDistrict.value = locationStore.allDistrict
  filteredCommune.value = locationStore.allCommune
  selectedProvince.value = locationStore.currentProvince
  selectedDistrict.value = locationStore.currentDistrict
  if (selectedDistrict.value) {
    setMapCenter(selectedDistrict.value.longitude, selectedDistrict.value.latitude)
  } else if (selectedProvince.value) {
    setMapCenter(selectedProvince.value.longitude, selectedProvince.value.latitude)
  }
})
const setMapCenter = (longitude: number, latitude: number) => {
  if (markerMapRef.value) {
    // @ts-ignore
    markerMapRef.value.setCenter(longitude, latitude)
  }
}
const handleOpenAllDayToggle = () => {
  if (isOpenAllDay.value) {
    openTime.value = 0 // 00:00 in seconds
    closeTime.value = 86400 // 24:00 in seconds (86400 seconds in a day)
  } else {
    // Reset to default times or keep the last set times
    openTime.value = 8 * 3600 // 08:00 in seconds
    closeTime.value = 23 * 3600 // 23:00 in seconds
  }
}
</script>
