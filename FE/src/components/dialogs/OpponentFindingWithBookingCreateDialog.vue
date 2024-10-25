<template>
  <Dialog
    :visible="open"
    :header="'Tìm đối'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    :style="{ width: '60rem' }"
  >
    <div class="relative" v-if="bookingStore.booking">
      <div
        v-if="opponentFindingStore.dialogActionLoading"
        class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
      >
        <ProgressSpinner strokeWidth="4" />
      </div>
      <form @submit.prevent="submitForm">
        <div class="mt-4">
          <p class="text-sm text-gray-600">
            <strong>Sân: </strong>
            {{ bookingStore.booking?.fieldName }}
          </p>
          <p class="text-sm text-gray-600">
            <strong>Ngày: </strong>{{ bookingStore.booking?.date }}
          </p>
          <p class="text-sm text-gray-600">
            <strong>Thời gian: </strong> {{ bookingStore.booking?.startTime }} -
            {{ bookingStore.booking?.endTime }}
          </p>
        </div>
        <Message
          :closable="false"
          v-if="opponentFindingStore.hasOverlapOpponentFinding"
          severity="error"
          >Bạn đã tạo một bài đăng trong thời gian này nên không thể tạo tiếp!</Message
        >
        <Message :closable="false" v-if="isBookingInThePast" severity="error"
          >Không thể tạo bài tìm đối vì bắt đầu trận bóng đã qua!</Message
        >
        <Message
          :closable="false"
          v-if="opponentFindingStore.hasOverlapAcceptedRequest"
          severity="error"
          >Một trong những yêu cầu của bạn đã được chấp nhận trong khoảng thời gian này. <br />
          Do đó bạn không thể tạo bài đăng trùng lặp.
        </Message>
        <div
          v-if="
            !(
              opponentFindingStore.hasOverlapOpponentFinding ||
              opponentFindingStore.hasOverlapAcceptedRequest
            )
          "
        >
          <div class="my-4 flex flex-col">
            <label for="content" class="mb-2 font-semibold">Nội dung</label>
            <Textarea
              v-model="content"
              name="content"
              as="textarea"
              rows="10"
              cols="30"
              class="p-2 border rounded-md"
            />
            <ErrorMessage name="content" class="text-red-500" />
          </div>
          <Button
            type="submit"
            label="Đăng"
            class="w-full mt-4 bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
          />
        </div>
      </form>
    </div>
    <div v-else>Không thể tải booking của bạn. Vui lòng thử lại</div>
  </Dialog>
</template>

<script setup lang="ts">
import { defineProps, defineEmits, onMounted, watch, computed } from 'vue'
import { useForm, ErrorMessage } from 'vee-validate'
import * as yup from 'yup'
import { type OpponentFindingWithBookingCreateRequest } from '@/api/opponentFindingApi'
import { useToast } from 'primevue/usetoast'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import { useOpponentFindingStore } from '@/stores/opponentFindingStore'
import Textarea from 'primevue/textarea'
import { useBookingStore } from '@/stores/bookingStoreOwner'
import router from '@/router'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'

const props = defineProps<{
  open: boolean
  bookingId?: number
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()
const isBookingInThePast = computed(() => {
  const booking = bookingStore.booking
  if (!booking) return false

  const bookingDateTime = new Date(`${booking.date}T${booking.startTime}`)
  const now = new Date()

  return bookingDateTime < now
})
const toast = useToast()
const opponentFindingStore = useOpponentFindingStore()
const bookingStore = useBookingStore()
const schema = yup.object({
  content: yup.string().required('Nội dung là bắt buộc')
})

const { handleSubmit, errors, defineField, resetForm, values } =
  useForm<OpponentFindingWithBookingCreateRequest>({
    validationSchema: schema,
    initialValues: {
      bookingId: props.bookingId
    }
  })

const [content] = defineField('content')

watch(
  () => props.open,
  async (value) => {
    if (value && props.bookingId) {
      const response = await bookingStore.getBookingById(props.bookingId)
      if (response.success) {
        await opponentFindingStore.checkOverlapOpponentFinding({
          date: response.data.date,
          startTime: response.data.startTime,
          endTime: response.data.endTime
        })
        await opponentFindingStore.checkAcceptedRequestWithParams({
          date: response.data.date,
          startTime: response.data.startTime,
          endTime: response.data.endTime
        })
      } else {
        toast.add({
          severity: 'error',
          detail: 'Thao tác không thành công. Vui lòng thử lại!',
          life: 3000
        })
      }
    }
  }
)

const submitForm = handleSubmit(async (values) => {
  try {
    const response = await opponentFindingStore.createOpponentFindingWithBooking(values)
    if (response.success) {
      toast.add({ severity: 'success', detail: 'Yêu cầu đã được tạo thành công!', life: 3000 })
      router.push('/user/find-opponent/mine/my-posts')
      emit('close')
    } else {
      toast.add({
        severity: 'error',
        detail: 'Thao tác không thành công. Vui lòng thử lại!',
        life: 3000
      })
    }
  } catch (error) {
    toast.add({ severity: 'error', detail: 'Đã xảy ra lỗi. Vui lòng thử lại!', life: 3000 })
  }
})
</script>
