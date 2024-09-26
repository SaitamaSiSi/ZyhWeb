<script>
import videojs from 'video.js';
import 'video.js/dist/video-js.css';
import {
  onBeforeMount,
  onMounted,
  onBeforeUnmount,
} from "vue";

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
  },
  setup(props, context) {
    var player = null;
    onBeforeMount(() => {
    });
    onMounted(() => {
      var videoPlayerBox = document.getElementById("videoJsPlayerBox" + props.videoId);
      player = videojs(videoPlayerBox, {
        autoplay: true,
        muted: true,
        preload: 'auto',
        controls: true,
        responsive: true,
        sources: [{
          src: props.videoUrl,
          type: 'application/x-mpegURL'
        }],
      }, () => {
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
  <video :id="'videoJsPlayerBox' + videoId" class="video-js vjs-default-skin" controls preload="auto" data-setup="{}">
    <!-- 可以在这里插入source标签以指定你的视频源 -->
    <!-- <source src="https://vjs.zencdn.net/v/oceans.mp4" type="video/mp4"> -->
  </video>
</template>

<style scoped></style>
