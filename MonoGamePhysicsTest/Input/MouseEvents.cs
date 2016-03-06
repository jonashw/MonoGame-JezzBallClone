using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePhysicsTest.Input
{
    public class MouseEvents
    {
        private readonly GameWindow _window;
        private readonly List<Action<int,int>> _leftHandlers = new List<Action<int,int>>();
        private readonly List<Action<int,int>> _rightHandlers = new List<Action<int,int>>();
        private ButtonState _left = ButtonState.Released;
        private ButtonState _right = ButtonState.Released;

        public MouseEvents(GameWindow window)
        {
            _window = window;
        }

        public void Update(GameTime gameTime)
        {
            var state = Mouse.GetState(_window);
            if (_left == ButtonState.Released)
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    foreach (var h in _leftHandlers)
                    {
                        h(state.X, state.Y);
                    }
                    _left = ButtonState.Pressed;
                }
            }
            else if (state.LeftButton == ButtonState.Released)
            {
                _left = ButtonState.Released;
            }

            if (_right == ButtonState.Released)
            {
                if (state.RightButton == ButtonState.Pressed)
                {
                    foreach (var h in _rightHandlers)
                    {
                        h(state.X, state.Y);
                    }
                    _right = ButtonState.Pressed;
                }
            }
            else if (state.RightButton == ButtonState.Released)
            {
                _right = ButtonState.Released;
            }
        }

        public void OnLeftClick(Action<int,int> handler)
        {
            _leftHandlers.Add(handler);
        }

        public void OnRightClick(Action<int, int> handler)
        {
            _rightHandlers.Add(handler);
        }
    }
}