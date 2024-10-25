<template>
  <Dialog
    :visible="open"
    :header="'Tìm đối'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    :style="{ width: '60rem' }"
  >
    <div class="relative">
      <div
        v-if="opponentFindingStore.dialogActionLoading"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <form @submit.prevent="submitForm">
        <div class="my-4">
          <ToggleButton
            v-model="hasField"
            onLabel="Có sân"
            offLabel="Không có sân"
            onIcon="pi pi-check"
            offIcon="pi pi-times"
            class="w-full"
          />
        </div>
        <div v-if="hasField" class="my-4">
          <div class="flex flex-col mb-4">
            <label for="fieldName" class="mb-2 font-semibold">Tên sân</label>
            <InputText id="fieldName" v-model="values.fieldName" class="p-2 border rounded-md" />
            <ErrorMessage name="fieldName" class="text-red-500" />
          </div>
          <div class="flex flex-col mb-4">
            <label for="fieldAddress" class="mb-2 font-semibold">Địa chỉ sân</label>
            <InputText
              id="fieldAddress"
              v-model="values.fieldAddress"
              class="p-2 border rounded-md"
            />
            <ErrorMessage name="fieldAddress" class="text-red-500" />
          </div>
        </div>
        <div class="my-4 flex flex-row justify-between">
          <div class="flex flex-col flex-grow">
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
            <ErrorMessage name="fieldProvince" class="text-red-500" />
          </div>
          <div class="flex flex-col flex-grow mx-2">
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
            <ErrorMessage name="fieldDistrict" class="text-red-500" />
          </div>
          <div class="flex flex-col flex-grow">
            <label class="font-semibold" for="district">Phường Xã</label>
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
            <ErrorMessage name="fieldCommune" class="text-red-500" />
          </div>
        </div>

        <div class="my-4">
          <label class="font-semibold" for="startTime"
            >Thời gian <span class="text-red-500">*</span></label
          >
          <div id="startTime" class="flex flex-row justify-between">
            <input
              type="datetime-local"
              v-model="startDateTimeInput"
              class="w-1/2 border border-gray-300 p-2"
            />
            <Dropdown
              v-model="selectedDuration"
              :options="durationOptions"
              optionLabel="label"
              optionValue="value"
              placeholder="Select Duration"
              class="w-1/2 ml-2"
            />
          </div>
          <div v-if="timeError" class="text-red-500">{{ timeError }}</div>
        </div>

        <Message
          :closable="false"
          v-if="opponentFindingStore.hasOverlapOpponentFinding"
          severity="error"
          >Bạn đã tạo một bài đăng trong thời gian này nên không thể tạo tiếp!</Message
        >
        <Message
          :closable="false"
          v-if="opponentFindingStore.hasOverlapAcceptedRequest"
          severity="error"
          >Một trong những yêu cầu của bạn đã được chấp nhận trong khoảng thời gian này. <br />
          Do đó bạn không thể tạo bài đăng trùng lặp.
        </Message>
        <div class="my-4 flex flex-col">
          <label for="content" class="mb-2 font-semibold"
            >Nội dung <span class="text-red-500">*</span></label
          >
          <Textarea
            v-model="content"
            name="content"
            as="textarea"
            rows="10"
            cols="30"
            class="p-2 border rounded-md"
          />
          <ErrorMessage name="content" class="text-red-500" />
        </div>
        <Button
          :disabled="
            opponentFindingStore.hasOverlapOpponentFinding ||
            opponentFindingStore.hasOverlapAcceptedRequest ||
            timeError !== ''
          "
          type="submit"
          label="Đăng"
          class="w-full mt-4 bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
        />
      </form>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { defineProps, defineEmits, ref, watch, onMounted } from 'vue'
import { useForm, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { type OpponentFindingWithoutBookingCreateRequest } from '@/api/opponentFindingApi'
import { useToast } from 'primevue/usetoast'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Calendar from 'primevue/calendar'
import { useOpponentFindingStore } from '@/stores/opponentFindingStore'
import { addDurationToDate, changeTimeToSeconds, getTimeFromDate } from '@/utils/timeUtil'
import Textarea from 'primevue/textarea'
import ToggleButton from 'primevue/togglebutton'
import Message from 'primevue/message'
import { getDateFormatted, getDateTimeFormatted } from '@/utils/dateUtil'
import ProgressSpinner from 'primevue/progressspinner'
import { useLocationStore } from '@/stores/locationStore'
import type { AutoCompleteItemSelectEvent } from 'primevue/autocomplete'
import AutoComplete from 'primevue/autocomplete'
import Dropdown from 'primevue/dropdown'

const props = defineProps<{
  open: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'onCreateSuccessfully'): void
}>()

const toast = useToast()
const opponentFindingStore = useOpponentFindingStore()
const locationStore = useLocationStore()

const timeError = ref('')
const hasField = ref(false)
const selectedProvince = ref()
const selectedDistrict = ref()
const selectedCommune = ref()
const filteredProvince = ref<any[]>([])
const filteredDistrict = ref<any[]>([])
const filteredCommune = ref<any[]>([])
// Get the current date and time
const currentDate = new Date()
// Add 2 hours to the current date and time
currentDate.setHours(currentDate.getHours() + 2)
currentDate.setMinutes(0)
const startDateTimeInput = ref(getDateTimeFormatted(currentDate))
const selectedDuration = ref(5400)
const durationOptions = [
  { label: '60 phút', value: 3600 },
  { label: '90 phút', value: 5400 },
  { label: '120 phút', value: 7200 }
]

const schema = yup.object({
  content: yup.string().required('Nội dung là bắt buộc'),
  fieldName: yup.string(),
  fieldAddress: yup.string(),
  fieldProvince: yup.string().required('Tỉnh Thành là bắt buộc'),
  fieldDistrict: yup.string().required('Quận Huyện là bắt buộc'),
  fieldCommune: yup.string(),
  startTime: yup.number().required('Thời gian bắt đầu là bắt buộc'),
  endTime: yup.number().required('Thời gian kết thúc là bắt buộc'),
  date: yup.date().required('Ngày diễn ra là bắt buộc')
})
const calculateInitialValues = () => {
  const startDateTime = new Date(startDateTimeInput.value)
  const duration = selectedDuration.value
  const endDateTime = addDurationToDate(startDateTime, duration)
  const startTime = changeTimeToSeconds(getTimeFromDate(startDateTime)).toString()
  const endTime = changeTimeToSeconds(getTimeFromDate(endDateTime)).toString()
  const date = getDateFormatted(startDateTimeInput.value.toString())
  return {
    date: date,
    startTime,
    endTime
  }
}
const initialValues = calculateInitialValues()

const { handleSubmit, errors, defineField, resetForm, values } =
  useForm<OpponentFindingWithoutBookingCreateRequest>({
    validationSchema: schema,
    initialValues
  })

const [content] = defineField('content')
const [fieldName] = defineField('fieldName')
const [fieldAddress] = defineField('fieldAddress')
const [fieldProvince] = defineField('fieldProvince')
const [fieldDistrict] = defineField('fieldDistrict')
const [fieldCommune] = defineField('fieldCommune')
const [startTime] = defineField('startTime')
const [endTime] = defineField('endTime')
const [date] = defineField('date')

watch([startDateTimeInput, selectedDuration], async () => {
  await validateTime()
})

watch(
  () => props.open,
  (newVal) => {
    if (newVal) {
      validateTime()
    }
  }
)
const validateTime = async () => {
  const currentDate = new Date()
  const startDateTime = new Date(startDateTimeInput.value)
  date.value = getDateFormatted(startDateTimeInput.value.toString())
  if (startDateTime < currentDate) {
    timeError.value = 'Thời gian bạn chọn đã qua'
  } else {
    timeError.value = ''
  }

  if (startDateTimeInput.value && selectedDuration.value) {
    const endDateTime = addDurationToDate(startDateTime, selectedDuration.value)
    startTime.value = changeTimeToSeconds(getTimeFromDate(startDateTime)).toString()
    endTime.value = changeTimeToSeconds(getTimeFromDate(endDateTime)).toString()

    await opponentFindingStore.checkOverlapOpponentFinding({
      date: getDateFormatted(startDateTimeInput.value.toString()),
      startTime: getTimeFromDate(startDateTime),
      endTime: getTimeFromDate(endDateTime)
    })
    await opponentFindingStore.checkAcceptedRequestWithParams({
      date: getDateFormatted(startDateTimeInput.value.toString()),
      startTime: getTimeFromDate(startDateTime),
      endTime: getTimeFromDate(endDateTime)
    })
  }
}

watch([selectedProvince, selectedDistrict, selectedCommune], () => {
  if (selectedProvince.value) {
    fieldProvince.value = selectedProvince.value.full_name
  } else {
    fieldProvince.value = ''
  }
  if (selectedDistrict.value) {
    fieldDistrict.value = selectedDistrict.value.full_name
  } else {
    fieldDistrict.value = ''
  }
  if (selectedCommune.value) {
    fieldCommune.value = selectedCommune.value.full_name
  } else {
    fieldCommune.value = ''
  }
})

const submitForm = handleSubmit(async (values) => {
  try {
    const response = await opponentFindingStore.createOpponentFindingWithoutBooking(values)

    if (response.success) {
      toast.add({ severity: 'success', detail: 'Bài đăng đã được tạo thành công!', life: 3000 })
      emit('onCreateSuccessfully')
      emit('close')
    } else {
      toast.add({
        severity: 'error',
        detail: 'Thao tác không thành công. Vui lòng thử lại!',
        life: 3000
      })
    }
  } catch (error) {
    toast.add({ severity: 'error', detail: 'Đã xảy ra lỗi. Vui lòng thử lại!', life: 3000 })
  }
})

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
  selectedDistrict.value = null
  locationStore.allDistrict = []
  filteredDistrict.value = []
  selectedCommune.value = null
  locationStore.allCommune = []
  filteredCommune.value = []
  searchDistrict(null)
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
  selectedCommune.value = null
  locationStore.allCommune = []
  filteredCommune.value = []
  searchCommune(null)
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

onMounted(async () => {
  await locationStore.fetchCurrentLocationDetails()
  if (locationStore.enableLocation) {
    filteredProvince.value = locationStore.allProvince
    filteredDistrict.value = locationStore.allDistrict
    filteredCommune.value = locationStore.allCommune
    selectedProvince.value = locationStore.currentProvince
    selectedDistrict.value = locationStore.currentDistrict
  }
})
</script>
