<script>
import FlvJsPlayer from '#/views/components/videoPlayer/flvJsPlayer.vue';

import { ref } from 'vue';

export default {
  name: 'Zlmediakit4',
  components: {
    FlvJsPlayer
  },
  setup() {
    const items = ref([1, 2, 3, 4]);
    return {
      items
    };
  }
};
</script>

<template>
  <div id="container">
    <h1>Vue3 与 flv.js flv视频播放器示例</h1>
    <CardContent class="flex flex-wrap p-0">
      <template v-for="(item, index) in items" :key="index">
        <div :class="{
          'border-r-0': index % 2 === 1,
          'border-b-0': index < 2,
          'pb-4': index > 1,
        }"
          class="border-border group w-full cursor-pointer border-b border-r border-t p-4 transition-all hover:shadow-xl md:w-1/2 lg:w-1/2">
          <FlvJsPlayer class="my-video-js" :videoId="index" :videoUrl="'http://192.168.1.134:80/live/554.live.flv'" />
        </div>
      </template>
    </CardContent>
  </div>
</template>

<style>
#container {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  color: #0e5194;
  margin-top: 60px;
}
.my-video-js {
  width: 100%;
}
</style>
