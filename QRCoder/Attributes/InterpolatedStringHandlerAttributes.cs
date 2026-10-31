#if !NET6_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Indicates the attributed type is an interpolated string handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal sealed class InterpolatedStringHandlerAttribute : Attribute
{
}

/// <summary>
/// Indicates which arguments an interpolated string handler passes through to the underlying handler constructor.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class InterpolatedStringHandlerArgumentAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolatedStringHandlerArgumentAttribute"/> class.
    /// </summary>
    /// <param name="argument">The name of the argument that should be passed to the handler.</param>
    public InterpolatedStringHandlerArgumentAttribute(string argument)
    {
        Arguments = new[] { argument };
    }

    /// <summary>
    /// Gets the arguments that should be passed to the handler.
    /// </summary>
    public string[] Arguments { get; }
}

#endif
