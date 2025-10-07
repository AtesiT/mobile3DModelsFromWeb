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
    </style>
</head>
<body>
    <div id='container'>
        <div id='info'>3D Scene with Shader Background</div>
        <canvas id='canvas'></canvas>
    </div>

    <script>
        console.log('Starting 3D viewer with shader background...');

        function init() {
            console.log('Initializing Three.js with shader background');
            
            try {
                // Создаем сцену
                var scene = new THREE.Scene();
                
                // Создаем камеру
                var camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
                camera.position.z = 5;
                
                // Создаем рендерер
                var canvas = document.getElementById('canvas');
                var renderer = new THREE.WebGLRenderer({ 
                    canvas: canvas,
                    antialias: true
                });
                renderer.setSize(window.innerWidth, window.innerHeight);
                
                // СОЗДАЕМ ШЕЙДЕРНЫЙ ФОН (ваш шейдер)
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
                            
                            float a1=.5, a2=.8; 
                            mat2 rot1=mat2(cos(a1),sin(a1),-sin(a1),cos(a1)); 
                            mat2 rot2=mat2(cos(a2),sin(a2),-sin(a2),cos(a2));
                            
                            dir.xz *= rot1; 
                            dir.xy *= rot2; 
                            
                            vec3 from = vec3(1.,.5,0.5); 
                            from += vec3(time*2.,time,-2.); 
                            from.xz *= rot1; 
                            from.xy *= rot2;
                            
                            float s=0.1, fade=1.; 
                            vec3 v=vec3(0.);
                            
                            for (int r=0; r<volsteps; r++) {
                                vec3 p=from+s*dir*.5; 
                                p=abs(vec3(0.850)-mod(p,vec3(0.850*2.)));
                                float pa,a=pa=0.;
                                
                                for (int i=0; i<iterations; i++) {
                                    p=abs(p)/dot(p,p)-formuparam; 
                                    a+=abs(length(p)-pa); 
                                    pa=length(p);
                                }
                                
                                float dm=max(0.,darkmatter-a*a*.001); 
                                a*=a*a; 
                                if (r>6) fade*=1.-dm;
                                v+=fade; 
                                v+=vec3(s,s*s,s*s*s*s)*a*brightness*fade; 
                                fade*=distfading; 
                                s+=stepsize;
                            }
                            
                            v=mix(vec3(length(v)),v,saturation); 
                            gl_FragColor=vec4(v*.01,1.);
                        }
                    `;
                    
                    const geometry = new THREE.SphereGeometry(1000, 64, 64);
                    const material = new THREE.ShaderMaterial({
                        uniforms: uniforms, 
                        vertexShader: vertexShader, 
                        fragmentShader: fragmentShader, 
                        side: THREE.BackSide, 
                        depthWrite: false
                    });
                    
                    return new THREE.Mesh(geometry, material);
                }

                // Добавляем шейдерный фон на сцену
                const shaderBackground = createShaderBackground();
                scene.add(shaderBackground);
                
                // Создаем куб (ваш 3D объект)
                var geometry = new THREE.BoxGeometry(2, 2, 2);
                
                // Разные материалы для каждой грани куба
                var materials = [
                    new THREE.MeshBasicMaterial({ color: 0xff0000, transparent: true, opacity: 0.8 }), // красный
                    new THREE.MeshBasicMaterial({ color: 0x00ff00, transparent: true, opacity: 0.8 }), // зеленый
                    new THREE.MeshBasicMaterial({ color: 0x0000ff, transparent: true, opacity: 0.8 }), // синий
                    new THREE.MeshBasicMaterial({ color: 0xffff00, transparent: true, opacity: 0.8 }), // желтый
                    new THREE.MeshBasicMaterial({ color: 0xff00ff, transparent: true, opacity: 0.8 }), // пурпурный
                    new THREE.MeshBasicMaterial({ color: 0x00ffff, transparent: true, opacity: 0.8 })  // голубой
                ];
                
                var cube = new THREE.Mesh(geometry, materials);
                scene.add(cube);
                
                // Добавляем освещение для лучшего вида
                var ambientLight = new THREE.AmbientLight(0x404040, 0.6);
                scene.add(ambientLight);
                
                var directionalLight = new THREE.DirectionalLight(0xffffff, 1);
                directionalLight.position.set(1, 1, 1);
                scene.add(directionalLight);
                
                // Создаем часы для анимации шейдера
                var clock = new THREE.Clock();
                
                // Анимация
                function animate() {
                    requestAnimationFrame(animate);
                    
                    // Обновляем время для шейдера
                    shaderBackground.material.uniforms.iTime.value = clock.getElapsedTime();
                    
                    // Вращаем куб
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
                console.log('Three.js with shader background started successfully');
                
            } catch (error) {
                console.error('Failed to initialize 3D with shader: ' + error.message);
                document.getElementById('info').textContent = 'Error: ' + error.message;
            }
        }

        // Загружаем Three.js
        function loadThreeJS() {
            console.log('Loading Three.js');
            
            if (typeof THREE !== 'undefined') {
                console.log('Three.js already loaded');
                init();
                return;
            }
            
            var script = document.createElement('script');
            script.src = 'https://cdnjs.cloudflare.com/ajax/libs/three.js/r128/three.min.js';
            script.onload = function() {
                console.log('Three.js loaded successfully');
                setTimeout(init, 100);
            };
            script.onerror = function() {
                console.error('Failed to load Three.js library');
                document.getElementById('info').textContent = 'Failed to load 3D library';
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