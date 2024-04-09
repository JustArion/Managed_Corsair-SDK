namespace Dawn.CorsairSDK.Rewrite;

public delegate Task AsyncEventHandler<in TEventArgs>(object? sender, TEventArgs e);