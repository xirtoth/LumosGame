using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class Animation
{
    private Dictionary<string, AnimationData> animations;
    private AnimationData currentAnimation;
    public string CurrentAnimationName { get; private set; }

    public Animation()
    {
        animations = new Dictionary<string, AnimationData>();
    }

    // Add a new animation
    public void AddAnimation(string name, Texture2D[] frames, float animationSpeed)
    {
        AnimationData animationData = new AnimationData(frames, animationSpeed);
        animations[name] = animationData;

        // Set the first added animation as the current animation
        if (currentAnimation == null)
        {
            currentAnimation = animationData;
        }
    }

    // Play a specific animation by name
    public void PlayAnimation(string name)
    {
        if (animations.ContainsKey(name))
        {
            currentAnimation = animations[name];
            currentAnimation.Reset();
            CurrentAnimationName = name;
        }
    }

    // Update the animation
    public void Update(GameTime gameTime)
    {
        if (currentAnimation != null)
        {
            currentAnimation.Update(gameTime);
        }
    }

    // Draw the current frame of the animation
    public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
    {
        if (currentAnimation != null)
        {
            currentAnimation.Draw(spriteBatch, position, spriteEffects);
        }
    }

    // AnimationData class to store animation-specific data
    private class AnimationData
    {
        private int currentFrameIndex;
        private float animationSpeed;
        private float frameDuration;
        private Texture2D[] frames;
        private float frameTimer;

        public AnimationData(Texture2D[] frames, float animationSpeed)
        {
            this.frames = frames;
            this.animationSpeed = animationSpeed;
            this.frameDuration = 1f / animationSpeed;
            this.currentFrameIndex = 0;
        }

        // Update the animation
        public void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameTimer += elapsedSeconds;

            // Check if enough time has passed to move to the next frame
            if (frameTimer >= frameDuration)
            {
                // Move to the next frame
                currentFrameIndex++;
                frameTimer = 0f;

                // Check if animation has reached the end
                if (currentFrameIndex >= frames.Length)
                {
                    // Reset animation back to the first frame
                    currentFrameIndex = 0;
                }
            }
        }

        // Draw the current frame of the animation
        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(frames[currentFrameIndex], position, null, Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }

        // Reset the animation to the first frame
        public void Reset()
        {
            currentFrameIndex = 0;
        }
    }
}