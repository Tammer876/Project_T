using System;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace silkgl;

public class Buffer<TDataType> : IDisposable
{
    private uint vbo;
    private uint length;
    public TDataType[] data { get; private set; }
    public BufferTargetARB bufferType { get; private set; }
    public BufferUsageARB bufferUsage { get; private set; }
    private GL gl;

    public Buffer(ref GL gl_ref, BufferTargetARB type, BufferUsageARB usage, Span<TDataType> data_span)
    {
        gl =  gl_ref;
        bufferType = type;
        bufferUsage = usage;
        length = (uint) data_span.Length;
        vbo = InitBuffer(data_span);
    }
    
    private unsafe uint InitBuffer(Span<TDataType> data_span)
    {
        data = data_span.ToArray();
        uint buffer = gl.GenBuffer();
        Bind();
        fixed (void* ptr = data_span)
        {
            gl.BufferData(bufferType, (uint) (data_span.Length * sizeof(TDataType)), ptr, bufferUsage);
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