<script lang="ts" setup>
import { Page } from '@vben/common-ui';
import { useRefresh } from '@vben/hooks';
     
import { ref } from 'vue';

import { ElButton, ElCard, ElInput, ElNotification, ElSpace } from 'element-plus';

import { corePostApi } from '#/api/core/core';
import { GetQrcodeApi, VertyQrcodeApi,
  FindAllApi, GetPagerApi, GetApi, InsertApi, UpdateApi, DeleteApi
 } from '#/api';

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
async function openGaussClick(flag) {
  switch (flag) {
    case 0: {
      const { IsVerty } = await VertyQrcodeApi({
    vertyValue: vertyCode.value
  });
      break;
    }
    case 1: {
      break;
    }
    case 2: {
      break;
    }
    case 3: {
      break;
    }
    case 4: {
      break;
    }
    case 5: {
      break;
    }
  }
}



import { AccessControl, useAccess } from '@vben/access';
import { useUserStore } from '@vben/stores';
import type { LoginAndRegisterParams } from '@vben/common-ui';
const { accessMode, hasAccessByCodes } = useAccess();
const userStore = useUserStore();

</script>

<template>
  <Page description="Home页面描述" title="Home页面">
    <ElCard class="mb-5">
      <template #header> 按钮 </template>
      <ElSpace>
        <ElButton type="primary" @click="openGaussClick(0)">
          查询
        </ElButton>
        <ElButton type="primary" @click="sendReqBtn(1)">
          分页查询
        </ElButton>
        <ElButton type="primary" @click="getQrcodeBtn(2)">
          获取
        </ElButton>
        <ElButton type="primary" @click="vertyQrcodeBtn(3)">
          插入
        </ElButton>
        <ElButton type="primary" @click="vertyQrcodeBtn(4)">
          编辑
        </ElButton>
        <ElButton type="primary" @click="vertyQrcodeBtn(5)">
          删除
        </ElButton>
      </ElSpace>
    </ElCard>

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

    <ElCard class="mb-5">
      <span class="font-semibold">当前角色:</span>
        <span class="text-primary mx-4 text-lg">
          {{ userStore.userRoles?.[0] }}
        </span>
    </ElCard>

    <ElCard class="mb-5">
      <span class="font-semibold">组件形式控制 - 权限码:</span>
      <AccessControl :codes="['AC_100100']" type="code">
        <ElButton class="mr-4"> Super 账号可见 ["AC_100100"] </ElButton>
      </AccessControl>
      <AccessControl :codes="['AC_100030']" type="code">
        <ElButton class="mr-4"> Admin 账号可见 ["AC_100030"] </ElButton>
      </AccessControl>
      <AccessControl :codes="['AC_1000001']" type="code">
        <ElButton class="mr-4"> User 账号可见 ["AC_1000001"] </ElButton>
      </AccessControl>
      <AccessControl :codes="['AC_100100', 'AC_100030']" type="code">
        <ElButton class="mr-4">
          Super & Admin 账号可见 ["AC_100100","AC_100030"]
        </ElButton>
      </AccessControl>
    </ElCard>

    <ElCard
      v-if="accessMode === 'frontend'"
      class="mb-5"
    >
    <span class="font-semibold">组件形式控制 - 角色:</span>
      <AccessControl :codes="['super']" type="role">
        <ElButton class="mr-4"> Super 角色可见 </ElButton>
      </AccessControl>
      <AccessControl :codes="['admin']" type="role">
        <ElButton class="mr-4"> Admin 角色可见 </ElButton>
      </AccessControl>
      <AccessControl :codes="['user']" type="role">
        <ElButton class="mr-4"> User 角色可见 </ElButton>
      </AccessControl>
      <AccessControl :codes="['super', 'admin']" type="role">
        <ElButton class="mr-4"> Super & Admin 角色可见 </ElButton>
      </AccessControl>
    </ElCard>

    <ElCard class="mb-5">
      <span class="font-semibold">函数形式控制:</span>
      <ElButton v-if="hasAccessByCodes(['AC_100100'])" class="mr-4">
        Super 账号可见 ["AC_100100"]
      </ElButton>
      <ElButton v-if="hasAccessByCodes(['AC_100030'])" class="mr-4">
        Admin 账号可见 ["AC_100030"]
      </ElButton>
      <ElButton v-if="hasAccessByCodes(['AC_1000001'])" class="mr-4">
        User 账号可见 ["AC_1000001"]
      </ElButton>
      <ElButton v-if="hasAccessByCodes(['AC_100100', 'AC_100030'])" class="mr-4">
        Super & Admin 账号可见 ["AC_100100","AC_100030"]
      </ElButton>
    </ElCard>

    <ElCard class="mb-5">
      <span class="font-semibold">指令方式 - 权限码:</span>
      <ElButton class="mr-4" v-access:code="['AC_100100']">
        Super 账号可见 ["AC_100100"]
      </ElButton>
      <ElButton class="mr-4" v-access:code="['AC_100030']">
        Admin 账号可见 ["AC_100030"]
      </ElButton>
      <ElButton class="mr-4" v-access:code="['AC_1000001']">
        User 账号可见 ["AC_1000001"]
      </ElButton>
      <ElButton class="mr-4" v-access:code="['AC_100100', 'AC_100030']">
        Super & Admin 账号可见 ["AC_100100","AC_100030"]
      </ElButton>
    </ElCard>

    <ElCard class="mb-5">
      <span class="font-semibold">指令方式 - 角色:</span>
      <ElButton class="mr-4" v-access:role="['super']"> Super 角色可见 </ElButton>
      <ElButton class="mr-4" v-access:role="['admin']"> Admin 角色可见 </ElButton>
      <ElButton class="mr-4" v-access:role="['user']"> User 角色可见 </ElButton>
      <ElButton class="mr-4" v-access:role="['super', 'admin']">
        Super & Admin 角色可见
      </ElButton>
    </ElCard>
  </Page>
</template>
