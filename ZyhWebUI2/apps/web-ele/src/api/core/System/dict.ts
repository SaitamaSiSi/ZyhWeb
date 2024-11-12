import { requestClient } from '#/api/request';

export namespace DictApi {
  export interface DictCondition {
    Id: string;
  }
}



export async function FindAllDictApi() {
  return requestClient.post<Array<DictApi.DictCondition>>('/dict/findAll');
}
