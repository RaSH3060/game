using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameEngine
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        // Engine components
        private Dictionary<string, object> _systems;
        private bool _isEditorMode = true; // Editor mode by default
        private string _currentLevel = "";
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Initialize engine systems
            _systems = new Dictionary<string, object>();
        }

        protected override void Initialize()
        {
            // Initialize configuration system first
            var configSystem = new ConfigSystem();
            configSystem.LoadConfigFile("sample_config.cg"); // Load the sample config file
            _systems["Config"] = configSystem;

            // Load configuration and set window settings
            Vector2 resolution = configSystem.GetResolution();
            _graphics.PreferredBackBufferWidth = (int)resolution.X;
            _graphics.PreferredBackBufferHeight = (int)resolution.Y;
            _graphics.IsFullScreen = configSystem.IsFullscreen();
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Initialize engine systems here
            InitializeEngineSystems();
        }

        private void InitializeEngineSystems()
        {
            // Initialize asset management system
            var assetManager = new AssetManager();
            assetManager.Initialize(GraphicsDevice, Content);
            assetManager.LoadFont("DefaultFont", "DefaultFont");
            _systems["Assets"] = assetManager;
            
            // Initialize entity system
            var entityManager = new EntityManager();
            _systems["Entities"] = entityManager;
            
            // Initialize rendering system
            var renderSystem = new RenderSystem(_graphics, GraphicsDevice, _spriteBatch);
            _systems["Renderer"] = renderSystem;

            // Initialize input system
            _systems["Input"] = new InputSystem(this.Window);

            // Initialize audio system
            _systems["Audio"] = new AudioSystem(Content);
            
            // Initialize particle system
            var particleSystem = new ParticleSystem();
            particleSystem.Initialize(renderSystem, assetManager);
            _systems["Particles"] = particleSystem;
            
            // Initialize screen shake system
            _systems["ScreenShake"] = new ScreenShakeSystem();
            
            // Initialize enemy system
            var enemySystem = new EnemySystem();
            enemySystem.Initialize(entityManager);
            _systems["Enemies"] = enemySystem;
            
            // Initialize level editor if enabled
            if (_isEditorMode)
            {
                var levelEditor = new LevelEditor();
                levelEditor.Initialize(
                    renderSystem, 
                    (InputSystem)_systems["Input"],
                    entityManager,
                    assetManager,
                    (ConfigSystem)_systems["Config"]
                );
                _systems["Editor"] = levelEditor;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update engine systems
            foreach (var system in _systems.Values)
            {
                if (system is IUpdateable updateable)
                    updateable.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSlateGray);

            // Draw engine systems
            foreach (var system in _systems.Values)
            {
                if (system is IDrawable drawable)
                    drawable.Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
    
    // Interface for updateable systems
    public interface IUpdateable
    {
        void Update(GameTime gameTime);
    }
    
    // Interface for drawable systems
    public interface IDrawable
    {
        void Draw(GameTime gameTime);
    }
}