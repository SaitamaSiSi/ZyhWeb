<script>
import { getCurrentInstance, onMounted } from 'vue';

import { gantt } from 'dhtmlx-gantt';

export default {
  emits: ['linkUpdated', 'taskUpdated'],
  props: {
    tasks: {
      default() {
        return { data: [], links: [] };
      },
      type: Object,
    },
  },
  setup(props, context) {
    const { proxy } = getCurrentInstance();
    onMounted(() => {
      gantt.clearAll();
      gantt.config.date_format = '%Y-%m-%d';

      gantt.init(proxy.$refs.ganttContainer);
      gantt.createDataProcessor((entity, action, data, id) => {
        context.emit(`${entity}Updated`, id, action, data);
      });
      gantt.parse(props.tasks);
    });
    return {};
  },
};
</script>

<template>
  <div ref="ganttContainer"></div>
</template>

<style>
@import 'dhtmlx-gantt/codebase/dhtmlxgantt.css';
</style>
