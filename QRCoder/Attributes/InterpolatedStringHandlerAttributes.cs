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
    /// Initializes a new instance of the <see cref="InterpolatedStringHandlerArgumentAttribute"/> class.
    /// </summary>
    /// <param name="argument1">The name of the first argument that should be passed to the handler.</param>
    /// <param name="argument2">The name of the second argument that should be passed to the handler.</param>
    public InterpolatedStringHandlerArgumentAttribute(string argument1, string argument2)
    {
        Arguments = new[] { argument1, argument2 };
    }

    /// <summary>
    /// Gets the arguments that should be passed to the handler.
    /// </summary>
    public string[] Arguments { get; }
}

/// <summary>
/// Indicates that compiler support for a particular feature is required for the location where this attribute is applied.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompilerFeatureRequiredAttribute"/> class.
    /// </summary>
    /// <param name="featureName">The name of the compiler feature.</param>
    public CompilerFeatureRequiredAttribute(string featureName)
    {
        FeatureName = featureName;
    }

    /// <summary>
    /// Gets the name of the compiler feature.
    /// </summary>
    public string FeatureName { get; }

    /// <summary>
    /// Gets a value that indicates whether the compiler can choose to allow access if it does not understand <see cref="FeatureName"/>.
    /// </summary>
    public bool IsOptional { get; set; }

    /// <summary>
    /// The feature name used for ref structs.
    /// </summary>
#pragma warning disable IDE1006 // Must match the BCL constant names
    public const string RefStructs = nameof(RefStructs);

    /// <summary>
    /// The feature name used for required members.
    /// </summary>
    public const string RequiredMembers = nameof(RequiredMembers);
#pragma warning restore IDE1006
}
#endif
