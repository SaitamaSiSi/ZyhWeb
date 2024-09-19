import { ref } from 'vue';

export default function mycount() {
  // 创建定时器增加count值
  const count = ref('1');
  setInterval(() => {
    count.value++;
  }, 1000);
  return {
    count,
  };
}
