using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.MaymayHelper
{
    [CustomEntity("MayMayHelper/RecallRefill")]
    public class RecallRefill : Refill
    {
        static public bool PlayerHasRecallDash = false;
        private readonly float RecallDelay;

        public RecallRefill(EntityData data, Vector2 offset) : base(data.Position + offset, data.Bool("twoDashes"), data.Bool("oneUse"))
        {
            RecallDelay = data.Float("recallDelay", 2);
            DynamicData refillData = new(typeof(Refill), this);
            string spritePath = "Maymayhelper/objects/RecallRefill-" + (data.Bool("twoDashes") ? '2' : '1');

            Remove(refillData.Get<Sprite>("sprite"));
            Sprite idleSprite = new(GFX.Game, $"{spritePath}/idle");
            idleSprite.AddLoop("idle", "", 0.15f);
            idleSprite.Play("idle");
            idleSprite.CenterOrigin();
            Add(idleSprite);
            refillData.Set("sprite", idleSprite);

            Remove(refillData.Get<Sprite>("flash"));

            Remove(refillData.Get<Image>("outline"));
            Image outlineImage = new(GFX.Game[$"{spritePath}/outline"])
            {
                Visible = false,
            };
            outlineImage.CenterOrigin();
            Add(outlineImage);
            refillData.Set("outline", outlineImage);



        }

        private new void OnPlayer(Player player)
        {
            if (!PlayerHasRecallDash)
            {
                player.Dashes = 0;
                Scene.Add(new PlayBackGhost(player, RecallDelay));
                PlayerHasRecallDash = true;

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
