// 获取assets静态资源
export const getAssetsImgFile = (url: string) => {
  return new URL(`/src/assets/images/${url}`, import.meta.url).href;
};
export const getAssetsVideoFile = (url: string) => {
  return new URL(`/src/assets/videos/${url}`, import.meta.url).href;
};
