#if HAS_SPAN && !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

/// <summary>
/// Instructs the compiler to omit the locals initialization flag for the attributed method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class SkipLocalsInitAttribute : Attribute
{
}
#endif
