#if HAS_SPAN
using System.Diagnostics;

namespace QRCoder;

/// <summary>
/// Batches single-byte writes to a stream using a caller-provided buffer.
/// </summary>
internal ref struct StackBufferedByteWriter
{
    private readonly Stream _target;
    private readonly Span<byte> _buffer;
    private int _count;

    public StackBufferedByteWriter(Stream target, Span<byte> buffer)
    {
        Debug.Assert(!buffer.IsEmpty);
        _target = target;
        _buffer = buffer;
    }

    public void WriteByte(byte value)
    {
        _buffer[_count++] = value;
        if (_count == _buffer.Length)
            Flush();
    }

    /// <summary>
    /// Writes pending bytes to the target stream.
    /// </summary>
    public void Flush()
    {
        if (_count == 0)
            return;

        _target.Write(_buffer.Slice(0, _count));
        _count = 0;
    }
}
#endif
