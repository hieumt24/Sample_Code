<template>
  <div class="shadow-inner w-full h-full">
    <div
      id="map"
      class="min-h-96 w-full h-full rounded-md overflow-hidden shadow-[inset_0px_4px_10px_0px_rgba(0,0,0,0.05)]"
    />
    <Dialog
      :visible="isSideBarOpen"
      :draggable="false"
      modal
      :header="fieldStore.field?.name"
      @update:visible="isSideBarOpen = false"
      :style="{ width: '30rem' }"
      class="transition-all duration-300 ease-in-out"
    >
      <div class="relative h-24 w-full overflow-hidden rounded-md">
        <img
          class="object-cover h-full w-full transition-transform duration-300 hover:scale-105"
          :src="fieldStore.field?.cover"
          :alt="fieldStore.field?.name + ' cover'"
        />
      </div>
      <div class="flex justify-between mt-4 space-y-2">
        <div class="space-y-1">
          <h3 class="font-semibold text-lg">
            {{ fieldStore.field?.address || 'Không có địa chỉ' }}
          </h3>
          <p class="text-sm text-gray-600 line-clamp-2">
            {{ fieldStore.field?.description }}
          </p>
        </div>
        <div class="flex flex-col items-end space-y-2">
          <button
            @click="onAddFavorite"
            class="text-red-300 hover:text-red-500 transition-colors duration-200"
            :aria-label="
              favoriteFieldStore.favoriteFieldIds.includes('[' + fieldStore.field?.id + ']')
                ? 'Remove from favorites'
                : 'Add to favorites'
            "
          >
            <i
              :class="[
                'pi',
                favoriteFieldStore.favoriteFieldIds.includes('[' + fieldStore.field?.id + ']')
                  ? 'pi-heart-fill'
                  : 'pi-heart'
              ]"
              style="font-size: 1.2rem"
            ></i>
            <span
              v-if="favoriteFieldStore.favoriteFieldIds.includes('[' + fieldStore.field?.id + ']')"
              class="text-xs ml-1"
            >
              (Đã thích)
            </span>
          </button>
          <div
            v-if="fieldStore.field?.rating && fieldStore.field?.rating !== 'N/A'"
            class="flex items-center"
          >
            <span class="mr-1">{{ fieldStore.field.rating }} / 5</span>
            <i class="pi pi-star-fill text-green-400" style="font-size: 1rem"></i>
          </div>
          <span v-else>Không có đánh giá</span>

          <span class="whitespace-nowrap"> {{ fieldStore.field?.price }} VND </span>
        </div>
      </div>
      <div class="flex justify-end gap-2 mt-8">
        <Button
          type="button"
          label="Đóng"
          severity="secondary"
          @click="isSideBarOpen = false"
          class="transition-colors duration-200"
        ></Button>
        <Button
          type="button"
          label="Xem chi tiết"
          @click="
            $router.push(
              authStore.identity
                ? `/user/field/${fieldStore.field?.id}`
                : `/field/${fieldStore.field?.id}`
            )
          "
          class="transition-colors duration-200"
        ></Button>
      </div>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { onBeforeUpdate, onMounted, reactive, ref, watch } from 'vue'
// @ts-ignore
import goongjs from '@goongmaps/goong-js'
import type { MapBound } from '@/types/Map'
import { useFieldStore } from '@/stores/fieldStore'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import { useFavoriteFieldStore } from '@/stores/favoriteFieldStore'
import { useToast } from 'primevue/usetoast'
import type { height } from '@fortawesome/free-brands-svg-icons/fa42Group'
import { useLocationStore } from '@/stores/locationStore'
import { VITE_APP_GOONG_MAP_KEY } from '@/constants/env'
import { useAuthStore } from '@/stores/authStore'
const toast = useToast()

const fieldStore = useFieldStore()
const authStore = useAuthStore()
const favoriteFieldStore = useFavoriteFieldStore()
const locationStore = useLocationStore()

const markers = reactive<goongjs.Marker[]>([])
const bound = ref<MapBound>()
const timeoutId = ref<number>()
const isSideBarOpen = ref(false)

const GOONG_MAP_KEY = VITE_APP_GOONG_MAP_KEY

let map: goongjs.Map | null = null
let marker: goongjs.Marker | null = null

const onMoveEnd = () => {
  bound.value = map.getBounds()
}

watch(
  () => bound.value,
  async () => {
    timeoutId.value = setTimeout(async () => {
      if (bound.value) {
        await fieldStore.getFieldListByMapBound(bound.value)
        addMarker()
      }
    }, 300)
  }
)

onMounted(async () => {
  goongjs.accessToken = GOONG_MAP_KEY
  map = new goongjs.Map({
    container: 'map',
    style: 'https://tiles.goong.io/assets/goong_map_web.json',
    center: [105.84478, 21.02897],
    zoom: 13
  })
  // Add geolocate control to the map.
  let GeolocateControl = new goongjs.GeolocateControl({
    positionOptions: {
      enableHighAccuracy: true
    },
    trackUserLocation: true,
    fitBoundsOptions: {
      maxZoom: 13
    }
  })
  map.addControl(GeolocateControl)
  map.on('load', function () {
    GeolocateControl.trigger() //<- Automatically activates
    // isSideBarOpen.value = true
  })
  map.on('moveend', onMoveEnd)
  bound.value = map.getBounds()
  if (locationStore.enableLocation && locationStore.latitude && locationStore.longitude) {
    setCenter(locationStore.longitude, locationStore.latitude)
  }
})

onBeforeUpdate(() => {
  if (timeoutId.value) {
    clearTimeout(timeoutId.value)
  }
})

const onAddFavorite = async () => {
  await favoriteFieldStore.addToFavoriteFields(fieldStore.field?.id ?? 0).then((data) => {
    if (data.success)
      toast.add({
        severity: 'success',
        summary: 'Thêm vào sân yêu thích',
        detail: 'Đã thêm sân ' + fieldStore.field?.name + ' vào sân yêu thích',
        life: 3000
      })
  })
}

const removeFromFavorite = async () => {
  await favoriteFieldStore.deleteFromFavoriteFields(fieldStore.field?.id ?? 0).then((data) => {
    if (data.success)
      toast.add({
        severity: 'success',
        summary: 'Xóa khỏi sân yêu thích',
        detail: 'Đã xóa sân ' + fieldStore.field?.name + ' khỏi sân yêu thích',
        life: 3000
      })
  })
}

const addMarker = () => {
  fieldStore.fieldsBound.forEach((field) => {
    const marker = new goongjs.Marker().setLngLat([field.longitude, field.latitude]).addTo(map)
    marker.getElement().addEventListener('click', async () => {
      await fieldStore.getFieldById(field.id)
      isSideBarOpen.value = true
    })
    markers.push(marker)
  })
}

const setCenter = (lng: number, lat: number) => {
  map.setCenter([lng, lat])
  map.setZoom(12)
}

defineExpose({
  setCenter
})
</script>
