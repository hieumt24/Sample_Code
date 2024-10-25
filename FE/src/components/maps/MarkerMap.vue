<template>
  <div id="map" style="height: 100%; min-height: 100%"></div>
</template>

<script setup lang="ts">
import { onMounted, watch } from 'vue'
// @ts-ignore
import goongjs from '@goongmaps/goong-js'
import { VITE_APP_GOONG_MAP_KEY } from '@/constants/env'

const GOONG_MAP_KEY = VITE_APP_GOONG_MAP_KEY
let isNeedCenterMap = true

const coordinate = defineModel<{ lat: number | null; lng: number | null }>('coordinate')
const props = defineProps({
  isDraggable: {
    type: Boolean,
    required: false,
    default: true
  }
})

let map: goongjs.Map | null = null
let marker: goongjs.Marker | null = null

const onDragEnd = () => {
  if (marker) {
    const lngLat = marker.getLngLat()
    coordinate.value = { lat: lngLat.lat, lng: lngLat.lng }
    isNeedCenterMap = false
  }
}
const onMapDblClick = (event: any) => {
  const lngLat = event.lngLat
  coordinate.value = { lat: lngLat.lat, lng: lngLat.lng }
}
onMounted(() => {
  goongjs.accessToken = GOONG_MAP_KEY
  map = new goongjs.Map({
    container: 'map',
    style: 'https://tiles.goong.io/assets/goong_map_web.json',
    center: [105.84478, 21.02897],
    zoom: 13,
    doubleClickZoom: false
  })
  marker = new goongjs.Marker({
    draggable: props.isDraggable
  })
  if (props.isDraggable) {
    marker.on('dragend', onDragEnd)
  }
  map.on('dblclick', onMapDblClick)
})

watch(
  () => [coordinate.value?.lat, coordinate.value?.lng],
  () => {
    if (isNeedCenterMap && map && marker) {
      map.setCenter([coordinate.value?.lng, coordinate.value?.lat])
      marker.setLngLat([coordinate.value?.lng, coordinate.value?.lat]).addTo(map)
    }
    isNeedCenterMap = true
  }
)
const setCenter = (longitude: number, latitude: number) => {
  map.setCenter([longitude, latitude])
  map.setZoom(12)
}
defineExpose({
  setCenter
})
</script>

<style scoped>
#search-input {
  width: 100%;
  padding: 10px;
  font-size: 16px;
  box-sizing: border-box;
}
.dropdown {
  position: relative;
  width: 100%;
}
#result {
  position: absolute;
  width: 100%;
  border: 1px solid #ccc;
  background: #fff;
  max-height: 200px;
  overflow-y: auto;
  z-index: 1000;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}
.suggestion {
  padding: 10px;
  cursor: pointer;
}
.suggestion:hover {
  background: #f0f0f0;
}
#map {
  width: 100%;
  height: 700px;
}
.details {
  margin-top: 20px;
  font-size: 16px;
}
.details span {
  font-weight: bold;
}
</style>
