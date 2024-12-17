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

export function getRandomInt(min: number, max: number): number {
  return Math.random() * (max - min + 1) + min;
}

export function _isMp4(path: string) {
  const reg = new RegExp('.mp4', 'i')
  return reg.test(path)
}

export function _isVideo(path: string) {
  const reg = new RegExp('video', 'i')
  return reg.test(path)
}

export function _isGif(path: string) {
  const reg = new RegExp('.gif', 'i')
  return reg.test(path)
}

export function _isImage(path: string) {
  const reg = new RegExp('.jpg|.jepg|.png|.bmp|.svg|.gif', 'i')
  return reg.test(path)
}

export function _isImageType(path: string) {
  const reg = new RegExp('image', 'i')
  return reg.test(path)
}

export function _isUnknowVideo(path: string) {
  const reg = new RegExp('.rm|.flv', 'i')
  return reg.test(path)
}

