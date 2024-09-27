<script>
import flvjs from 'flv.js';
import {
  onBeforeMount,
  onMounted,
  onBeforeUnmount,
} from "vue";

export default {
  name: 'FlvJsPlayer',
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
      var videoPlayerBox = document.getElementById("flvJsPlayerBox" + props.videoId);
      if (flvjs.isSupported()) {
        var flvPlayer = flvjs.createPlayer({
          isLive: true,
          lazyLoadMaxDuration: 15,
          lazyLoadRecoverDuration: 5,
          autoCleanupSourceBuffer: true,
          autoCleanupMaxBackwardDuration: 30,
          type: 'flv',
          url: props.videoUrl
        });
        flvPlayer.attachMediaElement(videoPlayerBox);
        flvPlayer.load();
        flvPlayer.play();
      }
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
  <video :id="'flvJsPlayerBox' + videoId" class="video-js" controls muted />
</template>

<style scoped></style>
