// Unity supports C# init-only properties used by records, but its framework
// does not provide IsExternalInit. This type is required by the compiler when
// compiling records with positional properties.
namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }
