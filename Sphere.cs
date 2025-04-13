using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace GamePlanet
{
    public class Sphere
    {
        private int _vao;
        private int _vbo;
        private int _ebo;
        private int _indexCount;

        public Sphere(float radius = 1f, int sectorCount = 36, int stackCount = 18)
        {
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();

            for (int i = 0; i <= stackCount; ++i)
            {
                float stackAngle = MathHelper.PiOver2 - i * MathHelper.Pi / stackCount;
                float xy = radius * MathF.Cos(stackAngle);
                float z = radius * MathF.Sin(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    float sectorAngle = j * MathHelper.TwoPi / sectorCount;
                    float x = xy * MathF.Cos(sectorAngle);
                    float y = xy * MathF.Sin(sectorAngle);

                    float u = (float)j / sectorCount;
                    float v = (float)i / stackCount;

                    // position (x, y, z), normal (x, y, z), texCoord (u, v)
                    vertices.AddRange(new float[] { x, y, z, x, y, z, u, v });
                }
            }

            for (int i = 0; i < stackCount; ++i)
            {
                int k1 = i * (sectorCount + 1);
                int k2 = k1 + sectorCount + 1;

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        indices.Add((uint)k1);
                        indices.Add((uint)k2);
                        indices.Add((uint)(k1 + 1));
                    }

                    if (i != (stackCount - 1))
                    {
                        indices.Add((uint)(k1 + 1));
                        indices.Add((uint)k2);
                        indices.Add((uint)(k2 + 1));
                    }
                }
            }

            _indexCount = indices.Count;

            // OpenGL буферы
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 8 * sizeof(float);

            // position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // texCoord
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
        }
    }
}