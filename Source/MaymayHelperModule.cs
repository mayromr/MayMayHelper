using System;

namespace Celeste.Mod.MaymayHelper
{
    public class MaymayHelperModule : EverestModule
    {
        public static MaymayHelperModule Instance { get; private set; }

        public override Type SettingsType => typeof(MaymayHelperModuleSettings);
        public static MaymayHelperModuleSettings Settings => (MaymayHelperModuleSettings)Instance._Settings;

        public override Type SessionType => typeof(MaymayHelperModuleSession);
        public static MaymayHelperModuleSession Session => (MaymayHelperModuleSession)Instance._Session;

        public MaymayHelperModule()
        {
            Instance = this;
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(MaymayHelperModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(MaymayHelperModule), LogLevel.Info);
#endif
        }

        public override void Load()
        {
            RecallRefill.Load();
            Hooks.Load();
        }

        public override void Unload()
        {
            RecallRefill.Unload();
            Hooks.Load();
        }
    }
}
