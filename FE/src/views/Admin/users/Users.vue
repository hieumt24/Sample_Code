<template>
  <div class="w-full flex">
    <div>
      <SearchTab @search="onSearch" @reset="onReset">
        <InputField
          name="userName"
          :label="$t('searchTab.userName')"
          v-model="userStore.search.userName"
        />
        <InputField name="email" :label="$t('searchTab.email')" v-model="userStore.search.email" />
        <InputField
          name="phoneNumber"
          :label="$t('searchTab.phoneNumber')"
          v-model="userStore.search.phoneNumber"
        />
        <InputField
          name="status"
          :label="$t('searchTab.status')"
          v-model="userStore.search.status"
        />
      </SearchTab>
    </div>
    <div class="flex-1 lg:px-10">
      <CustomTable
        :headers="headers"
        :items="
          userStore.users.map((user) => ({
            ...user,
            status: $t(`userStatus.${user.status}`)
          }))
        "
        :ban="true"
        no-edit
        @detail="onDetail"
        @edit="onEdit"
        @ban="onOpenConfirm"
        :no-delete="true"
        :total="userStore.total"
        :loading="userStore.loading"
        @change-page="userStore.changePageUser"
      />
    </div>
  </div>

  <UserDetailDialog :open="openDetail" :userId="selectedUserId" @close="openDetail = false" />

  <ConfirmDialog
    :open="isConfirmCancelDiablog"
    :message="'Vô hiệu hoá người dùng này?'"
    @close="isConfirmCancelDiablog = false"
    @submit="onBan"
  />
</template>
<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useLayoutStore } from '@/stores/layoutStore'
import CustomTable from '@/components/tables/CustomTable.vue'
import { useI18n } from 'vue-i18n'
import { useToast } from 'primevue/usetoast'
import { profileStore } from '@/stores/userStore'
import router from '@/router'
import SearchTab from '@/components/search/SearchTab.vue'
import { LIMIT_PER_PAGE } from '@/constants/tableValues'
import InputField from '@/components/inputs/InputField.vue'
import UserDetailDialog from '@/components/dialogs/UserDetailDialog.vue'
import ConfirmDialog from '@/components/dialogs/ConfirmDialog.vue'
const { t } = useI18n()

const layoutStore = useLayoutStore()
const toast = useToast()
const userStore = profileStore()

const openDetail = ref(false)
const selectedUserId = ref(0)
const selectedBanUser = ref(0)
const isConfirmCancelDiablog = ref(false)

const onDetail = async (id: number) => {
  selectedUserId.value = id
  openDetail.value = true
}

const onEdit = async (id: number) => {
  router.push(`/admin/user/${id}`)
}

const onSearch = async () => {
  await userStore.getUserList()
}

const onOpenConfirm = async (id: number) => {
  selectedBanUser.value = id
  const user = userStore.users.find((user) => user.id === id)
  if (user?.status === 'ACTIVE') {
    isConfirmCancelDiablog.value = true
  } else {
    toast.add({
      severity: 'info',
      summary: 'Thông báo',
      detail: 'Người dùng đã bị vô hiệu hoá',
      life: 3000
    })
  }
}

const onBan = async () => {
  const data = {
    id: selectedBanUser.value,
    status: 'IN_ACTIVE'
  }
  let response = await userStore.banUser(data)
  if (response.success) {
    toast.add({
      severity: 'success',
      summary: 'Thành công',
      detail: 'Vô hiệu hoá người dùng thành công',
      life: 3000
    })
    isConfirmCancelDiablog.value = false
    await userStore.getUserList()
  } else {
    toast.add({
      severity: 'error',
      summary: 'Thất bại',
      detail: 'Vô hiệu hoá người dùng thất bại',
      life: 3000
    })
  }
}

const onReset = async () => {
  userStore.search = {
    userName: '',
    email: '',
    phoneNumber: '',
    status: '',
    fromCreateDate: '',
    toCreateDate: '',
    limit: LIMIT_PER_PAGE,
    offset: 0
  }
  await userStore.getUserList()
}

onMounted(async () => {
  layoutStore.breadcrumb = [{ label: t('sidebar.admin.users') }]
  userStore.getUserList()
})

const headers = [
  { field: 'userName', header: 'Họ tên' },
  { field: 'email', header: 'Email' },
  { field: 'phoneNumber', header: 'Số điện thoại' },
  { field: 'status', header: 'Trạng thái' }
]
</script>
