using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class AssetManager : IUpdateable, IDrawable
    {
        private Dictionary<string, Texture2D> _textures;
        private Dictionary<string, SoundEffect> _sounds;
        private Dictionary<string, Song> _music;
        private Dictionary<string, SpriteFont> _fonts;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        
        public AssetManager()
        {
            _textures = new Dictionary<string, Texture2D>();
            _sounds = new Dictionary<string, SoundEffect>();
            _music = new Dictionary<string, Song>();
            _fonts = new Dictionary<string, SpriteFont>();
        }
        
        public void Initialize(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
        }

        public void LoadTexture(string name, string filePath)
        {
            try
            {
                if (_graphicsDevice != null)
                {
                    Texture2D texture = Texture2D.FromFile(_graphicsDevice, filePath);
                    _textures[name] = texture;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading texture {filePath}: {ex.Message}");
            }
        }

        public void LoadSound(string name, string filePath)
        {
            try
            {
                SoundEffect sound = SoundEffect.FromFile(filePath);
                _sounds[name] = sound;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sound {filePath}: {ex.Message}");
            }
        }

        public void LoadMusic(string name, string assetName)
        {
            try
            {
                Song music = _contentManager.Load<Song>(assetName);
                _music[name] = music;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading music {assetName}: {ex.Message}");
            }
        }

        public void LoadFont(string name, string assetName)
        {
            try
            {
                SpriteFont font = _contentManager.Load<SpriteFont>(assetName);
                _fonts[name] = font;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading font {assetName}: {ex.Message}");
            }
        }

        public Texture2D GetTexture(string name)
        {
            if (_textures.ContainsKey(name))
                return _textures[name];
            return null; // Should return a default texture in a real implementation
        }

        public SoundEffect GetSound(string name)
        {
            if (_sounds.ContainsKey(name))
                return _sounds[name];
            return null;
        }

        public Song GetMusic(string name)
        {
            if (_music.ContainsKey(name))
                return _music[name];
            return null;
        }

        public SpriteFont GetFont(string name)
        {
            if (_fonts.ContainsKey(name))
                return _fonts[name];
            return null;
        }

        public void PreloadAssetsFromConfig(string configPath)
        {
            // This would parse a config file to preload all assets
            // For now, we'll just have the basic loading methods
        }

        public void UnloadAllAssets()
        {
            foreach (var texture in _textures.Values)
            {
                texture?.Dispose();
            }
            _textures.Clear();

            foreach (var sound in _sounds.Values)
            {
                sound?.Dispose();
            }
            _sounds.Clear();

            // Note: Songs should not be disposed as they are handled by the MediaPlayer
            _music.Clear();
        }

        public void Update(GameTime gameTime)
        {
            // Asset manager doesn't need real-time updates
        }

        public void Draw(GameTime gameTime)
        {
            // Asset manager doesn't draw anything directly
        }
    }
}