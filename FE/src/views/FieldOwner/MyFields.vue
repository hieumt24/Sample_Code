<template>
  <div class="w-full flex">
    <SearchTab @search="onSearch" @reset="onReset">
      <ActionButton :value="$t('form.create')" @click="onCreate" :is-outlined="true" />
      <InputField name="name" :label="$t('searchTab.name')" v-model="fieldStore.ownerSearch.name" />
      <InputField
        name="address"
        :label="$t('searchTab.address')"
        v-model="fieldStore.ownerSearch.address"
      />
    </SearchTab>
    <div class="flex-1 lg:px-10">
      <CustomTable
        :loading="fieldStore.loading"
        :total="fieldStore.total"
        :headers="headers"
        :items="
          fieldStore.fields.map((field) => ({
            ...field,
            fieldStatus: field.status
          }))
        "
        no-detail
        no-delete
        @detail="onDetail"
        @edit="onEdit"
        has-field-status
        @change-page="fieldStore.changePageOwner"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import CustomTable from '@/components/tables/CustomTable.vue'
import router from '@/router'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { useFieldStore } from '@/stores/fieldStore'
import SearchTab from '@/components/search/SearchTab.vue'
import InputField from '@/components/inputs/InputField.vue'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'

const fieldStore = useFieldStore()

const onDetail = async (id: number) => {
  router.push(`/field/field-detail/${id}`)
}

const onEdit = async (id: number) => {
  router.push(`/field/update-field/${id}`)
}

const onCreate = () => {
  router.push(`/field/create-field`)
}

const onSearch = async () => {
  await fieldStore.getOwnerFieldList()
}

const onReset = async () => {
  fieldStore.ownerSearch = {
    name: '',
    address: '',
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  await fieldStore.getOwnerFieldList()
}

onMounted(async () => {
  fieldStore.ownerSearch.status = undefined
  await fieldStore.getOwnerFieldList()
})

const headers = [
  { field: 'name', header: 'Tên sân' },
  { field: 'address', header: 'Địa chỉ' }
]
</script>
