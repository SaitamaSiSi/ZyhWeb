import type { UserInfo } from '@vben/types';

import { requestClient } from '#/api/request';

/**
 * 获取用户信息
 */
export async function getUserInfoApi(username: string) {
  return requestClient.post<UserInfo>('/user/info', {
    username: username
  });
}
