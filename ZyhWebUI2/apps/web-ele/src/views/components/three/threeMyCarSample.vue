<script>
import * as THREE from "three";
import TWEEN from "@tweenjs/tween.js";
import { getAssetsModelFile, getAssetsImgFile } from "#/views/util/utils";
import { GLTFLoader } from "three/examples/jsm/loaders/GLTFLoader";
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import {
  onBeforeMount,
  onMounted,
  onBeforeUpdate,
  onUpdated,
  onBeforeUnmount,
  onUnmounted,
  ref,
} from "vue";

var g_scene = null;

export default {
  name: "threeMyCarSample",
  setup() {
    const width = ref(800);
    const height = ref(800);
    const canvas = ref(null);
    const renderer = ref(null);
    const camera = ref(null);
    const isMoveabled = ref(false);
    const isAnimationPaused = ref(false);
    var doDecompose = false;
    var deComposeDone = false;
    var tweenDecomposeList = [];
    var tweenComposeList = [];
    var mouseObj = {
      x: 0,
      y: 0,
      addX: 0,
      addY: 0,
    }
    function mousemove(event) {
      if (isMoveabled.value) {
        if (mouseObj.x != undefined && mouseObj.y != undefined && mouseObj.x != 0 && mouseObj.y != 0) {
          mouseObj.addX += event.clientX - mouseObj.x
          mouseObj.addY += event.clientY - mouseObj.y
        }
        mouseObj.x = event.clientX
        mouseObj.y = event.clientY
      }
    }
    function mouseleave(event) {
      isMoveabled.value = false
      mouseObj.x = 0
      mouseObj.y = 0
    }
    function mousedown(event) {
      mouseObj.x = event.clientX
      mouseObj.y = event.clientY
      isMoveabled.value = true
    }
    function keypress(event) {
      if (event.isTrusted) {
        switch (event.key.toUpperCase()) {
          case 'W': {
            camera.value.position.z -= 4
            break
          }
          case 'S': {
            camera.value.position.z += 4
            break
          }
          case 'A': {
            camera.value.position.x -= 4
            break
          }
          case 'D': {
            camera.value.position.x += 4
            break
          }
        }
      }
    }

    function keydown(event) {
      if (event.isTrusted) {
        switch (event.key.toUpperCase()) {
          case 'V': {
            {
              doDecompose = !doDecompose;
              deComposeDone = false;
              break;
            }
          }
        }
      }
    }
    //通过组合式API的形式去使用生命周期
    onBeforeMount(() => {
      console.log("onBeforeMount");
    });
    onMounted(() => {
      console.log("onMounted");
      canvas.value = document.getElementById("box");
      // 渲染器
      renderer.value = new THREE.WebGLRenderer({
        antialias: true, // 锯齿属性
        canvas: canvas.value,
      });
      // 获取你屏幕对应的设备像素比.devicePixelRatio告诉threejs,以免渲染模糊问题（为了适应不同的硬件设备屏幕）
      renderer.value.setPixelRatio(window.devicePixelRatio);
      // 设置背景颜色
      renderer.value.setClearColor(0x444444, 1);
      // 设置渲染器大小为画布大小，否则可能出现模糊
      renderer.value.setSize(width.value, height.value);

      // 相机参数
      const fov = 75; // 视场，前方可视的宽度，相机视锥体竖直方向视野角度
      const aspect = width.value / height.value; // 画布默认纵横比为1，相机视锥体水平方向和竖直方向长度比，一般设置为Canvas画布宽高比width / height
      // 实际观察物体处于近裁截面和远裁截面之间
      const near = 1; // 相机视锥体近裁截面相对相机距离
      const far = 1000; // 相机视锥体远裁截面相对相机距离，far-near构成了视锥体高度方向
      // 实例化一个透视投影相机对象
      camera.value = new THREE.PerspectiveCamera(fov, aspect, near, far);
      camera.value.position.set(0, 1, 8);

      // 创建3D场景对象Scene
      g_scene = new THREE.Scene();
      g_scene.background = new THREE.Color("skyBlue");
      // 添加辅助观察坐标系
      const axesHelper = new THREE.AxesHelper(50);
      g_scene.add(axesHelper);

      // 平行光
      var directionalLight = new THREE.DirectionalLight(0xffffff)
      directionalLight.position.set(0, 400, 400)
      // 设置用于计算阴影的光源对象
      directionalLight.castShadow = true
      // 设置计算阴影的区域，最好刚好紧密包围在对象周围
      // 计算阴影的区域过大：模糊  过小：看不到或显示不完整
      directionalLight.shadow.camera.near = 0
      directionalLight.shadow.camera.far = 1000
      directionalLight.shadow.camera.left = -400
      directionalLight.shadow.camera.right = 400
      directionalLight.shadow.camera.top = 400
      directionalLight.shadow.camera.bottom = -400
      // 设置mapSize属性可以使阴影更清晰，不那么模糊
      directionalLight.shadow.mapSize.set(1024, 1024)
      g_scene.add(directionalLight)

      // 设置相机控件轨道控制器OrbitControls
      let controls = new OrbitControls(camera.value, renderer.value.domElement);
      // 如果OrbitControls改变了相机参数，重新调用渲染器渲染三维场景
      controls.addEventListener('change', function () {
        renderer.value.render(g_scene, camera.value); //执行渲染操作
      });// 监听鼠标、键盘事件

      // 加载模型
      function setModelMeshDecompose(glowMaterialList, decompose) {
        // 如果当前模型只有一个材质则不进行拆解
        if (glowMaterialList.length <= 1) return false
        // 修改材质位置移动
        const modelDecomposeMove = (obj, position) => {
          const tween = new TWEEN.Tween(obj.position)
            .to(position, 500)
            .onUpdate(function (val) {
              obj.position.set(val.x || 0, val.y || 0, val.z || 0);
            })
            .start();
          return tween;
        }
        const length = glowMaterialList.length
        const angleStep = (2 * Math.PI) / length;
        // glowMaterialList：当前模型的所有材质列表名称
        glowMaterialList.forEach((name, i) => {
          let mesh = g_scene.getObjectByName(name);
          // 如果 type 类型为Mesh 则修改材质的位置
          if (mesh.type == 'Mesh') {
            // 拆解位置移动的计算公式
            const angle = i * angleStep;
            const x = (decompose) * Math.cos(angle);
            const y = (decompose) * Math.sin(angle);
            const position = {
              x, y, z: 0
            }
            // 执行拆解
            tweenDecomposeList.push(modelDecomposeMove(mesh, position));
            tweenComposeList.push(modelDecomposeMove(mesh, mesh.position.clone()));
          }
        })
      }
      var loader = new GLTFLoader();
      loader.setCrossOrigin("anonymous");
      loader.load(
        getAssetsModelFile("cars/Ferrari/scene.gltf"),
        function (gltf) {
          g_scene.add(gltf.scene);
          //相机观察目标指向Threejs 3D空间中某个位置
          camera.value.lookAt(gltf.scene.position);
          var meshNameList = [];
          gltf.scene.children[0].children[0].children.forEach(object => {
            if (object.type === 'Mesh') {
              meshNameList.push(object.name);
            }
          })
          setModelMeshDecompose(meshNameList, 50);
        }
      );

      // 模拟雨滴
      const texture = new THREE.TextureLoader().load(getAssetsImgFile("雨滴.png"));
      const spriteMaterial = new THREE.SpriteMaterial({
        map: texture,
      });
      const group = new THREE.Group();
      for (let i = 0; i < 160; i++) {
        // 精灵模型共享材质
        const sprite = new THREE.Sprite(spriteMaterial);
        group.add(sprite);
        sprite.scale.set(1, 1, 1);
        // 设置精灵模型位置，在长方体空间上上随机分布
        const x = 100 * (Math.random() - 0.5);
        const y = 60 * Math.random();
        const z = 100 * (Math.random() - 0.5);
        sprite.position.set(x, y, z)
      }
      g_scene.add(group);

      // 渲染函数
      const clock = new THREE.Clock();
      function render(time) {
        // 旋转摄像机端点
        if (renderer.value != null) {
          if (!isAnimationPaused.value) {
            const t = clock.getDelta();
            group.children.forEach(sprite => {
              // 雨滴的y坐标每次减t*60
              sprite.position.y -= t * 15;
              if (sprite.position.y < 0) {
                sprite.position.y = 60;
              }
            });
            renderer.value.render(g_scene, camera.value);
            if (!deComposeDone) {
              if (doDecompose) {
                tweenDecomposeList.forEach((tween) => {
                  tween.update(time);
                });
              } else {
                tweenComposeList.forEach((tween) => {
                  tween.update(time);
                });
              }
              deComposeDone = true;
            }
            requestAnimationFrame(render);
          }
        }
      }
      requestAnimationFrame(render);

      // 注册监听事件
      canvas.value.addEventListener('mousemove', mousemove, false)
      canvas.value.addEventListener('mouseleave', mouseleave, false)
      canvas.value.addEventListener('mousedown', mousedown, false)
      canvas.value.addEventListener('mouseup', mouseleave, false)
      window.addEventListener('keypress', keypress, false)
      window.addEventListener('keydown', keydown, false)
    });
    onBeforeUpdate(() => {
      console.log("onBeforeUpdate");
    });
    onUpdated(() => {
      console.log("onUpdated");
    });
    onBeforeUnmount(() => {
      console.log("onBeforeUnmount");

      // 取消注册监听事件
      canvas.value.removeEventListener('mousemove', mousemove, false)
      canvas.value.removeEventListener('mouseleave', mouseleave, false)
      canvas.value.removeEventListener('mousedown', mousedown, false)
      canvas.value.removeEventListener('mouseup', mouseleave, false)
      window.removeEventListener('keypress', keypress, false)
      window.removeEventListener('keydown', keydown, false)

      // 资源释放清理
      g_scene.traverse((child) => {
        if (child.material) {
          child.material.dispose();
        }
        if (child.geometry) {
          child.geometry.dispose();
        }
        child = null;
      });
      // 场景中的参数释放清理或者置空等
      g_scene.clear();
      g_scene = null;
      camera.value = null;
      renderer.value.domElement = null;
      renderer.value.forceContextLoss();
      renderer.value.dispose();
      renderer.value = null;
    });
    onUnmounted(() => {
      console.log("onUnmounted");
    });
    return {
      width,
      height,
    };
  },
};
</script>

<template>
  <canvas id="box" :style="{ height: height + 'px', width: width + 'px' }" />
</template>

<style>
html,
body {
  height: 100%;
  margin: 0;
  padding: 0;
}
</style>
