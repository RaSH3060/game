using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class RenderSystem : IUpdateable, IDrawable
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Dictionary<string, Effect> _shaders;
        private Matrix _transformMatrix;
        private Viewport _viewport;
        private bool _isFullscreen;
        private Point _resolution;
        private Texture2D _whitePixel;
        
        public GraphicsDevice GraphicsDevice => _graphicsDevice;

        public RenderSystem(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _shaders = new Dictionary<string, Effect>();
            _resolution = new Point(1024, 768);
            _isFullscreen = false;

            // Create a 1x1 white pixel texture for drawing rectangles
            _whitePixel = new Texture2D(_graphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });

            UpdateTransformMatrix();
        }
        
        public void SetResolution(int width, int height)
        {
            _resolution = new Point(width, height);
            _graphicsDevice.Viewport = new Viewport(0, 0, width, height);
            UpdateTransformMatrix();
        }
        
        public void ToggleFullscreen()
        {
            _isFullscreen = !_isFullscreen;
            _graphicsDeviceManager.ToggleFullScreen();
            UpdateTransformMatrix();
        }
        
        private void UpdateTransformMatrix()
        {
            _viewport = _graphicsDevice.Viewport;
            float scaleX = (float)_viewport.Width / _resolution.X;
            float scaleY = (float)_viewport.Height / _resolution.Y;
            _transformMatrix = Matrix.CreateScale(scaleX, scaleY, 1f);
        }
        
        public void BeginDraw()
        {
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                _transformMatrix
            );
        }
        
        public void EndDraw()
        {
            _spriteBatch.End();
        }
        
        public void DrawSprite(Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, 
                              Color? color = null, float rotation = 0f, Vector2? origin = null, 
                              Vector2? scale = null, SpriteEffects effects = SpriteEffects.None, 
                              float layerDepth = 0f)
        {
            _spriteBatch.Draw(
                texture,
                position,
                sourceRectangle,
                color ?? Color.White,
                rotation,
                origin ?? Vector2.Zero,
                scale ?? Vector2.One,
                effects,
                layerDepth
            );
        }
        
        public void DrawString(SpriteFont font, string text, Vector2 position, Color color, 
                              float rotation = 0f, Vector2? origin = null, Vector2? scale = null,
                              SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            _spriteBatch.DrawString(
                font,
                text,
                position,
                color,
                rotation,
                origin ?? Vector2.Zero,
                scale ?? Vector2.One,
                effects,
                layerDepth
            );
        }
        
        public void DrawRectangle(Rectangle rectangle, Color color, int thickness = 1)
        {
            // Draw a rectangle outline using the white pixel texture
            // Top line
            _spriteBatch.Draw(_whitePixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // Bottom line
            _spriteBatch.Draw(_whitePixel, new Rectangle(rectangle.X, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
            // Left line
            _spriteBatch.Draw(_whitePixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // Right line
            _spriteBatch.Draw(_whitePixel, new Rectangle(rectangle.Right - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
        
        public void FillRectangle(Rectangle rectangle, Color color)
        {
            // Draw a filled rectangle using the white pixel texture
            _spriteBatch.Draw(_whitePixel, rectangle, color);
        }
        
        public void ClearScreen(Color color)
        {
            _graphicsDevice.Clear(color);
        }
        
        public void Update(GameTime gameTime)
        {
            // Check for resolution changes if needed
            if (_graphicsDevice.Viewport.Width != _resolution.X || _graphicsDevice.Viewport.Height != _resolution.Y)
            {
                UpdateTransformMatrix();
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            // Render system handles drawing through BeginDraw/EndDraw calls from other systems
        }
    }
}