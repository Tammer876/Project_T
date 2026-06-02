using System;
using Silk.NET.OpenGL;

namespace silkgl;

public class VertexArray<TVertex, TIndex> : IDisposable
    
{
    private uint vao;
    private PrimitiveType primitiveType;
    private GL gl;

    public VertexArray(ref GL gl_ref, ref Span<TVertex> vertexArr, ref Span<TIndex> indexArr, BufferUsageARB bufferUsage, PrimitiveType primitive)
    {
        gl = gl_ref;
        primitiveType = primitive;
        // vao = initVertexArray();
    }

    public VertexArray(ref GL gl_ref, Buffer<TVertex> vbo, Buffer<TIndex> ebo, PrimitiveType primitive)
    {
        gl = gl_ref;
        primitiveType = primitive;
        vao = gl.GenVertexArray();
        Bind();
        vbo.Bind();
        ebo.Bind();
    }
    
    private unsafe uint initVertexArray(ref Span<TVertex> vertexArr, ref Span<TIndex> indexArr, BufferUsageARB bufferUsage)
    {
        uint vArray = gl.GenVertexArray();
        Bind();
        Buffer<TVertex> vbo = new(ref gl, BufferTargetARB.ArrayBuffer, bufferUsage, ref vertexArr);
        Buffer<TIndex> ebo = new(ref gl, BufferTargetARB.ElementArrayBuffer, bufferUsage, ref indexArr);
        
        return vArray;
    }

    public void Bind()
    {
        gl.BindVertexArray(vao);
    }

    public unsafe void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, void* pointer)
    {
        // gl.VertexAttribPointer();
    }
    
    public void Dispose()
    {
        gl.DeleteVertexArray(vao);
    }
}