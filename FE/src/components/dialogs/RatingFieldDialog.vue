<template>
  <Dialog
    :visible="open"
    :header="'Đánh giá sân bóng'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[800px] mx-20"
  >
    <div class="w-full flex items-center">
      <div class="flex flex-col mr-5">
        <Rating v-model="star" :cancel="false" class="!outline-none mb-4" />
        <div class="card flex justify-center">
          <Textarea v-model="comment" rows="5" cols="30" />
        </div>
      </div>
    </div>

    <div class="my-2 mt-5 border-t border-gray-300 pt-4">
      <ActionButton
        @click.stop="emit('close')"
        icon="times"
        class="bg-green-400 mr-2"
        is-outlined
        value="Cancel"
      />
      <ActionButton
        @click.stop="submit"
        :disable="!meta.valid || rateStore.loading"
        icon="check"
        class="bg-green-400 mx-auto"
        value="Đánh giá"
      />
      <i
        v-if="rateStore.loading"
        class="pi pi-spin pi-spinner mx-auto ml-4"
        style="font-size: 2rem"
      ></i>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import { useForm } from 'vee-validate'
import * as yup from 'yup'
import { useI18n } from 'vue-i18n'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { watch } from 'vue'
import { ref } from 'vue'
import Rating from 'primevue/rating'
import Textarea from 'primevue/textarea'
import { useRateStore } from '@/stores/ratingStore'
import type { RateCreateRequest } from '@/api/rateApi'
import { useToast } from 'primevue/usetoast'
import { useBookingStoreUser } from '@/stores/bookingStoreUser'

const { t } = useI18n()
const toast = useToast()

const rateStore = useRateStore()
const bookingStoreUser = useBookingStoreUser()

const props = defineProps<{
  open: boolean
  bookingId: number
  fieldId: number
  isCreate: boolean
}>()

const emit = defineEmits<(e: 'close') => void>()

const schema = yup.object({
  star: yup
    .number()
    .required(t('validation.required'))
    .min(1, t('validation.min', { value: 1 }))
    .max(5, t('validation.max', { value: 5 })),
  comment: yup.string()
})

const { meta, errors, defineField, handleSubmit, resetForm, values } = useForm<RateCreateRequest>({
  validationSchema: schema
})

const [star] = defineField('star')
const [comment] = defineField('comment')

const submit = handleSubmit(async (values) => {
  if (!props.bookingId) return

  const data = await rateStore.createRate({
    star: values.star,
    comment: values.comment,
    bookingId: props.bookingId
  })
  if (data.success) {
    toast.add({
      severity: 'success',
      summary: 'Thành công',
      detail: 'Đã đánh giá sân bóng',
      life: 3000
    })
    bookingStoreUser.booking!.isRated = true
    emit('close')
  } else {
    toast.add({
      severity: 'error',
      summary: 'Lỗi',
      detail: 'Có lỗi xảy ra, vui lòng thử lại',
      life: 3000
    })
  }
})

watch(
  () => props.open,
  async (value) => {
    console.log(props.isCreate)
    if (value) {
      if (!props.isCreate && props.bookingId) {
        await rateStore.getRateById(props.bookingId)
      }
    }
  }
)
</script>
