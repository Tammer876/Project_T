using System;
using Silk.NET.OpenGL;

namespace silkgl;

public class Buffer<TDataType> : IDisposable where TDataType : unmanaged
{
    private uint vbo;
    private BufferTargetARB bufferType;
    private BufferUsageARB bufferUsage;
    private GL gl;

    public Buffer(ref GL gl_ref, BufferTargetARB type, BufferUsageARB usage, ref Span<TDataType> data)
    {
        gl =  gl_ref;
        bufferType = type;
        bufferUsage = usage;
        vbo = InitBuffer(ref data);
    }
    
    private unsafe uint InitBuffer(ref Span<TDataType> data)
    {
        uint buffer = gl.GenBuffer();
        Bind();
        fixed (void* ptr = data)
        {
            gl.BufferData(bufferType, (uint) (data.Length * sizeof(TDataType)), ptr, bufferUsage);
        }
        
        return buffer;
    }

    private void Bind()
    {
        gl.BindBuffer(bufferType, vbo);
    }

    public void Dispose()
    {
        gl.DeleteBuffer(vbo);
    }
}