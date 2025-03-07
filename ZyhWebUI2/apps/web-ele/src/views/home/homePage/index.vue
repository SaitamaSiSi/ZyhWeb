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
  const data = await corePostApi({
    password: '123456',
    username: 'admin',
    withCredentials: true,
  });
  ElNotification({
    duration: 2500,
    message: `请求测试:${data}`,
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


// 主系统逻辑
const plugins = ref([]);
// 1. 加载插件注册表
// fetch('http://127.0.0.1:5000/api/file/v1/plugins-registry.json')
//   .then(response => response.json())
//   .then(registry => {
//     const enabledPlugins = registry.plugins.filter(p => p.enabled);
//     plugins.value = [...enabledPlugins]
//     renderTabs();
//   });

// 2. 渲染 Tab 栏和内容
function renderTabs() {
  const tabBar = document.getElementById('tab-bar');
  const tabContent = document.getElementById('tab-content');

  // 清空旧内容
  tabBar.innerHTML = '';
  tabContent.innerHTML = '';

  plugins.value.forEach((plugin, index) => {
    // 创建标签按钮
    const tabButton = document.createElement('button');
    tabButton.className = 'tab-button';
    tabButton.innerHTML = `
      <img src="${plugin.icon}" class="tab-icon" />
      ${plugin.name}
    `;
    tabButton.onclick = () => switchTab(plugin.id, index);

    // 默认激活第一个标签
    if (index === 0) tabButton.classList.add('active');
    tabBar.appendChild(tabButton);

    // 创建内容容器
    const contentDiv = document.createElement('div');
    contentDiv.className = 'tab-pane';
    contentDiv.id = `tab-${plugin.id}`;
    contentDiv.style.display = index === 0 ? 'block' : 'none';
    tabContent.appendChild(contentDiv);
  });

  // 初始化第一个标签内容
  if (plugins.value.length > 0) {
    loadPluginContent(plugins.value[0]);
  }
}

// 3. 切换标签页
function switchTab(pluginId, index) {
  // 更新按钮激活状态
  document.querySelectorAll('.tab-button').forEach((btn, i) => {
    btn.classList.toggle('active', i === index);
  });

  // 显示对应内容
  document.querySelectorAll('.tab-pane').forEach(pane => {
    pane.style.display = pane.id === `tab-${pluginId}` ? 'block' : 'none';
  });

  // 按需加载插件内容（避免重复加载）
  const plugin = plugins.value.find(p => p.id === pluginId);
  const contentDiv = document.getElementById(`tab-${pluginId}`);
  if (!contentDiv.hasChildNodes()) {
    loadPluginContent(plugin);
  }
}

// 4. 加载插件内容（iframe 或 Web Component）
function loadPluginContent(plugin) {
  const contentDiv = document.getElementById(`tab-${plugin.id}`);

  if (plugin.type === 'iframe') {
    const iframe = document.createElement('iframe');
    iframe.src = plugin.url;
    iframe.style.width = '100%';
    iframe.style.height = '100%';
    iframe.sandbox = 'allow-scripts allow-same-origin';
    contentDiv.appendChild(iframe);

    // 监听子系统的消息
    window.addEventListener('message', (event) => {
      if (event.origin !== new URL(plugin.url).origin) return;
      console.log('收到插件消息:', event.data);
    });
  } else if (plugin.type === 'web-component') {
    // 动态加载 Web Component 的 JS
    const script = document.createElement('script');
    script.src = plugin.url;
    script.onload = () => {
      const component = document.createElement(plugin.componentTag);
      contentDiv.appendChild(component);
    };
    document.head.appendChild(script);
  }
}

// 5. 子系统注册
// function registerPlugin(newPlugin) {
//   plugins.valuepush(newPlugin);
//   renderTabs(plugins.value)
// }

// // 子系统页面（iframe 内）, 发送消息给主系统
// window.parent.postMessage(
//   {
//     type: 'REQUEST_USER_INFO',
//     payload: { userId: '123' }
//   },
//   'https://main-system.example.com' // 主系统的 Origin
// );

// // 主系统逻辑, 监听子系统的消息
// window.addEventListener('message', (event) => {
//   const plugins = [...enabledPlugins]; // 从注册表获取的插件列表
//   const plugin = plugins.find(p => 
//     event.origin === new URL(p.url).origin
//   );

//   if (!plugin) return; // 忽略未知来源的消息

//   switch (event.data.type) {
//     case 'REQUEST_USER_INFO':
//       // 返回用户数据（需权限检查）
//       if (plugin.permissions.includes('access_user_info')) {
//         event.source.postMessage({
//           type: 'USER_INFO_RESPONSE',
//           payload: { name: 'Alice', role: 'user' }
//         }, event.origin);
//       }
//       break;
//   }
// });


import { AccessControl, useAccess } from '@vben/access';
import { useUserStore } from '@vben/stores';
import type { LoginAndRegisterParams } from '@vben/common-ui';
const { accessMode, hasAccessByCodes } = useAccess();
const userStore = useUserStore();

</script>

<template>
  <Page description="Home页面描述" title="Home页面">

    <ElCard class="mb-5">
      <!-- 主系统页面 -->
      <div class="tab-container">
        <!-- 标签栏 -->
        <div class="tab-bar" id="tab-bar">
          <!-- 动态生成标签按钮 -->
        </div>
        <!-- 内容区域 -->
        <div class="tab-content" id="tab-content">
          <!-- 动态加载插件内容 -->
        </div>
      </div>
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

    <ElCard v-if="accessMode === 'frontend'" class="mb-5">
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

<style lang="scss">
/* Tab 样式 */
.tab-container {
  width: 100%;
  height: 100vh;
}

.tab-bar {
  display: flex;
  border-bottom: 1px solid #ddd;
  background: #f5f5f5;
}

.tab-button {
  padding: 12px 24px;
  border: none;
  background: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  border-bottom: 2px solid transparent;
}

.tab-button.active {
  border-bottom-color: #007bff;
  color: #007bff;
}

.tab-icon {
  width: 20px;
  height: 20px;
}

.tab-pane {
  height: calc(100vh - 50px); /* 减去 Tab 栏高度 */
  display: none;
}
</style>
