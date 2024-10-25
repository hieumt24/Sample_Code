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
          <h3 class="font-medium text-lg">
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

          <span class="whitespace-nowrap">
            {{ Math.floor(fieldStore.field?.price ?? 0).toLocaleString() }} VNĐ</span
          >
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
          @click="$router.push(`/user/field/${fieldStore.field?.id}`)"
          class="transition-colors duration-200"
        ></Button>
      </div>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { onBeforeUpdate, onMounted, reactive, ref } from 'vue'
// @ts-ignore
import goongjs from '@goongmaps/goong-js'
import type { MapBound } from '@/types/Map'
import { useFieldStore } from '@/stores/fieldStore'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import { useFavoriteFieldStore } from '@/stores/favoriteFieldStore'
import { useToast } from 'primevue/usetoast'
import { useLocationStore } from '@/stores/locationStore'
import { useFieldStoreUser } from '@/stores/fieldStoreUser'
import type { Field } from '@/types/Field'
import { VITE_APP_GOONG_MAP_KEY } from '@/constants/env'
const toast = useToast()

const fieldStore = useFieldStore()
const favoriteFieldStore = useFavoriteFieldStore()
const locationStore = useLocationStore()

const markers = reactive<goongjs.Marker[]>([])
const bound = ref<MapBound>()
const isSideBarOpen = ref(false)
const focusField = ref<Field>()

const GOONG_MAP_KEY = VITE_APP_GOONG_MAP_KEY

let map: goongjs.Map | null = null

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
    GeolocateControl.trigger()
  })
  if (locationStore.enableLocation && locationStore.latitude && locationStore.longitude) {
    setCenter(locationStore.longitude, locationStore.latitude)
  }
  bound.value = map.getBounds()
  if (bound.value) {
    await fieldStore.getFieldListByMapBound(bound.value)
    addMarker(fieldStore.fieldsBound)
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

const addMarker = (fields: Field[]) => {
  clearMarkers()
  if (fields.length > 0) {
    fields.forEach((field) => {
      const marker = new goongjs.Marker({ color: '#3FB1CE' })
        .setLngLat([field.longitude, field.latitude])
        .addTo(map)
      marker.getElement().addEventListener('click', async () => {
        await showDetailDialog(field.id)
      })

      markers.push(marker)
    })
    setCenter(fields[0].longitude, fields[0].latitude)
  }
}
const clearMarkers = () => {
  markers.forEach((marker: any) => marker.remove())
  markers.splice(0, markers.length)
}

const showDetailDialog = async (fieldId: number) => {
  const field = await fieldStore.getFieldById(fieldId)
  isSideBarOpen.value = true
  focusMarker(field.data)
}
const setCenter = (longitude: number, latitude: number) => {
  map.setCenter([longitude, latitude])
  map.setZoom(12)
}
const focusMarker = (field: Field) => {
  if (focusField.value) {
    unfocusMarker(focusField.value)
  }
  const markerToFocus = markers.find((marker: any) => {
    const markerLngLat = marker.getLngLat()
    return markerLngLat.lng === field.longitude && markerLngLat.lat === field.latitude
  })
  if (markerToFocus) {
    markerToFocus.remove()
    markerToFocus.remove()
    const index = markers.indexOf(markerToFocus)
    if (index !== -1) {
      markers.splice(index, 1)
    }
    const newMarker = new goongjs.Marker({ color: 'red' })
      .setLngLat([field.longitude, field.latitude])
      .addTo(map)
    newMarker.getElement().addEventListener('click', async () => {
      await showDetailDialog(field.id)
    })
    focusField.value = field
    markers.push(newMarker)
  }
}

const unfocusMarker = (field: Field) => {
  const markerToUnfocus = markers.find((marker: any) => {
    const markerLngLat = marker.getLngLat()
    return markerLngLat.lng === field.longitude && markerLngLat.lat === field.latitude
  })
  if (markerToUnfocus) {
    markerToUnfocus.remove()
    const index = markers.indexOf(markerToUnfocus)
    if (index !== -1) {
      markers.splice(index, 1)
    }
    const newMarker = new goongjs.Marker({ color: '#3FB1CE' })
      .setLngLat([field.longitude, field.latitude])
      .addTo(map)
    newMarker.getElement().addEventListener('click', async () => {
      await showDetailDialog(field.id)
    })
    markers.push(newMarker)
  }
}

defineExpose({
  setCenter,
  addMarker,
  focusMarker,
  showDetailDialog
})
</script>
