<template>
  <div class="mx-20">
    <div class="h-56 w-full relative mb-20">
      <img
        class="object-cover h-full w-full rounded-md shadow-lg"
        :src="fieldStore?.field?.cover"
        alt="Hinh san 1"
      />
      <div class="absolute h-20 w-full left-20 -bottom-10 flex">
        <div class="rounded-full w-20 h-20 overflow-hidden ring-2 ring-gray-500">
          <img
            class="object-cover h-full w-full"
            :src="
              fieldStore?.field?.avatar || fieldStore?.field?.avatar === ''
                ? '/default-field-avt.png'
                : fieldStore?.field?.avatar
            "
            alt="Hinh san 1"
          />
        </div>
        <div class="h-full flex items-end pl-5 w-full justify-start">
          <div class="flex justify-start items-end">
            <span class="text-2xl font-semibold mr-2">{{ fieldStore?.field?.name }}</span>
          </div>
          <Button
            label="Xem bản đồ"
            class="!bg-orange-300 w-52 ml-5"
            @click="isShowMapDialog = true"
          ></Button>
        </div>
      </div>
      <div class="absolute right-0 -bottom-9 px-2 flex items-center">
        <span
          v-if="
            !favoriteFieldStore.favoriteFieldIds.includes('[' + routers.params.id.toString() + ']')
          "
        >
          <i
            class="pi pi-heart text-red-300 cursor-pointer mt-1 mr-2"
            @click="onAddFavorite"
            style="font-size: 1.2rem"
          ></i>
        </span>
        <span
          v-if="
            favoriteFieldStore.favoriteFieldIds.includes('[' + routers.params.id.toString() + ']')
          "
          class="flex items-center text-red-300"
          @click="removeFromFavorite"
        >
          (Đã thích)
          <i
            class="pi pi-heart-fill text-red-300 cursor-pointer ml-2"
            style="font-size: 1.2rem"
          ></i>
        </span>
        <i
          class="pi pi-flag text-red-300 cursor-pointer"
          style="font-size: 1.2rem"
          @click="isShowReportDialog = true"
        ></i>
      </div>
    </div>

    <!-- General Information -->
    <div class="flex flex-col md:flex-row md:justify-between bg-white mb-4 p-5">
      <div class="flex flex-col w-full lg:w-1/2">
        <span class="font-semibold">Địa chỉ: {{ fieldStore?.field?.address }}</span>
        <span class="font-semibold">Số điện thoại: {{ fieldStore?.field?.phoneNumber }}</span>
        <span>Mô tả: {{ fieldStore?.field?.description }}</span>
      </div>

      <div>
        <h3 class="text-xl font-bold mb-2 w-full lg:w-1/2">Hình ảnh sân</h3>
        <div class="card flex justify-center">
          <Galleria
            v-model:activeIndex="activeIndex"
            v-model:visible="displayCustom"
            :value="images"
            :responsiveOptions="responsiveOptions"
            :numVisible="7"
            containerStyle="max-width: 850px"
            :circular="true"
            :fullScreen="true"
            :showItemNavigators="true"
            :showThumbnails="false"
          >
            <template #item="slotProps">
              <img
                :src="slotProps.item.itemImageSrc"
                :alt="slotProps.item.alt"
                style="width: 100%; display: block"
              />
            </template>
            <template #thumbnail="slotProps">
              <img
                :src="slotProps.item.thumbnailImageSrc"
                :alt="slotProps.item.alt"
                style="display: block"
              />
            </template>
          </Galleria>

          <div v-if="images" class="grid grid-cols-5 w-full gap-2">
            <div v-for="(image, index) of images" :key="index" class="col-span-1">
              <div class="flex items-center">
                <img
                  :src="image.thumbnailImageSrc"
                  :alt="image.alt"
                  style="cursor: pointer"
                  class="object-cover w-36 h-28"
                  @click="imageClick(index)"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Rating -->
    <div class="grid grid-cols-1 xl:grid-cols-2 bg-white p-2">
      <div class="p-2 rounded-md pl-4 pt-6">
        <div class="mb-4">
          <span class="font-semibold text-xl text-gray-500">Đánh giá sân bóng</span>
        </div>
        <div v-if="!fieldStore.field?.rating && !rateStore.loading">
          <h1 class="text-lg text-red-400">Chưa có đánh giá</h1>
        </div>

        <div v-if="rateStore.loading">
          <i class="pi pi-spin pi-spinner" style="font-size: 2rem"></i>
        </div>

        <div class="bg-[#FFFBF8] w-full h-40 border border-gray-100 mb-5 flex justify-between">
          <div class="p-2 pt-5">
            <div>
              <i class="pi pi-star-fill text-green-400 mr-2" style="font-size: 1.6rem" />
              <span class="font-semibold text-2xl">{{ rateStore.starByField?.average }}</span>
              <span class="text-base"> trên 5</span>
            </div>
            <h1 class="text-base">
              Tổng cộng
              <span class="font-semibold"> {{ rateStore.starByField?.totalRate }} </span>
              đánh giá
            </h1>
          </div>
          <div class="flex flex-col p-4 items-end">
            <div class="flex items-center">
              <span class="font-semibold flex-none pb-0.5">5 sao</span>
              <span class="text-sm flex-none pb-0.5 w-8">({{ rateStore.starByField.star5 }})</span>
              <ProgressBar
                class="ml-4 w-60"
                :value="
                  Math.round((rateStore.starByField.star5 / rateStore.starByField.totalRate) * 100)
                "
              />
            </div>
            <div class="flex items-center">
              <span class="font-semibold flex-none pb-0.5">4 sao</span>
              <span class="text-sm flex-none pb-0.5 w-8">({{ rateStore.starByField.star4 }})</span>
              <ProgressBar
                class="ml-4 w-60"
                :value="
                  Math.round((rateStore.starByField.star4 / rateStore.starByField.totalRate) * 100)
                "
              />
            </div>
            <div class="flex items-center">
              <span class="font-semibold flex-none pb-0.5">4 sao</span>
              <span class="text-sm flex-none pb-0.5 w-8">({{ rateStore.starByField.star3 }})</span>
              <ProgressBar
                class="ml-4 w-60"
                :value="
                  Math.round((rateStore.starByField.star3 / rateStore.starByField.totalRate) * 100)
                "
              />
            </div>
            <div class="flex items-center">
              <span class="font-semibold flex-none pb-0.5">2 sao</span>
              <span class="text-sm flex-none pb-0.5 w-8">({{ rateStore.starByField.star2 }})</span>
              <ProgressBar
                class="ml-4 w-60"
                :value="
                  Math.round((rateStore.starByField.star2 / rateStore.starByField.totalRate) * 100)
                "
              />
            </div>
            <div class="flex items-center">
              <span class="font-semibold flex-none pb-0.5">1 sao</span>
              <span class="text-sm flex-none pb-0.5 w-8">({{ rateStore.starByField.star1 }})</span>
              <ProgressBar
                class="ml-4 w-60"
                :value="
                  Math.round((rateStore.starByField.star1 / rateStore.starByField.totalRate) * 100)
                "
              />
            </div>
          </div>
        </div>

        <div
          v-for="rate in rateStore.rates"
          :key="rate.bookingId"
          class="flex flex-col mb-2 p-2 bg-white border-b w-full h-fit pb-5 border-gray-300"
        >
          <div class="flex w-full justify-between">
            <Chip :label="rate.raterName" :image="rate.raterAvatar ?? '/noavatar.png'" />

            <div class="flex items-start p-1">
              <Rating readonly :model-value="rate.star" :cancel="false" />
            </div>
          </div>
          <div class="flex justify-between">
            <span v-if="rate.comment" class="w-4/5 mt-2 p-1">
              <p class="line-clamp-3">{{ rate.comment }}</p>
            </span>
            <div class="flex items-end">
              <p>{{ formatDateYYYYHHHHH(rate.createdAt) }}</p>
            </div>
          </div>
        </div>
        <div
          v-if="
            rateStore.total > rateStore.searchRateByField.offset + rateStore.searchRateByField.limit
          "
        >
          <span>xem thêm</span>
        </div>
      </div>

      <div class="mt-10 p-2 flex flex-wrap bg-green-50 rounded-md">
        <div
          v-for="field in partialFieldStore.partialFields"
          :key="field.id"
          class="bg-white h-20 w-72 m-2 rounded-md flex justify-between items-center p-2 shadow-md hover:shadow-xl hover:text-gray-600 cursor-pointer"
        >
          <div class="flex flex-col p-2">
            <span class="font-semibold">Tên: {{ field.name }}</span>
            <span class="font-semibold">Mô tả: {{ field.description }}</span>
          </div>
          <div class="flex items-center">
            <ActionButton
              class="bg-green-400"
              @click="onShowModal(field.id)"
              :loading="meta.pending"
              :is-outlined="true"
              value="Xem lịch"
            />
          </div>
        </div>
      </div>
    </div>

    <div class="mt-10 text-xl mb-1 text-gray-500 font-semibold">
      <h1>Có thể cũng phù hợp với bạn</h1>
    </div>
    <div class="w-full bg-white">
      <div
        class="w-full bg-white grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5 p-5"
      >
        <div
          v-for="field in fieldStoreUser.recommendedFields.filter(
            (f) => f.id !== fieldStore.field?.id
          )"
          :key="field.id"
          class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition duration-300 cursor-pointer"
        >
          <div class="relative h-44">
            <img :src="field.cover" :alt="field.name" class="w-full h-full object-cover" />
            <div class="absolute top-0 left-0 bg-blue-500 text-white px-2 py-1 rounded-br-lg">
              {{ field.province }}
            </div>
            <div
              class="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-4"
            >
              <h2 class="text-white font-bold text-xl">{{ field.name }}</h2>
            </div>
          </div>
          <div class="p-4 h-20">
            <p class="text-gray-600 text-sm mb-2 line-clamp-2">{{ field.address }}</p>
            <div class="flex justify-between items-center mb-2">
              <span>{{ field.price }} vnd</span>
              <Badge :value="`${field.numberOfBookings} lượt`" severity="success" />
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-500"
                >{{ field.openTime }} - {{ field.closeTime }}</span
              >
              <span v-if="field.rating" class="flex items-center">
                {{ field.rating }}
                <i class="pi pi-star-fill text-green-400 ml-2" style="font-size: 1rem" />
              </span>
              <span v-else>Không có đánh giá</span>
            </div>
          </div>
          <div class="px-4 pb-4 bottom-0">
            <Button
              label="Chi tiết"
              icon="pi pi-external-link"
              class="p-button-outlined w-full"
              @click="onNavigateToField(field.id)"
            />
          </div>
        </div>
      </div>
    </div>
  </div>

  <PartialBookingDialog
    :open="isShowDetailModal"
    :is-fixed-slot="fieldStore.field?.isFixedSlot ?? false"
    :partial-id="selectedPartialFieldId"
    :field-id="Number(routers.params.id)"
    :field-name="partialFieldStore.partialField?.fieldName"
    @close="isShowDetailModal = false"
  />

  <ReportCreateDialog
    :open="isShowReportDialog"
    :field-id="Number(routers.params.id)"
    @close="isShowReportDialog = false"
  />

  <MapFieldDetailDialog :open="isShowMapDialog" @close="isShowMapDialog = false" />
</template>

<script setup lang="ts">
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useFieldStore } from '@/stores/fieldStore'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { useLayoutStore } from '@/stores/layoutStore'
import { useRoute, useRouter } from 'vue-router'
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import PartialBookingDialog from '@/components/dialogs/PartialBookingDialog.vue'
import { useRateStore } from '@/stores/ratingStore'
import Rating from 'primevue/rating'
import Chip from 'primevue/chip'
import ProgressBar from 'primevue/progressbar'
import { useFavoriteFieldStore } from '@/stores/favoriteFieldStore'
import { useToast } from 'primevue/usetoast'
import MapFieldDetailDialog from '@/components/dialogs/MapFieldDetailDialog.vue'
import Button from 'primevue/button'
import ReportCreateDialog from '@/components/dialogs/ReportCreateDialog.vue'
import { useFieldStoreUser } from '@/stores/fieldStoreUser'
import { useImageStore } from '@/stores/imageStore'
import Galleria from 'primevue/galleria'
import Badge from 'primevue/badge'
import { formatDateYYYYHHHHH } from '@/utils/dateUtil'

const { t } = useI18n()
const routers = useRoute()
const toast = useToast()
const router = useRouter()

const fieldStore = useFieldStore()
const partialFieldStore = usePartialFieldStore()
const layoutStore = useLayoutStore()
const rateStore = useRateStore()
const favoriteFieldStore = useFavoriteFieldStore()
const fieldStoreUser = useFieldStoreUser()
const imageStore = useImageStore()

const isShowDetailModal = ref(false)
const selectedPartialFieldId = ref(0)
const isShowMapDialog = ref(false)
const isShowReportDialog = ref(false)

const schema = yup.object({
  email: yup.string().required(t('validation.required')).email(),
  password: yup.string().required(t('validation.required')).min(8, t('validation.passwordLength'))
})

const { meta, errors, defineField } = useForm({
  initialValues: {
    email: '',
    password: ''
  },
  validationSchema: schema
})

const onShowModal = (partialId: number) => {
  selectedPartialFieldId.value = partialId
  isShowDetailModal.value = true
}

const onAddFavorite = async () => {
  await favoriteFieldStore.addToFavoriteFields(Number(routers.params.id)).then((data) => {
    if (data.success)
      toast.add({
        severity: 'success',
        summary: 'Thêm vào sân yêu thích',
        detail: 'Đã thêm sân ' + fieldStore.field?.name + ' vào sân yêu thích',
        life: 3000
      })
  })
}

const onNavigateToField = async (id: number) => {
  router.push('/user/field/' + id)
  await init();
}

const removeFromFavorite = async () => {
  await favoriteFieldStore.deleteFromFavoriteFields(Number(routers.params.id)).then((data) => {
    if (data.success)
      toast.add({
        severity: 'success',
        summary: 'Xóa khỏi sân yêu thích',
        detail: 'Đã xóa sân ' + fieldStore.field?.name + ' khỏi sân yêu thích',
        life: 3000
      })
  })
}

const init = async () => {
  const id = Number(routers.params.id)
  if (!id) return

  await fieldStore.getFieldById(id)
  await partialFieldStore.getPartialFieldByFieldId(id)
  partialFieldStore.currentFieldId = id
  layoutStore.breadcrumb = [{ label: t('sidebar.admin.fields') }, { label: fieldStore.field?.name }]

  rateStore.rates = []
  rateStore.searchRateByField.fieldId = id
  rateStore.getRateByField()
  rateStore.getStarByField(id)
  fieldStoreUser.recommendLikelyField(id)
  await imageStore.getImagesByField(id)
  images.value = imageStore.images.map((image) => ({
    itemImageSrc: image.url,
    thumbnailImageSrc: image.url,
    alt: 'anh san'
  }))
}

onMounted(async () => {
  await init();
})

const images = ref()
const activeIndex = ref(0)
const responsiveOptions = ref([
  {
    breakpoint: '1024px',
    numVisible: 5
  },
  {
    breakpoint: '768px',
    numVisible: 3
  },
  {
    breakpoint: '560px',
    numVisible: 1
  }
])
const displayCustom = ref(false)

const imageClick = (index: number) => {
  activeIndex.value = index
  displayCustom.value = true
}
</script>
