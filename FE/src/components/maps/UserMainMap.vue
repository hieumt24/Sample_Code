<template>
  <div class="shadow-inner">
    <div
      id="map"
      class="min-h-96 w-full rounded-md overflow-hidden shadow-[inset_0px_4px_10px_0px_rgba(0,0,0,0.05)]"
    />
  </div>

  <Dialog
    :visible="isSideBarOpen"
    :draggable="false"
    modal
    :header="fieldStore.field?.name"
    @update:visible="isSideBarOpen = false"
    :style="{ width: '30rem' }"
  >
    <div class="h-24 w-full">
      <img class="object-cover h-24 w-full rounded-md" :src="fieldStore.field?.cover" alt="cover" />
    </div>
    <div class="flex justify-between mt-2">
      <div>
        <span class="font-semibold">{{
          fieldStore.field?.address == '' ? 'Không có địa chỉ' : fieldStore.field?.address
        }}</span>
        <span class="line-clamp-2">{{ fieldStore.field?.description }}</span>
      </div>
      <div class="pt-m flex flex-col items-end">
        <span
          v-if="!favoriteFieldStore.favoriteFieldIds.includes('[' + fieldStore.field?.id + ']')"
        >
          <i
            class="pi pi-heart text-red-300 cursor-pointer"
            @click="onAddFavorite"
            style="font-size: 1.2rem"
          ></i>
        </span>
        <span
          v-if="favoriteFieldStore.favoriteFieldIds.includes('[' + fieldStore.field?.id + ']')"
          class="flex items-center text-red-300"
          @click="removeFromFavorite"
        >
          (Đã thích)
          <i
            class="pi pi-heart-fill text-red-300 cursor-pointer ml-2"
            style="font-size: 1.2rem"
          ></i>
        </span>
        <div>
          <span>{{ fieldStore.field?.rating }} / 5</span>
          <i class="pi pi-star-fill text-green-400" style="font-size: 1rem"></i>
        </div>
        <span class="whitespace-nowrap">{{ fieldStore.field?.price }}</span>
      </div>
    </div>
    <div class="flex justify-end gap-2 mt-8">
      <Button
        type="button"
        label="Đóng"
        severity="secondary"
        @click="isSideBarOpen = false"
      ></Button>
      <Button
        type="button"
        label="Xem chi tiết"
        @click="$router.push(`/user/field/${fieldStore.field?.id}`)"
      ></Button>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { onBeforeUpdate, onMounted, reactive, ref, watch } from 'vue'
// @ts-ignore
import goongjs from '@goongmaps/goong-js'
import type { GeolocateEvent, MapBound } from '@/types/Map'
import { useFieldStore } from '@/stores/fieldStore'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import { useFavoriteFieldStore } from '@/stores/favoriteFieldStore'
import { useToast } from 'primevue/usetoast'
// @ts-ignore
import polyline from '@mapbox/polyline'
// @ts-ignore
import goongClient from '@goongmaps/goong-sdk'
// @ts-ignore
import goongDirections from '@goongmaps/goong-sdk/services/directions'
import { VITE_APP_GOONG_API_KEY, VITE_APP_GOONG_MAP_KEY } from '@/constants/env'

const GOONG_MAP_KEY = VITE_APP_GOONG_MAP_KEY
const GOONG_API_KEY = VITE_APP_GOONG_API_KEY

const baseClient = goongClient({ accessToken: GOONG_API_KEY })
const directionService = goongDirections(baseClient)

const toast = useToast()

const fieldStore = useFieldStore()
const favoriteFieldStore = useFavoriteFieldStore()

const markers = reactive<goongjs.Marker[]>([])
const bound = ref<MapBound>()
const timeoutId = ref<number>()
const isSideBarOpen = ref(false)

const mapRef = ref<goongjs.Map | null>(null)
let marker: goongjs.Marker | null = null

const onMoveEnd = () => {
  bound.value = mapRef.value.getBounds()
}

onMounted(async () => {
  goongjs.accessToken = GOONG_MAP_KEY
  const map = new goongjs.Map({
    container: 'map',
    style: 'https://tiles.goong.io/assets/goong_map_web.json',
    center: [105.84478, 21.02897],
    zoom: 13
  })

  mapRef.value = map
  // Add geolocate control to the map.
  let geolocateControl = new goongjs.GeolocateControl({
    positionOptions: {
      enableHighAccuracy: true
    },
    trackUserLocation: true,
    fitBoundsOptions: {
      maxZoom: 13
    }
  })
  map.addControl(geolocateControl)
  map.on('moveend', onMoveEnd)
  bound.value = map.getBounds()
  addMarker()
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

function addMarker() {
  if (fieldStore.field) {
    const marker = new goongjs.Marker()
      .setLngLat([fieldStore.field.longitude, fieldStore.field.latitude])
      .addTo(mapRef.value)
    marker.getElement().addEventListener('click', async () => {
      await fieldStore.getFieldById(fieldStore.field?.id ?? 0)
      isSideBarOpen.value = true
    })
    mapRef.value.setCenter([fieldStore.field.longitude, fieldStore.field.latitude])
    markers.push(marker)
  }
}
interface RoutePoints {
  origin: {
    lat: number
    lng: number
  }
  destination: {
    lat: number
    lng: number
  }
}
function direction(point: RoutePoints) {
  const originPlace = point.origin
  const destinationPlace = point.destination
  mapRef.value.remove()
  const map = new goongjs.Map({
    container: 'map',
    style: 'https://tiles.goong.io/assets/goong_map_web.json',
    center: [originPlace.lng, originPlace.lat],
    zoom: 13
  })
  mapRef.value = map
  let geolocateControl = new goongjs.GeolocateControl({
    positionOptions: {
      enableHighAccuracy: true
    },
    trackUserLocation: true,
    fitBoundsOptions: {
      maxZoom: 13
    }
  })
  map.addControl(geolocateControl)
  map.on('load', function () {
    geolocateControl.trigger()

    const layers = map.getStyle().layers
    // Find the index of the first symbol layer in the map style
    let firstSymbolId: any
    for (const layer of layers) {
      if (layer.type === 'symbol') {
        firstSymbolId = layer.id
        break
      }
    }

    if (originPlace) {
      const el = document.createElement('div')
      el.style.backgroundColor = 'red'
      el.style.width = '20px'
      el.style.height = '20px'
      el.style.borderRadius = '50%'
      const marker = new goongjs.Marker(el).setLngLat([originPlace.lng, originPlace.lat]).addTo(map)
    }
    if (destinationPlace) {
      const marker = new goongjs.Marker()
        .setLngLat([destinationPlace.lng, destinationPlace.lat])
        .addTo(map)
    }
    map.flyTo({
      center: [originPlace.lng, originPlace.lat],
      essential: true
    })
    directionService
      .getDirections({
        origin: `${originPlace.lat},${originPlace.lng}`,
        destination: `${destinationPlace.lat},${destinationPlace.lng}`,
        vehicle: 'bike'
      })
      .send()
      .then(function (response: any) {
        const directions = response.body
        const route = directions.routes[0]

        const geometry_string = route.overview_polyline.points
        const geoJSON = polyline.toGeoJSON(geometry_string)
        map.addSource('route', {
          type: 'geojson',
          data: geoJSON
        })

        map.addLayer(
          {
            id: 'route',
            type: 'line',
            source: 'route',
            layout: {
              'line-join': 'round',
              'line-cap': 'round'
            },
            paint: {
              'line-color': '#1e88e5',
              'line-width': 8
            }
          },
          firstSymbolId
        )
      })
  })
}
defineExpose({
  direction
})
</script>
