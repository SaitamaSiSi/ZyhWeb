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
  return requestClient.post<CoreApi.PostResult>('/core/post', param);
}

/**
 * get
 */
export async function coreGetApi() {
  return requestClient.get<string>('/core/get');
}

export async function coreUploadApi(fileData: Blob) {
  return requestClient.upload('/core/upload', fileData);
}

export async function coreUploadApi2(formData: FormData) {
  return requestClient.upload2('/core/upload', formData);
}
