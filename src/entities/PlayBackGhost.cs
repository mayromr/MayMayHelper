using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using static Celeste.Player;

namespace Celeste.Mod.MaymayHelper
{
    [Tracked(true)]
    public class PlayBackGhost : Entity
    {
        private const float MainGhostAlpha = 0.5f;
        private const float TrailAlpha = 0.25f;

        private readonly Player player;
        private readonly Hitbox normalPlayerHitBox;
        private readonly PlayerSprite mainGhostSprite;
        private readonly PlayerHair mainGhostHair;

        internal Queue<ChaserState> chaserStatesQueue;
        internal float pauseOffset = 0;
        private readonly float recallDelay;



        public PlayBackGhost(Player player, float recallDelay)
        {
            this.player = player;
            normalPlayerHitBox = (Hitbox)new DynamicData(this.player).Get("normalHitbox");
            this.recallDelay = recallDelay;
            Depth = this.player.Depth + 1;

            mainGhostSprite = new(PlayerSpriteMode.Playback);

            mainGhostHair = new(mainGhostSprite)
            {
                Border = Color.Black * MainGhostAlpha
            };

            TransitionListener transitionListener = new()
            {
                OnInEnd = InitChaserStates
            };

            Add(mainGhostHair);
            Add(mainGhostSprite);
            Add(transitionListener);

            InitChaserStates();
        }

        private void InitChaserStates()
        {
            chaserStatesQueue = new((int)(60 * recallDelay));
            pauseOffset = 0;

            if (player != null && !player.Dead)
            {
                chaserStatesQueue.Enqueue(new ChaserState(player)); ;
            }
        }

        public override void Update()
        {
            if (player != null && !player.Dead)
            {
                ChaserState newState = new(player);
                newState.TimeStamp -= pauseOffset;
                chaserStatesQueue.Enqueue(newState);
            }

            while (chaserStatesQueue.Count > 0 && ((Scene.TimeActive - (chaserStatesQueue.Peek().TimeStamp + pauseOffset)) > recallDelay))
            {
                chaserStatesQueue.Dequeue();
            }

            if (chaserStatesQueue.Count != 0)
            {
                ChaserState chaserState = chaserStatesQueue.Peek();

                if (chaserState.Animation != mainGhostSprite.CurrentAnimationID && chaserState.Animation != null && mainGhostSprite.Has(chaserState.Animation))
                {
                    mainGhostSprite.Play(chaserState.Animation, true, false);
                }

                mainGhostSprite.Scale = chaserState.Scale;
                mainGhostHair.Facing = chaserState.Facing;
                mainGhostHair.Color = chaserState.HairColor * MainGhostAlpha;
                mainGhostSprite.Color = chaserState.HairColor * MainGhostAlpha;
                Position = chaserState.Position;
            }

            base.Update();
        }
        public override void Render()
        {
            if (chaserStatesQueue.Count > 0)
            {
                Vector2 PlayerBodyOffset = new(0, normalPlayerHitBox.CenterY);

                var PrevState = chaserStatesQueue.Peek();
                foreach (var currentState in chaserStatesQueue.Skip(1))
                {
                    Draw.Line(PrevState.Position + PlayerBodyOffset, currentState.Position + PlayerBodyOffset, PrevState.HairColor * TrailAlpha);
                    PrevState = currentState;
                }

            }

            base.Render();
        }


        public Vector2 GetTeleportPosition()
        {
            return chaserStatesQueue.Count > 0 ? chaserStatesQueue.Peek().Position : player.Position;
        }
    }
}