using System.ComponentModel;

// Polyfills required by netstandard2.0 to use records and init-only properties.
namespace System.Runtime.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit { }
