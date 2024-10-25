<template>
  <div class="flex justify-center items-center">
    <div :style="{ width: '60rem' }">
      <PostDetailPanel></PostDetailPanel>
    </div>
  </div>
</template>

<script setup lang="ts">
import PostDetailPanel from '@/components/panels/PostDetailPanel.vue'
import { useOpponentFindingStore } from '@/stores/opponentFindingStore'
import { onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const opponentFindingStore = useOpponentFindingStore()

onMounted(async () => {
  const id = Number(route.params.id)
  if (!id) return
  await opponentFindingStore.getOpponentFinding(id)
})
</script>
