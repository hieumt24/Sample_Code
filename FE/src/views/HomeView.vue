<template>
  <div class="flex flex-col bg-header bg-contain !object-cover pt-24 h-auto">
    <!-- Search bar -->
    <div class="w-full h-[400px] mx-auto">
      <div class="bg-white p-4 bg-opacity-20 mt-4 w-full lg:w-[1000px] mx-auto rounded-md py-6">
        <div class="w-[70%] mx-auto">
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-6 text-white">
            <div class="lg:col-span-1">
              <CustomCalendar
                class="w-full"
                label="Chọn ngày"
                v-model="selectedDate"
                isBlueBorder
              />
            </div>

            <div class="lg:col-span-1">
              <TimeInput
                class="w-full"
                label="Chọn giờ"
                v-model="selectedTime"
                isBlueBorder
                :initValue="changeSecondToHour(selectedTime.toString())"
              />
            </div>

            <div class="flex flex-col lg:col-span-1">
              <span class="font-medium">Thời lượng</span>
              <Dropdown
                class="w-full rounded-lg border-[3px] border-blue-300"
                v-model="selectedDuration"
                :options="durationOptions"
                optionLabel="label"
                optionValue="value"
                placeholder="Select Duration"
              />
            </div>

            <div class="flex flex-col lg:col-span-1">
              <span>Chọn tỉnh</span>
              <AutoComplete
                forceSelection
                v-model="selectedProvince"
                :suggestions="filteredProvince"
                @complete="searchProvince"
                @item-select="onProvinceChange"
                optionLabel="full_name"
                dropdown
                :disabled="locationStore.isProvinceLoading"
                :dropdownIcon="
                  locationStore.isProvinceLoading
                    ? 'pi pi-spinner pi-spin animate-spin'
                    : 'pi pi-chevron-down'
                "
                :pt:input:class="'text-base leading-none appearance-none rounded-md rounded-tr-none rounded-br-none m-0 py-2 px-3 text-surface-700 dark:text-white/80 border bg-surface-0 dark:bg-surface-950 border-surface-300 dark:border-surface-700 focus:outline-none focus:outline-offset-0 focus:ring-1 focus:ring-primary-500 dark:focus:ring-primary-400 focus:z-10 transition-colors duration-200 w-full'"
                class="border-[3px] !border-blue-300 rounded-lg"
              />
            </div>

            <div class="flex flex-col lg:col-span-1">
              <span>Chọn huyện</span>
              <AutoComplete
                forceSelection
                v-model="selectedDistrict"
                :suggestions="filteredDistrict"
                @complete="searchDistrict"
                @item-select="onDistrictChange"
                optionLabel="full_name"
                dropdown
                :disabled="locationStore.isDistrictLoading || !selectedProvince"
                :dropdownIcon="
                  locationStore.isDistrictLoading
                    ? 'pi pi-spinner pi-spin animate-spin'
                    : 'pi pi-chevron-down'
                "
                :pt:input:class="'text-base leading-none appearance-none rounded-md rounded-tr-none rounded-br-none m-0 py-2 px-3 text-surface-700 dark:text-white/80 border bg-surface-0 dark:bg-surface-950 border-surface-300 dark:border-surface-700 focus:outline-none focus:outline-offset-0 focus:ring-1 focus:ring-primary-500 dark:focus:ring-primary-400 focus:z-10 transition-colors duration-200 w-full'"
                class="rounded-lg border-[3px] !border-blue-300"
              />
            </div>

            <div class="flex items-end lg:col-span-1">
              <Button
                class="w-full flex flex-col !bg-blue-300 border-[3px] !border-blue-400"
                value="search"
                @click="submit"
              >
                <div class="flex items-center justify-center">
                  <i class="pi pi-search mr-2" />Tìm kiếm
                </div>
              </Button>
            </div>
          </div>
        </div>
        <ProgressBar
          v-if="fieldStoreUser.loadingSuperSearch"
          mode="indeterminate"
          style="height: 6px"
        ></ProgressBar>
      </div>

      <div class="items-center w-full flex flex-col pt-16">
        <h1 class="text-3xl font-extrabold text-white mb-2">Tìm sân, hợp đối trong tầm tay</h1>

        <div class="mx-auto flex items-center">
          <InputText
            type="text"
            class="bg-transparent text-white !rounded-full w-96 mx-auto border-2 border-white !placeholder-slate-300"
            v-model="freeSearchKeyword"
            placeholder="Tìm bằng tên, địa chỉ, số điện thoại"
          />
          <i
            class="pi pi-search ml-2 text-white cursor-pointer"
            @click="onSearch"
            style="font-size: 1.5rem"
          ></i>
        </div>
      </div>
    </div>

    <!-- Map -->
    <div
      class="bg-[#FEF1F6] w-full h-[650px] rounded-t-[50px] p-5 flex flex-col md:flex-row lg:px-24 lg:pt-24"
    >
      <div class="w-full md:w-1/2 mx-auto h-full">
        <div class="flex justify-between items-end mb-2 flex-col lg:flex-row">
          <div class="flex flex-col">
            <h1 class="font-semibold text-xl">Tìm sân bóng trên bản đồ</h1>
            <h4 class="text-green-600 text-sm">*Kéo bản đồ để xem sân bóng</h4>
          </div>
          <div class="flex mt-2">
            <div class="flex flex-col mx-1">
              <span>Chọn tỉnh</span>
              <AutoComplete
                forceSelection
                v-model="selectedProvince"
                :suggestions="filteredProvince"
                @complete="searchProvince"
                @item-select="onProvinceChange"
                optionLabel="full_name"
                dropdown
                :disabled="locationStore.isProvinceLoading"
                :dropdownIcon="
                  locationStore.isProvinceLoading
                    ? 'pi pi-spinner pi-spin animate-spin'
                    : 'pi pi-chevron-down'
                "
              />
            </div>

            <div class="flex flex-col mx-1">
              <span>Chọn huyện</span>
              <AutoComplete
                forceSelection
                v-model="selectedDistrict"
                :suggestions="filteredDistrict"
                @complete="searchDistrict"
                @item-select="onDistrictChange"
                optionLabel="full_name"
                dropdown
                :disabled="locationStore.isDistrictLoading || !selectedProvince"
                :dropdownIcon="
                  locationStore.isDistrictLoading
                    ? 'pi pi-spinner pi-spin animate-spin'
                    : 'pi pi-chevron-down'
                "
              />
            </div>
          </div>
        </div>
        <div class="p-2 w-full rounded-md overflow-hidden border border-orange-400 bg-white">
          <MultipleFieldMarkerMap ref="multipleFieldMarkerMapRef" />
        </div>
      </div>
    </div>
  </div>
  <hr />

  <div class="px-10 md:px-28 lg:px-44">
    <ListBlogPosts />
  </div>

  <main class="px-44 mt-20">
    <div class="my-20">
      <h1 class="font-semibold text-xl text-center w-full text-gray-600 whitespace-nowrap mx-2">
        Những sân được đánh giá cao nhất
        <i class="pi pi-crown text-green-400" style="font-size: 1rem"></i>
      </h1>
      <Carousel
        :value="fieldStoreUser.mostStarFields"
        :numVisible="3"
        :numScroll="1"
        circular
        :autoplayInterval="2000"
      >
        <template #item="slotProps">
          <div
            class="bg-white w-80 rounded-lg shadow-md overflow-hidden hover:shadow-xl transition duration-300 cursor-pointer mx-auto"
            @click="$router.push('/field/' + slotProps.data.id)"
          >
            <div class="relative h-44">
              <img
                :src="slotProps.data?.cover"
                :alt="slotProps.data?.name"
                class="w-full h-full object-cover"
              />
              <div class="absolute top-0 left-0 bg-blue-500 text-white px-2 py-1 rounded-br-lg">
                {{ slotProps.data?.province }}
              </div>
              <div
                class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-4"
              >
                <h2 class="text-white font-bold text-xl">{{ slotProps.data?.name }}</h2>
              </div>
            </div>
            <div class="p-4 h-28">
              <p class="text-gray-600 text-sm mb-2 line-clamp-2">
                {{ slotProps.data?.address }}
              </p>
              <div class="flex justify-between items-center mb-2">
                <span>{{ slotProps.data?.price }} vnd</span>
                <Badge :value="`${slotProps.data?.numberOfBookings} lượt`" severity="success" />
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-500"
                  >{{ slotProps.data?.openTime }} - {{ slotProps.data?.closeTime }}</span
                >
                <span v-if="slotProps.data?.rating" class="flex items-center">
                  {{ slotProps.data?.rating }}
                  <i class="pi pi-star-fill text-green-400 ml-2" style="font-size: 1rem" />
                </span>
                <span v-else>Không có đánh giá</span>
              </div>
            </div>
            <div class="px-4 pb-4">
              <Button
                label="Chi tiết"
                icon="pi pi-external-link"
                class="p-button-outlined w-full"
                @click="$router.push('/user/field/' + slotProps.data?.id)"
              />
            </div>
          </div>
        </template>
      </Carousel>
    </div>
  </main>
</template>

<script setup lang="ts">
import AutoComplete, { type AutoCompleteItemSelectEvent } from 'primevue/autocomplete'
import { ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import MultipleFieldMarkerMap from '@/components/maps/MultipleFieldMarkerMap.vue'
import CustomCalendar from '@/components/calendar/CustomCalendar.vue'
import { useFieldStoreUser } from '@/stores/fieldStoreUser'
import { onMounted } from 'vue'
import Badge from 'primevue/badge'
import Carousel from 'primevue/carousel'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import router from '@/router'
import { useLocationStore } from '@/stores/locationStore'
import ProgressBar from 'primevue/progressbar'
import TimeInput from '@/components/calendar/TimeInput.vue'
import ListBlogPosts from '@/components/information/ListBlogPosts.vue'
import Dropdown from 'primevue/dropdown'
import { changeSecondToHour, getSecondsFromMidnightPlus2Hours } from '@/utils/timeUtil'
import { getCurrentDate } from '@/utils/dateUtil'

const { t } = useI18n()

const fieldStoreUser = useFieldStoreUser()
const locationStore = useLocationStore()

const filteredProvince = ref<any[]>([])
const filteredDistrict = ref<any[]>([])
const selectedProvince = ref()
const selectedDistrict = ref()
const selectedDate = ref(getCurrentDate())
const selectedTime = ref(getSecondsFromMidnightPlus2Hours())
const freeSearchKeyword = ref('')
const multipleFieldMarkerMapRef = ref<InstanceType<typeof MultipleFieldMarkerMap> | null>(null)
const selectedDuration = ref(5400)
const durationOptions = [
  { label: '60 phút', value: 3600 },
  { label: '90 phút', value: 5400 },
  { label: '120 phút', value: 7200 }
]
const submit = async () => {
  router.push({ name: 'guest-search' })
  await fieldStoreUser.getSuperSearchFieldToFreeWordPage()
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

const onProvinceChange = (event: AutoCompleteItemSelectEvent) => {
  selectedProvince.value = event.value
  selectedDistrict.value = null
  locationStore.allDistrict = []
  filteredDistrict.value = []
  setMapCenter(event.value)
  searchDistrict(null)
}

const onDistrictChange = (event: AutoCompleteItemSelectEvent) => {
  setMapCenter(event.value)
}

const setMapCenter = (value: any) => {
  if (multipleFieldMarkerMapRef.value) {
    if (value.longitude && value.latitude) {
      multipleFieldMarkerMapRef.value.setCenter(value.longitude, value.latitude)
    }
  }
}

const onSearch = async () => {
  if (router.currentRoute.value.name === 'free-search') return
  router.push({ name: 'free-search' })
  await fieldStoreUser.searchFieldListFreeWord(freeSearchKeyword.value)
}
watch(
  () => selectedProvince.value,
  async (newVal) => {
    fieldStoreUser.superSearch.province = newVal.full_name
  }
)
watch(
  () => selectedDistrict.value,
  async (newVal) => {
    fieldStoreUser.superSearch.district = newVal.full_name
  }
)
watch(
  [selectedDate, selectedTime],
  ([newDate, newTime]) => {
    fieldStoreUser.superSearch.startDateTime = `${newDate}T${changeSecondToHour(newTime.toString())}`
  },
  { immediate: true }
)
onMounted(async () => {
  fieldStoreUser.getMostStarBookingFields()
  await locationStore.fetchCurrentLocationDetails()
  if (locationStore.enableLocation) {
    filteredProvince.value = locationStore.allProvince
    filteredDistrict.value = locationStore.allDistrict
    selectedProvince.value = locationStore.currentProvince
    selectedDistrict.value = locationStore.currentDistrict
  }
})
</script>

<style scoped>
.bg-header {
  background-image: url('/bg-user.jpg');
}
</style>
