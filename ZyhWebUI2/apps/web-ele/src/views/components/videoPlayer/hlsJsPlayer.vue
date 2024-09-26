<script>
import hlsjs from 'hls.js';

import {
  onBeforeMount,
  onMounted,
  onBeforeUnmount,
} from "vue";

export default {
  name: 'HlsJsPlayer',
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
      if (hlsjs.isSupported()) {
        player = new hlsjs({
          levelTargetDuration: 5, // 设置目标缓冲时间为5秒
          minBufferLength: 3, // 设置最小缓冲长度为3秒
          maxBufferLength: 60, // 设置最大缓冲长度为60秒
        });
        player.loadSource(props.videoUrl);//设置播放路径
        player.attachMedia(videoPlayerBox);//解析到video标签上
        // console.log(player);
        player.on(hlsjs.Events.MANIFEST_PARSED, () => {
          videoPlayerBox.play();
          // console.log("加载成功");
        });
        player.on(hlsjs.Events.ERROR, (event, data) => {
          // 监听出错事件
          // console.log("加载失败");
        });

      } else {
        this.$message.error("不支持的格式");
        return;
      }
    });
    onBeforeUnmount(() => {
      if (player) {
        player.destroy();
      }
    });
    return {
    };
  }
};
</script>

<template>
  <video :id="'videoJsPlayerBox' + videoId" class="video-js" controls muted>
    <!-- 可以在这里插入source标签以指定你的视频源 -->
    <!-- <source src="https://vjs.zencdn.net/v/oceans.mp4" type="video/mp4"> -->
  </video>
</template>

<style scoped></style>
