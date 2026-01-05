using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    // Base Entity class
    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public List<Component> Components { get; set; }
        
        public Entity(int id, string name)
        {
            Id = id;
            Name = name;
            Position = Vector2.Zero;
            Size = new Vector2(32, 32); // Default size
            IsActive = true;
            Properties = new Dictionary<string, object>();
            Components = new List<Component>();
        }
        
        public void AddComponent(Component component)
        {
            Components.Add(component);
        }
        
        public T GetComponent<T>() where T : Component
        {
            foreach (var component in Components)
            {
                if (component is T tComponent)
                    return tComponent;
            }
            return null;
        }
        
        public bool HasComponent<T>() where T : Component
        {
            return GetComponent<T>() != null;
        }
    }
    
    // Base Component class
    public abstract class Component
    {
        public Entity Owner { get; set; }
        public bool IsActive { get; set; } = true;
        
        public Component(Entity owner)
        {
            Owner = owner;
        }
    }
    
    // Transform component
    public class TransformComponent : Component
    {
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; } = 0f;
        
        public TransformComponent(Entity owner) : base(owner)
        {
            Position = Vector2.Zero;
        }
    }
    
    // Sprite component
    public class SpriteComponent : Component
    {
        public string TextureName { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Color Tint { get; set; } = Color.White;
        public float LayerDepth { get; set; } = 0f;
        
        public SpriteComponent(Entity owner) : base(owner)
        {
            SourceRectangle = new Rectangle(0, 0, 0, 0);
        }
    }
    
    // Animation component
    public class AnimationComponent : Component
    {
        public string AnimationName { get; set; }
        public List<AnimationFrame> Frames { get; set; }
        public int CurrentFrameIndex { get; set; } = 0;
        public float FrameTimer { get; set; } = 0f;
        public bool IsPlaying { get; set; } = true;
        public bool IsLooping { get; set; } = true;
        
        public AnimationComponent(Entity owner) : base(owner)
        {
            Frames = new List<AnimationFrame>();
        }
    }
    
    public class AnimationFrame
    {
        public Rectangle SourceRectangle { get; set; }
        public float Duration { get; set; } // in seconds
        public Dictionary<string, object> Events { get; set; } // Events that happen on this frame
        
        public AnimationFrame(Rectangle sourceRect, float duration)
        {
            SourceRectangle = sourceRect;
            Duration = duration;
            Events = new Dictionary<string, object>();
        }
    }
    
    // Physics/Collision component
    public class PhysicsComponent : Component
    {
        public Vector2 Velocity { get; set; }
        public bool IsStatic { get; set; } = false;
        public Rectangle CollisionBox { get; set; }
        public bool IsCollidable { get; set; } = true;
        
        public PhysicsComponent(Entity owner) : base(owner)
        {
            Velocity = Vector2.Zero;
            CollisionBox = new Rectangle(0, 0, 32, 32);
        }
    }
    
    // Health component
    public class HealthComponent : Component
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public bool IsAlive { get; set; }
        
        public HealthComponent(Entity owner, int maxHealth) : base(owner)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            IsAlive = true;
        }
        
        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                IsAlive = false;
            }
        }
        
        public void Heal(int amount)
        {
            CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);
            if (CurrentHealth > 0)
                IsAlive = true;
        }
    }
    
    // AI component for enemies
    public class AIComponent : Component
    {
        public string BehaviorType { get; set; }
        public List<string> Abilities { get; set; }
        public float DetectionRange { get; set; }
        public float AttackRange { get; set; }
        public bool IsAggressive { get; set; }
        
        public AIComponent(Entity owner) : base(owner)
        {
            Abilities = new List<string>();
            DetectionRange = 100f;
            AttackRange = 50f;
            IsAggressive = true;
        }
    }
    
    // Player component
    public class PlayerComponent : Component
    {
        public int Score { get; set; }
        public int Lives { get; set; }
        public Dictionary<string, int> Inventory { get; set; }
        
        public PlayerComponent(Entity owner) : base(owner)
        {
            Score = 0;
            Lives = 3;
            Inventory = new Dictionary<string, int>();
        }
    }
    
    public class EntityManager : IUpdateable, IDrawable
    {
        private Dictionary<int, Entity> _entities;
        private int _nextEntityId;
        private List<int> _entitiesToRemove;
        
        public EntityManager()
        {
            _entities = new Dictionary<int, Entity>();
            _nextEntityId = 0;
            _entitiesToRemove = new List<int>();
        }
        
        public Entity CreateEntity(string name)
        {
            Entity entity = new Entity(_nextEntityId++, name);
            _entities[entity.Id] = entity;
            return entity;
        }
        
        public Entity GetEntity(int id)
        {
            if (_entities.ContainsKey(id))
                return _entities[id];
            return null;
        }
        
        public void RemoveEntity(int id)
        {
            if (_entities.ContainsKey(id))
            {
                _entitiesToRemove.Add(id);
            }
        }
        
        public void RemoveAllEntities()
        {
            _entities.Clear();
            _entitiesToRemove.Clear();
            _nextEntityId = 0;
        }
        
        public List<Entity> GetAllEntities()
        {
            return new List<Entity>(_entities.Values);
        }
        
        public List<Entity> GetEntitiesByName(string name)
        {
            List<Entity> result = new List<Entity>();
            foreach (var entity in _entities.Values)
            {
                if (entity.Name == name)
                    result.Add(entity);
            }
            return result;
        }
        
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Process entity removal
            foreach (int id in _entitiesToRemove)
            {
                _entities.Remove(id);
            }
            _entitiesToRemove.Clear();
            
            // Update all entities and their components
            foreach (var entity in _entities.Values)
            {
                if (!entity.IsActive) continue;
                
                foreach (var component in entity.Components)
                {
                    if (component is IUpdateable updateable)
                        updateable.Update(gameTime);
                }
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            // EntityManager doesn't draw directly, but systems that use entities do
        }
        
        public void ProcessCollisions()
        {
            var entities = GetAllEntities();
            
            for (int i = 0; i < entities.Count; i++)
            {
                for (int j = i + 1; j < entities.Count; j++)
                {
                    var entity1 = entities[i];
                    var entity2 = entities[j];
                    
                    if (!entity1.IsActive || !entity2.IsActive) continue;
                    
                    var physics1 = entity1.GetComponent<PhysicsComponent>();
                    var physics2 = entity2.GetComponent<PhysicsComponent>();
                    
                    if (physics1 != null && physics2 != null && 
                        physics1.IsCollidable && physics2.IsCollidable)
                    {
                        var rect1 = new Rectangle(
                            (int)entity1.Position.X + physics1.CollisionBox.X,
                            (int)entity1.Position.Y + physics1.CollisionBox.Y,
                            physics1.CollisionBox.Width,
                            physics1.CollisionBox.Height
                        );
                        
                        var rect2 = new Rectangle(
                            (int)entity2.Position.X + physics2.CollisionBox.X,
                            (int)entity2.Position.Y + physics2.CollisionBox.Y,
                            physics2.CollisionBox.Width,
                            physics2.CollisionBox.Height
                        );
                        
                        if (rect1.Intersects(rect2))
                        {
                            // Handle collision
                            OnCollision(entity1, entity2);
                        }
                    }
                }
            }
        }
        
        private void OnCollision(Entity entity1, Entity entity2)
        {
            // Collision handling logic would go here
        }
    }
}