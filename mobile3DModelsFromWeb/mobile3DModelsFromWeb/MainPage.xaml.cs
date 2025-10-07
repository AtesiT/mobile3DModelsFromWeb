using Xamarin.Forms;
using System;

namespace mobile3DModelsFromWeb
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadWebViewContent();
        }

        void LoadWebViewContent()
        {
            var htmlSource = new HtmlWebViewSource();

            htmlSource.Html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no' />
    <style>
        body { 
            margin: 0; 
            padding: 0; 
            background: #303336;
            overflow: hidden;
            font-family: Arial, sans-serif;
        }
        #container {
            width: 100vw;
            height: 100vh;
            position: relative;
        }
        canvas { 
            width: 100%; 
            height: 100%; 
            display: block; 
        }
        #info {
            position: absolute;
            top: 10px;
            left: 10px;
            color: white;
            background: rgba(0,0,0,0.7);
            padding: 10px;
            border-radius: 5px;
            font-size: 14px;
            z-index: 100;
        }
        #loading {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            color: white;
            font-size: 18px;
            text-align: center;
        }
    </style>
</head>
<body>
    <div id='container'>
        <div id='loading'>Loading 3D Viewer...</div>
        <div id='info' style='display:none;'>3D Cube Test</div>
        <canvas id='canvas'></canvas>
    </div>

    <script>
        console.log('Starting 3D viewer...');
        
        // Простая проверка WebGL
        function checkWebGL() {
            var canvas = document.createElement('canvas');
            var gl = canvas.getContext('webgl') || canvas.getContext('experimental-webgl');
            return !!gl;
        }

        function showMessage(message) {
            var info = document.getElementById('info');
            var loading = document.getElementById('loading');
            info.textContent = message;
            info.style.display = 'block';
            loading.style.display = 'none';
        }

        function showError(message) {
            showMessage('Error: ' + message);
            console.error(message);
        }

        function init() {
            console.log('Initializing Three.js');
            
            try {
                // Создаем сцену
                var scene = new THREE.Scene();
                scene.background = new THREE.Color(0x303336);
                
                // Создаем камеру
                var camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
                camera.position.z = 5;
                
                // Создаем рендерер
                var canvas = document.getElementById('canvas');
                var renderer = new THREE.WebGLRenderer({ 
                    canvas: canvas,
                    antialias: true,
                    alpha: true
                });
                renderer.setSize(window.innerWidth, window.innerHeight);
                renderer.setPixelRatio(window.devicePixelRatio);
                
                // Добавляем освещение
                var ambientLight = new THREE.AmbientLight(0x404040, 1.5);
                scene.add(ambientLight);
                
                var directionalLight = new THREE.DirectionalLight(0xffffff, 1);
                directionalLight.position.set(1, 1, 1).normalize();
                scene.add(directionalLight);
                
                // Создаем простой куб
                var geometry = new THREE.BoxGeometry(2, 2, 2);
                var material = new THREE.MeshBasicMaterial({ 
                    color: 0x00ff00,
                    wireframe: false 
                });
                var cube = new THREE.Mesh(geometry, material);
                scene.add(cube);
                
                // Добавляем сетку для ориентира
                var gridHelper = new THREE.GridHelper(10, 10);
                scene.add(gridHelper);
                
                // Анимация
                function animate() {
                    requestAnimationFrame(animate);
                    
                    cube.rotation.x += 0.01;
                    cube.rotation.y += 0.01;
                    
                    renderer.render(scene, camera);
                }
                
                // Обработчик изменения размера
                function onWindowResize() {
                    camera.aspect = window.innerWidth / window.innerHeight;
                    camera.updateProjectionMatrix();
                    renderer.setSize(window.innerWidth, window.innerHeight);
                }
                
                window.addEventListener('resize', onWindowResize, false);
                
                // Запускаем анимацию
                animate();
                showMessage('3D Viewer Loaded - Cube should be rotating');
                console.log('Three.js animation started successfully');
                
            } catch (error) {
                showError('Failed to initialize 3D: ' + error.message);
            }
        }

        // Основная функция загрузки
        function loadThreeJS() {
            if (!checkWebGL()) {
                showError('WebGL not supported on this device');
                return;
            }
            
            console.log('WebGL is available, loading Three.js');
            
            // Проверяем, не загружен ли уже Three.js
            if (typeof THREE !== 'undefined') {
                console.log('Three.js already loaded');
                init();
                return;
            }
            
            // Загружаем Three.js
            var script = document.createElement('script');
            script.src = 'https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js';
            script.onload = function() {
                console.log('Three.js loaded successfully');
                setTimeout(init, 100);
            };
            script.onerror = function() {
                showError('Failed to load Three.js library');
            };
            document.head.appendChild(script);
        }

        // Запускаем когда страница загружена
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', loadThreeJS);
        } else {
            loadThreeJS();
        }
    </script>
</body>
</html>";

            MyWebView.Source = htmlSource;
        }
    }
}