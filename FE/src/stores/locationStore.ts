import { VITE_APP_GOONG_API_KEY } from '@/constants/env'
import { fetchCommunes, fetchDistricts, fetchProvinces } from '@/utils/locationUtils'
import { defineStore } from 'pinia'

export interface State {
  enableLocation: boolean
  latitude?: number
  longitude?: number
  allProvince: any[]
  allDistrict: any[]
  allCommune: any[]
  isProvinceLoading: boolean
  isDistrictLoading: boolean
  isCommuneLoading: boolean
  currentProvince: any
  currentDistrict: any
  currentCommune: any
}

export const useLocationStore = defineStore({
  id: 'location',
  state: (): State => ({
    enableLocation: false,
    latitude: undefined,
    longitude: undefined,
    allProvince: [],
    allDistrict: [],
    allCommune: [],
    isProvinceLoading: false,
    isDistrictLoading: false,
    isCommuneLoading: false,
    currentProvince: undefined,
    currentDistrict: undefined,
    currentCommune: undefined
  }),
  actions: {
    async getAllProvinces() {
      this.isProvinceLoading = true
      try {
        const response = await fetchProvinces()
        this.allProvince = response.data
      } catch (error) {
        console.error(error)
      }
      this.isProvinceLoading = false
    },
    async getDistrictsByProvinceId(provinceId: string) {
      this.isDistrictLoading = true
      try {
        const response = await fetchDistricts(provinceId)
        this.allDistrict = response.data
      } catch (error) {
        console.error(error)
      }
      this.isDistrictLoading = false
    },
    async getCommunesByDistrictId(districtId: string) {
      this.isCommuneLoading = true
      try {
        const response = await fetchCommunes(districtId)
        this.allCommune = response.data
      } catch (error) {
        console.error(error)
      }
      this.isCommuneLoading = false
    },
    async getCurrentLocation() {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
          async (position) => {
            this.latitude = position.coords.latitude
            this.longitude = position.coords.longitude
            this.enableLocation = true
          },
          (error) => {
            console.error('Error getting location:', error)
            this.enableLocation = false
          }
        )
      } else {
        console.error('Geolocation is not supported by this browser.')
        this.enableLocation = false
      }
    },
    async fetchCurrentLocationDetails() {
      if (this.latitude && this.longitude) {
        const GOONG_API_KEY = VITE_APP_GOONG_API_KEY
        const url = `https://rsapi.goong.io/Geocode?latlng=${encodeURIComponent(
          this.latitude + ',' + this.longitude
        )}&api_key=${GOONG_API_KEY}`

        await fetch(url)
          .then((response) => {
            if (!response.ok) {
              throw new Error(`HTTP error! Status: ${response.status}`)
            }
            return response.json()
          })
          .then(async (data) => {
            if (data.status === 'OK' && data.results.length > 0) {
              const province = data.results[0].compound.province
              const district = data.results[0].compound.district
              const commune = data.results[0].compound.commune
              if (this.allCommune.length == 0) {
                await this.getAllProvinces()
              }
              this.currentProvince = this.allProvince.find((item) =>
                item.full_name.toLowerCase().includes(province.toLowerCase())
              )

              await this.getDistrictsByProvinceId(this.currentProvince.id)
              if (this.allDistrict.length > 0) {
                this.currentDistrict = this.allDistrict.find((item) =>
                  item.full_name.toLowerCase().includes(district.toLowerCase())
                )
                await this.getCommunesByDistrictId(this.currentDistrict.id)
              }
              if (this.allCommune.length > 0) {
                this.currentCommune = this.allCommune.find((item) =>
                  item.full_name.toLowerCase().includes(commune.toLowerCase())
                )
              }
            } else {
              throw new Error('Current location place details not found')
            }
          })
          .catch((error) => {
            console.error('Fetch error:', error)
          })
      }
    }
  }
})
