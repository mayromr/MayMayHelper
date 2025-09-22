using System.Linq;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;

namespace Celeste.Mod.MaymayHelper
{
    public static class Hooks
    {
        public static void Load()
        {
            On.Celeste.Player.DashBegin += OnDashBegin;
            On.Celeste.Player.Die += OnDeath;
            On.Celeste.LevelLoader.StartLevel += LevelLoader_StartLevel;
            On.Celeste.Level.Reload += Level_Reload;
            On.Celeste.Level.TransitionTo += Level_TransitionTo;
            IL.Celeste.Level.Update += PatchLevelUpdate;
        }

        public static void Unload()
        {
            IL.Celeste.Level.Update -= PatchLevelUpdate;
            On.Celeste.Level.TransitionTo -= Level_TransitionTo;
            On.Celeste.Level.Reload -= Level_Reload;
            On.Celeste.LevelLoader.StartLevel -= LevelLoader_StartLevel;
            On.Celeste.Player.Die -= OnDeath;
            On.Celeste.Player.DashBegin -= OnDashBegin;
        }

        private static void OnDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
        {
            if (RecallRefill.PlayerHasRecallDash && self.Dashes == 0)
            {
                PlayBackGhost playBackGhost = self.Scene.Tracker.GetEntity<PlayBackGhost>();
                if (playBackGhost != null)
                {
                    Vector2 teleportTarget = playBackGhost.GetTeleportPosition();

                    self.Position = teleportTarget;



                    if (self.Scene.CollideCheck<Solid>(self.Collider.Bounds) || self.Scene.CollideCheck<FloatySpaceBlock>(self.Collider.Bounds))
                    {
                        self.Die(Vector2.Zero);
                    }

                    CleanCustomState(self.Scene);
                }
                else
                {
                    Logger.Log(LogLevel.Warn, "MayMayHelper", "Player has HasRecallDash set to true but no PlaybackGhost was found!");
                    CleanCustomState(self.Scene);
                }
            }


            orig(self);
        }

        private static PlayerDeadBody OnDeath(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
        {
            CleanCustomState(self.Scene);
            return orig.Invoke(self, direction, evenIfInvincible, registerDeathInStats);
        }

        private static void Level_TransitionTo(On.Celeste.Level.orig_TransitionTo orig, Level self, LevelData next, Vector2 direction)
        {
            CleanCustomState(null);
            orig(self, next, direction);
        }

        private static void Level_Reload(On.Celeste.Level.orig_Reload orig, Level self)
        {
            CleanCustomState(null);
            orig(self);
        }

        private static void LevelLoader_StartLevel(On.Celeste.LevelLoader.orig_StartLevel orig, LevelLoader self)
        {
            CleanCustomState(null);
            orig(self);
        }

        private static void CleanCustomState(Scene scene)
        {
            if (scene != null)
            {
                foreach (var playbackGhost in scene.Tracker.GetEntities<PlayBackGhost>())
                {
                    playbackGhost.RemoveSelf();
                }

            }

            RecallRefill.PlayerHasRecallDash = false;
        }

        private static void PatchLevelUpdate(ILContext context)
        {
            ILCursor cursor = new(context);
            cursor.Emit(OpCodes.Ldarg_0).EmitDelegate((Level level) =>
            {
                float unpausedTimer = (float)new DynamicData(level).Get("unpauseTimer");
                if (unpausedTimer > 0f)
                {
                    foreach (PlayBackGhost playBackGhost in level.Tracker.GetEntities<PlayBackGhost>().Cast<PlayBackGhost>())
                    {
                        if (playBackGhost != null)
                        {
                            float offset = Engine.DeltaTime;

                            if (unpausedTimer - Engine.RawDeltaTime <= 0f)
                            {
                                offset *= 2;
                            }

                            playBackGhost.pauseOffset += offset;
                        }
                    }
                }
            });
        }
    }
}
