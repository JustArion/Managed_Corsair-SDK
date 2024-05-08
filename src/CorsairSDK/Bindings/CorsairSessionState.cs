namespace Corsair.Bindings;

public enum CorsairSessionState
{
    CSS_Invalid = 0,
    CSS_Closed = 1,
    CSS_Connecting = 2,
    CSS_Timeout = 3,
    CSS_ConnectionRefused = 4,
    CSS_ConnectionLost = 5,
    CSS_Connected = 6,
}
