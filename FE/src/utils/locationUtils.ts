import axios from 'axios'

export const fetchProvinces = async () => {
  try {
    const response = await axios.get('https://esgoo.net/api-tinhthanh/1/0.htm')
    return response.data
  } catch (error) {
    console.error('Error fetching provinces:', error)
    throw error
  }
}

export const fetchDistricts = async (provinceId: string) => {
  try {
    const response = await axios.get(`https://esgoo.net/api-tinhthanh/2/${provinceId}.htm`)
    return response.data
  } catch (error) {
    console.error('Error fetching districts:', error)
    throw error
  }
}

export const fetchCommunes = async (districtId: string) => {
  try {
    const response = await axios.get(`https://esgoo.net/api-tinhthanh/3/${districtId}.htm`)
    return response.data
  } catch (error) {
    console.error('Error fetching communes:', error)
    throw error
  }
}
