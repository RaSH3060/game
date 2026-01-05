using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GameEngine
{
    public class InputSystem : IUpdateable, IDrawable
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private GamePadState _currentGamePadState;
        private GamePadState _previousGamePadState;

        private int _mouseWheelValue;
        private int _previousMouseWheelValue;

        private Dictionary<string, Keys> _keyBindings;
        private Dictionary<string, MouseButton> _mouseBindings;
        private GameWindow _gameWindow;

        public Vector2 MousePosition => new Vector2(_currentMouseState.X, _currentMouseState.Y);
        public Vector2 MouseDelta => new Vector2(
            _currentMouseState.X - _previousMouseState.X,
            _currentMouseState.Y - _previousMouseState.Y
        );

        public int MouseWheelDelta => _mouseWheelValue - _previousMouseWheelValue;

        public InputSystem(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _keyBindings = new Dictionary<string, Keys>();
            _mouseBindings = new Dictionary<string, MouseButton>();

            // Default key bindings
            SetupDefaultBindings();
        }
        
        private void SetupDefaultBindings()
        {
            _keyBindings["MoveUp"] = Keys.W;
            _keyBindings["MoveDown"] = Keys.S;
            _keyBindings["MoveLeft"] = Keys.A;
            _keyBindings["MoveRight"] = Keys.D;
            _keyBindings["Jump"] = Keys.Space;
            _keyBindings["Use"] = Keys.E;
            _keyBindings["Attack"] = Keys.Z;
            _keyBindings["Inventory"] = Keys.Tab;
            _keyBindings["Pause"] = Keys.Escape;
            
            // Mouse bindings
            _mouseBindings["Attack"] = MouseButton.LeftButton;
            _mouseBindings["Use"] = MouseButton.RightButton;
        }
        
        public void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;
            _previousMouseWheelValue = _mouseWheelValue;
            _previousGamePadState = _currentGamePadState;

            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState(_gameWindow);
            _mouseWheelValue = _currentMouseState.ScrollWheelValue;
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }
        
        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }
        
        public bool IsKeyReleased(Keys key)
        {
            return _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);
        }
        
        public bool IsKeyDown(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }
        
        public bool IsKeyUp(Keys key)
        {
            return _currentKeyboardState.IsKeyUp(key);
        }
        
        public bool IsMouseButtonPressed(MouseButton button)
        {
            switch(button)
            {
                case MouseButton.LeftButton:
                    return _currentMouseState.LeftButton == ButtonState.Pressed && 
                           _previousMouseState.LeftButton == ButtonState.Released;
                case MouseButton.RightButton:
                    return _currentMouseState.RightButton == ButtonState.Pressed && 
                           _previousMouseState.RightButton == ButtonState.Released;
                case MouseButton.MiddleButton:
                    return _currentMouseState.MiddleButton == ButtonState.Pressed && 
                           _previousMouseState.MiddleButton == ButtonState.Released;
                default:
                    return false;
            }
        }
        
        public bool IsMouseButtonDown(MouseButton button)
        {
            switch(button)
            {
                case MouseButton.LeftButton:
                    return _currentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.RightButton:
                    return _currentMouseState.RightButton == ButtonState.Pressed;
                case MouseButton.MiddleButton:
                    return _currentMouseState.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        
        public bool IsActionPressed(string action)
        {
            if (_keyBindings.ContainsKey(action))
            {
                return IsKeyPressed(_keyBindings[action]);
            }
            
            if (_mouseBindings.ContainsKey(action))
            {
                return IsMouseButtonPressed(_mouseBindings[action]);
            }
            
            return false;
        }
        
        public bool IsActionDown(string action)
        {
            if (_keyBindings.ContainsKey(action))
            {
                return IsKeyDown(_keyBindings[action]);
            }
            
            if (_mouseBindings.ContainsKey(action))
            {
                return IsMouseButtonDown(_mouseBindings[action]);
            }
            
            return false;
        }
        
        public void BindKey(string action, Keys key)
        {
            _keyBindings[action] = key;
        }
        
        public void BindMouse(string action, MouseButton button)
        {
            _mouseBindings[action] = button;
        }
        
        public void Draw(GameTime gameTime)
        {
            // Input system doesn't draw anything
        }
    }
    
    public enum MouseButton
    {
        LeftButton,
        MiddleButton,
        RightButton
    }
}