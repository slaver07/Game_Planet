using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace GamePlanet
{
    internal class Game : GameWindow
    {
        private Sphere _earth;
        private Sphere _moon;

        private Texture _earthTexture;
        private Texture _moonTexture;

        private Shader _shader;

        private Camera _camera;

        private float _earthRotation;
        private float _moonOrbitAngle;

        private bool _fullscreen = false;
        private bool _wireframe = false;

        public Game(int width, int height) : base(
            GameWindowSettings.Default,
            new NativeWindowSettings
            {
                Size = new Vector2i(width, height),
                Title = "Earth and Moon - OpenTK",
                Flags = ContextFlags.ForwardCompatible
            })
        {
            CenterWindow(new Vector2i(width, height));
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();

            _earth = new Sphere();
            _moon = new Sphere();

            _earthTexture = new Texture("Textures/earth.jpg");
            _moonTexture = new Texture("Textures/moon.jpg");

            _camera = new Camera(new Vector3(0f, 0f, 10f));

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix(Size.X, Size.Y));

            // Земля
            Matrix4 earthModel = Matrix4.CreateRotationY(_earthRotation);
            _shader.SetMatrix4("model", earthModel);
            _earthTexture.Use();
            _earth.Render();

            // Луна
            Vector3 moonPosition = new Vector3(MathF.Cos(_moonOrbitAngle) * 3f, 0f, MathF.Sin(_moonOrbitAngle) * 3f);
            Matrix4 moonModel = Matrix4.CreateScale(0.27f) * Matrix4.CreateTranslation(moonPosition);
            _shader.SetMatrix4("model", moonModel);
            _moonTexture.Use();
            _moon.Render();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (!IsFocused) return;

            var keyboard = KeyboardState;
            var mouse = MouseState;

            // Управление окнами
            if (keyboard.IsKeyPressed(Keys.Escape))
                Close();

            if (keyboard.IsKeyPressed(Keys.F11))
            {
                _fullscreen = !_fullscreen;
                WindowState = _fullscreen ? WindowState.Fullscreen : WindowState.Normal;
            }

            if (keyboard.IsKeyPressed(Keys.F3))
            {
                _wireframe = !_wireframe;
                GL.PolygonMode(MaterialFace.FrontAndBack, _wireframe ? PolygonMode.Line : PolygonMode.Fill);
            }

            // Сброс камеры
            if (keyboard.IsKeyPressed(Keys.R))
            {
                _camera.Position = new Vector3(0f, 0f, 10f);
                _camera.Yaw = -90f;
                _camera.Pitch = 0f;
            }

            // Переключение состояния камеры (активна/неактивна)
            if (keyboard.IsKeyPressed(Keys.Space))
            {
                _camera.ToggleCameraControl();
                CursorState = _camera.IsCameraActive ? CursorState.Grabbed : CursorState.Normal;
            }

            // Обновление позиции камеры (если активна)
            _camera.Update(args, keyboard);

            if (_camera.IsCameraActive && IsFocused)
            {
                _camera.AddRotation(mouse.Delta.X, -mouse.Delta.Y); // -Y — инвертируем, т.к. мышь двигается вниз — pitch вверх
            }

            // Приближение/отдаление
            if (mouse.ScrollDelta.Y != 0)
                _camera.Zoom(mouse.ScrollDelta.Y * 5f);

            // Анимация
            _earthRotation += (float)args.Time * 0.5f;
            _moonOrbitAngle += (float)args.Time;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _earthTexture?.Dispose();
            _moonTexture?.Dispose();
            _shader?.Dispose();

            _earth?.Dispose();
            _moon?.Dispose();
        }
    }
}
