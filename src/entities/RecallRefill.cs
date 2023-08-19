using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.MaymayHelper
{
    [CustomEntity("MayMayHelper/RecallRefill")]
    public class RecallRefill : Refill
    {
        private readonly float RecallDelay;

        public RecallRefill(EntityData data, Vector2 offset) : base(data.Position + offset, data.Bool("twoDashes"), data.Bool("oneUse"))
        {
            RecallDelay = data.Float("recallDelay", 2);
        }

        private void OnPlayer(Player player)
        {
            if (!MaymayHelperModuleSession.HasRecallDash)
            {
                player.Dashes = 0;
                Scene.Add(new PlayBackGhost(player, RecallDelay));
                MaymayHelperModuleSession.HasRecallDash = true;

            }
        }

        static void OnPlayer(On.Celeste.Refill.orig_OnPlayer orig, Refill self, Player player)
        {
            if (self is RecallRefill recallRefill)
            {
                recallRefill.OnPlayer(player);
            }

            orig(self, player);
        }

        public static void Load()
        {
            On.Celeste.Refill.OnPlayer += OnPlayer;
        }

        public static void Unload()
        {
            On.Celeste.Refill.OnPlayer -= OnPlayer;
        }

    }
}