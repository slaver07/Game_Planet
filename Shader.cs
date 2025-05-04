using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace GamePlanet
{
    public class Shader : IDisposable
    {
        public int Handle { get; private set; }

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource = File.ReadAllText(GetFullPath(vertexPath));
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompile(vertexShader, "VERTEX");

            string fragmentShaderSource = File.ReadAllText(GetFullPath(fragmentPath));
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompile(fragmentShader, "FRAGMENT");

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckProgramLink(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private string GetFullPath(string relativePath)
        {
            string basePath = AppContext.BaseDirectory;
            return Path.Combine(basePath, "..", "..", "..", relativePath);
        }

        public void Use() => GL.UseProgram(Handle);

        public int GetAttribLocation(string attribName) => GL.GetAttribLocation(Handle, attribName);

        public void SetMatrix4(string name, OpenTK.Mathematics.Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location != -1)
            {
                GL.UniformMatrix4(location, false, ref matrix);
            }
        }

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location != -1)
            {
                GL.Uniform1(location, value);
            }
        }

        private void CheckShaderCompile(int shader, string type)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                throw new Exception($"{type} shader compilation failed:\n{info}");
            }
        }

        private void CheckProgramLink(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                throw new Exception($"Shader program linking failed:\n{info}");
            }
        }

        public void Dispose() => GL.DeleteProgram(Handle);
    }
}
