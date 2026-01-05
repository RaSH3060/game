using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class EnemySystem : IUpdateable, IDrawable
    {
        private EntityManager _entityManager;
        private Random _random;
        
        // Enemy abilities
        private Dictionary<string, Action<Entity>> _enemyAbilities;
        
        // Enemy behaviors
        private Dictionary<string, Func<Entity, GameTime, Vector2>> _enemyBehaviors;
        
        // Boss abilities
        private Dictionary<string, Action<Entity>> _bossAbilities;
        
        // Boss behaviors
        private Dictionary<string, Func<Entity, GameTime, Vector2>> _bossBehaviors;
        
        public EnemySystem()
        {
            _random = new Random();
            _enemyAbilities = new Dictionary<string, Action<Entity>>();
            _enemyBehaviors = new Dictionary<string, Func<Entity, GameTime, Vector2>>();
            _bossAbilities = new Dictionary<string, Action<Entity>>();
            _bossBehaviors = new Dictionary<string, Func<Entity, GameTime, Vector2>>();
            
            InitializeAbilitiesAndBehaviors();
        }
        
        public void Initialize(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }
        
        private void InitializeAbilitiesAndBehaviors()
        {
            // Initialize 10 enemy abilities
            _enemyAbilities["RangedAttack"] = (entity) => ExecuteRangedAttack(entity);
            _enemyAbilities["MeleeAttack"] = (entity) => ExecuteMeleeAttack(entity);
            _enemyAbilities["Teleport"] = (entity) => ExecuteTeleport(entity);
            _enemyAbilities["Summon"] = (entity) => ExecuteSummon(entity);
            _enemyAbilities["Heal"] = (entity) => ExecuteHeal(entity);
            _enemyAbilities["Invisibility"] = (entity) => ExecuteInvisibility(entity);
            _enemyAbilities["AreaDamage"] = (entity) => ExecuteAreaDamage(entity);
            _enemyAbilities["SpeedBoost"] = (entity) => ExecuteSpeedBoost(entity);
            _enemyAbilities["Shield"] = (entity) => ExecuteShield(entity);
            _enemyAbilities["Stun"] = (entity) => ExecuteStun(entity);
            
            // Initialize 5 enemy behaviors
            _enemyBehaviors["Patrol"] = (entity, gameTime) => ExecutePatrolBehavior(entity, gameTime);
            _enemyBehaviors["Aggressive"] = (entity, gameTime) => ExecuteAggressiveBehavior(entity, gameTime);
            _enemyBehaviors["Defensive"] = (entity, gameTime) => ExecuteDefensiveBehavior(entity, gameTime);
            _enemyBehaviors["Ambush"] = (entity, gameTime) => ExecuteAmbushBehavior(entity, gameTime);
            _enemyBehaviors["Flee"] = (entity, gameTime) => ExecuteFleeBehavior(entity, gameTime);
            
            // Initialize 6 boss abilities
            _bossAbilities["PhaseShift"] = (entity) => ExecutePhaseShift(entity);
            _bossAbilities["RageMode"] = (entity) => ExecuteRageMode(entity);
            _bossAbilities["SummonMinions"] = (entity) => ExecuteSummonMinions(entity);
            _bossAbilities["LaserBeam"] = (entity) => ExecuteLaserBeam(entity);
            _bossAbilities["Earthquake"] = (entity) => ExecuteEarthquake(entity);
            _bossAbilities["Nuke"] = (entity) => ExecuteNuke(entity);
            
            // Initialize 3 boss behaviors
            _bossBehaviors["Pattern1"] = (entity, gameTime) => ExecutePattern1Behavior(entity, gameTime);
            _bossBehaviors["Pattern2"] = (entity, gameTime) => ExecutePattern2Behavior(entity, gameTime);
            _bossBehaviors["PhaseTransition"] = (entity, gameTime) => ExecutePhaseTransitionBehavior(entity, gameTime);
        }
        
        public void ExecuteAbility(string abilityName, Entity entity)
        {
            if (_enemyAbilities.ContainsKey(abilityName))
            {
                _enemyAbilities[abilityName](entity);
            }
            else if (_bossAbilities.ContainsKey(abilityName))
            {
                _bossAbilities[abilityName](entity);
            }
        }
        
        public Vector2 ExecuteBehavior(string behaviorName, Entity entity, GameTime gameTime)
        {
            if (_enemyBehaviors.ContainsKey(behaviorName))
            {
                return _enemyBehaviors[behaviorName](entity, gameTime);
            }
            else if (_bossBehaviors.ContainsKey(behaviorName))
            {
                return _bossBehaviors[behaviorName](entity, gameTime);
            }
            return Vector2.Zero; // Default behavior: no movement
        }
        
        // Enemy Ability Implementations
        private void ExecuteRangedAttack(Entity entity)
        {
            // Create projectile entity
            var projectile = _entityManager.CreateEntity("EnemyProjectile");
            var transform = entity.GetComponent<TransformComponent>();
            if (transform != null)
            {
                projectile.Position = transform.Position;
            }
            
            // Add projectile components
            var physics = new PhysicsComponent(projectile);
            physics.Velocity = new Vector2(100f, 0f); // Direction towards player
            projectile.AddComponent(physics);
            
            var sprite = new SpriteComponent(projectile);
            sprite.TextureName = "EnemyProjectile";
            projectile.AddComponent(sprite);
        }
        
        private void ExecuteMeleeAttack(Entity entity)
        {
            // Melee attack logic - check for nearby player and apply damage
            var aiComponent = entity.GetComponent<AIComponent>();
            if (aiComponent != null)
            {
                // Check if player is within attack range
                // Apply damage to player if in range
            }
        }
        
        private void ExecuteTeleport(Entity entity)
        {
            var transform = entity.GetComponent<TransformComponent>();
            if (transform != null)
            {
                // Randomly teleport to a nearby location
                transform.Position += new Vector2(
                    (_random.NextFloat() - 0.5f) * 200f,
                    (_random.NextFloat() - 0.5f) * 200f
                );
            }
        }
        
        private void ExecuteSummon(Entity entity)
        {
            // Create a new enemy near this entity
            var summon = _entityManager.CreateEntity("SummonedEnemy");
            var transform = entity.GetComponent<TransformComponent>();
            if (transform != null)
            {
                summon.Position = transform.Position + new Vector2(
                    (_random.NextFloat() - 0.5f) * 50f,
                    (_random.NextFloat() - 0.5f) * 50f
                );
            }
            
            var ai = new AIComponent(summon);
            ai.BehaviorType = "Aggressive";
            summon.AddComponent(ai);
        }
        
        private void ExecuteHeal(Entity entity)
        {
            var health = entity.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.Heal(health.MaxHealth / 4); // Heal 25% of max health
            }
        }
        
        private void ExecuteInvisibility(Entity entity)
        {
            // Make the entity temporarily invisible
            var sprite = entity.GetComponent<SpriteComponent>();
            if (sprite != null)
            {
                sprite.Tint = Color.Transparent;
            }
            
            // Schedule to become visible again after a delay
            // This would use a timer system in a full implementation
        }
        
        private void ExecuteAreaDamage(Entity entity)
        {
            // Create an area effect that damages nearby entities
            var transform = entity.GetComponent<TransformComponent>();
            if (transform != null)
            {
                // Damage all entities within a radius
                var nearbyEntities = GetEntitiesInRadius(transform.Position, 100f);
                foreach (var nearbyEntity in nearbyEntities)
                {
                    if (nearbyEntity != entity) // Don't damage self
                    {
                        var health = nearbyEntity.GetComponent<HealthComponent>();
                        if (health != null)
                        {
                            health.TakeDamage(20); // Damage value from config
                        }
                    }
                }
            }
        }
        
        private void ExecuteSpeedBoost(Entity entity)
        {
            var physics = entity.GetComponent<PhysicsComponent>();
            if (physics != null)
            {
                // Temporarily increase movement speed
                physics.Velocity *= 2f;
            }
        }
        
        private void ExecuteShield(Entity entity)
        {
            // Add temporary shield to the entity
            var health = entity.GetComponent<HealthComponent>();
            if (health != null)
            {
                // Add shield value that absorbs damage
                // This would require extending the HealthComponent to support shields
            }
        }
        
        private void ExecuteStun(Entity entity)
        {
            // Stun nearby enemies
            var transform = entity.GetComponent<TransformComponent>();
            if (transform != null)
            {
                var nearbyEntities = GetEntitiesInRadius(transform.Position, 80f);
                foreach (var nearbyEntity in nearbyEntities)
                {
                    if (nearbyEntity != entity && nearbyEntity.HasComponent<AIComponent>())
                    {
                        // Apply stun effect to nearby entity
                        // This would require an effects system
                    }
                }
            }
        }
        
        // Boss Ability Implementations
        private void ExecutePhaseShift(Entity entity)
        {
            // Make the boss temporarily invulnerable and change appearance
            var sprite = entity.GetComponent<SpriteComponent>();
            if (sprite != null)
            {
                sprite.Tint = Color.Purple;
            }
            
            // Set temporary invulnerability
            // This would require extending the HealthComponent
        }
        
        private void ExecuteRageMode(Entity entity)
        {
            // Increase all stats dramatically
            var aiComponent = entity.GetComponent<AIComponent>();
            if (aiComponent != null)
            {
                aiComponent.AttackRange *= 2f;
                aiComponent.DetectionRange *= 2f;
            }
            
            var health = entity.GetComponent<HealthComponent>();
            if (health != null)
            {
                // Increase damage output
            }
        }
        
        private void ExecuteSummonMinions(Entity entity)
        {
            // Summon multiple enemies
            for (int i = 0; i < 3; i++)
            {
                var minion = _entityManager.CreateEntity("BossMinion");
                var transform = entity.GetComponent<TransformComponent>();
                if (transform != null)
                {
                    minion.Position = transform.Position + new Vector2(
                        (_random.NextFloat() - 0.5f) * 100f,
                        (_random.NextFloat() - 0.5f) * 100f
                    );
                }
                
                var ai = new AIComponent(minion);
                ai.BehaviorType = "Aggressive";
                minion.AddComponent(ai);
            }
        }
        
        private void ExecuteLaserBeam(Entity entity)
        {
            // Create a laser beam effect
            var transform = entity.GetComponent<TransformComponent>();
            if (transform != null)
            {
                // Create laser entity or particle effect
                // This would require a special laser entity or particle system
            }
        }
        
        private void ExecuteEarthquake(Entity entity)
        {
            // Cause screen shake and damage to nearby entities
            var screenShake = GetSystem<ScreenShakeSystem>("ScreenShake");
            if (screenShake != null)
            {
                screenShake.TriggerShake(8f, 1f);
            }
            
            var transform = entity.GetComponent<TransformComponent>();
            if (transform != null)
            {
                var nearbyEntities = GetEntitiesInRadius(transform.Position, 150f);
                foreach (var nearbyEntity in nearbyEntities)
                {
                    if (nearbyEntity.HasComponent<HealthComponent>())
                    {
                        var health = nearbyEntity.GetComponent<HealthComponent>();
                        if (health != null)
                        {
                            health.TakeDamage(10);
                        }
                    }
                }
            }
        }
        
        private void ExecuteNuke(Entity entity)
        {
            // Create a large explosion effect
            var particleSystem = GetSystem<ParticleSystem>("Particles");
            if (particleSystem != null)
            {
                var transform = entity.GetComponent<TransformComponent>();
                if (transform != null)
                {
                    particleSystem.SpawnExplosion(transform.Position, 50);
                }
            }
            
            // Damage all entities in a large radius
            var transform2 = entity.GetComponent<TransformComponent>();
            if (transform2 != null)
            {
                var allEntities = _entityManager.GetAllEntities();
                foreach (var otherEntity in allEntities)
                {
                    if (otherEntity != entity && otherEntity.HasComponent<HealthComponent>())
                    {
                        var distance = Vector2.Distance(transform2.Position, otherEntity.Position);
                        if (distance < 200f) // Nuke radius
                        {
                            var health = otherEntity.GetComponent<HealthComponent>();
                            if (health != null)
                            {
                                int damage = (int)(100 * (1f - distance / 200f)); // More damage closer to center
                                health.TakeDamage(damage);
                            }
                        }
                    }
                }
            }
        }
        
        // Behavior Implementations
        private Vector2 ExecutePatrolBehavior(Entity entity, GameTime gameTime)
        {
            var aiComponent = entity.GetComponent<AIComponent>();
            if (aiComponent != null)
            {
                // Simple patrol: move in a direction until hitting a wall, then turn around
                return new Vector2(50f, 0f); // Move right at 50 units per second
            }
            return Vector2.Zero;
        }
        
        private Vector2 ExecuteAggressiveBehavior(Entity entity, GameTime gameTime)
        {
            var aiComponent = entity.GetComponent<AIComponent>();
            if (aiComponent != null)
            {
                // Move towards player aggressively
                // This would require player position tracking
                return new Vector2(100f, 0f); // Move faster towards player
            }
            return Vector2.Zero;
        }
        
        private Vector2 ExecuteDefensiveBehavior(Entity entity, GameTime gameTime)
        {
            var aiComponent = entity.GetComponent<AIComponent>();
            if (aiComponent != null)
            {
                // Stay in position or move minimally
                return Vector2.Zero;
            }
            return Vector2.Zero;
        }
        
        private Vector2 ExecuteAmbushBehavior(Entity entity, GameTime gameTime)
        {
            var aiComponent = entity.GetComponent<AIComponent>();
            if (aiComponent != null)
            {
                // Stay still until player is nearby, then attack
                return Vector2.Zero; // Wait for player
            }
            return Vector2.Zero;
        }
        
        private Vector2 ExecuteFleeBehavior(Entity entity, GameTime gameTime)
        {
            var aiComponent = entity.GetComponent<AIComponent>();
            if (aiComponent != null)
            {
                // Move away from player
                return new Vector2(-75f, 0f); // Move left away from player
            }
            return Vector2.Zero;
        }
        
        private Vector2 ExecutePattern1Behavior(Entity entity, GameTime gameTime)
        {
            // Complex boss pattern 1
            return new Vector2(25f, 0f); // Move in a specific pattern
        }
        
        private Vector2 ExecutePattern2Behavior(Entity entity, GameTime gameTime)
        {
            // Complex boss pattern 2
            return new Vector2(0f, 30f); // Move in a different pattern
        }
        
        private Vector2 ExecutePhaseTransitionBehavior(Entity entity, GameTime gameTime)
        {
            // Behavior for when boss transitions to a new phase
            var health = entity.GetComponent<HealthComponent>();
            if (health != null)
            {
                // Change behavior based on health percentage
                float healthPercent = (float)health.CurrentHealth / health.MaxHealth;
                if (healthPercent < 0.3f)
                {
                    // Enter rage mode when below 30% health
                    ExecuteRageMode(entity);
                }
            }
            return Vector2.Zero;
        }
        
        // Helper methods
        private List<Entity> GetEntitiesInRadius(Vector2 center, float radius)
        {
            List<Entity> result = new List<Entity>();
            var allEntities = _entityManager.GetAllEntities();
            
            foreach (var entity in allEntities)
            {
                float distance = Vector2.Distance(center, entity.Position);
                if (distance <= radius)
                {
                    result.Add(entity);
                }
            }
            
            return result;
        }
        
        private T GetSystem<T>(string systemName) where T : class
        {
            // This would get the system from the main game's system dictionary
            // For now, return null - in a real implementation this would be connected to the main game
            return null;
        }
        
        public void Update(GameTime gameTime)
        {
            // Update all enemy entities with their behaviors and abilities
            var allEntities = _entityManager.GetAllEntities();
            
            foreach (var entity in allEntities)
            {
                var aiComponent = entity.GetComponent<AIComponent>();
                if (aiComponent != null)
                {
                    // Execute behavior
                    Vector2 movement = ExecuteBehavior(aiComponent.BehaviorType, entity, gameTime);
                    
                    // Apply movement to physics component if it exists
                    var physics = entity.GetComponent<PhysicsComponent>();
                    if (physics != null)
                    {
                        physics.Velocity = movement;
                    }
                }
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            // Enemy system doesn't draw directly
        }
    }
}