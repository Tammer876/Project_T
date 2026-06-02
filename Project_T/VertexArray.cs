using System;
using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace silkgl;

public class VertexArray<TIndex> : IDisposable
    where TIndex : unmanaged
{
    private uint vao;
    private Buffer<float> vbo;
    private Buffer<TIndex> ebo;
    private PrimitiveType primitiveType;
    private GL gl;

    public VertexArray(ref GL gl_ref, Span<float> vertexArr, Span<TIndex> indexArr, BufferUsageARB bufferUsage, PrimitiveType primitive)
    {
        gl = gl_ref;
        primitiveType = primitive;
        initVertexArray(vertexArr, indexArr, bufferUsage);
    }
    
    

    public VertexArray(VertexArray<TIndex> arr, Vector2D<float> offset)
    {
        gl = arr.gl;
        primitiveType = arr.primitiveType;


        var offsetArr = new float[arr.vbo.data.Length];
        arr.vbo.data.CopyTo(offsetArr, 0);

        for (int i = 0; i < offsetArr.Length; i += 3)
        {
            offsetArr[i] += offset.X;
            offsetArr[i + 1] += offset.Y;
        }

        initVertexArray(offsetArr.AsSpan(), arr.ebo.data.AsSpan(), arr.ebo.bufferUsage);
    }
    
    private unsafe void initVertexArray(Span<float> vertexArr, Span<TIndex> indexArr, BufferUsageARB bufferUsage)
    {
        vao = gl.GenVertexArray();
        Bind();
        vbo = new(ref gl, BufferTargetARB.ArrayBuffer, bufferUsage, vertexArr);
        ebo = new(ref gl, BufferTargetARB.ElementArrayBuffer, bufferUsage, indexArr);

        VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)(3 * sizeof(float)), null);
    }

    public void Bind()
    {
        gl.BindVertexArray(vao);
    }

    public unsafe void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, uint stride, void* pointer)
    {
        gl.VertexAttribPointer(index, size, type, normalized, stride, pointer);
        gl.EnableVertexAttribArray(index);
    }

    public uint IndexCount => ebo.Length;
    public void Dispose()
    {
        gl.DeleteVertexArray(vao);
    }
}