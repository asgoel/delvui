using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.ClientState.Structs.JobGauge;
using Dalamud.Plugin;
using ImGuiNET;

namespace DelvUIPlugin.Interface
{
    public class DragoonHudWindow : HudWindow
    {
        public override uint JobId => Jobs.DRG;

        protected int EyeOfTheDragonBarHeight => PluginConfiguration.DRGEyeOfTheDragonHeight;
        protected int EyeOfTheDragonBarWidth => PluginConfiguration.DRGEyeOfTheDragonBarWidth;
        protected int EyeOfTheDragonPadding => PluginConfiguration.DRGEyeOfTheDragonPadding;
        protected new int XOffset => PluginConfiguration.DRGBaseXOffset;
        protected new int YOffset => PluginConfiguration.DRGBaseYOffset;
        protected int BloodBarHeight => PluginConfiguration.DRGBloodBarHeight;
        protected int InterBarOffset => PluginConfiguration.DRGInterBarOffset;
        protected Dictionary<string, uint> EyeOfTheDragonColor => PluginConfiguration.JobColorMap[Jobs.DRG * 1000];
        protected Dictionary<string, uint> BloodOfTheDragonColor => PluginConfiguration.JobColorMap[Jobs.DRG * 1000 + 1];
        protected Dictionary<string, uint> LifeOftheDragonColor => PluginConfiguration.JobColorMap[Jobs.DRG * 1000 + 2];
        protected Dictionary<string, uint> EmptyColor => PluginConfiguration.JobColorMap[Jobs.DRG * 1000 + 3];


        public DragoonHudWindow(DalamudPluginInterface pluginInterface, PluginConfiguration pluginConfiguration) : base(pluginInterface, pluginConfiguration) { }

        protected override void Draw(bool _)
        {
            DrawHealthBar();
            var nextHeight = DrawEyeOfTheDragonBars();
            DrawBloodOfTheDragonBar(nextHeight);
            DrawTargetBar();
            DrawFocusBar();
        }

        private int DrawEyeOfTheDragonBars()
        {
            var gauge = PluginInterface.ClientState.JobGauges.Get<DRGGauge>();

            var barSize = new Vector2(EyeOfTheDragonBarWidth, EyeOfTheDragonBarHeight);
            var xPos = CenterX - XOffset;
            var yPos = CenterY + YOffset;
            var cursorPos = new Vector2(xPos, yPos);
            var eyeCount = gauge.EyeCount;
            var drawList = ImGui.GetWindowDrawList();

            for (byte i = 0; i < 2; i++) {
                cursorPos = new Vector2(cursorPos.X + (EyeOfTheDragonBarWidth + EyeOfTheDragonPadding) * i, cursorPos.Y);
                if (eyeCount >= (i + 1))
                {
                    drawList.AddRectFilledMultiColor(
                        cursorPos, cursorPos + barSize,
                        EyeOfTheDragonColor["gradientLeft"], EyeOfTheDragonColor["gradientRight"], EyeOfTheDragonColor["gradientRight"], EyeOfTheDragonColor["gradientLeft"]
                    );
                } else
                {
                    drawList.AddRectFilled(cursorPos, cursorPos + barSize, EmptyColor["background"]);
                }
                drawList.AddRect(cursorPos, cursorPos + barSize, 0xFF000000);
            }

            return EyeOfTheDragonBarHeight;
        }

        private void DrawBloodOfTheDragonBar(int initialHeight)
        {
            var gauge = PluginInterface.ClientState.JobGauges.Get<DRGGauge>();

            var xPos = CenterX - XOffset;
            var yPos = CenterY + YOffset + initialHeight + InterBarOffset;
            var barWidth = EyeOfTheDragonBarWidth * 2 + EyeOfTheDragonPadding;
            var cursorPos = new Vector2(xPos, yPos);
            var barSize = new Vector2(barWidth, BloodBarHeight);

            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(cursorPos, cursorPos + barSize, EmptyColor["background"]);
            drawList.AddRect(cursorPos, cursorPos + barSize, 0xFF000000);

            var maxTimerMs = 30 * 1000;
            var currTimerMs = gauge.BOTDTimer;
            var currTimerSec = Math.Round(currTimerMs / 1000f).ToString();
            var currTimerSecSize = ImGui.CalcTextSize(currTimerSec);
            var textXPos = cursorPos.X + barWidth / 2f + currTimerSecSize.X;
            var textYPos = cursorPos.Y + (BloodBarHeight + currTimerSecSize.Y) / 2f;
            var textPos = new Vector2(textXPos, textYPos);
            if (currTimerMs == 0)
            {
                DrawOutlinedText(currTimerSec, textPos);
                return;
            }
            var scale = (float)currTimerMs / maxTimerMs;
            var botdBarSize = new Vector2(barWidth * scale, BloodBarHeight);
            if (gauge.BOTDState == BOTDState.LOTD)
            {
                drawList.AddRectFilledMultiColor(
                    cursorPos, cursorPos + botdBarSize,
                    LifeOftheDragonColor["gradientLeft"], LifeOftheDragonColor["gradientRight"], LifeOftheDragonColor["gradientRight"], LifeOftheDragonColor["gradientLeft"]);
            }
            else
            {
                drawList.AddRectFilledMultiColor(
                    cursorPos, cursorPos + botdBarSize,
                    BloodOfTheDragonColor["gradientLeft"], BloodOfTheDragonColor["gradientRight"], BloodOfTheDragonColor["gradientRight"], BloodOfTheDragonColor["gradientLeft"]);
            }
            DrawOutlinedText(currTimerSec, textPos);
        }
    }
}