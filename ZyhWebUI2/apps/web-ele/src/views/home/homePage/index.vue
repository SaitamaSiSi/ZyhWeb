<script lang="ts" setup>
import { Page } from '@vben/common-ui';
import { useRefresh } from '@vben/hooks';
     
import { ref } from 'vue';

import { ElButton, ElCard, ElInput, ElNotification, ElSpace } from 'element-plus';

import { corePostApi } from '#/api/core/core';
import { GetQrcodeApi, VertyQrcodeApi } from '#/api';

const { refresh } = useRefresh();
type NotificationType = 'error' | 'info' | 'success' | 'warning';

// 创建一个ref来存储引用
const imgRef = ref<HTMLVideoElement | null>(null);
// 创建一个ref来存储src
const imgSrc = ref<string>('');
const vertyCode = ref<string>('');

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
async function getQrcodeBtn() {
  const { Url } = await GetQrcodeApi({
    vertyValue: ''
  });
  if (Url !== undefined && imgRef.value !== null) {
    imgSrc.value = 'data:image/png;base64,' + Url;
    imgRef.value.src = imgSrc.value;
  }
}
async function vertyQrcodeBtn() {
  const { IsVerty } = await VertyQrcodeApi({
    vertyValue: vertyCode.value
  });
  ElNotification({
    duration: 2500,
    message: `验证${(IsVerty ? '成功' : '失败')}`,
    type: `${(IsVerty ? 'success' : 'error')}`,
  });    
}
</script>

<template>
  <Page description="Home页面描述" title="Home页面">
    <ElCard class="mb-5">
      <template #header> 按钮 </template>
      <ElSpace>
        <img width="200" height="200" alt="Embedded QR Code" ref="imgRef" />
        <ElInput v-model="vertyCode" style="width: 240px" placeholder="Please input" />
        <ElButton type="primary" @click="refreshBtn('success')">
          刷新
        </ElButton>
        <ElButton type="primary" @click="sendReqBtn('success')">
          请求
        </ElButton>
        <ElButton type="primary" @click="getQrcodeBtn()">
          获取二维码
        </ElButton>
        <ElButton type="primary" @click="vertyQrcodeBtn()">
          验证二维码
        </ElButton>
      </ElSpace>
    </ElCard>
  </Page>
</template>
