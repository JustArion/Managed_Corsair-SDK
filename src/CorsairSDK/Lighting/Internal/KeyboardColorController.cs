﻿using Corsair.Device.Devices;

namespace Corsair.Lighting.Internal;

using System.Diagnostics;
using System.Drawing;
using System.Reactive.Disposables;
using Connection;
using Connection.Internal.Contracts;
using Contracts;
using Exceptions;

// TODO: Move away from locks (It's currently a simple implementation)
// The current implementation acts as scaffolding for future optimization
// It uses a limited amount of methods in the interop layer to set lighting
internal class KeyboardColorController(IDeviceConnectionHandler connectionHandler) : IKeyboardColorController
{
    public Keyboard Device { get; private set; } = null!;

    /// <summary>
    /// We only allow disposal to occur if the receipt holder has the correct reciept (The IDisposable is the receipt)
    /// </summary>
    private readonly ReceiptHandler<int> _receiptHandler = new();

    public ILightingInterop NativeInterop => _lighting;

    internal readonly ILightingInterop _lighting = new LightingInterop();

    private readonly List<IDisposable?> _disposables = [];

    internal void SetContext(Keyboard keyboard, AccessLevel accessLevel)
    {
        _lighting.SetDeviceContext(keyboard);
        Device = keyboard;
        SyncKeyboardKeys();

        _disposables.Add(_lighting.RequestControl(accessLevel));

        Debug.WriteLine($"Context set for device {keyboard.Model} as a Keyboard", "Color Controller");
    }

    private void SyncKeyboardKeys()
        => _keyboardKeys = [
            ..(from keyboardKey in Enum.GetValues<KeyboardKey>()
                select keyboardKey into keyId
                let position = _lighting.GetPosition((int)keyId)
                where position != default
                select new KeyboardKeyState(keyId, position) { Color = Color.Black })
            .ToArray()
        ];

    internal void DirectlySetColor(int ledId, Color color)
    {
        _lighting.SetLedColor(ledId, color);

        UpdateKeyboardMap(ledId, color);
    }

    private void ClearColor(int ledId)
    {
        _lighting.ClearLedColor(ledId);

        UpdateKeyboardMap(ledId, Color.Black);
    }

    private void UpdateKeyboardMap(int ledId, Color color)
    {
        var keyboardKey = KeyboardKeys.FirstOrDefault(x => x.Id == ledId);
        if (keyboardKey == null)
            Debug.WriteLine($"Unknown key of Id {ledId}");
        else
            keyboardKey.Color = color;
    }

    private HashSet<KeyboardKeyState> _keyboardKeys = [];
    public IReadOnlySet<KeyboardKeyState> KeyboardKeys => _keyboardKeys;

    public IDisposable SetFromBitmap(byte[] bitmap)
    {
        ThrowIfDisconnected();

        using var ms = new MemoryStream(bitmap);
        using var bmp = new Bitmap(ms);

        var keys = KeyboardKeys.Where(x => bmp.Width > x.Coordinate.X && bmp.Height > x.Coordinate.Y).Select(key => {

            var pixel = bmp.GetPixel((int)key.Coordinate.X, (int)key.Coordinate.Y);

            DirectlySetColor(key.Id, pixel);

            return key.Id;
        }).ToArray();

       return  _receiptHandler.Set(keys, id => Disposable.Create(id, ClearColor));
    }

    private static IEnumerable<KeyboardKey> GetKeysFromZone(KeyboardZones zones, Keyboard device)
        => ZoneUtility.GetKeysFromZones(zones, device);

    public IDisposable SetKeys(Color color, IEnumerable<KeyboardKey> keys)
    {
        ThrowIfDisconnected();

        return Internal_SetKeys(color, keys);
    }
    public IDisposable SetKeys(Color color, params KeyboardKey[] keys)
    {
        ThrowIfDisconnected();

        return Internal_SetKeys(color, keys);
    }

    public IDisposable SetKeys(Color color, params KeyboardKeyState[] keys)
    {
        ThrowIfDisconnected();

        return Internal_SetKeys(color, keys.Select(x => x.Key));
    }

    internal IDisposable Internal_SetKeys(Color color, IEnumerable<KeyboardKey> keys)
    {

        // Skip all the keys that are in _keyboardKeys that have the same color

        keys = GetKeysThatAreNot(color, keys);

        var ids = ZoneUtility.GetIdsFromKeys(keys);


        var leys = ids.Where(IsActualKey).Select(id => {
            DirectlySetColor(id, color);

            return id;
        });

        return _receiptHandler.Set(leys, id => Disposable.Create(id, ClearColor));
    }

    /// <summary>
    /// This is for the case (which was tested in) for if the user doesn't have a key that we have on file.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool IsActualKey(int id) => KeyboardKeys.FirstOrDefault(x => x.Id == id) != null;

    public void ClearKeys(params KeyboardKey[] keys) => ClearKeys((IEnumerable<KeyboardKey>)keys);

    public void ClearKeys(IEnumerable<KeyboardKey> keys)
    {
        ThrowIfDisconnected();

        Internal_ClearKeys(keys);
    }

    private IEnumerable<KeyboardKey> GetKeysThatAreNot(Color color, IEnumerable<KeyboardKey> keys) =>
        keys.Where(x => {
            var keyboardKey = _keyboardKeys.FirstOrDefault(y => y.Key == x);
            if (keyboardKey == null)
                return false;

            return keyboardKey.Color != color;
        });

    private void Internal_ClearKeys(IEnumerable<KeyboardKey> keys)
    {
        keys = GetKeysThatAreNot(Color.Black, keys);

        var ids = ZoneUtility.GetIdsFromKeys(keys);

        _receiptHandler.RelinquishAccess(ids);
    }

    public IDisposable SetZones(Color color, KeyboardZones zones)
    {
        ThrowIfDisconnected();

        var keys = GetKeysFromZone(zones, Device);
        return Internal_SetKeys(color, keys);
    }

    public void ClearZones(KeyboardZones zones)
    {
        ThrowIfDisconnected();

        var keys = GetKeysFromZone(zones, Device);
        Internal_ClearKeys(keys);
    }

    public IDisposable SetMultiple(params KeyboardKeyColor[] keyColors)
    {
        ThrowIfDisconnected();

        var disposables = new List<IDisposable>();
        foreach (var (color, keys) in keyColors)
            disposables.Add(Internal_SetKeys(color, keys));

        return Disposable.Create(disposables.ToArray(), disposableKeys =>
        {
            foreach (var keyDisposable in disposableKeys)
                keyDisposable.Dispose();
        });
    }

    public void ClearMultiple(params KeyboardKeyColor[] keyColors)
    {
        ThrowIfDisconnected();

        var keys = keyColors.SelectMany(x => x.Keys);

        Internal_ClearKeys(keys);
    }

    public IDisposable SetGlobal(Color color)
    {
        ThrowIfDisconnected();

        return SetZones(color, KeyboardZones.AllZones);
    }

    public void ClearAll()
    {
        ThrowIfDisconnected();

        _receiptHandler.DisposeAndClear();
    }

    [StackTraceHidden]
    internal void ThrowIfDisconnected()
    {
        switch (connectionHandler.ConnectionState)
        {
            case ConnectionState.Connected:
                return;
            case ConnectionState.Connecting:
                // var token = connectionHandler.ReconnectPolicy.GetMaxWaitTime();
                var token = CancellationToken.None;
                connectionHandler.Wait(token);
                break;
            case ConnectionState.Disconnected:
            default:
                throw new DeviceNotConnectedException();
        }
    }

    public void Dispose()
    {
        ClearAll();

        foreach (var disposable in _disposables)
            disposable?.Dispose();
    }
}
