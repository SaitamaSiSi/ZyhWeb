<script>
import * as THREE from 'three'
import { getAssetsImgFile } from '#/views/util/utils'
import * as threeUtils from '#/views/util/threeUtils'

var gRenderer = null
// 正常的网格对象填充
var gScene = null
// 使用“拾取材质”的网格对象填充
var gPickingScene = null

export default {
  name: 'threeRayCasterSample',
  data() {
    return {
      mCanvas: null,
      mCamera: null,
      mCameraPole: null,

      mPickHelper: null,
      mPickPosition: { x: 0, y: 0 },
    };
  },
  mounted() {
    this.initThreeJS()
  },
  methods: {
    initThreeJS() {
      this.mCanvas = document.getElementById('box');
	    gRenderer = new THREE.WebGLRenderer( { antialias: true, canvas: this.mCanvas } );

      // 相机参数
	    const fov = 60;
	    const aspect = 2; // 画布默认纵横比为2
	    const near = 0.1;
	    const far = 200;
	    this.mCamera = new THREE.PerspectiveCamera( fov, aspect, near, far );
	    this.mCamera.position.z = 30;

      // 场景参数
	    gScene = new THREE.Scene();
	    gScene.background = new THREE.Color( 'white' );

	    gPickingScene = new THREE.Scene();
	    gPickingScene.background = new THREE.Color( 0 );

      // 把摄像机放到自拍杆上 (把它添加为一个对象的子元素)
      // 如此，我们就能通过旋转自拍杆，来移动摄像机
	    this.mCameraPole = new THREE.Object3D();
	    gScene.add(this.mCameraPole);
	    this.mCameraPole.add(this.mCamera);

      // 灯光参数
		  const color = 0xFFFFFF;
		  const intensity = 3;
		  const light = new THREE.DirectionalLight(color, intensity);
		  light.position.set( - 1, 2, 4 );
      // 把光源也绑定到摄像机上，这样光源就会随着摄像机移动
		  this.mCamera.add(light);

      // 模型参数
    	const boxWidth = 1;
	    const boxHeight = 1;
	    const boxDepth = 1;
	    const geometry = new THREE.BoxGeometry( boxWidth, boxHeight, boxDepth );

	    const loader = new THREE.TextureLoader();
	    const texture = loader.load(getAssetsImgFile ('frame.png'));

      // 生成100个立方体，每个立方体的颜色，位置，朝向，缩放都随机
	    const idToObject = {};
	    const numObjects = 100;
	    for ( let i = 0; i < numObjects; ++ i ) {
    		const id = i + 1;
        // 立方体材质
    		const material = new THREE.MeshPhongMaterial({
		    	color: this.randomColor(),
			    map: texture,
			    transparent: true,
			    side: THREE.DoubleSide,
			    alphaTest: 0.5,
		    });

        // 立方体对象
    		const cube = new THREE.Mesh( geometry, material );
  	  	gScene.add( cube );
		    idToObject[ id ] = cube;

        // 随机位置
		    cube.position.set( threeUtils.rand( - 20, 20 ), threeUtils.rand( - 20, 20 ), threeUtils.rand( - 20, 20 ) );
		    cube.rotation.set( threeUtils.rand( Math.PI ), threeUtils.rand( Math.PI ), 0 );
		    cube.scale.set( threeUtils.rand( 3, 6 ), threeUtils.rand( 3, 6 ), threeUtils.rand( 3, 6 ) );

        // 创建可拾取立方体,根据其ID生成不同的颜色，来对应不同的立方体
		    const pickingMaterial = new THREE.MeshPhongMaterial({
			    emissive: new THREE.Color().setHex( id, THREE.NoColorSpace ),
			    color: new THREE.Color( 0, 0, 0 ),
			    specular: new THREE.Color( 0, 0, 0 ),
			    map: texture,
			    transparent: true,
			    side: THREE.DoubleSide,
			    alphaTest: 0.5, // 只渲染纹理的alpha值大于该属性值的部分
			    blending: THREE.NoBlending, // alpha通道不会作用到id生成色
		    });
		    const pickingCube = new THREE.Mesh(geometry, pickingMaterial);
		    gPickingScene.add(pickingCube);
		    pickingCube.position.copy(cube.position);
		    pickingCube.rotation.copy(cube.rotation);
		    pickingCube.scale.copy(cube.scale);
	    }

      // 拾取管理类
    	class GPUPickHelper {
		    constructor() {
			    // 创建一个1px的渲染目标
			    this.pickingTexture = new THREE.WebGLRenderTarget( 1, 1 );
			    this.pixelBuffer = new Uint8Array( 4 );
			    this.pickedObject = null;
			    this.pickedObjectSavedColor = 0;
		    }
		    pick(cssPosition, scene, camera, time) {
			    const { pickingTexture, pixelBuffer } = this;
			    // 恢复上一个被拾取对象的颜色
			    if (this.pickedObject) {
				    this.pickedObject.material.emissive.setHex(this.pickedObjectSavedColor);
				    this.pickedObject = undefined;
			    }

			    // 设置视野偏移来表现鼠标下的1px
			    const pixelRatio = gRenderer.getPixelRatio();
			    camera.setViewOffset(
	    			gRenderer.getContext().drawingBufferWidth, // full width
	    			gRenderer.getContext().drawingBufferHeight, // full top
	     			cssPosition.x * pixelRatio | 0, // rect x
		    		cssPosition.y * pixelRatio | 0, // rect y
		    		1, // rect width
		    		1, // rect height
		    	);
		    	// 渲染场景
		    	gRenderer.setRenderTarget(pickingTexture);
		    	gRenderer.render(scene, camera);
		    	gRenderer.setRenderTarget(null);
		    	
          // 清理视野偏移，回归正常
		    	camera.clearViewOffset();
		    	// 读取像素
		    	gRenderer.readRenderTargetPixels(
		    		pickingTexture,
			    	0, // x
			    	0, // y
			    	1, // width
			    	1, // height
			    	pixelBuffer );

		    	const id = ( pixelBuffer[ 0 ] << 16 ) | ( pixelBuffer[ 1 ] << 8 ) | ( pixelBuffer[ 2 ] );

          // 获取与射线相交的对象
			    const intersectedObject = idToObject[ id ];
			    if (intersectedObject) {
			    	// 找到第一个对象，它是离鼠标最近的对象
			    	this.pickedObject = intersectedObject;
			    	// 保存它的颜色
		    		this.pickedObjectSavedColor = this.pickedObject.material.emissive.getHex();
		    		// 设置它的发光为 黄色/红色闪烁
		    		this.pickedObject.material.emissive.setHex( ( time * 8 ) % 2 > 1 ? 0xFFFF00 : 0xFF0000 );
	    		}
	    	}
    	}

	    this.mPickHelper = new GPUPickHelper();
	    this.clearPickPosition();

	    requestAnimationFrame(this.render);

	    window.addEventListener( 'mousemove', this.setPickPosition );
	    window.addEventListener( 'mouseout', this.clearPickPosition );
	    window.addEventListener( 'mouseleave', this.clearPickPosition );

      // 移动端支持
    	window.addEventListener('touchstart', (event) => {
		    // prevent the window from scrolling
    		event.preventDefault();
    		this.setPickPosition(event.touches[0]);
	    }, { passive: false });
	    window.addEventListener('touchmove', (event) => {
		    this.setPickPosition(event.touches[0]);
	    });
	    window.addEventListener('touchend', this.clearPickPosition);
    },
    // 随机颜色
    randomColor() {
		  return `hsl(${threeUtils.rand(360) | 0}, ${threeUtils.rand(50, 100) | 0}%, 50%)`;
	  },
    // 将位置归一化
    getCanvasRelativePosition(event) {
      const rect = this.mCanvas.getBoundingClientRect();
      return {
        x: (event.clientX - rect.left) * this.mCanvas.width / rect.width,
        y: (event.clientY - rect.top) * this.mCanvas.height / rect.height,
      };
    },
    // 根据用户鼠标位置设置选中位置，像素点拾取
    setPickPosition(event) {
      const pos = this.getCanvasRelativePosition(event);
      this.mPickPosition.x = pos.x;
      this.mPickPosition.y = pos.y;
    },
    // 将选中位置设置为场景之外，即用户未进行选择，清空重置
    clearPickPosition() {
      // 对于触屏，不像鼠标总是能有一个位置坐标，
      // 如果用户不在触摸屏幕，我们希望停止拾取操作。
      // 因此，我们选取一个特别的值，表明什么都没选中
      this.mPickPosition.x = - 100000;
      this.mPickPosition.y = - 100000;
    },
    // 重置窗口大小
    resizeRendererToDisplaySize(renderer) {
      const canvas = renderer.domElement;
      const width = canvas.clientWidth;
      const height = canvas.clientHeight;
      const needResize = canvas.width !== width || canvas.height !== height;
      if (needResize) {
        renderer.setSize( width, height, false );
      }
      return needResize;
    },
    // 渲染函数
    render(time) {
      // 旋转摄像机端点
      time *= 0.001; // 将毫秒单位转换为秒单位;
      if (this.resizeRendererToDisplaySize(gRenderer)) {
        const canvas = gRenderer.domElement;
        this.mCamera.aspect = canvas.clientWidth / canvas.clientHeight;
        this.mCamera.updateProjectionMatrix();
      }
      this.mCameraPole.rotation.y = time * .1;
      this.mPickHelper.pick( this.mPickPosition, gPickingScene, this.mCamera, time );
      gRenderer.render( gScene, this.mCamera );
      requestAnimationFrame(this.render);
    }
  }
};
</script>

<template>
  <canvas id="box" />
</template>

<style>
html,
body {
  height: 100%;
  margin: 0;
  padding: 0;
}
.container {
  height: 100%;
  width: 100%;
}
</style>
