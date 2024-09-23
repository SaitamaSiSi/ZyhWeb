<script>
import { ref, watch } from 'vue';
import ThreeSample from '#/views/components/three/threeSample.vue'
import ThreeRayCasterSample from '#/views/components/three/threeRayCasterSample.vue'

export default {
  name: 'Three',
  components: {
    ThreeSample,
    ThreeRayCasterSample
  },
  props: {
  },
  setup() {
    return {
    };
  }
};
</script>

<template>
  <div class="container">
    <div class="right-container">
      <ul class="gantt-messages">
        <li class="gantt-message">
          备用
        </li>
      </ul>
    </div>
    <div class="left-container">
      <!-- <ThreeSample /> -->
      <ThreeRayCasterSample />
    </div>
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
.left-container {
  overflow: hidden;
  position: relative;
  height: 100%;
}
.right-container {
  border-right: 1px solid #cecece;
  float: right;
  height: 100%;
  width: 340px;
  box-shadow: 0 0 5px 2px #aaa;
  position: relative;
  z-index: 2;
}
.gantt-messages {
  list-style-type: none;
  height: 50%;
  margin: 0;
  overflow-x: hidden;
  overflow-y: auto;
  padding-left: 5px;
}
.gantt-messages > .gantt-message {
  background-color: #f4f4f4;
  box-shadow: inset 5px 0 #d69000;
  font-family: Geneva, Arial, Helvetica, sans-serif;
  font-size: 14px;
  margin: 5px 0;
  padding: 8px 0 8px 10px;
}
</style>
