import { requestClient } from '#/api/request';

export namespace QrcoderApi {
  /** 登录接口参数 */
  export interface QrcodeParams {
    vertyValue: string;
  }

  /** 登录接口返回值 */
  export interface QrcodeResult {
    url: string;
    leftTime: number;
    isVerty: boolean;
  }
}

/**
 * 获取二维码
 */
export async function GetQrcodeApi(data: QrcoderApi.QrcodeParams) {
  return requestClient.post<QrcoderApi.QrcodeResult>('/qrcode/getQrcode', data);
}

/**
 * 验证二维码
 */
export async function VertyQrcodeApi(data: QrcoderApi.QrcodeParams) {
  return requestClient.post<QrcoderApi.QrcodeResult>('/qrcode/vertyQrcode', data);
}
