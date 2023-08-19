using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using static Celeste.Player;

namespace Celeste.Mod.MaymayHelper
{
    [Tracked(true)]
    public class PlayBackGhost : Entity
    {
        private readonly Player Player;
        private readonly PlayerSprite MainGhostSprite;
        private readonly PlayerHair MainGhostHair;

        private LinkedList<ChaserState> chaserStates;
        private readonly float RecallDelay;

        const float MainGhostAlpha = 0.5f;
        const float TrailAlpha = 0.25f;


        public PlayBackGhost(Player player, float recallDelay)
        {
            Player = player;
            RecallDelay = recallDelay;
            MainGhostSprite = new(PlayerSpriteMode.Playback);

            MainGhostHair = new(MainGhostSprite)
            {
                Border = Color.Black * MainGhostAlpha
            };

            TransitionListener transitionListener = new()
            {
                OnInEnd = InitChaserStates
            };

            Add(MainGhostHair);
            Add(MainGhostSprite);
            Add(transitionListener);

            InitChaserStates();
        }

        private void InitChaserStates()
        {
            chaserStates = new();

            if (Player != null && !Player.Dead)
            {
                chaserStates.AddFirst(new ChaserState(Player)); ;
            }
        }

        public override void Update()
        {
            if (Player != null && !Player.Dead)
            {
                chaserStates.AddFirst(new ChaserState(Player));
            }
            while (chaserStates.Count > 0 && ((Scene.TimeActive - chaserStates.Last.Value.TimeStamp) > RecallDelay))
            {
                chaserStates.RemoveLast();
            }

            if (chaserStates.Count != 0)
            {
                ChaserState chaserState = chaserStates.Last.Value;
                if (chaserState.Animation != MainGhostSprite.CurrentAnimationID && chaserState.Animation != null && MainGhostSprite.Has(chaserState.Animation))
                {
                    MainGhostSprite.Play(chaserState.Animation, true, false);
                }

                MainGhostSprite.Scale = chaserState.Scale;
                MainGhostHair.Facing = chaserState.Facing;
                MainGhostHair.Color = chaserState.HairColor * MainGhostAlpha;
                MainGhostSprite.Color = chaserState.HairColor * MainGhostAlpha;
                Position = chaserState.Position;
            }

            base.Update();
        }
        public override void Render()
        {
            var head = chaserStates.First;
            var normalHitbox = (Hitbox)new DynamicData(Player).Get("normalHitbox");
            Vector2 offset = new(0, normalHitbox.CenterY);

            while (head != null && head.Next != null)
            {
                Draw.Line(head.Value.Position + offset, head.Next.Value.Position + offset, head.Value.HairColor * TrailAlpha);

                head = head.Next;
            }

            base.Render();
        }


        public ChaserState PeekOldestChaserState()
        {
            return chaserStates.Last?.Value ?? new ChaserState();
        }
    }
}