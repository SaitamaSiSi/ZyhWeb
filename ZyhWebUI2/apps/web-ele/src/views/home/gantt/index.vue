<!-- eslint-disable vue/order-in-components -->
<script name="Gantt">
import { gantt } from 'dhtmlx-gantt';

import 'dhtmlx-gantt/codebase/dhtmlxgantt.css';

export default {
  data() {
    return {
      options: [
        {
          label: '全部',
          value: '1',
        },
        {
          label: '完成',
          value: '2',
        },
        {
          label: '正常',
          value: '3',
        },
        {
          label: '异常',
          value: '4',
        },
        {
          label: '未启动',
          value: '5',
        },
      ],
      tasks: {
        data: [],
      },
      value: '1',
    };
  },
  methods: {
    // 开始时间-结束时间参数
    DateDifference(strDateStart, strDateEnd) {
      const begintime_ms = Date.parse(
        new Date(strDateStart.replaceAll('-', '/')),
      ); // begintime 为开始时间
      const endtime_ms = Date.parse(new Date(strDateEnd.replaceAll('-', '/'))); // endtime 为结束时间
      const date3 = endtime_ms - begintime_ms; // 时间差的毫秒数
      const days = Math.floor(date3 / (24 * 3600 * 1000));
      return days;
    },
    initData() {
      this.tasks.data = [
        {
          duration: 10,
          id: 1,
          open: true, // 默认打开，
          progress: 0.6,
          start_date: '2020-04-08',
          status: 'parent',
          text: '概念设计',
          toolTipsTxt: 'xxx项目概念设计',
        },
        {
          duration: 3, // 任务时长，从start_date开始计算
          id: 11, // 任务id
          parent: 1, // 父任务ID
          progress: 0.5,
          start_date: '2020-04-08', // 开始时间
          status: 'yellow',
          text: '项目启动会-外部', // 任务名
          toolTipsTxt: 'xxx项目-项目启动会',
          type: 1,
        },
        {
          duration: 2,
          id: 12,
          parent: 1,
          progress: 0.6,
          start_date: '2020-04-11',
          status: 'pink',
          text: '项目启动会-内部',
          toolTipsTxt: 'xxx项目-项目启动会议',
          type: 2,
        },
        {
          duration: 4,
          id: 13,
          parent: 1,
          progress: 1,
          start_date: '2020-04-13',
          status: 'green',
          text: '项目开工会',
          toolTipsTxt: 'xxx项目开工会',
          type: 3,
        },
        {
          duration: 4,
          id: 14,
          parent: 1,
          progress: 0.6,
          start_date: '2020-04-13',
          status: 'popular',
          text: '项目分析',
          toolTipsTxt: 'xxx项目-项目分析',
          type: 4,
        },

        {
          duration: 8,
          id: 2,
          open: true,
          // color:"#409EFF", //设置颜色
          progress: 0.6,
          start_date: '2020-04-08',
          state: 'default',
          status: 'parent',
          text: '方案设计',
          toolTipsTxt: 'xxx方案设计',
        },
        {
          duration: 2,
          id: 21,
          parent: 2,
          progress: 0.6,
          start_date: '2020-04-08',
          status: 'yellow',
          text: '原型图设计',
          toolTipsTxt: 'xxx新项目原型图设计',
          type: 1,
        },
        {
          duration: 2,
          id: 22,
          parent: 2,
          progress: 0.6,
          start_date: '2020-04-09',
          status: 'pink',
          text: '设计图设计',
          toolTipsTxt: 'xxx项目-项目设计图',
          type: 2,
        },
        {
          duration: 2,
          id: 23,
          parent: 2,
          progress: 1,
          start_date: '2020-04-11',
          status: 'green',
          text: '项目确认',
          toolTipsTxt: 'xxx项目-项目确认',
          type: 3,
        },
      ].map((current, ind, arry) => {
        let newObj = {};
        if (current.type) {
          // 存在type字段 说明非一级菜单，判断阶段的具体类型 设置不同颜色
          switch (current.type) {
            case 1: {
              // 冒烟
              newObj = Object.assign({}, current, {
                color: '#fcca02',
              });
              break;
            }
            case 2: {
              // 单元
              newObj = Object.assign({}, current, {
                color: '#fec0dc',
              });
              break;
            }
            case 3: {
              // 回归
              newObj = Object.assign({}, current, {
                color: '#62ddd4',
              });
              break;
            }
            case 4: {
              newObj = Object.assign({}, current, {
                color: '#d1a6ff',
              });
              break;
            }
            // No default
          }
        } else {
          // 一级菜单是蓝色的
          newObj = Object.assign({}, current, {
            color: '#5692f0',
          });
        }
        return newObj;
      });
    },
    selectChange(val) {
      // 测试用例
      const obj = {
        duration: 2, // 任务时长，从start_date开始计算
        id: 24, // 任务id
        parent: 2, // 父任务ID
        progress: 0,
        start_date: '2020-04-15', // 开始时间
        status: 'popular',
        text: '新增任务', // 任务名
        toolTipsTxt: '新增任务',
        type: 4,
      };
      this.tasks.data.push(obj);

      // 数据解析
      gantt.parse(this.tasks);
      // 刷新数据
      gantt.refreshData();
    },
  },
  mounted() {
    this.initData();

    gantt.clearAll();
    // 自适应甘特图的尺寸大小, 使得在不出现滚动条的情况下, 显示全部任务
    gantt.config.autosize = true;
    // 只读模式
    gantt.config.readonly = true;
    // 是否显示左侧树表格
    gantt.config.show_grid = true;
    // 表格列设置
    gantt.config.columns = [
      {
        label: '阶段名字',
        name: 'text',
        onrender(task, node) {
          node.setAttribute(
            'class',
            `gantt_cell gantt_last_cell gantt_cell_tree ${task.status}`,
          );
        },
        tree: true,
        width: '280',
      },
      {
        align: 'center',
        hide: true,
        label: '时长',
        name: 'duration',
        template(obj) {
          return `${obj.duration}天`;
        },
      },
    ];

    const weekScaleTemplate = function (date) {
      const dateToStr = gantt.date.date_to_str('%m %d');
      const endDate = gantt.date.add(
        gantt.date.add(date, 1, 'week'),
        -1,
        'day',
      );
      const weekNum = gantt.date.date_to_str('第 %W 周');
      return weekNum(date);
    };
    const daysStyle = function (date) {
      const dateToStr = gantt.date.date_to_str('%D');
      if (dateToStr(date) === '六' || dateToStr(date) === '日')
        return 'weekend';
      return '';
    };
    gantt.config.subscales = [
      {
        step: 1,
        template: weekScaleTemplate,
        unit: 'week',
      },
      {
        format: '%d',
        step: 1,
        unit: 'day',
      },
    ];

    gantt.plugins({
      tooltip: true,
    });
    gantt.attachEvent('onGanttReady', () => {
      const tooltips = gantt.ext.tooltips;
      gantt.templates.tooltip_text = function (start, end, task) {
        return (
          `${task.toolTipsTxt}<br/>` +
          `阶段：${task.text}<br/>${gantt.templates.tooltip_date_format(start)}`
        );
      };
    });

    // 设置任务条进度内容
    gantt.templates.progress_text = function (start, end, task) {
      return `<div style='text-align:left;color:#fff;padding-left:20px'>${Math.round(
        task.progress * 100,
      )}% </div>`;
    };

    // 任务条显示内容
    gantt.templates.task_text = function (start, end, task) {
      // return task.text + '(' + task.duration + '天)';
      return (
        `<div style='text-align:center;color:#fff'>${task.text}(${
          task.duration
        }天)` + `</div>`
      );
    };

    // gantt.templates.scale_cell_class = function(date) {
    //     /*if(date.getDay()== 0 || date.getDay()== 6){
    //       return "weekend";
    //     }*/
    //     return 'weekend'
    // }

    // 任务栏周末亮色
    /* gantt.templates.task_cell_class = function(item,date){
        if(date.getDay()== 0 || date.getDay()== 6){
          return "weekend";
        }
      };*/

    // 任务条上的文字大小 以及取消border自带样式
    gantt.templates.task_class = function (start, end, item) {
      return item.$level === 0 ? 'firstLevelTask' : 'secondLevelTask';
    };

    gantt.config.layout = {
      cols: [
        {
          min_width: 280,
          rows: [
            {
              scrollable: true,
              scrollX: 'gridScroll',
              scrollY: 'scrollVer',
              view: 'grid',
            },
            {
              group: 'horizontal',
              id: 'gridScroll',
              view: 'scrollbar',
            },
          ],
          width: 280,
        },
        {
          resizer: true,
          width: 1,
        },
        {
          rows: [
            {
              scrollX: 'scrollHor',
              scrollY: 'scrollVer',
              view: 'timeline',
            },
            {
              group: 'horizontal',
              id: 'scrollHor',
              view: 'scrollbar',
            },
          ],
        },
        {
          id: 'scrollVer',
          view: 'scrollbar',
        },
      ],
      css: 'gantt_container',
    };

    // 时间轴图表中，任务条形图的高度
    // gantt.config.task_height = 28
    // 时间轴图表中，甘特图的高度
    // gantt.config.row_height = 36
    // 时间轴图表中，如果不设置，只有行边框，区分上下的任务，设置之后带有列的边框，整个时间轴变成格子状。
    gantt.config.show_task_cells = true;
    // 当task的长度改变时，自动调整图表坐标轴区间用于适配task的长度
    gantt.config.fit_tasks = true;
    gantt.config.min_column_width = 50;
    gantt.config.auto_types = true;
    gantt.config.xml_date = '%Y-%m-%d';
    gantt.config.scale_unit = 'month';
    gantt.config.step = 1;
    gantt.config.date_scale = '%Y年%M';
    gantt.config.start_on_monday = true;
    gantt.config.scale_height = 90;
    gantt.config.autoscroll = true;
    gantt.config.calendar_property = 'start_date';
    gantt.config.calendar_property = 'end_date';
    gantt.config.readonly = true;
    gantt.i18n.setLocale('cn');

    // 初始化
    gantt.init(this.$refs.gantt);
    // 数据解析
    gantt.parse(this.tasks);
  },
  name: 'Gantt1',
};
</script>

<template>
  <div class="container">
    <div class="select-wrap">
      <el-select v-model="value" placeholder="请选择" @change="selectChange">
        <el-option
          v-for="item in options"
          :key="item.value"
          :label="item.label"
          :value="item.value"
        />
      </el-select>
    </div>
    <div ref="gantt" class="gantt-container"></div>
  </div>
</template>

<style lang="scss">
.firstLevelTask {
  border: none;

  .gantt_task_content {
    font-size: 13px;
  }
}

.secondLevelTask {
  border: none;
}

.thirdLevelTask {
  border: 2px solid #da645d;
  color: #da645d;
  background: #da645d;
}

.milestone-default {
  border: none;
  background: rgba(0, 0, 0, 0.45);
}

.milestone-unfinished {
  border: none;
  background: #5692f0;
}

.milestone-finished {
  border: none;
  background: #84bd54;
}

.milestone-canceled {
  border: none;
  background: #da645d;
}

html,
body {
  margin: 0px;
  padding: 0px;
  height: 100%;
  overflow: hidden;
}

.container {
  height: 100%;
  width: 100%;
  position: relative;
  .gantt_grid_head_cell {
    padding-left: 20px;
    text-align: left !important;
    font-size: 14px;
    color: #333;
  }

  .select-wrap {
    position: absolute;
    top: 25px;
    z-index: 99;
    width: 90px;
    left: 180px;

    .el-input__inner {
      border: none;
    }
  }

  .left-container {
    height: 100%;
  }

  .parent {
    .gantt_tree_icon {
      &.gantt_folder_open {
        background-image: url(assets/gantt-icon.svg) !important;
      }
      &.gantt_folder_closed {
        background-image: url(assets/gantt-icon-up.svg) !important;
      }
    }
  }

  .green,
  .yellow,
  .pink,
  .popular {
    .gantt_tree_icon.gantt_file {
      background: none;
      position: relative;

      &::before {
        content: '';
        width: 10px;
        height: 10px;
        border-radius: 50%;
        position: absolute;
        left: 50%;
        top: 50%;
        transform: translate(-50%, -50%);
      }
    }
  }

  .green {
    .gantt_tree_icon.gantt_file {
      &::before {
        background: #84bd54;
      }
    }
  }

  .yellow {
    .gantt_tree_icon.gantt_file {
      &::before {
        background: #fcca02;
      }
    }
  }

  .pink {
    .gantt_tree_icon.gantt_file {
      &::before {
        background: #da645d;
      }
    }
  }

  .popular {
    .gantt_tree_icon.gantt_file {
      &::before {
        background: #d1a6ff;
      }
    }
  }
}

.left-container {
  height: 100%;
}

.gantt_task_content {
  text-align: left;
  padding-left: 10px;
}
</style>
