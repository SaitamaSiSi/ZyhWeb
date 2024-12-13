import { baseRequestClient, requestClient } from '#/api/request';

export namespace AuthApi {
  /** 登录接口参数 */
  export interface LoginParams {
    password: string;
    username: string;
  }

  /** 登录接口返回值 */
  export interface LoginResult {
    accessToken: string;
    refreshToken: string;
    desc: string;
    realName: string;
    userId: string;
    username: string;
  }
}

/**
 * 登录
 */
export async function loginApi(data: AuthApi.LoginParams) {
  return requestClient.post<AuthApi.LoginResult>('/auth/login', data);
}

/**
 * 刷新accessToken
 */
export async function refreshTokenApi(data : object) {
  return requestClient.post<AuthApi.LoginResult>('/auth/refresh', data);
}

/**
 * 退出登录
 */
export async function logoutApi() {
  return requestClient.post('/auth/logout', {
  });
}

/**
 * 注册
 */
export async function registerApi(data: AuthApi.LoginParams) {
  return requestClient.post<AuthApi.LoginResult>('/auth/register', data);
}

/**
 * 获取用户权限码
 */

export async function getAccessCodesApi(username: string) {
  return requestClient.post<string[]>('/auth/codes', {
    username: username
  });
}
