using Corsair.Device.Internal.Contracts;
using Corsair.Lighting.Animations.Internal;
using Corsair.Lighting.Contracts;

namespace Corsair.Lighting.Animations;


public sealed class ProgressAnimation : LightingAnimation
{
    private readonly ProgressAnimationOptions _options;
    private readonly IKeyboardColorController _keyboardControl;

    protected override IGrouping<float, KeyValuePair<int, LedInfo>>[] Positions { get; init; }

    public ProgressAnimation(ProgressAnimationOptions options, IKeyboardColorController keyboardColorController)
    {
        _options = options;
        _keyboardControl = keyboardColorController;
        var positions = keyboardColorController.NativeInterop.GetPositionInfo();

        var isVertical = options.StartPosition is StartingPosition.TopToBottom or StartingPosition.BottomToTop;

        var keys = options.Keys.ToHashSet();
        var keyInfos = positions.Where(x => keys.Contains((KeyboardKeys)x.Key)).ToArray();

        var axisGroup = keyInfos.OrderBy(x =>
                isVertical
                    ? x.Value.Position.Y
                    : x.Value.Position.X)
            .GroupBy(x =>
                isVertical
                    ? x.Value.Position.Y
                    : x.Value.Position.X);

        Positions = (options.StartPosition is StartingPosition.LeftToRight or StartingPosition.TopToBottom
            ? axisGroup
            : axisGroup.Reverse())
            .ToArray();
    }



    public override async Task Play()
    {
        using (await DeviceAnimationSemaphore.WaitAsync(_keyboardControl.Device))
        {

        }
    }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}
