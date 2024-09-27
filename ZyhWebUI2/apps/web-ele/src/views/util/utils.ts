// 获取assets静态资源
export const getAssetsImgFile = (url: string) => {
  return new URL(`/src/assets/images/${url}`, import.meta.url).href;
};
export const getAssetsVideoFile = (url: string) => {
  return new URL(`/src/assets/videos/${url}`, import.meta.url).href;
};
export const getAssetsModelFile = (url: string) => {
  return new URL(`/src/assets/models/${url}`, import.meta.url).href;
};
export const getVideoStreamType = (type: string) => {
  switch (type) {
    case 'hls':
      return 'application/x-mpegURL';
    case 'flv':
      return 'video/x-flv';
    case 'opus':
    case 'ogv':
      return 'video/ogg';
    case 'mkv':
      return 'video/x-matroska';
    case 'm4a':
      return 'audio/mp4';
    case 'mp3':
      return 'audio/mpeg'
    case 'mp4':
    case 'mov':
    case 'm4v':
    default:
      return 'video/mp4';
  }
};
