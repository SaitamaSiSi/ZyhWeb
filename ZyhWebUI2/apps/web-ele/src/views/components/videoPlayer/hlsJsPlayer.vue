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
      var videoPlayerBox = document.getElementById("hlsJsPlayerBox" + props.videoId);
      if (hlsjs.isSupported()) {
        player = new hlsjs({
          // backBufferLength: Infinity 表示对 back buffer length 没有限制;0 保持最小数量。最小量等于区段的目标持续时间，以确保当前播放不会中断
          backBufferLength: 0,
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
  <video :id="'hlsJsPlayerBox' + videoId" class="video-js" controls muted />
</template>

<style scoped></style>
