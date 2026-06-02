using System;
using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace silkgl;

public class VertexArray<TIndex> : IDisposable
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
        vao = initVertexArray(ref vertexArr, ref indexArr, bufferUsage);
    }
    
    

    public VertexArray(ref GL gl_ref, Buffer<float> vbo_ref, Buffer<TIndex> ebo_ref, PrimitiveType primitive)
    {
        gl = gl_ref;
        primitiveType = primitive;
        vao = gl.GenVertexArray();
        vbo = vbo_ref;
        ebo = ebo_ref;
        Bind();
        vbo.Bind();
        ebo.Bind();
    }

    public VertexArray(VertexArray<TIndex> arr, Vector2D<float> offset)
    {
        gl = arr.gl;
        primitiveType = arr.primitiveType;
        vao = arr.vao;
        ebo = arr.ebo;

        var offsetArr = arr.vbo.data;
        for (int i = 0; i < offsetArr.Length; i += 3)
        {
            offsetArr[i] += offset.X;
            offsetArr[i + 1] += offset.Y;
        }

        vbo = new(ref gl, arr.vbo.bufferType, arr.vbo.bufferUsage, offsetArr.AsSpan());
    }
    
    private unsafe uint initVertexArray(ref Span<float> vertexArr, ref Span<TIndex> indexArr, BufferUsageARB bufferUsage)
    {
        uint vArray = gl.GenVertexArray();
        Bind();
        vbo = new(ref gl, BufferTargetARB.ArrayBuffer, bufferUsage, vertexArr);
        ebo = new(ref gl, BufferTargetARB.ElementArrayBuffer, bufferUsage, indexArr);
        
        return vArray;
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