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
        {
            // Clear the count so disposal cannot retry a failed write.
            _count = 0;
            _target.Write(_buffer);
        }
    }

    /// <summary>
    /// Writes pending bytes to the target stream.
    /// </summary>
    public void Dispose()
    {
        if (_count == 0)
            return;

        ReadOnlySpan<byte> pending = _buffer.Slice(0, _count);
        _count = 0;
        _target.Write(pending);
    }
}
#endif
