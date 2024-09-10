import { baseRequestClient, requestClient } from '#/api/request';

export namespace CoreApi {
  /** post接口参数 */
  export interface PostParams {
    withCredentials: boolean;
    password: string;
    username: string;
  }

  /** post接口返回值 */
  export interface PostResult {
    code: number;
    msg: string;
  }
}

/**
 * post
 */
export async function corePostApi(param: CoreApi.PostParams) {
  return baseRequestClient.post<CoreApi.PostResult>('/core/post', param);
}

/**
 * get
 */
export async function coreGetApi() {
  return requestClient.get<string>('/core/get');
}
