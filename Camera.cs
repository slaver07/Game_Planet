using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GamePlanet
{
    public class Camera
    {
        private Vector3 _position;
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        private float _pitch;
        private float _yaw = -90f;

        private float _speed = 1.5f;
        private float _sensitivity = 0.2f;

        private Vector2 _lastMousePosition;
        private bool _firstMove = true;

        public Camera(Vector3 position)
        {
            _position = position;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(_position, _position + _front, _up);
        }

        public Matrix4 GetProjectionMatrix(float width, float height)
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), width / height, 0.1f, 100f);
        }

        public void Update(FrameEventArgs args, KeyboardState input)
        {
            float delta = (float)args.Time * _speed;

            if (input.IsKeyDown(Keys.W))
                _position += _front * delta;
            if (input.IsKeyDown(Keys.S))
                _position -= _front * delta;
            if (input.IsKeyDown(Keys.A))
                _position -= _right * delta;
            if (input.IsKeyDown(Keys.D))
                _position += _right * delta;
            if (input.IsKeyDown(Keys.Space))
                _position += _up * delta;
            if (input.IsKeyDown(Keys.LeftShift))
                _position -= _up * delta;
        }

        public void MouseMove(Vector2 mousePosition)
        {
            if (_firstMove)
            {
                _lastMousePosition = mousePosition;
                _firstMove = false;
            }

            var delta = mousePosition - _lastMousePosition;
            _lastMousePosition = mousePosition;

            _yaw += delta.X * _sensitivity;
            _pitch -= delta.Y * _sensitivity;

            _pitch = MathHelper.Clamp(_pitch, -89f, 89f);

            Vector3 direction;
            direction.X = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Cos(MathHelper.DegreesToRadians(_yaw));
            direction.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
            direction.Z = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Sin(MathHelper.DegreesToRadians(_yaw));

            _front = Vector3.Normalize(direction);
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public Vector3 Position => _position;
    }
}