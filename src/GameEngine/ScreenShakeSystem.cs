using Microsoft.Xna.Framework;
using System;

namespace GameEngine
{
    public class ScreenShakeSystem : IUpdateable, IDrawable
    {
        private float _currentShakeIntensity;
        private float _currentShakeDuration;
        private float _maxShakeDuration;
        private Vector2 _shakeOffset;
        private Random _random;
        private float _shakeDecayRate;
        
        public ScreenShakeSystem()
        {
            _currentShakeIntensity = 0f;
            _currentShakeDuration = 0f;
            _maxShakeDuration = 0f;
            _shakeOffset = Vector2.Zero;
            _random = new Random();
            _shakeDecayRate = 1.0f;
        }
        
        public void TriggerShake(float intensity, float duration)
        {
            _currentShakeIntensity = Math.Max(_currentShakeIntensity, intensity);
            _currentShakeDuration = Math.Max(_currentShakeDuration, duration);
            _maxShakeDuration = _currentShakeDuration;
        }
        
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_currentShakeDuration > 0)
            {
                _currentShakeDuration -= deltaTime;
                
                if (_currentShakeDuration <= 0)
                {
                    _currentShakeIntensity = 0f;
                    _shakeOffset = Vector2.Zero;
                }
                else
                {
                    // Calculate shake intensity based on remaining time
                    float intensity = _currentShakeIntensity * (_currentShakeDuration / _maxShakeDuration);
                    
                    // Generate random shake offset
                    _shakeOffset = new Vector2(
                        (_random.NextFloat() - 0.5f) * 2f * intensity,
                        (_random.NextFloat() - 0.5f) * 2f * intensity
                    );
                }
            }
            else
            {
                // Smoothly return to zero if there's any remaining shake
                _shakeOffset = Vector2.Lerp(_shakeOffset, Vector2.Zero, deltaTime * 10f);
                if (_shakeOffset.Length() < 0.1f)
                    _shakeOffset = Vector2.Zero;
            }
        }
        
        public Vector2 GetShakeOffset()
        {
            return _shakeOffset;
        }
        
        public void Draw(GameTime gameTime)
        {
            // Screen shake affects the camera/viewport, which would be handled by the render system
            // This system just calculates the offset that other systems can use
        }
    }
    

}