import { requestClient } from '#/api/request';

export namespace openGaussApi {
  export interface LedEquipEntity {
    Id: string;
    Name: string;
    Type: number | undefined;
    Alarm: boolean | undefined;
    ApplyAt: Date | undefined;
  }

  export interface LedEquipCondition {
    Id: string;
    Name: string;
    Ids: Array<string>;
  }
}



export async function FindAllApi() {
  return requestClient.post<Array<openGaussApi.LedEquipEntity>>('/openGauss/findAll');
}

export async function GetPagerApi(data : openGaussApi.LedEquipCondition) {
  return requestClient.post<Array<openGaussApi.LedEquipEntity>>('/openGauss/getPager', data);
}

export async function GetApi(data: openGaussApi.LedEquipCondition) {
  return requestClient.post<openGaussApi.LedEquipEntity>('/openGauss/get', data);
}

export async function InsertApi(data: openGaussApi.LedEquipEntity) {
  return requestClient.post<string>('/openGauss/insert', data);
}

export async function UpdateApi(data: openGaussApi.LedEquipEntity) {
  return requestClient.post<string>('/openGauss/update', data);
}

export async function DeleteApi(data: openGaussApi.LedEquipCondition) {
  return requestClient.post<string>('/openGauss/delete', data);
}
