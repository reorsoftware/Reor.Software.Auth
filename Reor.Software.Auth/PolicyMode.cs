namespace Reor.Software.Auth;


[Flags]
public enum PolicyMode
{
    None = 0,
    
    FallbackPolicy = 1 << 0,
    DefaultPolicy = 1 << 1,
    
    All = FallbackPolicy | DefaultPolicy,
}