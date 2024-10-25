export type DetailPlace = {
  result: {
    place_id: string
    formatted_address: string
    geometry: {
      location: {
        lat: number
        lng: number
      }
    }
    plus_code: {
      compound_code: string
      global_code: string
    }
    compound: {
      district: string
      commune: string
      province: string
    }
    name: string
    url: string
    types: string[]
  }
  status: string
}

export type Prediction = {
  description: string
  matched_substrings: []
  place_id: string
  reference: string
  terms: []
  has_children: boolean
  display_type: string
  score: number
  plus_code: {
    compound_code: string
    global_code: string
  }
  compound: {
    district: string
    commune: string
    province: string
  }
}

export type AutoCompletePlace = {
  predictions: Prediction[]
  executed_time: number
  executed_time_all: number
  status: string
}

export type Coordinate = {
  lat: number
  lng: number
}

export type MapBound = {
  _ne: { lng: number; lat: number }
  _sw: { lng: number; lat: number }
}

export type GeolocateEvent = {
  coords: {
    latitude: number
    longitude: number
    accuracy: number
    altitude?: number
    altitudeAccuracy?: number
    heading?: number
    speed?: number
  }
  timestamp: number
}
