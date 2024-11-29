<script lang="ts" setup>
import type { LoginAndRegisterParams } from '@vben/common-ui';
import { registerApi } from '#/api';

import { ref } from 'vue';

import { AuthenticationRegister } from '@vben/common-ui';
import { LOGIN_PATH } from '@vben/constants';

defineOptions({ name: 'Register' });

const loading = ref(false);

async function handleSubmit(value: LoginAndRegisterParams) {
   
  console.log('register submit:', value);
  // const { accessToken, username, desc } = await registerApi(value);
  const { accessToken, username, desc } = await registerApi(value);
  if (username == '') {
    alert(desc)
  }
}
</script>

<template>
  <AuthenticationRegister
    :loading="loading"
    :login-path="LOGIN_PATH"
    @submit="handleSubmit"
  />
</template>
