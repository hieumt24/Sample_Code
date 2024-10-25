<template>
  <Panel pt:header:class="hidden" @click.stop="$emit('showDetailDialog', opponentFinding.id)">
    <div class="flex items-center justify-between py-3">
      <div class="flex items-center gap-2">
        <Avatar
          :image="opponentFinding.userFindingAvatar ?? '/noavatar.png'"
          size="large"
          shape="circle"
        />
        <div>
          <span class="font-bold">{{ opponentFinding.userFindingName }}</span>
          <div>
            <span
              class="text-sm text-gray-500"
              v-tooltip.bottom="{
                value: formatDate(opponentFinding?.createdAt ?? ''),
                autoHide: false
              }"
            >
              {{ formatTimeAgo(opponentFinding?.createdAt ?? '') }}
            </span>
          </div>
        </div>
      </div>
      <div>
        <div v-if="selectedMode === 'Yêu cầu đã gửi'">
          <Tag
            :severity="
              getMyRequestSeverity(
                opponentFinding?.status ?? '',
                opponentFinding?.myRequest?.isAccepted ?? false,
                opponentFinding?.myRequest?.status,
                opponentFinding.isOverdue
              )
            "
            :value="
              getMyRequestVNStatus(
                opponentFinding?.status ?? '',
                opponentFinding?.myRequest?.isAccepted ?? false,
                opponentFinding?.myRequest?.status,
                opponentFinding.isOverdue
              )
            "
          ></Tag>
        </div>
        <div v-if="selectedMode !== 'Yêu cầu đã gửi'">
          &nbsp;&nbsp;
          <Tag
            :severity="getSeverity(opponentFinding?.status ?? '', opponentFinding.isOverdue)"
            :value="getVNStatus(opponentFinding?.status ?? '', opponentFinding.isOverdue)"
          ></Tag>
        </div>
      </div>
    </div>
    <div>
      <p class="m-0">
        {{ isExpanded ? opponentFinding?.content : truncatedContent }}
      </p>
      <div v-if="!isExpanded">
        <Button
          v-if="isTruncated"
          label="Xem thêm"
          plain
          text
          @click.stop="$emit('showDetailDialog', opponentFinding.id)"
        />
      </div>
    </div>
    <div class="mt-4">
      <p class="text-sm text-gray-600">
        <strong>Sân: </strong>
        <a v-if="opponentFinding.fieldId" :href="`field/${opponentFinding.fieldId}`">{{
          opponentFinding.fieldName
        }}</a>
        <span v-else-if="opponentFinding.fieldName">{{ opponentFinding.fieldName }}</span>
        <span v-else>Chưa có sân</span>
      </p>
      <p class="text-sm text-gray-600">
        <strong>Địa chỉ: </strong>
        {{
          opponentFinding.address
            ? opponentFinding.address
            : [opponentFinding.commune, opponentFinding.district, opponentFinding.province]
                .filter((value) => value)
                .join(', ')
        }}
      </p>
      <p class="text-sm text-gray-600">
        <strong>Ngày: </strong>{{ formatDateWithoutHHmm(opponentFinding.date) }}
      </p>
      <p class="text-sm text-gray-600">
        <strong>Thời gian: </strong> {{ opponentFinding.startTime }} - {{ opponentFinding.endTime }}
      </p>
      <p class="text-sm text-gray-600">
        <strong>SĐT: </strong> {{ opponentFinding.userFindingPhoneNumber }}
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
            :label="opponentFinding.totalRequest.toString()"
            @click.stop="$emit('showDetailDialog', opponentFinding.id)"
          ></Button>
          <Button
            icon="pi pi-share-alt"
            severity="secondary"
            rounded
            text
            @click.stop="copyLink"
          ></Button>
        </div>
        <div
          v-if="opponentFinding?.lastUpdatedAt"
          class="text-sm text-gray-500"
          v-tooltip.bottom="{
            value: formatDate(opponentFinding?.lastUpdatedAt ?? ''),
            autoHide: false
          }"
        >
          Cập nhật {{ formatTimeAgo(opponentFinding?.lastUpdatedAt ?? '') }}
        </div>
      </div>
    </template>
  </Panel>
</template>
<script setup lang="ts">
import { BASE_URL } from '@/constants/env'
import { postMaxLength } from '@/constants/opponentFinding'
import type { OpponentFinding } from '@/types/OpponentFinding'
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
import Avatar from 'primevue/avatar'
import Button from 'primevue/button'
import Panel from 'primevue/panel'
import Tag from 'primevue/tag'
import { useToast } from 'primevue/usetoast'
import { computed, ref } from 'vue'

const toast = useToast()

const props = defineProps<{
  opponentFinding: OpponentFinding
  selectedMode: string
}>()

const emit = defineEmits<{
  (e: 'showDetailDialog', opponentFindingId: number): Promise<void>
}>()

const isExpanded = ref(false)

const truncatedContent = computed(() => {
  if ((props.opponentFinding?.content?.length ?? 0) > postMaxLength) {
    return props.opponentFinding?.content.substring(0, postMaxLength) + '...'
  }
  return props.opponentFinding?.content
})

const isTruncated = computed(() => {
  return (props.opponentFinding?.content?.length ?? 0) > postMaxLength
})

const copyLink = () => {
  const link = `${BASE_URL}/user/find-opponent/${props.opponentFinding.id}`
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
