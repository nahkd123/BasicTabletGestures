using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Keyboard;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;
using OpenTabletDriver.Plugin.Timing;

namespace nahkd123.BasicTabletGestures
{
    [PluginName("Basic Tablet Tap Gestures")]
    public class BasicTabletTapGesturesFilter : IPositionedPipelineElement<IDeviceReport>
    {
        public PipelinePosition Position => PipelinePosition.PreTransform;
        public event Action<IDeviceReport>? Emit;

        [Resolved]
        public required IVirtualKeyboard Keyboard { get; set; }

        [Property("Touch deadline (ms)"), ToolTip("How long does it takes to consider a tap"), DefaultPropertyValue(80f)]
        public float TouchDeadline { get; set; } = 80f;

        [Property("Pen ignore time (ms)"), ToolTip("How long after the pen lifted up to enable touch gestures (use negative value to disable)"), DefaultPropertyValue(100f)]
        public float PenIgnoreTime { get; set; } = 100f;

        // You bet all of this is generated using script
        // I'm sorry

        [Property("1-finger tap action"), DefaultPropertyValue("")]
        public string TapAction1 { get => actions[0].ToString(); set => actions[0] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("2-finger tap action"), DefaultPropertyValue("")]
        public string TapAction2 { get => actions[1].ToString(); set => actions[1] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("3-finger tap action"), DefaultPropertyValue("")]
        public string TapAction3 { get => actions[2].ToString(); set => actions[2] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("4-finger tap action"), DefaultPropertyValue("")]
        public string TapAction4 { get => actions[3].ToString(); set => actions[3] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("5-finger tap action"), DefaultPropertyValue("")]
        public string TapAction5 { get => actions[4].ToString(); set => actions[4] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("6-finger tap action"), DefaultPropertyValue("")]
        public string TapAction6 { get => actions[5].ToString(); set => actions[5] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("7-finger tap action"), DefaultPropertyValue("")]
        public string TapAction7 { get => actions[6].ToString(); set => actions[6] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("8-finger tap action"), DefaultPropertyValue("")]
        public string TapAction8 { get => actions[7].ToString(); set => actions[7] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("9-finger tap action"), DefaultPropertyValue("")]
        public string TapAction9 { get => actions[8].ToString(); set => actions[8] = Action.Parse(value, Keyboard.SupportedKeys); }

        [Property("10-finger tap action"), DefaultPropertyValue("")]
        public string TapAction10 { get => actions[9].ToString(); set => actions[9] = Action.Parse(value, Keyboard.SupportedKeys); }

        private Action[] actions = new Action[10];
        private HPETDeltaStopwatch stopwatch = new(false);
        private bool penPresent = false;
        private bool tapped = false;
        private int maxTouches = 0;

        public BasicTabletTapGesturesFilter()
        {
            actions.Initialize();
        }

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport && PenIgnoreTime >= 0f)
            {
                stopwatch.Restart();
                penPresent = true;
            }

            if (value is ITouchReport touchReport)
            {
                var touches = touchReport.Touches.Where(touch => touch != null).Count();
                HandleTouch(touches);
            }


            Emit?.Invoke(value);
        }

        private void HandleTouch(int touches)
        {
            var elapsed = stopwatch.Elapsed.TotalMilliseconds;
            
            if (penPresent && elapsed <= PenIgnoreTime) return;
            penPresent = false;
                
            if (maxTouches == 0 && touches != 0)
            {
                tapped = false;
                stopwatch.Restart();
            }
            else if (maxTouches != 0 && touches == 0)
            {
                if (!tapped) Tap(maxTouches);
                maxTouches = 0;
                return;
            }
            else if (elapsed >= TouchDeadline && !tapped)
            {
                Tap(maxTouches);
            }

            maxTouches = Math.Max(maxTouches, touches);
        }

        private void Tap(int touches)
        {
            actions[Math.Clamp(touches - 1, 0, actions.Length - 1)].Trigger(Keyboard);
            tapped = true;
        }
    }

    public struct Action
    {
        public IEnumerable<string> Keys;

        public Action()
        {
            Keys = [];
        }

        public void Trigger(IVirtualKeyboard keyboard)
        {
            foreach (var key in Keys) keyboard.Press(key);
            foreach (var key in Keys) keyboard.Release(key);
        }

        public static Action Parse(string value, IEnumerable<string> validKeys)
        {
            return new Action
            {
                Keys = value.Split('+')
                .Select(key => key.Trim())
                .Select(key => key switch
                {
                    "control" or "Ctrl" or "ctrl" => "Control",
                    "alt" => "Alt",
                    "shift" => "Shift",
                    _ => key
                })
                .Where(key => validKeys.Contains(key))
            };
        }

        public override readonly string ToString()
        {
            if (!Keys.Any()) return "";
            return Keys.Aggregate((a, b) => $"{a} + {b}");
        }
    }
}
