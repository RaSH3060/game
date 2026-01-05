using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEngine
{
    public class LevelEditor : IUpdateable, IDrawable
    {
        private enum EditorMode
        {
            MainMenu,
            LevelEditor,
            ObjectPlacement,
            PropertyInspector,
            PreviewMode
        }
        
        private EditorMode _currentMode;
        private RenderSystem _renderSystem;
        private InputSystem _inputSystem;
        private EntityManager _entityManager;
        private AssetManager _assetManager;
        private ConfigSystem _configSystem;
        
        // UI elements
        private Dictionary<string, UIElement> _uiElements;
        private List<EditorTool> _tools;
        private EditorTool _currentTool;
        
        // Level data
        private string _currentLevelPath;
        private Dictionary<Vector2, Tile> _tiles;
        private List<Entity> _placedObjects;
        private Vector2 _cameraPosition;
        private float _zoomLevel;
        
        // Object placement
        private string _selectedObjectType;
        private bool _isPlacingObject;
        
        public LevelEditor()
        {
            _currentMode = EditorMode.MainMenu;
            _uiElements = new Dictionary<string, UIElement>();
            _tools = new List<EditorTool>();
            _tiles = new Dictionary<Vector2, Tile>();
            _placedObjects = new List<Entity>();
            _cameraPosition = Vector2.Zero;
            _zoomLevel = 1.0f;
            
            InitializeTools();
            InitializeUI();
        }
        
        public void Initialize(RenderSystem renderSystem, InputSystem inputSystem, 
                              EntityManager entityManager, AssetManager assetManager, 
                              ConfigSystem configSystem)
        {
            _renderSystem = renderSystem;
            _inputSystem = inputSystem;
            _entityManager = entityManager;
            _assetManager = assetManager;
            _configSystem = configSystem;
        }
        
        private void InitializeTools()
        {
            _tools.Add(new EditorTool { Name = "Select", Icon = "select_icon" });
            _tools.Add(new EditorTool { Name = "PlaceWall", Icon = "wall_icon" });
            _tools.Add(new EditorTool { Name = "PlaceFloor", Icon = "floor_icon" });
            _tools.Add(new EditorTool { Name = "PlaceObject", Icon = "object_icon" });
            _tools.Add(new EditorTool { Name = "PlaceTrigger", Icon = "trigger_icon" });
            _tools.Add(new EditorTool { Name = "Erase", Icon = "erase_icon" });
        }
        
        private void InitializeUI()
        {
            // Main menu buttons
            _uiElements["NewProjectBtn"] = new UIButton(new Rectangle(100, 100, 200, 50), "New Project");
            _uiElements["OpenProjectBtn"] = new UIButton(new Rectangle(100, 170, 200, 50), "Open Project");
            _uiElements["SettingsBtn"] = new UIButton(new Rectangle(100, 240, 200, 50), "Settings");
            _uiElements["PlayGameBtn"] = new UIButton(new Rectangle(100, 310, 200, 50), "Play Game");
            
            // Editor toolbar
            _uiElements["Toolbar"] = new UIContainer(new Rectangle(0, 0, 80, 600));
            _uiElements["SaveBtn"] = new UIButton(new Rectangle(10, 10, 60, 30), "Save");
            _uiElements["LoadBtn"] = new UIButton(new Rectangle(10, 50, 60, 30), "Load");
            _uiElements["PreviewBtn"] = new UIButton(new Rectangle(10, 90, 60, 30), "Preview");
            _uiElements["ExitBtn"] = new UIButton(new Rectangle(10, 130, 60, 30), "Exit");
        }
        
        public void Update(GameTime gameTime)
        {
            // Handle global key presses (like escape to return to main menu)
            if (_inputSystem.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                _currentMode = EditorMode.MainMenu;
            }
            
            switch (_currentMode)
            {
                case EditorMode.MainMenu:
                    UpdateMainMenu();
                    break;
                case EditorMode.LevelEditor:
                    UpdateLevelEditor();
                    break;
                case EditorMode.PreviewMode:
                    // In preview mode, update normally but don't allow editing
                    UpdatePreviewMode();
                    break;
            }
        }
        
        private void UpdateMainMenu()
        {
            foreach (var uiElement in _uiElements.Values)
            {
                if (uiElement is UIButton button)
                {
                    if (IsMouseOver(button.Bounds) && _inputSystem.IsMouseButtonPressed(MouseButton.LeftButton))
                    {
                        HandleMainMenuButton(button.Text);
                    }
                }
            }
        }
        
        private void UpdateLevelEditor()
        {
            // Handle camera movement
            HandleCameraMovement();
            
            // Handle tool selection
            HandleToolSelection();
            
            // Handle object placement
            HandleObjectPlacement();
            
            // Handle UI interactions
            HandleUIInteractions();
        }
        
        private void UpdatePreviewMode()
        {
            // Similar to level editor but without placement abilities
            HandleCameraMovement();
        }
        
        private void HandleCameraMovement()
        {
            Vector2 mouseDelta = _inputSystem.MouseDelta;
            if (_inputSystem.IsMouseButtonDown(MouseButton.RightButton))
            {
                _cameraPosition -= mouseDelta / _zoomLevel;
            }
            
            // Zoom with mouse wheel
            int mouseWheelDelta = _inputSystem.MouseWheelDelta;
            if (mouseWheelDelta != 0)
            {
                float zoomFactor = mouseWheelDelta > 0 ? 1.1f : 0.9f;
                _zoomLevel *= zoomFactor;
                
                // Limit zoom range
                _zoomLevel = MathHelper.Clamp(_zoomLevel, 0.1f, 5.0f);
            }
        }
        
        private void HandleToolSelection()
        {
            // Tool selection logic - number keys 1-6 for different tools
            if (_inputSystem.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D1))
            {
                _currentTool = _tools[0]; // Select tool
            }
            else if (_inputSystem.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D2))
            {
                _currentTool = _tools[1]; // PlaceWall tool
                _selectedObjectType = "Wall";
                _isPlacingObject = true;
            }
            else if (_inputSystem.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D3))
            {
                _currentTool = _tools[2]; // PlaceFloor tool
                _selectedObjectType = "Floor";
                _isPlacingObject = true;
            }
            else if (_inputSystem.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D4))
            {
                _currentTool = _tools[3]; // PlaceObject tool
                _selectedObjectType = "Object";
                _isPlacingObject = true;
            }
            else if (_inputSystem.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D5))
            {
                _currentTool = _tools[4]; // PlaceTrigger tool
                _selectedObjectType = "Trigger";
                _isPlacingObject = true;
            }
            else if (_inputSystem.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.D6))
            {
                _currentTool = _tools[5]; // Erase tool
                _selectedObjectType = null;
                _isPlacingObject = false;
            }
        }
        
        private void HandleObjectPlacement()
        {
            if (_inputSystem.IsMouseButtonPressed(MouseButton.LeftButton) && _selectedObjectType != null)
            {
                Vector2 worldPos = ScreenToWorld(_inputSystem.MousePosition);
                PlaceObject(_selectedObjectType, worldPos);
            }
        }
        
        private void HandleUIInteractions()
        {
            foreach (var uiElement in _uiElements.Values)
            {
                if (uiElement is UIButton button)
                {
                    if (IsMouseOver(button.Bounds) && _inputSystem.IsMouseButtonPressed(MouseButton.LeftButton))
                    {
                        HandleEditorButton(button.Text);
                    }
                }
            }
        }
        
        private void HandleMainMenuButton(string buttonText)
        {
            switch (buttonText)
            {
                case "New Project":
                    CreateNewLevel();
                    _currentMode = EditorMode.LevelEditor;
                    break;
                case "Open Project":
                    // Open file dialog to select level file
                    _currentMode = EditorMode.LevelEditor;
                    break;
                case "Settings":
                    // Show settings menu
                    break;
                case "Play Game":
                    _currentMode = EditorMode.PreviewMode;
                    break;
            }
        }
        
        private void HandleEditorButton(string buttonText)
        {
            switch (buttonText)
            {
                case "Save":
                    SaveLevel();
                    break;
                case "Load":
                    LoadLevel();
                    break;
                case "Preview":
                    _currentMode = EditorMode.PreviewMode;
                    break;
                case "Exit":
                    _currentMode = EditorMode.MainMenu;
                    break;
            }
        }
        
        private Vector2 ScreenToWorld(Vector2 screenPos)
        {
            // Convert screen coordinates to world coordinates based on camera and zoom
            Vector2 worldPos = screenPos;
            worldPos.X = (worldPos.X / _zoomLevel) + _cameraPosition.X;
            worldPos.Y = (worldPos.Y / _zoomLevel) + _cameraPosition.Y;
            return worldPos;
        }
        
        private bool IsMouseOver(Rectangle bounds)
        {
            Vector2 mousePos = _inputSystem.MousePosition;
            return bounds.Contains((int)mousePos.X, (int)mousePos.Y);
        }
        
        private void PlaceObject(string objectType, Vector2 position)
        {
            Entity entity = _entityManager.CreateEntity(objectType);
            entity.Position = position;
            
            // Add default components based on object type
            switch (objectType)
            {
                case "PlayerSpawn":
                    // Add player-specific components
                    break;
                case "Enemy":
                    // Add enemy-specific components
                    var aiComponent = new AIComponent(entity);
                    aiComponent.BehaviorType = "Patrol";
                    entity.AddComponent(aiComponent);
                    break;
                case "Wall":
                    // Add wall-specific components
                    var physicsComponent = new PhysicsComponent(entity);
                    physicsComponent.IsCollidable = true;
                    entity.AddComponent(physicsComponent);
                    break;
                case "Trigger":
                    // Add trigger-specific components
                    break;
            }
            
            _placedObjects.Add(entity);
        }
        
        private void CreateNewLevel()
        {
            _tiles.Clear();
            _placedObjects.Clear();
            _currentLevelPath = "";
        }
        
        private void LoadLevel()
        {
            // Load level from .cg file
            if (string.IsNullOrEmpty(_currentLevelPath))
                return;
                
            if (!File.Exists(_currentLevelPath))
                return;
                
            // Parse the .cg file and populate the level
            string[] lines = File.ReadAllLines(_currentLevelPath);
            
            foreach (string line in lines)
            {
                // Parse level data from .cg file
                // This would implement the specific .cg format parsing
            }
        }
        
        private void SaveLevel()
        {
            // Save level to .cg file
            List<string> lines = new List<string>();
            
            // Add level header
            lines.Add("[Level]");
            lines.Add($"Resolution={_configSystem.GetConfigValue("Resolution.Width", "1024")},{_configSystem.GetConfigValue("Resolution.Height", "768")}");
            
            // Add tiles
            lines.Add("[Tiles]");
            foreach (var tile in _tiles)
            {
                lines.Add($"{tile.Key.X},{tile.Key.Y}={tile.Value.Type}");
            }
            
            // Add placed objects
            lines.Add("[Objects]");
            foreach (var entity in _placedObjects)
            {
                lines.Add($"{entity.Name}:{entity.Position.X},{entity.Position.Y}");
            }
            
            // Add collisions
            lines.Add("[Collisions]");
            foreach (var entity in _placedObjects)
            {
                var physicsComponent = entity.GetComponent<PhysicsComponent>();
                if (physicsComponent != null && physicsComponent.IsCollidable)
                {
                    lines.Add($"Collision:{entity.Position.X},{entity.Position.Y},{physicsComponent.CollisionBox.Width},{physicsComponent.CollisionBox.Height}");
                }
            }
            
            if (!string.IsNullOrEmpty(_currentLevelPath))
            {
                File.WriteAllLines(_currentLevelPath, lines.ToArray());
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            switch (_currentMode)
            {
                case EditorMode.MainMenu:
                    DrawMainMenu();
                    break;
                case EditorMode.LevelEditor:
                case EditorMode.PreviewMode:
                    DrawLevelEditor();
                    break;
            }
        }
        
        private void DrawMainMenu()
        {
            _renderSystem.BeginDraw();

            // Draw main menu UI
            foreach (var uiElement in _uiElements.Values)
            {
                if (uiElement is UIButton button)
                {
                    // Draw button background
                    _renderSystem.FillRectangle(button.Bounds, Color.Gray);
                    _renderSystem.DrawRectangle(button.Bounds, Color.DarkGray, 2);

                    // Draw button text using a simple character-based approach
                    // until proper font rendering is available
                    DrawButtonText(button);
                }
            }

            _renderSystem.EndDraw();
        }
        
        private void DrawButtonText(UIButton button)
        {
            // Get the default font from asset manager
            var font = _assetManager.GetFont("DefaultFont");
            if (font != null)
            {
                // Calculate text position to center it in the button
                Vector2 textSize = font.MeasureString(button.Text);
                Vector2 textPosition = new Vector2(
                    button.Bounds.X + (button.Bounds.Width - textSize.X) / 2,
                    button.Bounds.Y + (button.Bounds.Height - textSize.Y) / 2
                );

                // Draw the text
                _renderSystem.DrawString(font, button.Text, textPosition, button.Color);
            }
            else
            {
                // Fallback: simple character-based rendering if font is not available
                int charWidth = 8;
                int charHeight = 8;
                int x = button.Bounds.X + 5;
                int y = button.Bounds.Y + (button.Bounds.Height - charHeight) / 2;

                foreach (char c in button.Text)
                {
                    // Draw a simple rectangle for each character as fallback
                    _renderSystem.FillRectangle(new Rectangle(x, y, charWidth, charHeight), Color.Black);
                    x += charWidth;
                }
            }
        }
        
        private void DrawLevelEditor()
        {
            _renderSystem.BeginDraw();

            // Draw grid
            DrawGrid();

            // Draw placed objects
            foreach (var entity in _placedObjects)
            {
                // Draw entity based on its type and components
                var spriteComponent = entity.GetComponent<SpriteComponent>();
                if (spriteComponent != null)
                {
                    Texture2D texture = _assetManager.GetTexture(spriteComponent.TextureName);
                    if (texture != null)
                    {
                        _renderSystem.DrawSprite(
                            texture,
                            entity.Position,
                            spriteComponent.SourceRectangle,
                            spriteComponent.Tint,
                            0f,
                            null,
                            null,
                            SpriteEffects.None,
                            spriteComponent.LayerDepth
                        );
                    }
                }
            }

            // Draw UI elements
            foreach (var uiElement in _uiElements.Values)
            {
                if (uiElement is UIButton button)
                {
                    _renderSystem.FillRectangle(button.Bounds, Color.Gray);
                    _renderSystem.DrawRectangle(button.Bounds, Color.DarkGray, 2);
                    
                    // Draw button text
                    DrawButtonText(button);
                }
                else if (uiElement is UIContainer container)
                {
                    _renderSystem.FillRectangle(container.Bounds, new Color(50, 50, 50, 200)); // Semi-transparent dark gray
                    _renderSystem.DrawRectangle(container.Bounds, Color.LightGray, 1);
                }
            }

            // Draw selection indicator if placing object
            if (_isPlacingObject && _selectedObjectType != null)
            {
                // Draw ghost/object placement indicator
            }

            _renderSystem.EndDraw();
        }
        
        private void DrawGrid()
        {
            // Draw grid lines for level editing
            int gridSize = 32; // 32 pixels per grid cell
            int screenWidth = _renderSystem.GraphicsDevice.Viewport.Width;
            int screenHeight = _renderSystem.GraphicsDevice.Viewport.Height;
            
            // Calculate grid bounds based on camera position
            int startX = (int)(_cameraPosition.X / gridSize) * gridSize;
            int startY = (int)(_cameraPosition.Y / gridSize) * gridSize;
            int endX = startX + screenWidth;
            int endY = startY + screenHeight;
            
            // Draw vertical grid lines
            for (int x = startX; x <= endX; x += gridSize)
            {
                // Calculate screen position of grid line
                float screenX = (x - _cameraPosition.X) * _zoomLevel;
                _renderSystem.DrawRectangle(new Rectangle((int)screenX, 0, 1, screenHeight), Color.LightGray, 1);
            }
            
            // Draw horizontal grid lines
            for (int y = startY; y <= endY; y += gridSize)
            {
                // Calculate screen position of grid line
                float screenY = (y - _cameraPosition.Y) * _zoomLevel;
                _renderSystem.DrawRectangle(new Rectangle(0, (int)screenY, screenWidth, 1), Color.LightGray, 1);
            }
        }
    }
    
    // Supporting classes for the level editor
    public class UIElement
    {
        public Rectangle Bounds { get; set; }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
    }
    
    public class UIButton : UIElement
    {
        public string Text { get; set; }
        public Color Color { get; set; } = Color.White;
        
        public UIButton(Rectangle bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }
    }
    
    public class UIContainer : UIElement
    {
        public List<UIElement> Children { get; set; }
        
        public UIContainer(Rectangle bounds)
        {
            Bounds = bounds;
            Children = new List<UIElement>();
        }
    }
    
    public class EditorTool
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }
    
    public class Tile
    {
        public string Type { get; set; }
        public string TextureName { get; set; }
        public Rectangle SourceRect { get; set; }
        public bool IsCollidable { get; set; }
    }
}