<script>
import { ref, watch } from 'vue';

import GanttComponent from '#/views/components/GanttComponent.vue';
import mycount from '#/views/util/count.js';

export default {
  name: 'Gantt2',
  components: { GanttComponent },
  props: {
    user: {
      type: String,
      default: 'test user',
    },
  },
  setup() {
    // 创建定时器增加count值
    const { count } = mycount();
    watch(count, (newValue, oldValue) => {
      // console.log(newValue, oldValue);
      // console.log(count.value);
    });

    const messages = ref([]);
    const addMessage = (message) => {
      messages.value.unshift(message);
      if (messages.value.length > 40) {
        messages.value.pop();
      }
    };
    const logTaskUpdate = (id, mode, task) => {
      const text = task && task.text ? ` (${task.text})` : '';
      const message = `Task ${mode}: ${id} ${text}`;
      addMessage(message);
    };
    const logLinkUpdate = (id, mode, link) => {
      let message = `Link ${mode}: ${id}`;
      if (link) {
        message += ` ( source: ${link.source}, target: ${link.target} )`;
      }
      addMessage(message);
    };

    return {
      count,
      messages,
      addMessage,
      logLinkUpdate,
      logTaskUpdate,
    };
  },
  data() {
    return {
      tasks: {
        data: [
          {
            id: 1,
            text: 'Task #1',
            start_date: '2020-01-17',
            duration: 3,
            progress: 0.6,
          },
          {
            id: 2,
            text: 'Task #2',
            start_date: '2020-01-20',
            duration: 3,
            progress: 0.4,
          },
        ],
        links: [{ id: 1, source: 1, target: 2, type: '0' }],
      },
    };
  },
};
</script>

<template>
  <div class="container">
    <div class="right-container">
      <ul class="gantt-messages">
        <li
          v-for="(message, index) in messages"
          :key="index"
          class="gantt-message"
        >
          {{ message }}
        </li>
      </ul>
    </div>
    <GanttComponent
      :tasks="tasks"
      class="left-container"
      @link-updated="logLinkUpdate"
      @task-updated="logTaskUpdate"
    />
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
