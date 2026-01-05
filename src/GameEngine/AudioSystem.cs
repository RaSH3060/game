using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class AudioSystem : IUpdateable, IDrawable
    {
        private Dictionary<string, SoundEffect> _soundEffects;
        private Dictionary<string, Song> _musicTracks;
        private Dictionary<string, SoundEffectInstance> _activeSounds;
        private ContentManager _contentManager;

        private float _masterVolume = 1.0f;
        private float _soundVolume = 1.0f;
        private float _musicVolume = 1.0f;

        private string _currentMusicTrack = "";
        private bool _isMusicPlaying = false;

        public AudioSystem(ContentManager contentManager)
        {
            _soundEffects = new Dictionary<string, SoundEffect>();
            _musicTracks = new Dictionary<string, Song>();
            _activeSounds = new Dictionary<string, SoundEffectInstance>();
            _contentManager = contentManager;
        }
        
        public void LoadSoundEffect(string name, string assetName)
        {
            try
            {
                SoundEffect sound = _contentManager.Load<SoundEffect>(assetName);
                _soundEffects[name] = sound;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sound effect {assetName}: {ex.Message}");
            }
        }

        public void LoadMusicTrack(string name, string assetName)
        {
            try
            {
                Song song = _contentManager.Load<Song>(assetName);
                _musicTracks[name] = song;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading music track {assetName}: {ex.Message}");
            }
        }
        
        public void PlaySound(string name, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f)
        {
            if (_soundEffects.ContainsKey(name))
            {
                SoundEffectInstance instance = _soundEffects[name].CreateInstance();
                instance.Volume = MathHelper.Clamp(volume * _soundVolume * _masterVolume, 0f, 1f);
                instance.Pitch = MathHelper.Clamp(pitch, -1f, 1f);
                instance.Pan = MathHelper.Clamp(pan, -1f, 1f);
                instance.Play();
                
                // Store the instance to prevent garbage collection
                string instanceKey = name + Guid.NewGuid().ToString();
                _activeSounds[instanceKey] = instance;
                
                // Clean up finished sounds occasionally
                CleanupFinishedSounds();
            }
        }
        
        public void PlayMusic(string name, bool loop = true)
        {
            if (_musicTracks.ContainsKey(name))
            {
                if (_isMusicPlaying && _currentMusicTrack == name)
                    return; // Already playing this track
                
                StopMusic();
                
                MediaPlayer.Volume = _musicVolume * _masterVolume;
                MediaPlayer.IsRepeating = loop;
                MediaPlayer.Play(_musicTracks[name]);
                
                _currentMusicTrack = name;
                _isMusicPlaying = true;
            }
        }
        
        public void StopMusic()
        {
            if (_isMusicPlaying)
            {
                MediaPlayer.Stop();
                _isMusicPlaying = false;
                _currentMusicTrack = "";
            }
        }
        
        public void PauseMusic()
        {
            if (_isMusicPlaying)
            {
                MediaPlayer.Pause();
                _isMusicPlaying = false;
            }
        }
        
        public void ResumeMusic()
        {
            if (!_isMusicPlaying && !string.IsNullOrEmpty(_currentMusicTrack) && _musicTracks.ContainsKey(_currentMusicTrack))
            {
                MediaPlayer.Resume();
                _isMusicPlaying = true;
            }
        }
        
        public void SetMasterVolume(float volume)
        {
            _masterVolume = MathHelper.Clamp(volume, 0f, 1f);
            UpdateMusicVolume();
        }
        
        public void SetSoundVolume(float volume)
        {
            _soundVolume = MathHelper.Clamp(volume, 0f, 1f);
        }
        
        public void SetMusicVolume(float volume)
        {
            _musicVolume = MathHelper.Clamp(volume, 0f, 1f);
            UpdateMusicVolume();
        }
        
        private void UpdateMusicVolume()
        {
            if (_isMusicPlaying)
            {
                MediaPlayer.Volume = _musicVolume * _masterVolume;
            }
        }
        
        private void CleanupFinishedSounds()
        {
            List<string> toRemove = new List<string>();
            foreach (var kvp in _activeSounds)
            {
                if (kvp.Value.State == SoundState.Stopped)
                {
                    kvp.Value.Dispose();
                    toRemove.Add(kvp.Key);
                }
            }
            
            foreach (string key in toRemove)
            {
                _activeSounds.Remove(key);
            }
        }
        
        public void Update(GameTime gameTime)
        {
            // Clean up finished sounds periodically
            if (gameTime.TotalGameTime.TotalMilliseconds % 1000 < 16) // roughly every second
            {
                CleanupFinishedSounds();
            }
        }
        
        public void Draw(GameTime gameTime)
        {
            // Audio system doesn't draw anything
        }
    }
}