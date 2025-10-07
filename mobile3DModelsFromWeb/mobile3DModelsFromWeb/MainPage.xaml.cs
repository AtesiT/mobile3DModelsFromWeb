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
    <title>3D Scenes with Shader Background</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            background: #303336;
            font-family: sans-serif;
            overflow: hidden;
            width: 100vw;
            height: 100vh;
        }

        .container {
            display: flex;
            width: 100%;
            height: 100%;
        }

        .canvas-container {
            flex: 1;
            height: 100%;
            position: relative;
            border: 1px solid #555;
        }

        canvas {
            width: 100%;
            height: 100%;
            display: block;
        }

        .scene-label {
            position: absolute;
            top: 10px;
            left: 10px;
            color: white;
            background: rgba(0,0,0,0.7);
            padding: 5px 10px;
            border-radius: 5px;
            font-size: 12px;
            z-index: 100;
        }

        /* Основной контейнер панели управления */
        #controls-container {
            position: absolute;
            bottom: 20px;
            left: 50%;
            transform: translateX(-50%);
            display: flex;
            flex-direction: column;
            gap: 10px;
            z-index: 1000;
        }

        /* Панели управления */
        .controls-panel {
            background-color: rgba(40, 40, 40, 0.9);
            border-radius: 10px;
            padding: 10px 15px;
            display: flex;
            align-items: center;
            gap: 10px;
            color: white;
            border: 1px solid #555;
        }

        .controls-panel button {
            padding: 8px 12px;
            border: 1px solid #777;
            background-color: #4a4d50;
            color: white;
            border-radius: 5px;
            cursor: pointer;
            font-size: 12px;
            transition: background-color 0.2s;
        }

        .controls-panel button:hover {
            background-color: #6a6d70;
        }

        .controls-panel button:active {
            background-color: #2a2d30;
        }

        .speed-control {
            display: flex;
            align-items: center;
            gap: 5px;
        }

        #speed-display {
            font-weight: bold;
            min-width: 25px;
            text-align: center;
        }

        /* Стили для кнопок вращения */
        .rotation-controls {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            grid-template-rows: repeat(3, 1fr);
            gap: 5px;
            margin-left: 10px;
        }

        .rotation-btn {
            width: 40px;
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
        }

        .btn-up { grid-column: 2; grid-row: 1; }
        .btn-left { grid-column: 1; grid-row: 2; }
        .btn-right { grid-column: 3; grid-row: 2; }
        .btn-down { grid-column: 2; grid-row: 3; }

        #debug {
            position: absolute;
            top: 10px;
            right: 10px;
            color: white;
            background: rgba(255,0,0,0.7);
            padding: 10px;
            border-radius: 5px;
            font-size: 12px;
            z-index: 1000;
        }
    </style>
</head>
<body>
    <div id='debug'>Loading...</div>
    
    <div class='container'>
        <div class='canvas-container'>
            <div class='scene-label'>Сцена 1 - Куб</div>
            <canvas id='canvas1'></canvas>
        </div>
        <div class='canvas-container'>
            <div class='scene-label'>Сцена 2 - Сфера</div>
            <canvas id='canvas2'></canvas>
        </div>
    </div>

    <!-- Контейнер для двухстрочной панели управления -->
    <div id='controls-container'>
        <!-- Верхняя панель: основные кнопки -->
        <div class='controls-panel' id='main-controls'>
            <button id='btn-start'>▶️ Старт</button>
            <button id='btn-stop'>⏹️ Стоп</button>
            <div class='speed-control'>
                <span>Скорость:</span>
                <button id='btn-speed-down'>-</button>
                <span id='speed-display'>5</span>
                <button id='btn-speed-up'>+</button>
            </div>
            <button id='btn-change-model'>Сменить модель</button>
        </div>

        <!-- Нижняя панель: кнопки вращения -->
        <div class='controls-panel' id='rotation-controls'>
            <span style='margin-right: 10px;'>Ручное вращение:</span>
            <div class='rotation-controls'>
                <button class='rotation-btn btn-up' id='btn-rotate-up' title='Вращать вверх'>⬆️</button>
                <button class='rotation-btn btn-left' id='btn-rotate-left' title='Вращать влево'>⬅️</button>
                <button class='rotation-btn btn-right' id='btn-rotate-right' title='Вращать вправо'>➡️</button>
                <button class='rotation-btn btn-down' id='btn-rotate-down' title='Вращать вниз'>⬇️</button>
            </div>
        </div>
    </div>

    <script>
        console.log('Starting 3D scenes with shader background...');
        let isAutoRotating = true;
        let rotationSpeed = 5;
        let currentModelSet = 0;

        function updateDebug(message) {
            document.getElementById('debug').textContent = message;
            console.log(message);
        }

        function checkWebGL() {
            try {
                const canvas = document.createElement('canvas');
                const gl = canvas.getContext('webgl') || canvas.getContext('experimental-webgl');
                return !!gl;
            } catch (e) {
                return false;
            }
        }

        // Функция создания шейдерного фона
        function createShaderBackground() {
            const uniforms = {
                iTime: { value: 0 }
            };

            const vertexShader = `
                varying vec3 vPos;
                void main() {
                    vPos = position;
                    gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
                }
            `;

            const fragmentShader = `
                varying vec3 vPos;
                uniform float iTime;

                #define iterations 17
                #define formuparam 0.53
                #define volsteps 20
                #define stepsize 0.1
                #define speed 0.010
                #define brightness 0.0015
                #define darkmatter 0.300
                #define distfading 0.730
                #define saturation 0.850

                void main() {
                    vec3 dir = normalize(vPos);
                    float time = iTime * speed + .25;

                    float a1 = .5, a2 = .8;
                    mat2 rot1 = mat2(cos(a1), sin(a1), -sin(a1), cos(a1));
                    mat2 rot2 = mat2(cos(a2), sin(a2), -sin(a2), cos(a2));

                    dir.xz *= rot1;
                    dir.xy *= rot2;

                    vec3 from = vec3(1., .5, 0.5);
                    from += vec3(time * 2., time, -2.);
                    from.xz *= rot1;
                    from.xy *= rot2;

                    float s = 0.1, fade = 1.;
                    vec3 v = vec3(0.);

                    for (int r = 0; r < volsteps; r++) {
                        vec3 p = from + s * dir * .5;
                        p = abs(vec3(0.850) - mod(p, vec3(0.850 * 2.)));
                        float pa, a = pa = 0.;

                        for (int i = 0; i < iterations; i++) {
                            p = abs(p) / dot(p, p) - formuparam;
                            a += abs(length(p) - pa);
                            pa = length(p);
                        }

                        float dm = max(0., darkmatter - a * a * .001);
                        a *= a * a;
                        if (r > 6) fade *= 1. - dm;
                        v += fade;
                        v += vec3(s, s * s, s * s * s * s) * a * brightness * fade;
                        fade *= distfading;
                        s += stepsize;
                    }

                    v = mix(vec3(length(v)), v, saturation);
                    gl_FragColor = vec4(v * .01, 1.);
                }
            `;

            const geometry = new THREE.SphereGeometry(500, 32, 32);
            const material = new THREE.ShaderMaterial({
                uniforms: uniforms,
                vertexShader: vertexShader,
                fragmentShader: fragmentShader,
                side: THREE.BackSide,
                depthWrite: false
            });

            const mesh = new THREE.Mesh(geometry, material);
            return mesh;
        }

        function initScene(canvasId, objectType) {
            updateDebug('Initializing ' + canvasId + ' with ' + objectType);
            
            const canvas = document.getElementById(canvasId);
            if (!canvas) {
                updateDebug('Canvas not found: ' + canvasId);
                return null;
            }

            try {
                const scene = new THREE.Scene();
                const camera = new THREE.PerspectiveCamera(75, canvas.clientWidth / canvas.clientHeight, 0.1, 1000);
                camera.position.z = 5;

                const renderer = new THREE.WebGLRenderer({ 
                    canvas: canvas,
                    antialias: true,
                    alpha: false
                });
                renderer.setSize(canvas.clientWidth, canvas.clientHeight);
                renderer.setPixelRatio(window.devicePixelRatio);

                // Шейдерный фон
                const shaderBackground = createShaderBackground();
                scene.add(shaderBackground);

                // Освещение
                const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
                scene.add(ambientLight);

                const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
                directionalLight.position.set(5, 5, 5);
                scene.add(directionalLight);

                // Создаем объект
                let geometry, material, object;

                if (objectType === 'cube') {
                    geometry = new THREE.BoxGeometry(2, 2, 2);
                    material = new THREE.MeshPhongMaterial({ 
                        color: 0xff0000,
                        shininess: 100,
                        transparent: true,
                        opacity: 0.9
                    });
                } else if (objectType === 'sphere') {
                    geometry = new THREE.SphereGeometry(1, 32, 32);
                    material = new THREE.MeshPhongMaterial({ 
                        color: 0x00ff00,
                        shininess: 100,
                        transparent: true,
                        opacity: 0.9
                    });
                } else if (objectType === 'torus') {
                    geometry = new THREE.TorusGeometry(1, 0.4, 16, 100);
                    material = new THREE.MeshPhongMaterial({ 
                        color: 0x0000ff,
                        shininess: 100,
                        transparent: true,
                        opacity: 0.9
                    });
                } else if (objectType === 'cone') {
                    geometry = new THREE.ConeGeometry(1, 2, 32);
                    material = new THREE.MeshPhongMaterial({ 
                        color: 0xffff00,
                        shininess: 100,
                        transparent: true,
                        opacity: 0.9
                    });
                }

                object = new THREE.Mesh(geometry, material);
                scene.add(object);

                updateDebug(canvasId + ' initialized with shader background');

                return {
                    scene: scene,
                    camera: camera,
                    renderer: renderer,
                    object: object,
                    shaderBackground: shaderBackground
                };

            } catch (error) {
                updateDebug('Error in ' + canvasId + ': ' + error.message);
                console.error(error);
                return null;
            }
        }

        function init() {
            updateDebug('Starting initialization with shaders...');

            if (!checkWebGL()) {
                updateDebug('WebGL not supported!');
                return;
            }

            if (typeof THREE === 'undefined') {
                updateDebug('Three.js not loaded!');
                return;
            }

            updateDebug('Three.js version: ' + THREE.REVISION);

            // Инициализируем сцены
            const scene1 = initScene('canvas1', 'cube');
            const scene2 = initScene('canvas2', 'sphere');

            if (!scene1 || !scene2) {
                updateDebug('Failed to initialize scenes');
                return;
            }

            // Обработчик изменения размера окна
            function onWindowResize() {
                [scene1, scene2].forEach(scene => {
                    if (scene) {
                        const canvas = scene.renderer.domElement;
                        scene.camera.aspect = canvas.clientWidth / canvas.clientHeight;
                        scene.camera.updateProjectionMatrix();
                        scene.renderer.setSize(canvas.clientWidth, canvas.clientHeight);
                    }
                });
            }

            window.addEventListener('resize', onWindowResize);

            // Инициализация управления
            function initControls() {
                const btnStart = document.getElementById('btn-start');
                const btnStop = document.getElementById('btn-stop');
                const btnSpeedUp = document.getElementById('btn-speed-up');
                const btnSpeedDown = document.getElementById('btn-speed-down');
                const btnChangeModel = document.getElementById('btn-change-model');
                const btnRotateUp = document.getElementById('btn-rotate-up');
                const btnRotateDown = document.getElementById('btn-rotate-down');
                const btnRotateLeft = document.getElementById('btn-rotate-left');
                const btnRotateRight = document.getElementById('btn-rotate-right');
                const speedDisplay = document.getElementById('speed-display');

                function updateSpeedDisplay() {
                    speedDisplay.textContent = rotationSpeed;
                }

                // Основные кнопки управления
                btnStart.addEventListener('click', () => {
                    isAutoRotating = true;
                    updateDebug('Автовращение включено');
                });

                btnStop.addEventListener('click', () => {
                    isAutoRotating = false;
                    updateDebug('Автовращение выключено');
                });

                btnSpeedUp.addEventListener('click', () => {
                    rotationSpeed = Math.min(20, rotationSpeed + 1);
                    updateSpeedDisplay();
                    updateDebug('Скорость: ' + rotationSpeed);
                });

                btnSpeedDown.addEventListener('click', () => {
                    rotationSpeed = Math.max(1, rotationSpeed - 1);
                    updateSpeedDisplay();
                    updateDebug('Скорость: ' + rotationSpeed);
                });

                btnChangeModel.addEventListener('click', () => {
                    currentModelSet = (currentModelSet + 1) % 2;
                    updateDebug('Смена моделей на набор ' + (currentModelSet + 1));
                    
                    // Удаляем старые объекты
                    scene1.scene.remove(scene1.object);
                    scene2.scene.remove(scene2.object);
                    
                    // Создаем новые объекты
                    if (currentModelSet === 0) {
                        // Набор 1: Куб и Сфера
                        scene1.object = new THREE.Mesh(
                            new THREE.BoxGeometry(2, 2, 2),
                            new THREE.MeshPhongMaterial({ 
                                color: 0xff0000,
                                transparent: true,
                                opacity: 0.9
                            })
                        );
                        scene2.object = new THREE.Mesh(
                            new THREE.SphereGeometry(1, 32, 32),
                            new THREE.MeshPhongMaterial({ 
                                color: 0x00ff00,
                                transparent: true,
                                opacity: 0.9
                            })
                        );
                    } else {
                        // Набор 2: Тор и Конус
                        scene1.object = new THREE.Mesh(
                            new THREE.TorusGeometry(1, 0.4, 16, 100),
                            new THREE.MeshPhongMaterial({ 
                                color: 0x0000ff,
                                transparent: true,
                                opacity: 0.9
                            })
                        );
                        scene2.object = new THREE.Mesh(
                            new THREE.ConeGeometry(1, 2, 32),
                            new THREE.MeshPhongMaterial({ 
                                color: 0xffff00,
                                transparent: true,
                                opacity: 0.9
                            })
                        );
                    }
                    
                    scene1.scene.add(scene1.object);
                    scene2.scene.add(scene2.object);
                });

                // Кнопки ручного вращения
                const rotationStep = 0.1; // Шаг вращения в радианах

                btnRotateUp.addEventListener('click', () => {
                    if (!isAutoRotating) {
                        scene1.object.rotation.x -= rotationStep;
                        scene2.object.rotation.x -= rotationStep;
                        updateDebug('Вращение вверх');
                    }
                });

                btnRotateDown.addEventListener('click', () => {
                    if (!isAutoRotating) {
                        scene1.object.rotation.x += rotationStep;
                        scene2.object.rotation.x += rotationStep;
                        updateDebug('Вращение вниз');
                    }
                });

                btnRotateLeft.addEventListener('click', () => {
                    if (!isAutoRotating) {
                        scene1.object.rotation.y -= rotationStep;
                        scene2.object.rotation.y -= rotationStep;
                        updateDebug('Вращение влево');
                    }
                });

                btnRotateRight.addEventListener('click', () => {
                    if (!isAutoRotating) {
                        scene1.object.rotation.y += rotationStep;
                        scene2.object.rotation.y += rotationStep;
                        updateDebug('Вращение вправо');
                    }
                });

                updateSpeedDisplay();
            }

            initControls();

            // Анимация
            let startTime = Date.now();
            
            function animate() {
                requestAnimationFrame(animate);

                const currentTime = (Date.now() - startTime) * 0.001;

                // Обновляем шейдеры
                if (scene1.shaderBackground) {
                    scene1.shaderBackground.material.uniforms.iTime.value = currentTime;
                }
                if (scene2.shaderBackground) {
                    scene2.shaderBackground.material.uniforms.iTime.value = currentTime;
                }

                if (isAutoRotating) {
                    const rotation = currentTime * rotationSpeed * 0.1;
                    
                    if (scene1 && scene1.object) {
                        scene1.object.rotation.x = rotation;
                        scene1.object.rotation.y = rotation;
                    }
                    
                    if (scene2 && scene2.object) {
                        scene2.object.rotation.x = rotation;
                        scene2.object.rotation.y = rotation;
                    }
                }

                // Рендерим сцены
                if (scene1) {
                    scene1.renderer.render(scene1.scene, scene1.camera);
                }
                if (scene2) {
                    scene2.renderer.render(scene2.scene, scene2.camera);
                }
            }

            animate();
            updateDebug('3D сцены с шейдерным фоном и управлением готовы!');
        }

        // Загружаем Three.js
        if (typeof THREE === 'undefined') {
            updateDebug('Loading Three.js...');
            const script = document.createElement('script');
            script.src = 'https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js';
            script.onload = function() {
                updateDebug('Three.js loaded, initializing...');
                setTimeout(init, 100);
            };
            script.onerror = function() {
                updateDebug('Failed to load Three.js!');
            };
            document.head.appendChild(script);
        } else {
            updateDebug('Three.js already loaded');
            init();
        }
    </script>
</body>
</html>";

            MyWebView.Source = htmlSource;
        }
    }
}