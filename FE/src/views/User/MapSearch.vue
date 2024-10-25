<template>
  <div class="relative w-screen h-[calc(100vh-60px)] p-0 m-0 lg:-mx-36">
    <MapSearchSideBar
      @add-marker="addMarker"
      @set-map-center="setMapCenter"
      @focus-marker="focusMarker"
      @show-detail-dialog="showDetailDialog"
    />
    <SearchMultipleFieldMarkerMap ref="searchMultipleFieldMarkerMapRef" />
  </div>
</template>

<script setup lang="ts">
import SearchMultipleFieldMarkerMap from '@/components/maps/SearchMultipleFieldMarkerMap.vue'
import MapSearchSideBar from '@/components/sidebars/MapSearchSideBar.vue'
import type { Field } from '@/types/Field'
import { ref } from 'vue'

const searchMultipleFieldMarkerMapRef = ref<InstanceType<
  typeof SearchMultipleFieldMarkerMap
> | null>(null)

const addMarker = (fields: Field[]) => {
  if (searchMultipleFieldMarkerMapRef.value) {
    searchMultipleFieldMarkerMapRef.value.addMarker(fields)
  }
}

const setMapCenter = (field: Field) => {
  if (searchMultipleFieldMarkerMapRef.value) {
    searchMultipleFieldMarkerMapRef.value.setCenter(field.longitude, field.latitude)
  }
}

const focusMarker = (field: Field) => {
  if (searchMultipleFieldMarkerMapRef.value) {
    searchMultipleFieldMarkerMapRef.value.focusMarker(field)
  }
}

const showDetailDialog = (fieldId: number) => {
  if (searchMultipleFieldMarkerMapRef.value) {
    searchMultipleFieldMarkerMapRef.value.showDetailDialog(fieldId)
  }
}
</script>
