using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.IO;

namespace GamePlanet
{
    internal class Game : GameWindow
    {
        private Sphere earthSphere;
        private Sphere moonSphere;

        private Texture earthTexture;
        private Texture moonTexture;

        private Shader shader;

        private Camera camera;

        private float earthRotation = 0f;
        private float moonAngle = 0f;

        public Game(int width, int height) : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            Size = new Vector2i(width, height),
            Title = "Earth and Moon"
        })
        {
            CenterWindow(new Vector2i(width, height));
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Enable(EnableCap.DepthTest);

            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();

            earthSphere = new Sphere();
            moonSphere = new Sphere();

            earthTexture = new Texture("earth.jpg");
            moonTexture = new Texture("moon.jpg");

            camera = new Camera(new Vector3(0f, 0f, 10f));

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();

            // Передаём общую матрицу проекции и вида
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix(Size.X, Size.Y));

            // Отрисовка Земли
            Matrix4 earthModel = Matrix4.CreateRotationY(earthRotation);
            shader.SetMatrix4("model", earthModel);
            earthTexture.Use();
            earthSphere.Render();

            // Отрисовка Луны
            Vector3 moonPosition = new Vector3(MathF.Cos(moonAngle) * 3f, 0, MathF.Sin(moonAngle) * 3f);
            Matrix4 moonModel = Matrix4.CreateScale(0.27f) * Matrix4.CreateTranslation(moonPosition);
            shader.SetMatrix4("model", moonModel);
            moonTexture.Use();
            moonSphere.Render();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused) return;

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            camera.Update(args, KeyboardState);

            var mouse = MouseState;
            camera.MouseMove(mouse.Position);

            // Вращение Земли и Луны
            earthRotation += (float)args.Time * 0.5f;
            moonAngle += (float)args.Time;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            earthTexture.Dispose();
            moonTexture.Dispose();
            shader.Dispose();
        }
    }
}
