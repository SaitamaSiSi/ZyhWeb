let selectedElement = null;
let currentTransform = null;
let isDragging = false;
let isResizing = false;
let isRotating = false;
let startX, startY;
let currentAngle = 0;
let originalWidth, originalHeight;
let originalAspectRatio = 1;

// 初始化SVG画布
const canvas = document.getElementById('canvas');
let elements = [];

// 添加画布点击事件，用于取消选中
canvas.addEventListener('mousedown', function(e) {
    // 如果点击的是画布背景或网格
    if (e.target === canvas || e.target.getAttribute('fill') === 'url(#grid)') {
        // 清除所有元素的选中状态
        document.querySelectorAll('.element-group').forEach(el => {
            el.classList.remove('selected');
        });
        selectedElement = null;
        updatePropertiesPanel();
    }
});

// 获取宽高比锁定状态
function isAspectRatioLocked() {
    return document.getElementById('lockAspectRatio').checked;
}

// 添加文本
function addText() {
    const text = document.createElementNS("http://www.w3.org/2000/svg", "text");
    text.setAttribute("x", "0");
    text.setAttribute("y", "20"); // 调整文本基线位置
    text.setAttribute("font-size", "20");
    text.setAttribute("class", "draggable-text");
    text.textContent = "双击编辑文本";
    
    const group = createElementGroup(text);
    group.setAttribute("transform", "translate(100,100) rotate(0)");
    canvas.appendChild(group);
    elements.push(group);
    
    // 获取文本实际大小并更新边框
    const bbox = text.getBBox();
    const border = group.querySelector('.element-border');
    border.setAttribute('width', bbox.width);
    border.setAttribute('height', bbox.height);
    
    // 更新resize手柄位置
    const resizeHandle = group.querySelector('.resize-handle');
    resizeHandle.setAttribute('x', bbox.width - 10);
    resizeHandle.setAttribute('y', bbox.height - 10);
}

// 添加图片
function addImage() {
    const input = document.getElementById('imageInput');
    input.onchange = function(e) {
        const file = e.target.files[0];
        const reader = new FileReader();
        reader.onload = function(event) {
            // 预加载图片以获取原始宽高比
            const img = new Image();
            img.onload = function() {
                const aspectRatio = img.width / img.height;
                const svgImg = document.createElementNS("http://www.w3.org/2000/svg", "image");
                svgImg.setAttribute("x", "0");
                svgImg.setAttribute("y", "0");
                svgImg.setAttribute("width", "100");
                svgImg.setAttribute("height", String(100 / aspectRatio));
                svgImg.setAttribute("href", event.target.result);
                svgImg.setAttribute("class", "draggable-image");
                svgImg.setAttribute("preserveAspectRatio", "none");  // 默认不保持宽高比
                
                const group = createElementGroup(svgImg);
                group.setAttribute("transform", "translate(100,100) rotate(0)");
                canvas.appendChild(group);
                elements.push(group);
                
                // 更新边框大小
                const border = group.querySelector('.element-border');
                border.setAttribute('width', 100);
                border.setAttribute('height', 100 / aspectRatio);
            };
            img.src = event.target.result;
        };
        reader.readAsDataURL(file);
    };
    input.click();
}

// 添加视频
function addVideo() {
    const input = document.getElementById('videoInput');
    input.onchange = function(e) {
        const file = e.target.files[0];
        const url = URL.createObjectURL(file);
        
        const video = document.createElementNS("http://www.w3.org/2000/svg", "foreignObject");
        video.setAttribute("width", "320");
        video.setAttribute("height", "240");
        video.setAttribute("class", "draggable-video");
        
        const videoPlayer = document.createElement("video");
        videoPlayer.src = url;
        videoPlayer.controls = true;
        videoPlayer.style.width = "100%";
        videoPlayer.style.height = "100%";
        videoPlayer.style.objectFit = "fill";  // 默认填充模式
        
        video.appendChild(videoPlayer);
        
        const group = createElementGroup(video);
        group.setAttribute("transform", "translate(100,100) rotate(0)");
        canvas.appendChild(group);
        elements.push(group);
        
        // 更新边框大小
        const border = group.querySelector('.element-border');
        border.setAttribute('width', 320);
        border.setAttribute('height', 240);
        
        // 更新resize手柄位置
        const resizeHandle = group.querySelector('.resize-handle');
        resizeHandle.setAttribute('x', 310);
        resizeHandle.setAttribute('y', 230);
    };
    input.click();
}

// 创建元素组
function createElementGroup(element) {
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.setAttribute("class", "element-group");
    group.appendChild(element);
    
    // 添加控制点
    addControlPoints(group);
    
    // 添加事件监听器
    addEventListeners(group);
    
    return group;
}

// 添加控制点
function addControlPoints(group) {
    // 创建边框
    const border = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    border.setAttribute("class", "element-border");
    border.setAttribute("width", "100");
    border.setAttribute("height", "100");
    border.setAttribute("fill", "none");
    border.setAttribute("stroke", "#4CAF50");
    border.setAttribute("stroke-width", "1");
    border.setAttribute("stroke-dasharray", "3,3");
    
    // 调整大小的控制点
    const resizeHandle = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    resizeHandle.setAttribute("class", "resize-handle control-point");
    resizeHandle.setAttribute("width", "10");
    resizeHandle.setAttribute("height", "10");
    resizeHandle.setAttribute("x", "90");
    resizeHandle.setAttribute("y", "90");
    
    group.insertBefore(border, group.firstChild);
    group.appendChild(resizeHandle);
}

// 添加事件监听器
function addEventListeners(group) {
    group.addEventListener('mousedown', startDragging);
    group.addEventListener('dblclick', handleDoubleClick);
    
    const resizeHandle = group.querySelector('.resize-handle');
    if (resizeHandle) {
        resizeHandle.addEventListener('mousedown', startResizing);
    }
}

// 开始拖拽
function startDragging(e) {
    if (e.target.classList.contains('resize-handle')) return;
    
    // 阻止事件冒泡，防止触发画布的点击事件
    e.stopPropagation();
    
    selectedElement = this;
    updatePropertiesPanel();
    
    if (e.ctrlKey) {
        isRotating = true;
        const bbox = selectedElement.getBBox();
        const centerX = bbox.x + bbox.width/2;
        const centerY = bbox.y + bbox.height/2;
        startX = Math.atan2(e.clientY - centerY, e.clientX - centerX);
        
        document.addEventListener('mousemove', rotate);
        document.addEventListener('mouseup', stopRotating);
    } else {
        isDragging = true;
        const transform = parseTransform(selectedElement.getAttribute('transform'));
        startX = e.clientX - transform.translateX;
        startY = e.clientY - transform.translateY;
        currentAngle = transform.rotate;
        
        document.addEventListener('mousemove', drag);
        document.addEventListener('mouseup', stopDragging);
    }
    
    // 清除其他元素的选中状态
    document.querySelectorAll('.element-group').forEach(el => {
        el.classList.remove('selected');
    });
    selectedElement.classList.add('selected');
    
    e.preventDefault();
}

// 开始调整大小
function startResizing(e) {
    isResizing = true;
    selectedElement = this.parentElement;
    
    const element = selectedElement.querySelector('image, text, foreignObject');
    originalWidth = parseFloat(element.getAttribute('width') || 0);
    originalHeight = parseFloat(element.getAttribute('height') || 0);
    originalAspectRatio = originalWidth / originalHeight;
    startX = e.clientX;
    startY = e.clientY;
    
    document.addEventListener('mousemove', resize);
    document.addEventListener('mouseup', stopResizing);
    
    e.preventDefault();
    e.stopPropagation();
}

// 拖拽
function drag(e) {
    if (!isDragging) return;
    
    const dx = e.clientX - startX;
    const dy = e.clientY - startY;
    
    selectedElement.setAttribute('transform', 
        `translate(${dx},${dy}) rotate(${currentAngle})`);
}

// 调整大小
function resize(e) {
    if (!isResizing) return;
    
    const dx = e.clientX - startX;
    const dy = e.clientY - startY;
    const element = selectedElement.querySelector('image, text, foreignObject');
    const border = selectedElement.querySelector('.element-border');
    
    let newWidth, newHeight;
    
    if (isAspectRatioLocked()) {
        // 根据鼠标移动的较大距离来确定缩放基准
        if (Math.abs(dx) > Math.abs(dy)) {
            newWidth = Math.max(20, originalWidth + dx);
            newHeight = newWidth / originalAspectRatio;
        } else {
            newHeight = Math.max(20, originalHeight + dy);
            newWidth = newHeight * originalAspectRatio;
        }
    } else {
        newWidth = Math.max(20, originalWidth + dx);
        newHeight = Math.max(20, originalHeight + dy);
        
        // 如果是图片或视频，调整其样式以填充边框
        if (element.tagName === 'image') {
            element.setAttribute('preserveAspectRatio', 'none');
        } else if (element.tagName === 'foreignObject') {
            const video = element.querySelector('video');
            if (video) {
                video.style.objectFit = 'fill';
            }
        }
    }
    
    // 更新边框大小
    border.setAttribute('width', newWidth);
    border.setAttribute('height', newHeight);
    
    // 更新元素大小，使其填充边框
    element.setAttribute('width', newWidth);
    element.setAttribute('height', newHeight);
    
    // 更新调整大小手柄的位置
    const resizeHandle = selectedElement.querySelector('.resize-handle');
    resizeHandle.setAttribute('x', newWidth - 10);
    resizeHandle.setAttribute('y', newHeight - 10);
}

// 旋转
function rotate(e) {
    if (!isRotating) return;
    
    const bbox = selectedElement.getBBox();
    const centerX = bbox.x + bbox.width/2;
    const centerY = bbox.y + bbox.height;
    
    const angle = Math.atan2(e.clientY - centerY, e.clientX - centerX) - startX;
    const degrees = angle * (180/Math.PI);
    
    const transform = parseTransform(selectedElement.getAttribute('transform'));
    selectedElement.setAttribute('transform',
        `translate(${transform.translateX},${transform.translateY}) rotate(${degrees})`);
}

// 停止拖拽
function stopDragging() {
    isDragging = false;
    document.removeEventListener('mousemove', drag);
    document.removeEventListener('mouseup', stopDragging);
}

// 停止调整大小
function stopResizing() {
    isResizing = false;
    document.removeEventListener('mousemove', resize);
    document.removeEventListener('mouseup', stopResizing);
}

// 停止旋转
function stopRotating() {
    isRotating = false;
    document.removeEventListener('mousemove', rotate);
    document.removeEventListener('mouseup', stopRotating);
}

// 处理双击编辑文本
function handleDoubleClick(e) {
    const textElement = this.querySelector('text');
    if (textElement) {
        const newText = prompt('编辑文本', textElement.textContent);
        if (newText) {
            textElement.textContent = newText;
        }
    }
}

// 解析transform属性
function parseTransform(transform) {
    if (!transform) return { translateX: 0, translateY: 0, rotate: 0 };
    
    const translateMatch = transform.match(/translate\((.*?),(.*?)\)/);
    const rotateMatch = transform.match(/rotate\((.*?)\)/);
    
    return {
        translateX: translateMatch ? parseFloat(translateMatch[1]) : 0,
        translateY: translateMatch ? parseFloat(translateMatch[2]) : 0,
        rotate: rotateMatch ? parseFloat(rotateMatch[1]) : 0
    };
}

// 设置背景图片
function setBackgroundImage(url) {
    const background = document.getElementById('background');
    background.setAttribute('href', url);
}

// 添加背景图片
function addBackground() {
    const input = document.getElementById('backgroundInput');
    input.onchange = function(e) {
        const file = e.target.files[0];
        const reader = new FileReader();
        reader.onload = function(event) {
            setBackgroundImage(event.target.result);
        };
        reader.readAsDataURL(file);
    };
    input.click();
}

// 更新属性面板
function updatePropertiesPanel() {
    const panel = document.getElementById('elementProperties');
    
    if (!selectedElement) {
        panel.classList.add('hidden');
        return;
    }
    
    panel.classList.remove('hidden');
    
    const transform = parseTransform(selectedElement.getAttribute('transform'));
    const element = selectedElement.querySelector('image, text, foreignObject');
    const width = parseFloat(element.getAttribute('width'));
    const height = parseFloat(element.getAttribute('height'));
    
    document.getElementById('elementX').value = Math.round(transform.translateX);
    document.getElementById('elementY').value = Math.round(transform.translateY);
    document.getElementById('elementWidth').value = Math.round(width);
    document.getElementById('elementHeight').value = Math.round(height);
    document.getElementById('elementAngle').value = Math.round(transform.rotate);
}

// 更新元素属性
function updateElementProperty(property, value) {
    if (!selectedElement) return;
    
    const element = selectedElement.querySelector('image, text, foreignObject');
    const transform = parseTransform(selectedElement.getAttribute('transform'));
    
    switch(property) {
        case 'x':
            transform.translateX = parseFloat(value);
            break;
        case 'y':
            transform.translateY = parseFloat(value);
            break;
        case 'width':
            const newWidth = Math.max(20, parseFloat(value));
            element.setAttribute('width', newWidth);
            const border = selectedElement.querySelector('.element-border');
            border.setAttribute('width', newWidth);
            const resizeHandle = selectedElement.querySelector('.resize-handle');
            resizeHandle.setAttribute('x', newWidth - 10);
            break;
        case 'height':
            const newHeight = Math.max(20, parseFloat(value));
            element.setAttribute('height', newHeight);
            const borderH = selectedElement.querySelector('.element-border');
            borderH.setAttribute('height', newHeight);
            const resizeHandleH = selectedElement.querySelector('.resize-handle');
            resizeHandleH.setAttribute('y', newHeight - 10);
            break;
        case 'angle':
            transform.rotate = parseFloat(value);
            break;
    }
    
    selectedElement.setAttribute('transform', 
        `translate(${transform.translateX},${transform.translateY}) rotate(${transform.rotate})`);
}