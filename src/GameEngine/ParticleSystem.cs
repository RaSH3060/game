using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class Particle
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public float LifeSpan { get; set; }
        public float MaxLifeSpan { get; set; }
        public Texture2D Texture { get; set; }
        public bool IsActive { get; set; }
        
        public Particle()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            Color = Color.White;
            Rotation = 0f;
            Scale = Vector2.One;
            LifeSpan = 0f;
            MaxLifeSpan = 1f;
            IsActive = false;
        }
    }
    
    public class ParticleSystem : IUpdateable, IDrawable
    {
        private List<Particle> _particles;
        private Random _random;
        private RenderSystem _renderSystem;
        private AssetManager _assetManager;
        
        public int MaxParticles { get; set; }
        
        public ParticleSystem()
        {
            _particles = new List<Particle>();
            _random = new Random();
            MaxParticles = 1000;
        }
        
        public void Initialize(RenderSystem renderSystem, AssetManager assetManager)
        {
            _renderSystem = renderSystem;
            _assetManager = assetManager;
        }
        
        public Particle CreateParticle()
        {
            // Find an inactive particle or create a new one
            for (int i = 0; i < _particles.Count; i++)
            {
                if (!_particles[i].IsActive)
                {
                    _particles[i].IsActive = true;
                    return _particles[i];
                }
            }
            
            // If all particles are active, create a new one if we haven't reached the limit
            if (_particles.Count < MaxParticles)
            {
                Particle newParticle = new Particle();
                newParticle.IsActive = true;
                _particles.Add(newParticle);
                return newParticle;
            }
            
            return null; // No available particles
        }
        
        public void SpawnBloodParticles(Vector2 position, int count = 5)
        {
            for (int i = 0; i < count; i++)
            {
                Particle particle = CreateParticle();
                if (particle != null)
                {
                    particle.Position = position;
                    particle.Velocity = new Vector2(
                        (_random.NextFloat() - 0.5f) * 100f,
                        (_random.NextFloat() - 0.5f) * 100f
                    );
                    particle.Color = Color.Red;
                    particle.Scale = new Vector2(_random.NextFloat() * 0.5f + 0.25f);
                    particle.LifeSpan = _random.NextFloat() * 2f + 1f;
                    particle.MaxLifeSpan = particle.LifeSpan;
                    particle.Rotation = _random.NextFloat() * MathHelper.TwoPi;
                    particle.Texture = _assetManager.GetTexture("BloodParticle") ?? CreateDefaultParticleTexture();
                }
            }
        }
        
        public void SpawnMuzzleFlash(Vector2 position)
        {
            for (int i = 0; i < 8; i++)
            {
                Particle particle = CreateParticle();
                if (particle != null)
                {
                    particle.Position = position;
                    particle.Velocity = new Vector2(
                        (_random.NextFloat() - 0.5f) * 50f,
                        (_random.NextFloat() - 0.5f) * 50f
                    );
                    particle.Color = Color.Yellow;
                    particle.Scale = new Vector2(_random.NextFloat() * 0.5f + 0.5f);
                    particle.LifeSpan = 0.2f;
                    particle.MaxLifeSpan = particle.LifeSpan;
                    particle.Texture = _assetManager.GetTexture("MuzzleFlash") ?? CreateDefaultParticleTexture();
                }
            }
        }
        
        public void SpawnExplosion(Vector2 position, int count = 20)
        {
            for (int i = 0; i < count; i++)
            {
                Particle particle = CreateParticle();
                if (particle != null)
                {
                    particle.Position = position;
                    float angle = _random.NextFloat() * MathHelper.TwoPi;
                    float speed = _random.NextFloat() * 200f + 50f;
                    particle.Velocity = new Vector2(
                        (float)Math.Cos(angle) * speed,
                        (float)Math.Sin(angle) * speed
                    );
                    particle.Color = new Color(
                        _random.Next(200, 256),
                        _random.Next(100, 150),
                        _random.Next(0, 50)
                    );
                    particle.Scale = new Vector2(_random.NextFloat() * 0.8f + 0.2f);
                    particle.LifeSpan = _random.NextFloat() * 1f + 0.5f;
                    particle.MaxLifeSpan = particle.LifeSpan;
                    particle.Texture = _assetManager.GetTexture("ExplosionParticle") ?? CreateDefaultParticleTexture();
                }
            }
        }
        
        public void SpawnImpactEffect(Vector2 position, int count = 6)
        {
            for (int i = 0; i < count; i++)
            {
                Particle particle = CreateParticle();
                if (particle != null)
                {
                    particle.Position = position;
                    float angle = _random.NextFloat() * MathHelper.TwoPi;
                    float speed = _random.NextFloat() * 80f + 20f;
                    particle.Velocity = new Vector2(
                        (float)Math.Cos(angle) * speed,
                        (float)Math.Sin(angle) * speed
                    );
                    particle.Color = Color.Gray;
                    particle.Scale = new Vector2(_random.NextFloat() * 0.3f + 0.1f);
                    particle.LifeSpan = _random.NextFloat() * 0.8f + 0.2f;
                    particle.MaxLifeSpan = particle.LifeSpan;
                    particle.Texture = _assetManager.GetTexture("ImpactParticle") ?? CreateDefaultParticleTexture();
                }
            }
        }
        
        private Texture2D CreateDefaultParticleTexture()
        {
            // In a real implementation, we'd create a simple 1x1 white texture
            // For now, we'll return null and handle it in the rendering code
            return null;
        }
        
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                Particle particle = _particles[i];
                
                if (particle.IsActive)
                {
                    particle.LifeSpan -= deltaTime;
                    
                    if (particle.LifeSpan <= 0)
                    {
                        particle.IsActive = false;
                    }
                    else
                    {
                        // Update particle position based on velocity
                        particle.Position += particle.Velocity * deltaTime;

                        // Apply gravity (optional)
                        particle.Velocity = new Vector2(particle.Velocity.X, particle.Velocity.Y + 200f * deltaTime); // Gravity
                        
                        // Fade out as life decreases
                        float lifeRatio = particle.LifeSpan / particle.MaxLifeSpan;
                        particle.Color = particle.Color * lifeRatio;
                    }
                }
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            _renderSystem.BeginDraw();
            
            for (int i = 0; i < _particles.Count; i++)
            {
                Particle particle = _particles[i];
                
                if (particle.IsActive && particle.Texture != null)
                {
                    _renderSystem.DrawSprite(
                        particle.Texture,
                        particle.Position,
                        null,
                        particle.Color,
                        particle.Rotation,
                        new Vector2(0.5f, 0.5f), // Origin at center
                        particle.Scale,
                        SpriteEffects.None,
                        0.5f // Layer depth for particles
                    );
                }
            }
            
            _renderSystem.EndDraw();
        }
    }
    
    // Extension method for Random to generate float between 0 and 1
    public static class RandomExtensions
    {
        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }
    }
}