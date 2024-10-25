<template>
  <!-- <div class="w-full flex flex-col md:flex-row mb-5">
    <div class="p-2 w-full md:w-3/5 h-full">
      <UserMainMap />
    </div>
    <div class="bg-white w-full md:w-2/5 h-full flex flex-col">
      <div class="flex flex-col md:flex-row flex-wrap">
        <div class="flex flex-col">
          <label for="date">Date</label>
          <Calendar id="date" v-model="date" showIcon />
        </div>
        <div class="flex flex-col">
          <label for="from-date">Date</label>
          <Calendar id="from-date" class="mr-2" v-model="fromDate" showIcon />
        </div>
        <div class="flex flex-col">
          <label for="to-date">Date</label>
          <Calendar id="to-date" class="mr-2" v-model="toDate" showIcon />
        </div>
      </div>

      <div class="flex flex-col">
        <label for="province">Select province</label>
        <AutoComplete
          class="my-2"
          v-model="selectedProvince"
          optionLabel="province"
          :suggestions="filteredProvince"
          @complete="search"
        />
      </div>
      <ActionButton @click="submit" :loading="meta.pending" value="search" />
    </div>
  </div> -->
  <hr />
  <div class="mt-5 flex w-full flex-wrap">
    <div
      v-for="field in fieldStore.fields"
      :key="field.id"
      class="bg-gray-200 h-52 w-60 mx-2 rounded-md"
    >
      <div class="h-24 w-full">
        <img class="object-cover h-24 w-full rounded-t-md" :src="field.cover" alt="" />
      </div>
      <div class="flex flex-col p-2">
        <span class="font-semibold">{{ field.name }}</span>
        <span class="font-semibold">{{ field.address }}</span>
      </div>
      <div class="flex justify-evenly">
        <ActionButton
          @click="$router.push('/user/field/1')"
          :loading="meta.pending"
          value="Detaill"
        />
      </div>
    </div>
  </div>
  <div class="h-52 w-full bg-white"></div>
</template>

<script setup lang="ts">
import UserMainMap from '@/components/maps/UserMainMap.vue'
import AutoComplete from 'primevue/autocomplete'
import Calendar from 'primevue/calendar'
import * as yup from 'yup'
import { useForm } from 'vee-validate'
import { onMounted, ref, reactive } from 'vue'
import { province } from '@/constants/area'
import { useI18n } from 'vue-i18n'
import { useFieldStore } from '@/stores/fieldStore'
import ActionButton from '@/components/buttons/ActionButton.vue'
const { t } = useI18n()

const fieldStore = useFieldStore()

const filteredProvince = ref(province)

const schema = yup.object({
  email: yup.string().required(t('validation.required')).email(),
  password: yup.string().required(t('validation.required')).min(8, t('validation.passwordLength'))
})

const { meta, errors, defineField, handleSubmit } = useForm({
  initialValues: {
    email: '',
    password: ''
  },
  validationSchema: schema
})

const [date] = defineField('email')
const [fromDate] = defineField('password')
const [toDate] = defineField('password')
const [selectedProvince] = defineField('password')

const submit = handleSubmit((values) => {
  console.log(values)
})

const search = (event: any) => {
  console.log(event.query.toLowerCase())
  setTimeout(() => {
    if (event.query.length === 0) {
      filteredProvince.value = [...province]
    } else {
      filteredProvince.value = province.filter((p) => {
        // return p.name.toLowerCase().startsWith(event.query.toLowerCase())
      })
    }
  }, 250)
}
</script>
