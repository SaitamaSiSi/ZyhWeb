<script>
import { ref, getCurrentInstance, onMounted, onBeforeUnmount } from 'vue';
import { Vector } from "#/views/models/Vector";
import { Particle } from "#/views/models/Particle";
import { PlayerBlock } from "#/views/models/PlayerBlock";
import { Platform } from "#/views/models/Platform";
import { ParticleManager } from "#/views/models/ParticleManager";
import { PlatformManager } from "#/views/models/PlatformManager";
import { getRandomInt } from "#/views/util/utils";

export default {
  name: 'CanvasPage',
  components: {
  },
  props: {
  },
  setup(props, context) {
    const { proxy } = getCurrentInstance();
    let canvasWidth = 860;
    let canvasHeight = 540;
    let isRunning = false
    let playerBlock = new PlayerBlock();
    let particleManager = new ParticleManager();
    let startPos = 440;

    // 初始化玩家信息
    playerBlock.position.x = 150
    playerBlock.position.y = startPos;

    // 初始化平台信息
    let platformManager = new PlatformManager(canvasWidth, canvasHeight, canvasHeight - startPos - playerBlock.width);

    function anamation() {
      if (isRunning) {
        requestAnimationFrame(anamation)
      }

      // 重置画布
      var c = proxy.$refs.canvasRef;
      var ctx = c.getContext("2d");
      ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
      ctx.clearRect(0, 0, canvasWidth, canvasHeight);

      let isOver = platformManager.isIntersectLeft(playerBlock);
      let isTouch = false;
      // 如果解除平台则触发
      if (isTouch) {
        // 绘制粒子
        particleManager.update(playerBlock.position);
        particleManager.draw(ctx);
      }
      
      // 绘制平台
      platformManager.update();
      platformManager.draw(ctx);

      // 玩家移动
      let buttonY = platformManager.getButtonPos(playerBlock.position.x, playerBlock.width, playerBlock.height)
      buttonY = (buttonY == -1) ? (playerBlock.position.y + playerBlock.height) : buttonY;
      playerBlock.update(buttonY);
      playerBlock.draw(ctx);

      console.log('update => ', playerBlock.velocity.y, playerBlock.position.y, buttonY)
    }

    function startClick() {
      if (!isRunning) {
        isRunning = true;
        anamation();
      }
    }
    function jumpClick() {
      console.log('跳 => ', playerBlock.velocity.y, playerBlock.position.y)
      if (isRunning && playerBlock.isJumpable()) {
          playerBlock.jump = true;
        }
    }
    function pauseClick() {
      isRunning = false;
    }
    function stopClick() {
    }

    const handleKeyDown = (event) => {
      // if (event.key === ' ') {
      //   // 跳
      //   console.log('跳 => ', isRunning, playerBlock.isJumpable())
      //   if (isRunning && playerBlock.isJumpable()) {
      //     playerBlock.jump = true;
      //   }
      // }
    }
    onMounted(() => {
      // 添加键盘事件监听
      document.addEventListener('keydown', handleKeyDown)

      // 放大再缩小
      var c = proxy.$refs.canvasRef;
      c.width = canvasWidth * window.devicePixelRatio
      c.height = canvasHeight * window.devicePixelRatio
      c.style.width = c.width / window.devicePixelRatio
      c.style.height = c.height / window.devicePixelRatio

      var ctx = c.getContext("2d");
      ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
      playerBlock.draw(ctx);
    });

    onBeforeUnmount(() => {
      document.removeEventListener('keydown', handleKeyDown)
    })

    return {
      startClick,
      jumpClick,
      pauseClick,
      stopClick
    };
  },
};
</script>

<template>
  <div class="container">
    <el-row>
      <el-col :span="24" style="display: grid;justify-content: center;">
        <canvas ref="canvasRef" />
      </el-col>
      <el-col :span="6" style="text-align: center;">
        <el-button type="primary" @click="startClick">Start</el-button>
      </el-col>
      <el-col :span="6" style="text-align: center;">
        <el-button type="primary" @click="jumpClick">Jump</el-button>
      </el-col>
      <el-col :span="6" style="text-align: center;">
        <el-button type="primary" @click="pauseClick">Pause</el-button>
      </el-col>
      <el-col :span="6" style="text-align: center;">
        <el-button :disabled="true" type="primary" @click="stopClick">Stop</el-button>
      </el-col>
    </el-row>
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
  width: 100%;
  /* display: grid;
  justify-content: center; */
  /* place-items: center; */
}
</style>
