<script>
import { ref, watch } from 'vue';
import * as THREE from 'three'
import { getAssetsImgFile, getAssetsVideoFile } from '#/views/util/utils'

// 下列全局变量不能定义在data中,否则访问不到read_only对象modelViewMatrix
// 场景
var g_scene = null;
// 网格模型对象Group
var g_group = null;

export default {
  name: 'threeSample',
  components: {
  },
  props: {
  },
  setup() {
    return {
    };
  },
  data() {
    return {
      open: false,
      canvas: null,
      threeWidth: 500,
      threeHeight: 500,
      // 渲染器
      renderer: null,
      // 照相机
      camera: null,
      // 相机类型
      cameraType: '1',

      // 上次动画时间
      lastAnimationTime: new Date(),
      oldMouseX: 0,
      oldMouseY: 0,
      addMouseX: 0,
      addMouseY: 0,
      isMoveabled: false,

      // 地面纹理1
      texture1: null,
      // 地面纹理2
      texture2: null,
      // 选择的小车name
      choosenName: '小车1',
      // 视频
      video: null,

      directionalLight: null,
      lightHelper: null,
      shadowCameraHelper: null
    };
  },
  mounted() {
    this.init()
  },
  methods: {
    init() {
      g_scene = null
      g_group = null

      this.canvas = null
      this.renderer = null
      this.camera = null
      this.cameraType = '1'
      this.texture1 = null
      this.texture2 = null
      this.choosenName = '小车1'
      this.oldMouseX = 0
      this.oldMouseY = 0
      this.addMouseX = 0
      this.addMouseY = 0
      this.isMoveabled = false
      this.open = true
      setTimeout(() => {
        this.threeExcute()
      }, 100)
    },
    // 场景设置
    threeExcute() {
      // 初始化渲染器
      this.initRenderer()
      // 初始化场景
      this.initScene()
      // 初始化相机
      this.initCamera()
      // 初始化光源
      this.initLight()
      // 初始化控制器
      this.initControls()

      this.TestFunc()
      // this.TestFunc1()
      // this.TestFunc2()
      // this.TestFunc3()
      // this.TestFunc4()

      this.renderAmination()
    },
    // 伸拉成型模型
    TestFunc4() {
      // 创建拉伸网格模型
      var shape = new THREE.Shape()
      // 四条直线绘制一个矩形轮廓
      shape.moveTo(0, 0) // 起点
      shape.lineTo(0, 100) // 第2点
      shape.lineTo(100, 100) // 第3点
      shape.lineTo(100, 0) // 第4点
      shape.lineTo(0, 0) // 第5点
      // 创建轮廓的扫描轨迹(3D样条曲线)
      // var curve1 = new THREE.SplineCurve3([
      //   new THREE.Vector3(-10, -50, -50),
      //   new THREE.Vector3(10, 0, 0),
      //   new THREE.Vector3(8, 50, 50),
      //   new THREE.Vector3(-5, 0, 100)
      // ])
      var p1 = new THREE.Vector3(-10, -50, -50)
      var p2 = new THREE.Vector3(10, 0, 0)
      var p3 = new THREE.Vector3(8, 50, 50)
      var p4 = new THREE.Vector3(-5, 0, 100)
      // 三维二次贝赛尔曲线
      var curve2 = new THREE.QuadraticBezierCurve3(p1, p2, p3, p4)
      var geometry = new THREE.ExtrudeGeometry( // 拉伸造型
        shape, // 二维轮廓
        // 拉伸参数
        {
          // amount: 120, // 拉伸长度
          bevelEnabled: false, // 无倒角
          extrudePath: curve2, // 选择扫描轨迹
          steps: 50 // 扫描方向分数
        }
      )

      var material = new THREE.PointsMaterial({
        color: 0x0000ff,
        size: 5.0 // 点对象像素尺寸
      }) // 材质对象
      var mesh = new THREE.Points(geometry, material) // 点模型对象
      g_scene.add(mesh) // 点模型添加到场景中
    },
    // 旋转成型模型
    TestFunc3() {
      // 创建旋转网格模型
      var points3 = [
        new THREE.Vector2(-50, -50),
        new THREE.Vector2(-60, 0),
        new THREE.Vector2(0, 50),
        new THREE.Vector2(60, 0),
        new THREE.Vector2(50, -50),
        new THREE.Vector2(-50, -50)
      ]
      var shape3 = new THREE.Shape() // 创建Shape对象
      shape3.absarc(0, 0, 100, 0, 2 * Math.PI) // 圆弧轮廓

      var shape12 = new THREE.Shape() // 创建Shape对象
      var points1 = [
        new THREE.Vector2(50, 60),
        new THREE.Vector2(25, 0),
        new THREE.Vector2(50, -60)
      ]

      var p1 = new THREE.Vector3(-80, 0, 0)
      var p2 = new THREE.Vector3(0, 100, 0)
      var p3 = new THREE.Vector3(80, 0, 0)
      // 三维二次贝赛尔曲线
      var curve = new THREE.QuadraticBezierCurve3(p1, p2, p3)
      var points2 = curve.getPoints(100)

      shape12.splineThru(points2) // 顶点带入样条插值计算函数
      var splinePoints = shape12.getPoints(20) // 插值计算细分数20
      var geometry2 = new THREE.LatheGeometry(splinePoints, 30) // 旋转造型

      var geometry3 = new THREE.ShapeGeometry(shape3, 25)
      var geometry1 = new THREE.LatheGeometry(points1, 30)
      var material = new THREE.MeshPhongMaterial({
        color: 0x0000ff, // 三角面颜色
        side: THREE.DoubleSide, // 两面可见
        wireframe: true
      })// 材质对象
      material.wireframe = true// 线条模式渲染(查看细分数)
      var mesh = new THREE.Mesh(geometry2, material)// 旋转网格模型对象
      g_scene.add(mesh)// 旋转网格模型添加到场景中
    },
    // 自己绘制多边形和点状图案
    TestFunc2() {
      var geometry1 = new THREE.BufferGeometry() // 创建一个Buffer类型几何体对象
      var geometry2 = new THREE.BufferGeometry() // 创建一个Buffer类型几何体对象
      // 类型数组创建顶点数据
      var vertices1 = new Float32Array([
        0, 0, 0, // 顶点1坐标
        0, 200, 0, // 顶点2坐标
        100, 0, 0, // 顶点3坐标

        0, 0, 0, // 顶点4坐标
        0, 0, 200, // 顶点5坐标
        100, 0, 0, // 顶点6坐标

        0, 200, 0, // 顶点7坐标
        0, 0, 200, // 顶点8坐标
        0, 0, 0 // 顶点9坐标
      ])
      var vertices2 = new Float32Array([
        200, 0, 0, // 顶点1坐标
        300, 0, 0, // 顶点2坐标
        300, 100, 0, // 顶点3坐标
        200, 100, 0 // 顶点4坐标
      ])
      // 创建属性缓冲区对象
      var attribue1 = new THREE.BufferAttribute(vertices1, 3) // 3个为一组，表示一个顶点的xyz坐标
      var attribue2 = new THREE.BufferAttribute(vertices2, 3) // 3个为一组，表示一个顶点的xyz坐标
      // 设置几何体attributes属性的位置属性
      geometry1.attributes.position = attribue1
      geometry2.attributes.position = attribue2
      // 类型数组创建顶点颜色color数据
      var colors1 = new Float32Array([
        1, 0, 0, // 顶点1颜色
        0, 1, 0, // 顶点2颜色
        0, 0, 1, // 顶点3颜色

        1, 1, 0, // 顶点4颜色
        0, 1, 1, // 顶点5颜色
        1, 0, 1, // 顶点6颜色

        0, 0, 1, // 顶点7颜色
        0, 1, 0, // 顶点8颜色
        1, 0, 0 // 顶点9颜色
      ])
      var colors2 = new Float32Array([
        1, 0, 0, // 顶点1颜色
        0, 1, 0, // 顶点2颜色
        0, 0, 1, // 顶点3颜色
        1, 1, 0 // 顶点4颜色
      ])
      // 设置几何体attributes属性的颜色color属性
      geometry1.attributes.color = new THREE.BufferAttribute(colors1, 3) // 3个为一组,表示一个顶点的颜色数据RGB
      geometry2.attributes.color = new THREE.BufferAttribute(colors2, 3) // 3个为一组,表示一个顶点的颜色数据RGB
      var normals1 = new Float32Array([
        0, 0, 1, // 顶点1法向量
        0, 0, 1, // 顶点2法向量
        0, 0, 1, // 顶点3法向量

        0, 1, 0, // 顶点4法向量
        0, 1, 0, // 顶点5法向量
        0, 1, 0, // 顶点6法向量

        1, 0, 0, // 顶点7法向量
        1, 0, 0, // 顶点8法向量
        1, 0, 0 // 顶点9法向量
      ])
      var normals2 = new Float32Array([
        0, 0, 1, // 顶点1法向量
        0, 0, 1, // 顶点2法向量
        0, 0, 1, // 顶点3法向量
        0, 0, 1 // 顶点4法向量
      ])
      // 设置几何体attributes属性的位置normal属性
      geometry1.attributes.normal = new THREE.BufferAttribute(normals1, 3) // 3个为一组,表示一个顶点的法向量数据
      geometry2.attributes.normal = new THREE.BufferAttribute(normals2, 3) // 3个为一组,表示一个顶点的法向量数据

      // Uint16Array类型数组创建顶点索引数据
      var indexes = new Uint16Array([
        // 0对应第1个顶点位置数据、第1个顶点法向量数据
        // 1对应第2个顶点位置数据、第2个顶点法向量数据
        // 索引值3个为一组，表示一个三角形的3个顶点
        0, 1, 2,
        0, 2, 3
      ])
      // 索引数据赋值给几何体的index属性
      geometry2.index = new THREE.BufferAttribute(indexes, 1) // 1个为一组

      // 三角面(网格)渲染模式
      var material1 = new THREE.MeshBasicMaterial({
        // color: 0x0000ff, // 三角面颜色
        vertexColors: true, // 以顶点颜色为准
        // 前面FrontSide  背面：BackSide 双面：DoubleSide
        side: THREE.FrontSide
      }) // 材质对象
      var mesh1 = new THREE.Mesh(geometry1, material1) // 网格模型对象Mesh
      // 三角面(网格)渲染模式
      var material12 = new THREE.MeshBasicMaterial({
        // color: 0x0000ff, // 三角面颜色
        vertexColors: true, // 以顶点颜色为准
        side: THREE.BackSide
      }) // 材质对象
      var mesh2 = new THREE.Mesh(geometry2, material12) // 网格模型对象Mesh

      // 点渲染模式
      var material2 = new THREE.PointsMaterial({
        // 使用顶点颜色数据渲染模型，不需要再定义color属性
        // color: 0xff0000,
        vertexColors: true, // 以顶点颜色为准
        size: 10.0 // 点对象像素尺寸
      }) // 材质对象
      var points = new THREE.Points(geometry1, material2) // 点模型对象

      // 线条渲染模式
      var material3 = new THREE.LineBasicMaterial({
        color: 0xff0000 // 线条颜色
      })// 材质对象
      var line = new THREE.Line(geometry1, material3)// 线条模型对象

      g_scene.add(mesh1)
      g_scene.add(mesh2)
      // g_scene.add(points) // 点对象添加到场景中
      // g_scene.add(line)// 线条对象添加到场景中
    },
    // 多种材质和几何体对象模板
    TestFunc1() {
      g_group = new THREE.Group()
      // 创建网格模型
      var geometry1 = this.createGeometry('Box')
      geometry1.rotateY(90)
      geometry1.rotateZ(40)
      var geometry2 = this.createGeometry('Sphere')
      var geometry3 = this.createGeometry('Cylinder')
      // 将本地坐标轴缩放
      // geometry3.scale(0.5, 1.5, 2)
      var geometry4 = this.createGeometry('Icosahedron')
      // 材质对象Material
      var material1 = this.createMaterial('Lambert')
      var material2 = this.createMaterial('Phong')
      var material3 = this.createMaterial('Basic')
      var material4 = this.createMaterial('Standard')

      var mesh1 = new THREE.Mesh(geometry1, material1)
      mesh1.castShadow = true
      mesh1.name = '立方体'
      g_group.add(mesh1)

      var mesh2 = new THREE.Mesh(geometry2, material2)
      mesh2.castShadow = true
      // 设置mesh2模型对象沿x轴正方向平移120
      mesh2.translateX(200)
      mesh2.name = '圆柱体'
      g_group.add(mesh2)

      var mesh3 = new THREE.Mesh(geometry3, material3)
      mesh3.castShadow = true
      // 设置mesh3模型对象沿Y轴正方向平移120
      mesh3.translateY(200)
      mesh3.name = '球体'
      // mesh3.scale.set(0.5, 1.5, 2)
      g_group.add(mesh3)

      var mesh4 = new THREE.Mesh(geometry4, material4)
      mesh4.castShadow = true
      // 设置mesh4模型对象的xyz坐标为0,0,120
      mesh4.position.set(0, 0, 200)
      mesh4.name = '正二十面体'
      g_group.add(mesh4)

      g_group.position.set(0, 50, 0)

      console.log('本地坐标', mesh1.position)
      // getWorldPosition()方法获得世界坐标
      // 该语句默认在threejs渲染的过程中执行,如果渲染之前想获得世界矩阵属性、世界位置属性等属性，需要通过代码更新
      g_scene.updateMatrixWorld(true)
      var worldPosition = new THREE.Vector3()
      mesh1.getWorldPosition(worldPosition)
      console.log('世界坐标', worldPosition)

      // 聚光灯
      // this.addStopLight(mesh1)
      // 网格模型对象Mesh
      g_scene.add(g_group)
      // 递归遍历
      // this.getAllNode()

      // g_scene.remove(g_group)

      // 立方体法线凹陷贴图
      var geometry5 = this.createGeometry('Box')
      geometry5.scale(2, 2, 2)
      // TextureLoader创建一个纹理加载器对象，可以加载图片作为几何体纹理
      var textureLoader = new THREE.TextureLoader()
      // 加载法线贴图
      var textureNormal = textureLoader.load(getAssetsImgFile ('normal3_256.jpg'))
      var material5 = new THREE.MeshPhongMaterial({
        color: 0xff0000,
        normalMap: textureNormal, // 法线贴图
        // 设置深浅程度，默认值(1,1)。
        normalScale: new THREE.Vector2(3, 3)
      }) // 材质对象Material
      var mesh5 = new THREE.Mesh(geometry5, material5) // 网格模型对象Mesh
      mesh5.castShadow = true
      mesh5.position.set(200, 200, 200)
      g_scene.add(mesh5)

      // 地图贴图
      var geometry6 = new THREE.SphereGeometry(100, 25, 25) // 球体
      // TextureLoader创建一个纹理加载器对象，可以加载图片作为几何体纹理
      var textureLoader1 = new THREE.TextureLoader()
      // 加载纹理贴图
      var texture = textureLoader1.load(getAssetsImgFile('Earth.png'))
      // 加载法线贴图
      var textureNormal1 = textureLoader1.load(getAssetsImgFile('EarthNormal.png'))
      var material6 = new THREE.MeshPhongMaterial({
        map: texture, // 普通颜色纹理贴图
        normalMap: textureNormal1, // 法线贴图
        // 设置深浅程度，默认值(1,1)。
        normalScale: new THREE.Vector2(1.2, 1.2)
      }) // 材质对象Material
      var mesh6 = new THREE.Mesh(geometry6, material6) // 网格模型对象Mesh
      mesh6.castShadow = true
      mesh6.position.set(-200, 200, 200)
      g_scene.add(mesh6)
    },
    // 模拟
    TestFunc() {
      // 小车1
      var planeGeometry1 = this.createGeometry('Box')
      planeGeometry1.scale(2, 2, 2)
      var textureLoader1 = new THREE.TextureLoader()
      // 执行load方法，加载纹理贴图成功后，返回一个纹理对象Texture
      textureLoader1.load(getAssetsImgFile('07204.jpg'), (texture) => {
        var planeMaterial = new THREE.MeshLambertMaterial({
          // color: 0x0000ff,
          // lightMap: texture, // 设置光照贴图
          // lightMapIntensity: 0.5, // 烘培光照的强度. 默认 1.
          // 设置颜色纹理贴图：Texture对象作为材质map属性的属性值
          map: texture // 设置颜色贴图属性值
        }) // 材质对象Material
        var planeMesh = new THREE.Mesh(planeGeometry1, planeMaterial) // 网格模型对象Mesh
        planeMesh.name = '小车1'
        // 设置接收阴影的投影面
        planeMesh.castShadow = true
        planeMesh.receiveShadow = true
        planeMesh.translateX(200)
        planeMesh.translateZ(200)
        g_scene.add(planeMesh) // 网格模型添加到场景中
      })

      // 小车2
      var planeGeometry2 = this.createGeometry('Box')
      planeGeometry2.scale(2, 2, 2)
      var textureLoader2 = new THREE.TextureLoader()
      // 执行load方法，加载纹理贴图成功后，返回一个纹理对象Texture
      textureLoader2.load(getAssetsImgFile('07204.jpg'), (texture) => {
        var planeMaterial = new THREE.MeshLambertMaterial({
          // color: 0x0000ff,
          // 设置颜色纹理贴图：Texture对象作为材质map属性的属性值
          map: texture // 设置颜色贴图属性值
        }) // 材质对象Material
        var planeMesh = new THREE.Mesh(planeGeometry2, planeMaterial) // 网格模型对象Mesh
        planeMesh.name = '小车2'
        // 设置接收阴影的投影面
        planeMesh.castShadow = true
        planeMesh.receiveShadow = true
        planeMesh.translateX(-200)
        planeMesh.translateZ(-200)
        g_scene.add(planeMesh) // 网格模型添加到场景中
      })

      // 警示牌
      var showGeometry = new THREE.PlaneGeometry(200, 200) // 矩形平面
      var showTextureLoader = new THREE.TextureLoader()
      // 执行load方法，加载纹理贴图成功后，返回一个纹理对象Texture
      showTextureLoader.load(getAssetsImgFile('07166.jpg'), (texture) => {
        var showMaterial = new THREE.MeshLambertMaterial({
          // color: 0x0000ff,
          // 设置颜色纹理贴图：Texture对象作为材质map属性的属性值
          map: texture, // 设置颜色贴图属性值
          // 前面FrontSide  背面：BackSide 双面：DoubleSide
          side: THREE.BackSide
        }) // 材质对象Material
        var shouwMesh = new THREE.Mesh(showGeometry, showMaterial) // 网格模型对象Mesh
        // 设置接收阴影的投影面
        shouwMesh.castShadow = true
        shouwMesh.receiveShadow = true
        shouwMesh.translateX(-250)
        shouwMesh.translateY(200)
        g_scene.add(shouwMesh) // 网格模型添加到场景中
      })

      // canvas牌
      var canvasGroetry = new THREE.PlaneGeometry(200, 200)
      // canvas画布对象作为CanvasTexture的参数重建一个纹理对象
      // canvas画布可以理解为一张图片
      // 加载纹理贴图
      const picCanvas = document.createElement('canvas')
      //document.body.appendChild(canvas)
      const ctx = picCanvas.getContext('2d')
      ctx.fillStyle = 'green'
      ctx.fillRect(0, 0, picCanvas.width, picCanvas.height)
      var canvasTexture = new THREE.CanvasTexture(picCanvas)
      // 加载高光贴图
      var canvasTextureLoader = new THREE.TextureLoader()
      var canvasTextureSpecular = canvasTextureLoader.load(getAssetsImgFile('canvas_animation_sun.png'))
      var canvasMaterial = new THREE.MeshPhongMaterial({
        // specular: 0xff0000, // 高光部分的颜色
        shininess: 30, // 高光部分的亮度，默认30
        map: canvasTexture, // 设置纹理贴图
        specularMap: canvasTextureSpecular, // 高光贴图
        side: THREE.FrontSide
      })
      // 创建一个矩形平面网模型，Canvas画布作为矩形网格模型的纹理贴图
      var canvasMesh = new THREE.Mesh(canvasGroetry, canvasMaterial)
      canvasMesh.translateX(250)
      canvasMesh.translateY(200)
      g_scene.add(canvasMesh)

      // 视频牌
      // 创建video对象
      this.video = document.createElement('video')
      this.video.src = getAssetsVideoFile('101611.mp4') // 设置视频地址
      this.video.autoplay = 'autoplay' // 要设置播放
      this.video.loop = true // 循环播放
      this.video.muted = false // 静音
      this.video.style.display = 'none'
      this.canvas.appendChild(this.video)
      // video对象作为VideoTexture参数创建纹理对象
      var videoTexture = new THREE.VideoTexture(this.video)
      var videoGeometry = new THREE.PlaneGeometry(108, 192) // 矩形平面
      var videoMaterial = new THREE.MeshPhongMaterial({
        map: videoTexture, // 设置纹理贴图
        side: THREE.DoubleSide
      }) // 材质对象Material
      var videoMesh = new THREE.Mesh(videoGeometry, videoMaterial) // 网格模型对象Mesh
      videoMesh.translateY(250)
      g_scene.add(videoMesh) // 网格模型添加到场景中
    },
    getAllNode(targetId, targetName) {
      var targetObj = null
      // 递归遍历
      g_scene.traverse(function(obj) {
        if (targetId == obj.id || targetName == obj.name) {
          targetObj = JSON.parse(JSON.stringify(obj))
        }
        // 打印id属性
        console.log(obj.id)
        // 打印name属性
        console.log(obj.name)
        // 打印该对象的父对象
        console.log(obj.parent)
        // 打印该对象的子对象
        console.log(obj.children)
      })

      // 通过ID 和 name 查找对象
      // var idNode = g_scene.getObjectById(id)
      // var nameNode = g_scene.getObjectByName(name)

      return targetObj
    },
    // 动画
    renderAmination() {
      var nowTime = new Date()
      var timeSpan = nowTime - this.lastAnimationTime
      this.lastAnimationTime = nowTime

      // 整体旋转
      // g_group.rotateY(0.001 * timeSpan)
      // 旋转角速度0.001弧度每毫秒
      if (g_group != null) {
        g_group.children.forEach((mesh) => {
          mesh.rotateX(0.001 * timeSpan)
          mesh.rotateY(0.002 * timeSpan)
          mesh.rotateZ(0.003 * timeSpan)
        })
      }
      if (this.texture1 != null) {
        this.texture1.offset.y -= 0.04
      }
      if (this.texture2 != null) {
        this.texture2.offset.y -= 0.04
      }

      if (this.isMoveabled && !isNaN(this.addMouseX) && !isNaN(this.addMouseY)) {
        this.camera.translateX(0 - this.addMouseX)
        this.addMouseX = 0
        this.camera.translateY(this.addMouseY)
        this.addMouseY = 0
        this.camera.lookAt(g_scene.position)
      }
      // 执行渲染操作
      if (this.renderer != null) {
        this.renderer.render(g_scene, this.camera)
      }
      if (this.open) {
        requestAnimationFrame(this.renderAmination)
      }
    },
    // 初始化渲染器
    initRenderer() {
      // 窗口宽度
      // this.threeWidth = window.innerWidth
      // 窗口高度
      // this.threeHeight = window.innerHeight
      // box元素宽度
      this.threeWidth = document.getElementById('box').clientWidth
      // bos元素高度
      this.threeHeight = document.getElementById('box').clientHeight
      // 生成渲染器对象（属性：抗锯齿效果为设置有效）
      this.renderer = new THREE.WebGLRenderer({
        antialias: true
      })
      // 生成渲染器对象（属性：抗锯齿效果为设置有效）
      this.renderer.setSize(this.threeWidth, this.threeHeight)
      // 设置canvas背景色(clearColor)和背景色透明度（clearAlpha）
      this.renderer.shadowMap.enabled = true // 显示阴影
      this.renderer.setClearColor(0x3f3f3f, 1)
      // box元素中插入canvas对象
      document.getElementById('box').appendChild(this.renderer.domElement)
    },
    // 初始化场景
    initScene() {
      g_scene = new THREE.Scene()
      // 辅助坐标系  参数250表示坐标系大小，可以根据场景大小去设置
      var axesHelper = new THREE.AxesHelper(400)
      g_scene.add(axesHelper)

      // 纹理贴图映射到一个矩形平面上
      var planeGeometry = new THREE.PlaneGeometry(400, 800) // 矩形平面
      // TextureLoader创建一个纹理加载器对象，可以加载图片作为几何体纹理
      var textureLoader1 = new THREE.TextureLoader()
      // 执行load方法，加载纹理贴图成功后，返回一个纹理对象Texture
      textureLoader1.load(getAssetsImgFile('07112.jpg'), (texture) => {
        // 设置阵列
        texture.wrapS = THREE.RepeatWrapping
        texture.wrapT = THREE.RepeatWrapping
        // uv两个方向纹理重复数量
        texture.repeat.set(5, 10)
        var planeMaterial = new THREE.MeshLambertMaterial({
          // color: 0x0000ff,
          // 设置颜色纹理贴图：Texture对象作为材质map属性的属性值
          map: texture // 设置颜色贴图属性值
        }) // 材质对象Material
        var planeMesh = new THREE.Mesh(planeGeometry, planeMaterial) // 网格模型对象Mesh
        planeMesh.rotation.x = -0.5 * Math.PI
        planeMesh.position.x = 200
        planeMesh.position.y = -100
        // 设置接收阴影的投影面
        planeMesh.castShadow = true
        planeMesh.receiveShadow = true
        g_scene.add(planeMesh) // 网格模型添加到场景中
        this.texture1 = texture
      })
      // TextureLoader创建一个纹理加载器对象，可以加载图片作为几何体纹理
      var textureLoader2 = new THREE.TextureLoader()
      // 执行load方法，加载纹理贴图成功后，返回一个纹理对象Texture
      textureLoader2.load(getAssetsImgFile('07112.jpg'), (texture) => {
        // 设置阵列
        texture.wrapS = THREE.RepeatWrapping
        texture.wrapT = THREE.RepeatWrapping
        // uv两个方向纹理重复数量
        texture.repeat.set(5, 10)
        var planeMaterial = new THREE.MeshLambertMaterial({
          // color: 0x0000ff,
          // 设置颜色纹理贴图：Texture对象作为材质map属性的属性值
          map: texture // 设置颜色贴图属性值
        }) // 材质对象Material
        var planeMesh = new THREE.Mesh(planeGeometry, planeMaterial) // 网格模型对象Mesh
        planeMesh.rotation.x = -0.5 * Math.PI
        planeMesh.position.x = -200
        planeMesh.position.y = -100
        texture.rotation = Math.PI
        // 设置接收阴影的投影面
        planeMesh.castShadow = true
        planeMesh.receiveShadow = true
        g_scene.add(planeMesh) // 网格模型添加到场景中
        this.texture2 = texture
      })
    },
    // 初始化相机
    initCamera() {
      // 窗口宽高比
      var k = this.threeWidth / this.threeHeight
      // 三维场景显示范围控制系数，系数越大，显示的范围越大
      var s = 500
      // 创建正投影相机
      this.camera = new THREE.OrthographicCamera(-s * k, s * k, s, -s, 1, 1000)
      this.cameraType = '1'
      // 创建透视投影相机
      //this.camera = new THREE.PerspectiveCamera(60, k, 1, 1000)
      //this.cameraType = '2'

      // 设置相机位置
      this.camera.lookAt(g_scene.position)
      // 设置相机方向(指向的场景对象)
      this.camera.position.set(0, 0, 500)
      // 将camera添加到scene中,由于动画函数中会刷新，暂时可以不用添加
      // g_scene.add(this.camera)
    },
    // 初始化光源
    initLight() {
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

      // 点光源
      var point = new THREE.PointLight(0xff0000)
      // 设置光源位置
      point.position.set(400, 200, 300)
      // 将光源添加到场景
      // g_scene.add(point)

      // 聚光光源
      var spotLight = new THREE.SpotLight(0xffffff)
      // 设置聚光光源位置
      spotLight.position.set(0, 400, 400)
      // 设置聚光光源发散角度
      spotLight.angle = 30
      // 设置用于计算阴影的光源对象
      spotLight.castShadow = true
      // 设置计算阴影的区域，注意包裹对象的周围
      spotLight.shadow.camera.near = 1
      spotLight.shadow.camera.far = 300
      spotLight.shadow.camera.fov = 20
      spotLight.shadow.mapSize.set(1024, 1024)
      // g_scene.add(spotLight)

      // 环境光
      var ambient = new THREE.AmbientLight(0xffffff, 0.5)
      g_scene.add(ambient)

      // 多个删除
      // g_scene.remove(ambient, directionalLight)

      // 创建辅助工具
      var lightHelper = new THREE.DirectionalLightHelper(directionalLight)
      g_scene.add(lightHelper)
      var shadowCameraHelper = new THREE.CameraHelper(
        directionalLight.shadow.camera
      )
      g_scene.add(shadowCameraHelper)

      g_scene.add(new THREE.AxesHelper(20))
    },
    // 设置聚光灯
    addStopLight(mesh) {
      // 聚光光源
      var spotLight = new THREE.SpotLight(0xff0000)
      // 设置聚光光源位置
      spotLight.position.set(0, 400, 400)
      // 聚光灯光源指向网格模型mesh2
      spotLight.target = mesh
      // 设置聚光光源发散角度
      spotLight.angle = Math.PI / 6
      // 光对象添加到scene场景中
      g_scene.add(spotLight)
    },
    // 初始化控制器
    initControls() {
      // 创建控件对象
      // var controls = new THREE.OrbitControls(this.camera, this.renderer.domElement)
      var el = document.getElementsByTagName('canvas')
      if (el != null && el.length >= 0) {
        this.canvas = el[0]
        this.canvas.addEventListener('mousemove', this.onDocumentMouseMove, false)
        this.canvas.addEventListener('mousedown', this.onMouseDown, false)
        this.canvas.addEventListener('mouseup', this.onMouseUp, false)
        this.canvas.addEventListener('mouseleave', this.onMouseUp, false)
        window.addEventListener('keypress', this.onKeyPress, false)
        window.addEventListener('keydown', this.onKeyDown, false)
      }
    },
    onMouseDown(event) {
      // console.log('onMouseDown: event = ', event)
      this.isMoveabled = true
      this.mouseX = 0
      this.mouseY = 0
    },
    onMouseUp(event) {
      // console.log('onMouseUp: event = ', event)
      this.isMoveabled = false
      this.mouseX = 0
      this.mouseY = 0
    },
    onDocumentMouseMove(event) {
      // console.log('onDocumentMouseMove: event = ', event)
      if (this.isMoveabled) {
        if (this.mouseX != undefined && this.mouseY != undefined && this.mouseX != 0 && this.mouseY != 0) {
          this.addMouseX += event.clientX - this.mouseX
          this.addMouseY += event.clientY - this.mouseY
        }
        this.mouseX = event.clientX
        this.mouseY = event.clientY
      }
    },
    onKeyPress(event) {
      // console.log('onKeyPress: event = ', event)
      if (this.cameraType == '1') {
        var targetObj = g_scene.getObjectByName(this.choosenName)
        if (event.isTrusted && targetObj != null) {
          switch (event.key.toUpperCase()) {
            case 'W': {
              targetObj.position.z -= 4
              break
            }
            case 'S': {
              targetObj.position.z += 4
              break
            }
            case 'A': {
              targetObj.position.x -= 4
              break
            }
            case 'D': {
              targetObj.position.x += 4
              break
            }
          }
        }
      } else if (this.cameraType == '2') {
        if (event.isTrusted) {
          switch (event.key.toUpperCase()) {
            case 'W': {
              this.camera.position.z -= 4
              break
            }
            case 'S': {
              this.camera.position.z += 4
              break
            }
            case 'A': {
              this.camera.position.x -= 4
              break
            }
            case 'D': {
              this.camera.position.x += 4
              break
            }
          }
        }
      }
    },
    onKeyDown(event) {
      // console.log('onKeyDown: event = ', event)
      if (event.isTrusted) {
        switch (event.key.toUpperCase()) {
          case 'TAB': {
            if (this.choosenName == '小车1') {
              this.choosenName = '小车2'
            } else if (this.choosenName == '小车2') {
              this.choosenName = '小车1'
            }
            break
          }
          case 'V': {
            this.video.muted = !this.video.muted
            break
          }
        }
      }
    },
    // 创建几何对象, Box,Sphere,Cylinder,Octahedron,Dodecahedron,Icosahedron
    createGeometry(type) {
      var geometry = new THREE.BoxGeometry(50, 50, 50)

      if (type == 'Box') {
        // 长方体 参数：长，宽，高
        geometry = new THREE.BoxGeometry(50, 50, 50)
      } else if (type == 'Sphere') {
        // 球体 参数：半径60  经纬度细分数40,40
        geometry = new THREE.SphereGeometry(30, 20, 20)
      } else if (type == 'Cylinder') {
        // 圆柱  参数：圆柱面顶部、底部直径50,50   高度100  圆周分段数
        geometry = new THREE.CylinderGeometry(25, 25, 50, 25)
      } else if (type == 'Octahedron') {
        // 正八面体
        geometry = new THREE.OctahedronGeometry(25)
      } else if (type == 'Dodecahedron') {
        // 正十二面体
        geometry = new THREE.DodecahedronGeometry(25)
      } else if (type == 'Icosahedron') {
        // 正二十面体
        geometry = new THREE.IcosahedronGeometry(25)
      }

      return geometry
    },
    // 创建材质，Basic,Lambert,Phong,Standard
    createMaterial(type) {
      var material = new THREE.MeshLambertMaterial({
        color: 0xFF0000,
        // 透明度
        opacity: 0.6,
        // 是否开启透明
        transparent: true,
        // 将几何图形渲染为线框
        wireframe: true
      })

      if (type == 'Basic') {
        // 基础网格材质，不受光照影响的材质
        material = new THREE.MeshBasicMaterial({
          color: 0xFF0000,
          // 透明度
          opacity: 1,
          // 是否开启透明
          transparent: false,
          // 将几何图形渲染为线框
          wireframe: false
        })
      } else if (type == 'Lambert') {
        // 漫反射,Lambert网格材质，与光照有反应，漫反射
        material = new THREE.MeshLambertMaterial({
          color: 0xffffff,
          // 透明度
          opacity: 1,
          // 是否开启透明
          transparent: false,
          // 将几何图形渲染为线框
          wireframe: false
        })
      } else if (type == 'Phong') {
        // 镜面反射,高光Phong材质,与光照有反应
        material = new THREE.MeshPhongMaterial({
          color: 0xff0000,
          // 高光颜色
          specular: 0x444444,
          // 光照强度系数
          shininess: 50
        })
      } else if (type == 'Standard') {
        // PBR物理材质，相比较高光Phong材质可以更好的模拟金属、玻璃等效果
        material = new THREE.MeshStandardMaterial({
          color: 0x0000FF
        })
      }

      return material
    },
    // 取消按钮
    cancel() {
      if (this.canvas != null) {
        this.canvas.removeEventListener('mousemove', this.onDocumentMouseMove, false)
        this.canvas.removeEventListener('mousedown', this.onMouseDown, false)
        this.canvas.removeEventListener('mouseup', this.onMouseUp, false)
        this.canvas.removeEventListener('mouseleave', this.onMouseUp, false)
        window.removeEventListener('mouseleave', this.onMouseUp, false)
        window.removeEventListener('keydown', this.onKeyDown, false)
      }
      this.clearScene()
      this.open = false
    },
    // 清空场景，包括 scene 场景下的动画，子物体，renderer,camera,control，以及自己使用过的变量置空处理 等
    clearScene() {
      g_scene.traverse((child) => {
        if (child.material) {
          child.material.dispose()
        }
        if (child.geometry) {
          child.geometry.dispose()
        }
        child = null
      })

      // 场景中的参数释放清理或者置空等
      this.renderer.domElement = null
      this.renderer.forceContextLoss()
      this.renderer.dispose()
      g_scene.clear()
      g_scene = null
      this.camera = null
      // this.controls = null
      this.renderer = null
      this.sceneDomElement = null
    }
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
      <div id="box" style="width:500px;height:500px;" />
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
