﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Welt.MonoGame.Extended.Sprites;

namespace Welt.MonoGame.Extended.Animations.SpriteSheets
{
    public class SpriteSheetAnimator : IUpdate
    {
        public SpriteSheetAnimator(SpriteSheetAnimationFactory animationFactory)
        {
            _animationFactory = animationFactory;
        }

        private readonly SpriteSheetAnimationFactory _animationFactory;
        private SpriteSheetAnimation _currentAnimation;

        public Sprite TargetSprite { get; set; }
        
        public SpriteSheetAnimation Play(string name, Action onCompleted = null)
        {
            if (_currentAnimation == null || _currentAnimation.IsComplete || _currentAnimation.Name != name)
            {
                _currentAnimation = _animationFactory.Create(name);
                _currentAnimation.OnCompleted = onCompleted;
            }

            return _currentAnimation;
        }

        public void Update(float deltaTime)
        {
            if (_currentAnimation != null && !_currentAnimation.IsComplete)
            {
                _currentAnimation.Update(deltaTime);

                if (TargetSprite != null)
                    TargetSprite.TextureRegion = _currentAnimation.CurrentFrame;
            }
        }

        public void Update(GameTime gameTime)
        {
            Update(gameTime.GetElapsedSeconds());
        }

        public Sprite CreateSprite(Vector2 position)
        {
            var textureRegion = _currentAnimation != null ? _currentAnimation.CurrentFrame : _animationFactory.Frames.FirstOrDefault();
            return TargetSprite = new Sprite(textureRegion) { Position = position };
        }

        public Sprite CreateSprite()
        {
            return CreateSprite(Vector2.Zero);
        }
    }
}