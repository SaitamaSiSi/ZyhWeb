<script>
import { ref, getCurrentInstance } from 'vue'
import ThreeSample from '#/views/components/three/threeSample.vue'
import ThreeRayCasterSample from '#/views/components/three/threeRayCasterSample.vue'
import MyCarSample from '#/views/components/three/threeMyCarSample.vue'
import { _isMp4, _isVideo, _isImage, _isImageType, _isUnknowVideo } from '#/views/util/utils'
import { coreUploadApi2 } from '#/api/core/core';
import CryptoJS from 'crypto-js';

export default {
  name: 'ThreePage',
  components: {
    ThreeSample,
    ThreeRayCasterSample,
    MyCarSample
  },
  props: {
  },
  setup() {
    const { proxy } = getCurrentInstance();
    const fileList = ref([]);
    const proccessFlag = ref(false);
    const uploadType = ref(0);

    function handleChange(file, list) {
      if (list.length > 0) {
        fileList.value = [list[list.length - 1]] // 这一步，是 展示最后一次选择的csv文件
      }

      var names = file.raw.name.split('.')
      var Name = names[0]
      // Name = file.raw.name
      var Size = file.raw.size // (file.raw.size / 1024.0).toFixed(2);
      var Mime = file.raw.type
      var Width = 0
      var Height = 0
      var Duration = 0
      var localUrl = null

      if (localUrl != null) {
        window.URL.revokeObjectURL(localUrl)
        localUrl = null
      }
      if (_isImageType(file.raw.type) || _isImage(file.raw.name)) {
        uploadType.value = 1;
        var img = new Image()
        img.onload = function () {
          document.body.appendChild(this)
          Width = this.offsetWidth
          Height = this.offsetHeight
          document.body.removeChild(this)
        }
        localUrl = window.URL.createObjectURL(file.raw)
        img.src = localUrl
        fileList.value[0].url = localUrl
      } else if (_isVideo(file.raw.type) || _isUnknowVideo(file.raw.name)) {
        uploadType.value = 2;
        var videoElement = document.createElement('video')
        videoElement.addEventListener('loadedmetadata', function () {
          Width = videoElement.videoWidth
          Height = videoElement.videoHeight
          Duration = parseInt(videoElement.duration)
        })
        localUrl = window.URL.createObjectURL(file.raw)
        videoElement.src = localUrl
        fileList.value[0].url = localUrl
        videoElement.load()
      } else {
        uploadType.value = 0;
        proxy.$message.error("文件格式不支持");
      }
    }

    function calculateMD5InChunks(file, chunkSize) {
      return new Promise((resolve) => {
        const md5 = CryptoJS.algo.MD5.create();
        const reader = new FileReader();
        let currentChunk = 0;

        reader.onload = function (e) {
          // Update hash for this chunk
          md5.update(CryptoJS.lib.WordArray.create(e.target.result));
          currentChunk++;

          if (currentChunk < Math.ceil(file.size / chunkSize)) {
            // Read next chunk
            reader.readAsArrayBuffer(file.slice(currentChunk * chunkSize, (currentChunk + 1) * chunkSize));
          } else {
            // All chunks read and processed, get the final hash
            const hash = md5.finalize().toString(CryptoJS.enc.Hex);
            resolve(hash);
          }
        };

        // Start reading the file in chunks
        reader.readAsArrayBuffer(file.slice(0, chunkSize));
      })
    }

    async function uploadBtn(options) {
      calculateMD5InChunks(options.file, 1024 * 64).then(async (rs) => {
        const formData = new FormData()
        formData.append('file', options.file)
        formData.append('Md5', rs)
        const data = await coreUploadApi2(formData);
        console.log('data => ', data)
      });
    }

    function beforeUpload() {
      // proccessFlag.value = true
      proxy.$refs.upload.submit()
    }

    return {
      fileList,
      proccessFlag,
      uploadType,
      handleChange,
      uploadBtn,
      beforeUpload,
      calculateMD5InChunks
    };
  }
};
</script>

<template>
  <div class="container">
    <div class="right-container">
      <el-card class="mb-5">
        <template #header> 预览 </template>
        <el-space v-if="fileList[0] && fileList[0].url">
          <img v-if="uploadType === 1" :src="fileList[0].url" />
          <video v-if="uploadType === 2" :src="fileList[0].url" autoplay loop />
        </el-space>
      </el-card>
      <el-card class="mb-5">
        <template #header> 操作 </template>
        <el-space>
          <el-upload ref="upload" action="#" :on-change="handleChange" :file-list="fileList" :with-credentials="true"
            :auto-upload="false" :show-file-list="false" :http-request="uploadBtn">
            <template #trigger>
              <el-button :disabled="proccessFlag" size="small" type="primary">选择</el-button>
            </template>
          </el-upload>
          <el-button :disabled="proccessFlag" size="small" type="primary" @click="beforeUpload">提交</el-button>
        </el-space>
      </el-card>
      <ul class="gantt-messages">
        <li class="gantt-message">
          备用
        </li>
      </ul>
    </div>
    <div class="left-container">
      <!-- <ThreeSample /> -->
      <!-- <ThreeRayCasterSample /> -->
      <!-- <MyCarSample /> -->
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

.gantt-messages>.gantt-message {
  background-color: #f4f4f4;
  box-shadow: inset 5px 0 #d69000;
  font-family: Geneva, Arial, Helvetica, sans-serif;
  font-size: 14px;
  margin: 5px 0;
  padding: 8px 0 8px 10px;
}
</style>
