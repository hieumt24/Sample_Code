<template>
  <div class="card flex justify-center">
    <Dialog
      :visible="open"
      @update:visible="emit('close')"
      modal
      :draggable="false"
      :header="isCreate ? 'Tạo mới sân nhỏ' : 'Chỉnh sửa sân nhỏ'"
      :style="{ width: '28rem' }"
    >
      <div v-if="partialFieldStore.loadingPartial" class="w-full flex">
        <i class="pi pi-hourglass mx-auto" style="font-size: 2rem" />
        <h1 class="text-center">đang tải</h1>
      </div>
      <div>
        <div class="flex flex-col h-14 mb-4">
          <label for="name" class="font-semibold w-[6rem]">Tên</label>
          <InputText id="name" class="flex-auto" v-model="name" autocomplete="off" />
          <small class="text-red-400 flex items-start">{{ errors.name }}</small>
        </div>
        <div class="flex flex-col mb-4 h-14">
          <label for="description" class="font-semibold w-[6rem]">Mô tả</label>
          <InputText id="description" class="flex-auto" v-model="description" autocomplete="off" />
          <small class="text-red-400 flex items-start">{{ errors.description }}</small>
        </div>
        <ActionButton
          v-if="!isCreate"
          class="w-full mt-2"
          :severity="'warning'"
          :disable="!meta.valid || partialFieldStore.loadingPartial"
          value="Vô hiệu hoá"
          isOutlined
          @click.prevent="isConfirmCancelDiablog = true"
        />
        <div class="flex mt-7">
          <ActionButton
            class="mx-auto"
            :disable="!meta.valid || partialFieldStore.loadingPartial"
            value="Đóng"
            isOutlined
            @click.prevent="cancel"
          />

          <ActionButton
            class="mx-auto"
            :disable="!meta.valid || partialFieldStore.loadingPartial"
            value="Lưu"
            @click.prevent="submit"
          />
        </div>
      </div>
    </Dialog>
  </div>

  <Dialog
    :visible="isConfirmCancelDiablog"
    @update:visible="isConfirmCancelDiablog = false"
    modal
    :draggable="false"
    header="Xem các thay đổi"
    :style="{ width: '25rem' }"
  >
    <div>
      <h1>Bạn có chắc chắn muốn vô hiệu hoá sân bóng nhỏ này không?</h1>
      <h1>
        Số lượng ịch đặt đang chờ sẽ bị huỷ:
        {{ partialFieldStore.partialField?.numberWaiting ?? 0 }}
      </h1>
      <h1>
        Số lượng lịch đã đặt sẽ bị huỷ: {{ partialFieldStore.partialField?.numberAccepted ?? 0 }}
      </h1>
      <div class="grid grid-cols-2 gap-4 mt-5">
        <Button @click="isConfirmCancelDiablog = false" label="Đóng" outlined />
        <Button @click="onDisablePartialField" label="Đồng ý" />
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import * as yup from 'yup'
import { useI18n } from 'vue-i18n'
import { useForm } from 'vee-validate'
import { ref, watch } from 'vue'
import { usePartialFieldStore } from '@/stores/partialFieldStore'
import { useToast } from 'primevue/usetoast'
import type { CreatePartialFieldRequest, UpdatePartialFieldRequest } from '@/api/partialFieldApi'
import ActionButton from '../buttons/ActionButton.vue'
import Button from 'primevue/button'

const { t } = useI18n()
const partialFieldStore = usePartialFieldStore()
const toast = useToast()
const isConfirmCancelDiablog = ref(false)

const props = defineProps<{
  open: boolean
  isCreate: boolean
  fieldId: number
  partialFieldId?: number
}>()

const schema = yup.object({
  name: yup.string().required(t('validation.required')),
  description: yup.string().required(t('validation.required'))
})

const { meta, errors, defineField, handleSubmit, resetForm, values } =
  useForm<CreatePartialFieldRequest>({
    validationSchema: schema
  })

const emit = defineEmits<{ (e: 'close'): void; (e: 'afterSuccess'): void }>()

const cancel = () => {
  emit('close')
}

const onDisablePartialField = () => {
  isConfirmCancelDiablog.value = false
  if (props.partialFieldId) {
    partialFieldStore
      .updatePartialField({
        ...values,
        id: props.partialFieldId,
        status: 'INACTIVE'
      })
      .then((response) => {
        if (response.success) {
          toast.add({
            severity: 'success',
            summary: 'Thành công',
            detail: 'Vô hiệu hoá sân nhỏ thành công',
            life: 3000
          })
          emit('afterSuccess')
        } else {
          toast.add({
            severity: 'error',
            summary: 'Thất bại',
            detail: 'Vô hiệu hoá sân nhỏ thất bại, vui lòng thử lại',
            life: 3000
          })
        }
      })
  }
}

const submit = handleSubmit(async (values) => {
  if (props.isCreate) {
    if (!props.fieldId) return

    let response = await partialFieldStore.createPartialField({
      ...values,
      fieldId: props.fieldId
    })
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Thành công',
        detail: 'Đã thêm sân nhỏ thành công',
        life: 3000
      })
      emit('afterSuccess')
    } else {
      toast.add({
        severity: 'error',
        summary: 'Thất bại',
        detail: 'Thêm sân nhỏ thất bại, vui lòng thử lại',
        life: 3000
      })
    }
  } else if (props.partialFieldId) {
    let response = await partialFieldStore.updatePartialField({
      ...values,
      id: props.partialFieldId
    })
    if (response.success) {
      toast.add({
        severity: 'success',
        summary: 'Thành công',
        detail: 'Cập nhật sân nhỏ thành công',
        life: 3000
      })
      emit('afterSuccess')
    }
  }
})
const [name] = defineField('name')
const [description] = defineField('description')

watch(
  () => props.open,
  async (value) => {
    if (value) {
      if (props.isCreate) {
        values.description = ''
        values.name = ''
      } else if (props.partialFieldId) {
        await partialFieldStore.getPartialFieldById(props.partialFieldId)
        resetForm({
          values: partialFieldStore.partialField
        })
      }
    }
  }
)
</script>
