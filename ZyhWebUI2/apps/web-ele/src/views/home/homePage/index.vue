<script lang="ts" setup>
import { Page } from '@vben/common-ui';
import { useRefresh } from '@vben/hooks';

import { ElButton, ElCard, ElNotification, ElSpace } from 'element-plus';

import { corePostApi } from '#/api/core/core';

const { refresh } = useRefresh();
type NotificationType = 'error' | 'info' | 'success' | 'warning';

function refreshBtn(type: NotificationType) {
  ElNotification({
    duration: 2500,
    message: '刷新页面',
    type,
  });
  refresh();
}
async function sendReqBtn(type: NotificationType) {
  const { data } = await corePostApi({
    password: '123456',
    username: 'admin',
    withCredentials: true,
  });
  const { code, msg } = data;
  ElNotification({
    duration: 2500,
    message: `请求测试:${code},${msg}`,
    type,
  });
}
</script>

<template>
  <Page description="Home页面描述" title="Home页面">
    <ElCard class="mb-5">
      <template #header> 按钮 </template>
      <ElSpace>
        <ElButton type="primary" @click="refreshBtn('success')">
          刷新
        </ElButton>
        <ElButton type="primary" @click="sendReqBtn('success')">
          请求
        </ElButton>
      </ElSpace>
    </ElCard>
  </Page>
</template>
