<template>
  <div class="py-4">
    <div class="flex items-start space-x-4 pb-2">
      <div class="flex flex-col items-center space-y-4">
        <Avatar
          :image="request.userRequestingAvatar ?? '/noavatar.png'"
          size="large"
          shape="circle"
        />
        <Button
          v-if="
            opponentFindingStore.opponentFinding?.status === 'FINDING' &&
            opponentFindingStore.opponentFinding.isMine &&
            !request.status &&
            !opponentFindingStore.opponentFinding.isOverdue
          "
          icon="pi pi-check"
          text
          raised
          rounded
          @click.stop="acceptRequestConfirmation(request.id)"
        />

        <i
          v-if="request.isAccepted && !request.status"
          class="pi pi-check-circle"
          style="color: green; font-size: 1.5rem"
        ></i>
        <i
          v-if="
            opponentFindingStore.opponentFinding?.status === 'ACCEPTED' &&
            !request.isAccepted &&
            !opponentFindingStore.opponentFinding.isMine
          "
          class="pi pi-times-circle"
          style="color: red; font-size: 1.5rem"
        ></i>
      </div>
      <div class="flex-1 p-4 border bg-white shadow rounded-lg">
        <div class="flex justify-between items-center">
          <div>
            <h4 class="text-gray-800 font-semibold">
              {{ request.userRequestingName }} - {{ request.userRequestingPhoneNumber }}
            </h4>
            <span
              v-if="request?.lastUpdatedAt"
              class="text-sm text-gray-500 text-right"
              v-tooltip.bottom="{
                value: `Gửi: ${formatDate(request?.createdAt ?? '')} \n 
                Cập nhật: ${formatDate(request.lastUpdatedAt ?? '')}
                `,
                autoHide: false
              }"
            >
              {{ formatTimeAgo(request?.lastUpdatedAt ?? '') }} (Cập nhât)
            </span>
            <span
              v-else
              class="text-sm text-gray-500"
              v-tooltip.bottom="{
                value: formatDate(request?.createdAt ?? ''),
                autoHide: false
              }"
            >
              {{ formatTimeAgo(request?.createdAt ?? '') }}
            </span>
          </div>
          <div class="inline">
            <Tag
              v-if="opponentFindingStore.opponentFinding?.isMine && request.status"
              severity="warning"
              :value="getMyVNRequestStatus(request.status)"
            ></Tag>
            &nbsp;&nbsp;
            <Button
              v-if="commentMenuItems && !opponentFindingStore.opponentFinding?.isMine"
              type="button"
              icon="pi pi-ellipsis-h"
              @click="toggle"
              severity="secondary"
              text
              rounded
              aria-haspopup="true"
              aria-controls="overlay_menu"
            />
            <Menu ref="commentMenu" id="overlay_menu" :model="commentMenuItems" :popup="true" />
          </div>
        </div>
        <div>
          <div v-if="isExpanded">
            <Button v-if="isTruncated" label="Thu gọn" plain text @click="toggleContent" />
          </div>
          <p class="mt-2 text-gray-700">
            {{ isExpanded ? request.message : truncatedContent }}
          </p>
          <div v-if="!isExpanded">
            <Button v-if="isTruncated" label="Xem thêm" plain text @click="toggleContent" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { OpponentFindingRequestStatus, postMaxLength } from '@/constants/opponentFinding'
import { useOpponentFindingStore } from '@/stores/opponentFindingStore'
import type { RequestOfOpponentFindingResponse } from '@/types/OpponentFinding'
import { formatDate, formatTimeAgo } from '@/utils/dateUtil'
import Avatar from 'primevue/avatar'
import Button from 'primevue/button'
import Menu from 'primevue/menu'
import Tag from 'primevue/tag'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'
import { computed } from 'vue'
import { ref } from 'vue'

const props = defineProps<{
  request: RequestOfOpponentFindingResponse
}>()

const toast = useToast()
const confirm = useConfirm()
const opponentFindingStore = useOpponentFindingStore()

const commentMenu = ref()
const isExpanded = ref(false)
const commentMenuItems = computed(() => {
  if (opponentFindingStore.opponentFinding?.isOverdue) {
    return undefined
  }
  if (
    props.request.status === OpponentFindingRequestStatus.OVERLAPPED_CANCELLED &&
    props.request.isCanRestore
  ) {
    return [
      {
        items: [
          {
            label: 'Huỷ yêu cầu',
            command: async () => {
              await cancelRequestConfirmation()
            }
          },
          {
            label: 'Khôi phục yêu cầu',
            command: async () => {
              await restoreOverlapRequestByIdConfirmation()
            }
          }
        ]
      }
    ]
  }
  if (!props.request.status) {
    return [
      {
        items: [
          {
            label: 'Huỷ yêu cầu',
            command: async () => {
              await cancelRequestConfirmation()
            }
          }
        ]
      }
    ]
  }
  return undefined
})

const truncatedContent = computed(() => {
  if ((props.request?.message?.length ?? 0) > postMaxLength) {
    return props.request?.message?.substring(0, postMaxLength) + '...'
  }
  return props.request?.message
})

const isTruncated = computed(() => {
  return (props.request?.message?.length ?? 0) > postMaxLength
})

const toggleContent = () => {
  isExpanded.value = !isExpanded.value
}

const toggle = (event: Event) => {
  commentMenu.value.toggle(event)
}

const acceptRequestConfirmation = (requestId: number) => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await acceptRequestOpponentFinding(requestId)
    }
  })
}

const cancelRequestConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await cancelRequestOpponentFinding()
    }
  })
}

const restoreOverlapRequestByIdConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await restoreOverlapRequestById()
    }
  })
}

const acceptRequestOpponentFinding = async (requestId: number) => {
  const response = await opponentFindingStore.acceptRequestOfOpponentFinding(requestId)
  if (response?.success) {
    toast.add({ severity: 'success', detail: 'Bạn đã xác nhận chọn đối thành công!', life: 3000 })
  } else {
    toast.add({
      severity: 'error',
      detail: 'Thao tác không thành công.\nVui lòng thử lại!',
      life: 3000
    })
  }
}

const cancelRequestOpponentFinding = async () => {
  const response = await opponentFindingStore.cancelMyRequest()
  if (response?.success) {
    toast.add({ severity: 'success', detail: 'Bạn huỷ yêu cầu ghép đối thành công!', life: 3000 })
  } else {
    toast.add({
      severity: 'error',
      detail: 'Thao tác không thành công.\nVui lòng thử lại!',
      life: 3000
    })
  }
}

const restoreOverlapRequestById = async () => {
  const response = await opponentFindingStore.restoreOverlapRequestById()
  if (response?.success) {
    toast.add({
      severity: 'success',
      detail: 'Khôi phục yêu cầu bị trùng lịch thành công!',
      life: 3000
    })
  } else {
    toast.add({
      severity: 'error',
      detail: 'Thao tác không thành công.\nVui lòng thử lại!',
      life: 3000
    })
  }
}
const getMyVNRequestStatus = (requestStatus: string) => {
  switch (requestStatus) {
    case 'SELF_CANCELLED':
      if (props.request.isAccepted) return 'Đã huỷ kèo'
      return 'Đã huỷ yêu cầu'
    case 'CANCELLED':
      return 'Đã huỷ yêu cầu bởi bạn'
    case 'OVERLAPPED_CANCELLED':
      return 'Huỷ yêu cầu do trùng lịch'
    default:
      return undefined
  }
}
</script>
