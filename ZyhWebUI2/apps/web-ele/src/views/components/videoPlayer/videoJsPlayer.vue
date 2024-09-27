<script>
import videojs from 'video.js';
import 'video.js/dist/video-js.css';
import "videojs-flvjs-es6";
import {
  onBeforeMount,
  onMounted,
  onBeforeUnmount,
} from "vue";
import { getVideoStreamType } from "#/views/util/utils";

export default {
  name: 'VideoJsPlayer',
  components: {
  },
  props: {
    videoId: {
      type: Number,
      default: 0,
    },
    videoUrl: {
      type: String,
      default: '',
    },
    videoType: {
      type: String,
      default: 'mp4',
    }
  },
  setup(props, context) {
    var player = null;
    onBeforeMount(() => {
    });
    onMounted(() => {
      var videoPlayerBox = document.getElementById("videoJsPlayerBox" + props.videoId);
      var videoJsSetting = {
        autoplay: true,
        muted: true,
        preload: 'auto',
        controls: true,
        responsive: true,
        sources: [{
          src: props.videoUrl,
          type: getVideoStreamType(props.videoType)
        }],
      };
      switch (props.videoType) {
        case 'hls':
          {
            break;
          }
        case 'flv':
          {
            videoJsSetting['techOrder'] = ["html5", "flvjs"]; // 兼容顺序
            videoJsSetting['flvjs'] = {
              mediaDataSource: {
                isLive: false,
                cors: true,
                withCredentials: false
              }
            }
            break;
          }
      }
      player = videojs(videoPlayerBox, videoJsSetting, () => {
        // player.log('onPlayerReady'); 
      });
    });
    onBeforeUnmount(() => {
      if (player) {
        player.dispose();
      }
    });
    return {
    };
  }
};
</script>

<template>
  <video :id="'videoJsPlayerBox' + videoId" class="video-js vjs-default-skin" data-setup="{}">
    <!-- 可以在这里插入source标签以指定你的视频源 -->
    <!-- <source src="https://vjs.zencdn.net/v/oceans.mp4" type="video/mp4"> -->
  </video>
</template>

<style scoped></style>
