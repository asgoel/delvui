﻿using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Plugin;
using System.Collections.Generic;
using System.Numerics;

namespace DelvUI.Interface.Jobs
{
    public class JobHud : HudElement, IHudElementWithActor
    {
        protected DalamudPluginInterface PluginInterface => Plugin.PluginInterface;

        public JobConfig Config => (JobConfig)_config;

        public Actor Actor { get; set; } = null;

        public virtual List<uint> GetJobSpecificBuffs()
        {
            return new List<uint>();
        }

        public JobHud(string ID, JobConfig config) : base(ID, config)
        {
        }

        public override void Draw(Vector2 origin)
        {
        }
    }
}
