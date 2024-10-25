<template>
  <Dialog
    :visible="open"
    :header="'Chi tiết người dùng'"
    :modal="true"
    @update:visible="emit('close')"
    :draggable="false"
    class="w-[32rem]"
  >
    <div>
      <div>
        <h1>Tên: {{ userStore.user?.userName }}</h1>
        <h1>Email: {{ userStore.user?.email }}</h1>
        <h1>Số điện thoại: {{ userStore.user?.phoneNumber }}</h1>
        <h1>Ngày đăng kí: {{ formatDateYYYYHHHHH(userStore.user?.createdAt) }}</h1>
      </div>
    </div>
    <div class="my-2 mt-5 border-t border-gray-300 pt-4">
      <ActionButton
        @click.stop="emit('close')"
        class="bg-green-400 mr-2"
        is-outlined
        value="Đóng"
      />
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import Dialog from 'primevue/dialog'
import ActionButton from '@/components/buttons/ActionButton.vue'
import { profileStore } from '@/stores/userStore'
import { watch } from 'vue'
import { formatDateYYYYHHHHH } from '@/utils/dateUtil'

const userStore = profileStore()
const emit = defineEmits<(e: 'close') => void>()

const props = defineProps<{
  open: boolean
  userId?: number
}>()

watch(
  () => props.open,
  async (value) => {
    if (value && props.userId) {
      await userStore.getData(props.userId)
    }
  }
)
</script>
