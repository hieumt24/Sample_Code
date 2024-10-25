<template>
  <div class="relative w-full">
    <div
      v-if="opponentFindingStore.dialogActionLoading"
      class="absolute inset-0 flex justify-center items-center bg-white bg-opacity-75 z-10"
    >
      <ProgressSpinner strokeWidth="4" />
    </div>
    <div v-if="opponentFindingStore.getSpecificOpponentFindingLoading" class="rounded-lg space-y-3">
      <Skeleton height="15rem"></Skeleton>
    </div>
    <div v-else>
      <Panel pt:header:class="hidden">
        <div class="flex items-center justify-between py-3">
          <div class="flex items-center gap-2">
            <Avatar
              :image="opponentFindingDetail?.userFindingAvatar ?? '/noavatar.png'"
              size="large"
              shape="circle"
            />
            <div>
              <div class="font-bold">{{ opponentFindingDetail?.userFindingName }}</div>
              <span
                class="text-sm text-gray-500"
                v-tooltip.bottom="{
                  value: formatDate(opponentFindingDetail?.createdAt ?? ''),
                  autoHide: false
                }"
              >
                {{ formatTimeAgo(opponentFindingDetail?.createdAt ?? '') }}
              </span>
            </div>
          </div>
          <div>
            <div v-if="!opponentFindingDetail?.isMine && opponentFindingDetail?.myRequest">
              <Tag
                :severity="
                  getMyRequestSeverity(
                    opponentFindingDetail?.status ?? '',
                    opponentFindingDetail?.myRequest?.isAccepted ?? false,
                    opponentFindingDetail?.myRequest?.status,
                    opponentFindingDetail?.isOverdue
                  )
                "
                :value="
                  getMyRequestVNStatus(
                    opponentFindingDetail?.status ?? '',
                    opponentFindingDetail?.myRequest?.isAccepted ?? false,
                    opponentFindingDetail?.myRequest?.status,
                    opponentFindingDetail?.isOverdue
                  )
                "
              ></Tag>
              <div v-if="postMenuItems" class="inline">
                &nbsp;&nbsp;
                <Button
                  type="button"
                  icon="pi pi-ellipsis-h"
                  @click="toggle"
                  severity="secondary"
                  text
                  rounded
                  aria-haspopup="true"
                  aria-controls="overlay_menu"
                />
                <Menu ref="postMenu" id="overlay_menu" :model="postMenuItems" :popup="true" />
              </div>
            </div>
            <div v-else>
              &nbsp;&nbsp;
              <Tag
                :severity="
                  getSeverity(opponentFindingDetail?.status ?? '', opponentFindingDetail?.isOverdue)
                "
                :value="
                  getVNStatus(opponentFindingDetail?.status ?? '', opponentFindingDetail?.isOverdue)
                "
              ></Tag>
              <div v-if="postMenuItems" class="inline">
                &nbsp;&nbsp;
                <Button
                  type="button"
                  icon="pi pi-ellipsis-h"
                  @click="toggle"
                  severity="secondary"
                  text
                  rounded
                  aria-haspopup="true"
                  aria-controls="overlay_menu"
                />
                <Menu ref="postMenu" id="overlay_menu" :model="postMenuItems" :popup="true" />
              </div>
            </div>
          </div>
        </div>
        <div>
          <div v-if="isExpanded">
            <Button v-if="isTruncated" label="Thu gọn" plain text @click="toggleContent" />
          </div>
          <p class="m-0">
            {{ isExpanded ? opponentFindingDetail?.content : truncatedContent }}
          </p>
          <div v-if="!isExpanded">
            <Button v-if="isTruncated" label="Xem thêm" plain text @click="toggleContent" />
          </div>
        </div>
        <div class="mt-4">
          <p class="text-sm text-gray-600">
            <strong>Sân: </strong>
            <a
              v-if="opponentFindingDetail?.fieldId"
              :href="`field/${opponentFindingDetail?.fieldId}`"
              >{{ opponentFindingDetail?.fieldName }}</a
            >
            <span v-else-if="opponentFindingDetail?.fieldName">{{
              opponentFindingDetail?.fieldName
            }}</span>
            <span v-else>Chưa có sân</span>
          </p>
          <p class="text-sm text-gray-600">
            <strong>Địa chỉ: </strong>
            {{
              opponentFindingDetail?.address
                ? opponentFindingDetail?.address
                : [
                    opponentFindingDetail?.commune,
                    opponentFindingDetail?.district,
                    opponentFindingDetail?.province
                  ]
                    .filter((value) => value)
                    .join(', ')
            }}
          </p>
          <p class="text-sm text-gray-600">
            <strong>Ngày: </strong>{{ formatDateWithoutHHmm(opponentFindingDetail?.date ?? '') }}
          </p>
          <p class="text-sm text-gray-600">
            <strong>Thời gian: </strong> {{ opponentFindingDetail?.startTime }} -
            {{ opponentFindingDetail?.endTime }}
          </p>
          <p class="text-sm text-gray-600">
            <strong>SĐT: </strong> {{ opponentFindingDetail?.userFindingPhoneNumber }}
          </p>
        </div>
        <template #footer>
          <div class="flex flex-wrap items-center justify-between gap-3">
            <div class="flex items-center gap-2">
              <Button
                class="w-fit"
                icon="pi pi-comment"
                severity="secondary"
                rounded
                text
                :label="opponentFindingDetail?.totalRequest.toString()"
                disabled
              ></Button>
              <Button
                icon="pi pi-share-alt"
                severity="secondary"
                @click.stop="copyLink"
                rounded
                text
              ></Button>
            </div>
            <div
              v-if="opponentFindingDetail?.lastUpdatedAt"
              class="text-sm text-gray-500"
              v-tooltip.bottom="{
                value: formatDate(opponentFindingDetail?.lastUpdatedAt ?? ''),
                autoHide: false
              }"
            >
              Cập nhật {{ formatTimeAgo(opponentFindingDetail?.lastUpdatedAt ?? '') }}
            </div>
          </div>
        </template>
      </Panel>

      <div class="mx-auto">
        <Message
          v-if="opponentFindingStore.OverlapCanRestoreOpponentFinding.length > 0"
          :closable="false"
          severity="info"
        >
          Bạn có thể khôi phục
          <span class="text-green-900 font-bold text-lg"
            >{{ opponentFindingStore.OverlapCanRestoreOpponentFinding.length }} bài đăng</span
          >
          đã bị hệ thống huỷ do trùng thời gian với bài đăng này!
        </Message>
        <Message
          v-if="opponentFindingStore.OverlapCanRestoreRequest.length > 0"
          :closable="false"
          severity="info"
        >
          Bạn có thể khôi phục
          <span class="text-green-900 font-bold text-lg"
            >{{ opponentFindingStore.OverlapCanRestoreRequest.length }} yêu cầu</span
          >
          đã bị hệ thống huỷ do trùng thời gian với bài đăng này!
        </Message>
        <Message v-if="opponentFindingDetail?.isOverdue" :closable="false" severity="warn">
          Bài tìm đối này đã hết hạn!
        </Message>
        <Message
          v-if="
            !opponentFindingDetail?.isOverdue &&
            opponentFindingDetail?.isMine &&
            opponentFindingDetail?.status === OpponentFindingStatus.OVERLAPPED_CANCELLED &&
            opponentFindingDetail?.isCanRestore
          "
          :closable="false"
          severity="info"
        >
          Có thể khôi phục bài đăng này!
        </Message>
        <Message
          v-if="
            !opponentFindingDetail?.isOverdue &&
            opponentFindingDetail?.myRequest &&
            opponentFindingDetail?.myRequest.status ===
              OpponentFindingRequestStatus.OVERLAPPED_CANCELLED &&
            opponentFindingDetail?.myRequest.isCanRestore
          "
          :closable="false"
          severity="info"
        >
          Có thể khôi phục yêu cầu này!
        </Message>
        <div
          v-if="
            !opponentFindingDetail?.isOverdue &&
            opponentFindingDetail?.status === OpponentFindingStatus.FINDING &&
            !opponentFindingDetail.myRequest &&
            !opponentFindingDetail.isMine
          "
          class="p-4 bg-white"
        >
          <Message
            :closable="false"
            v-if="opponentFindingStore.hasOverlapAcceptedRequest"
            severity="error"
          >
            Một trong những yêu cầu của bạn đã được chấp nhận trong khoảng thời gian này.<br />
            Do đó bạn không thể gửi yêu cầu cho bài đăng này.
          </Message>
          <Message
            :closable="false"
            v-else-if="opponentFindingStore.hasOverlapAcceptedPost"
            severity="error"
          >
            Một trong những bài đăng của bạn đã được chấp nhận trong khoảng thời gian này.<br />
            Do đó bạn không thể gửi yêu cầu cho bài đăng này.
          </Message>
          <div v-else>
            <span class="text-surface-600 dark:text-surface-0/70 block mb-5"
              >Bạn có muốn gửi yêu cầu ghép trận này không?</span
            >
            <Message v-if="opponentFindingStore.hasOverlapRequest" severity="warn">
              Bạn đã gửi yêu cầu đến người khác trong khoảng thời gian này.
              <br />
              Nếu họ chấp nhận yêu cầu của bạn thì các yêu cầu có thời gian chồng chéo nên nhau sẽ
              bị huỷ bỏ.
            </Message>

            <div class="flex items-start space-x-4">
              <Avatar :image="myAvatarImg ?? '/noavatar.png'" size="large" shape="circle" />

              <div class="flex-1 flex flex-col">
                <Textarea autoResize rows="5" cols="30" v-model="requestMessage" />
                <div class="mt-2 flex justify-end">
                  <Button
                    :loading="opponentFindingStore.sendRequestLoading"
                    :disabled="!requestMessage"
                    type="button"
                    label="Xác nhận"
                    @click.stop="confirmRequest()"
                  ></Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Request -->
      <div>
        <div
          class="mx-auto"
          v-for="request in opponentFindingStore.opponentFindingRequests"
          :key="request.id"
        >
          <RequestCommentPanel :request="request"></RequestCommentPanel>
        </div>
        <div
          v-if="opponentFindingDetail?.isMine"
          class="w-full bg-white flex items-center justify-between mb-4 px-4 border shadow rounded-lg"
        >
          <div
            v-if="opponentFindingStore.opponentFindingRequests.length > 0"
            class="flex items-center"
          >
            <span class="mr-2">Yêu cầu mới nhất: </span>
            <InputSwitch
              v-model="isLatestRequest"
              @change="changeOpponentFindingRequestSort"
              class="mr-4"
            />
          </div>
          <PagingElement
            :limit="opponentFindingStore.requestPagination.limit"
            :total="opponentFindingDetail?.totalRequest ?? 0"
            @change-page="opponentFindingStore.changePageRequestsOfOpponentFinding"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import PagingElement from '@/components/pagination/PagingElement.vue'
import RequestCommentPanel from '@/components/panels/RequestCommentPanel.vue'
import { BASE_URL } from '@/constants/env'
import {
  OpponentFindingRequestStatus,
  OpponentFindingStatus,
  postMaxLength
} from '@/constants/opponentFinding'
import { useOpponentFindingStore } from '@/stores/opponentFindingStore'
import {
  formatDate,
  formatDateWithoutHHmm,
  formatTimeAgo,
  getDateFormattedShow
} from '@/utils/dateUtil'
import {
  getMyRequestSeverity,
  getMyRequestVNStatus,
  getSeverity,
  getVNStatus
} from '@/utils/opponentFindingTagUtils'
import { storeToRefs } from 'pinia'
import Avatar from 'primevue/avatar'
import Button from 'primevue/button'
import InputSwitch from 'primevue/inputswitch'
import Menu from 'primevue/menu'
import Message from 'primevue/message'
import Panel from 'primevue/panel'
import ProgressSpinner from 'primevue/progressspinner'
import Skeleton from 'primevue/skeleton'
import Tag from 'primevue/tag'
import Textarea from 'primevue/textarea'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'
import { computed, ref } from 'vue'

const toast = useToast()
const confirm = useConfirm()

const opponentFindingStore = useOpponentFindingStore()
const { opponentFinding: opponentFindingDetail } = storeToRefs(opponentFindingStore)

const isLatestRequest = ref(false)
const requestMessage = ref()
const postMenu = ref()
const isExpanded = ref(false)

const myAvatarImg = computed(() => localStorage.getItem('avatar'))
const postMenuItems = computed(() => {
  if (opponentFindingDetail.value?.isOverdue) {
    return undefined
  }
  if (
    opponentFindingDetail.value?.myRequest?.status === OpponentFindingRequestStatus.CANCELLED ||
    (opponentFindingDetail.value?.myRequest?.status ===
      OpponentFindingRequestStatus.SELF_CANCELLED &&
      opponentFindingDetail.value?.myRequest?.isAccepted)
  ) {
    const items = []
    if (opponentFindingStore.OverlapCanRestoreOpponentFinding.length > 0) {
      items.push({
        label: 'Khôi phục bài đăng bị trùng lịch',
        command: async () => {
          await restoreCancelledOverlapOpponentFindingConfirmation()
        }
      })
    }
    if (opponentFindingStore.OverlapCanRestoreRequest.length > 0) {
      items.push({
        label: 'Khôi phục yêu cầu bị trùng lịch',
        command: async () => {
          await restoreCancelledOverlapRequestConfirmation()
        }
      })
    }
    if (items.length > 0) {
      return [
        {
          items: items
        }
      ]
    }
    return undefined
  }
  if (opponentFindingDetail.value?.isMine) {
    switch (opponentFindingDetail.value?.status) {
      case OpponentFindingStatus.ACCEPTED:
        return [
          {
            items: [
              {
                label: 'Huỷ tìm đối',
                command: async () => {
                  await cancelOpponentFindingConfirmation()
                }
              },
              {
                label: 'Huỷ yêu cầu đã chấp nhận',
                command: async () => {
                  await cancelAcceptedRequestConfirmation()
                }
              }
            ]
          }
        ]
      case OpponentFindingStatus.FINDING:
        return [
          {
            items: [
              {
                label: 'Huỷ tìm đối',
                command: async () => {
                  await cancelOpponentFindingConfirmation()
                }
              }
            ]
          }
        ]
      case OpponentFindingStatus.OPPONENT_CANCELLED:
        return [
          {
            items: [
              {
                label: 'Huỷ tìm đối',
                command: async () => {
                  await cancelOpponentFindingConfirmation()
                }
              },
              {
                label: 'Tìm đối khác',
                command: async () => {
                  await restoreFindingStatusOpponentFindingConfirmation()
                }
              }
            ]
          }
        ]
      case OpponentFindingStatus.OVERLAPPED_CANCELLED: {
        const items = [
          {
            label: 'Huỷ tìm đối',
            command: async () => {
              await cancelOpponentFindingConfirmation()
            }
          }
        ]
        if (opponentFindingDetail.value?.isCanRestore) {
          items.push({
            label: 'Khôi phục bài đăng bị trùng lịch',
            command: async () => {
              await restoreOverlapOpponentFindingByIdConfirmation()
            }
          })
        }
        return [
          {
            items: items
          }
        ]
      }
      default:
        return undefined
    }
  }
  return undefined
})
const truncatedContent = computed(() => {
  if ((opponentFindingDetail.value?.content?.length ?? 0) > postMaxLength) {
    return opponentFindingDetail.value?.content.substring(0, postMaxLength) + '...'
  }
  return opponentFindingDetail.value?.content
})

const isTruncated = computed(() => {
  return (opponentFindingDetail.value?.content?.length ?? 0) > postMaxLength
})

const toggleContent = () => {
  isExpanded.value = !isExpanded.value
}

const toggle = (event: Event) => {
  postMenu.value.toggle(event)
}

const changeOpponentFindingRequestSort = async () => {
  opponentFindingStore.resetRequestPagination()
  if (isLatestRequest.value) {
    opponentFindingStore.requestPagination.IsSortDescByCreatedAt = true
  } else {
    opponentFindingStore.requestPagination.IsSortDescByCreatedAt = false
  }
  await opponentFindingStore.getRequestsOfOpponentFinding()
}

const confirmRequest = async () => {
  const response = await opponentFindingStore.createRequestOfOpponentFinding(requestMessage.value)
  if (response?.success) {
    toast.add({
      severity: 'success',
      detail: 'Bạn đã gửi yêu cầu ghép đối thành công!',
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

const cancelOpponentFinding = async () => {
  const response = await opponentFindingStore.cancelOpponentFinding()
  if (response?.success) {
    toast.add({ severity: 'success', detail: 'Bạn đã huỷ tìm đối thành công!', life: 3000 })
  } else {
    toast.add({
      severity: 'error',
      detail: 'Thao tác không thành công.\nVui lòng thử lại!',
      life: 3000
    })
  }
}

const cancelOpponentFindingConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await cancelOpponentFinding()
    }
  })
}

const cancelAcceptedRequest = async () => {
  const response = await opponentFindingStore.cancelAcceptedRequest()
  if (response?.success) {
    toast.add({
      severity: 'success',
      detail: 'Bạn đã thành công huỷ yêu cầu được chấp nhận!',
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

const cancelAcceptedRequestConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await cancelAcceptedRequest()
    }
  })
}

const restoreFindingStatusOpponentFinding = async () => {
  const response = await opponentFindingStore.restoreFindingState()
  if (response?.success) {
    toast.add({ severity: 'success', detail: 'Chuyển trạng thái tìm đối thành công!', life: 3000 })
  } else {
    toast.add({
      severity: 'error',
      detail: 'Thao tác không thành công.\nVui lòng thử lại!',
      life: 3000
    })
  }
}

const restoreFindingStatusOpponentFindingConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await restoreFindingStatusOpponentFinding()
    }
  })
}

const restoreCancelledOverlapRequest = async () => {
  const response = await opponentFindingStore.restoreCancelledOverlapRequest()
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

const restoreCancelledOverlapRequestConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await restoreCancelledOverlapRequest()
    }
  })
}

const restoreCancelledOverlapOpponentFinding = async () => {
  const response = await opponentFindingStore.restoreCancelledOverlapOpponentFinding()
  if (response?.success) {
    toast.add({
      severity: 'success',
      detail: 'Khôi phục bài đăng bị trùng lịch thành công!',
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

const restoreCancelledOverlapOpponentFindingConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await restoreCancelledOverlapOpponentFinding()
    }
  })
}
const restoreOverlapOpponentFindingById = async () => {
  const response = await opponentFindingStore.restoreOverlapPostById()
  if (response?.success) {
    toast.add({
      severity: 'success',
      detail: 'Khôi phục bài đăng bị trùng lịch thành công!',
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

const restoreOverlapOpponentFindingByIdConfirmation = () => {
  confirm.require({
    group: 'headless',
    header: 'Bạn có chắc chắn với lựa chọn không?',
    message: 'Hãy xác nhận để hoàn tất.',
    accept: async () => {
      await restoreOverlapOpponentFindingById()
    }
  })
}
const copyLink = () => {
  const link = `${BASE_URL}/user/find-opponent/${opponentFindingDetail.value?.id}`
  navigator.clipboard
    .writeText(link)
    .then(() => {
      toast.add({
        severity: 'success',
        detail: 'Sao chép đường dẫn thành công!',
        life: 3000
      })
    })
    .catch((err) => {
      toast.add({
        severity: 'error',
        detail: 'Sao chép đường dẫn không thành công.\nVui lòng thử lại!',
        life: 3000
      })
    })
}
</script>
