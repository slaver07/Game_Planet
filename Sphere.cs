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

        public Sphere(float radius = 1f, int sectorCount = 360, int stackCount = 180)
        {
            List<float> vertices = new();
            List<uint> indices = new();

            float sectorStep = MathHelper.TwoPi / sectorCount;
            float stackStep = MathF.PI / stackCount;

            for (int i = 0; i <= stackCount; ++i)
            {
                float stackAngle = MathF.PI / 2 - i * stackStep;
                float xy = radius * MathF.Cos(stackAngle);
                float z = radius * MathF.Sin(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    float sectorAngle = j * sectorStep;
                    float x = xy * MathF.Cos(sectorAngle);
                    float y = xy * MathF.Sin(sectorAngle);

                    float s = (float)j / sectorCount;
                    float t = (float)i / stackCount;

                    vertices.AddRange(new float[]
                    {
                        x, y, z,
                        s, t
                    });
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

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            int stride = 5 * sizeof(float);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

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
