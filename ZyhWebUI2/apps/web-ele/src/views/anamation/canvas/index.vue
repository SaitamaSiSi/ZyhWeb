<script>
import { ref, getCurrentInstance, onMounted } from 'vue';

export default {
  name: 'CanvasPage',
  components: {
  },
  props: {
  },
  setup(props, context) {
    const { proxy } = getCurrentInstance();
    onMounted(() => {
      var c2 = proxy.$refs.canvasRef2;
      const ctx2 = c2.getContext('2d')
      // 设置填充颜色
      ctx2.fillStyle = "#1D80FC";
      ctx2.fillRect(0, 0, 100, 100);
      // 挖空中间绘制边框
      ctx2.clearRect(20, 20, 60, 60);

      // 放大再缩小
      var c = proxy.$refs.canvasRef1;
      c.width = c.width * window.devicePixelRatio
      c.height = c.height * window.devicePixelRatio
      c.style.width = c.width / window.devicePixelRatio
      c.style.height = c.height / window.devicePixelRatio

      var ctx = c.getContext("2d");
      ctx.fillStyle = "#1D80FC";
      // ctx 缩放，这样就不需要每个数字都做缩放。
      ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
      ctx.fillRect(0, 0, 100, 100);
      ctx.clearRect(20, 20, 60, 60);

    });
    return {
    };
  }
};
</script>

<template>
  <div class="container">
    <canvas ref="canvasRef1" width="100" height="100" />
    <canvas ref="canvasRef2" width="100" height="100" />
  </div>
</template>

<style>
html,
body {
  height: 100%;
  margin: 0;
  padding: 0;
}

.container {
  height: 100vh;
  width: 100%;
}
</style>
