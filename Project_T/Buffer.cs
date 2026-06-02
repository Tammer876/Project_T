using System;
using Silk.NET.OpenGL;

namespace silkgl;

public class Buffer<TDataType> : IDisposable
{
    private uint vbo;
    private uint length;
    private BufferTargetARB bufferType;
    private BufferUsageARB bufferUsage;
    private GL gl;

    public Buffer(ref GL gl_ref, BufferTargetARB type, BufferUsageARB usage, ref Span<TDataType> data)
    {
        gl =  gl_ref;
        bufferType = type;
        bufferUsage = usage;
        length = (uint) data.Length;
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

    public void Bind()
    {
        gl.BindBuffer(bufferType, vbo);
    }
    
    public uint Length => length;

    public void Dispose()
    {
        gl.DeleteBuffer(vbo);
    }
}