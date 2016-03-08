using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePhysicsTest.Input
{
    public class MouseEvents
    {
        private readonly GameWindow _window;
        private readonly MouseButtonEvents _rightButtonEvents = new MouseButtonEvents();
        private readonly MouseButtonEvents _leftButtonEvents = new MouseButtonEvents();
        private readonly MouseButtonEvents _middleButtonEvents = new MouseButtonEvents();

        public MouseEvents(GameWindow window)
        {
            _window = window;
        }

        public void Update(GameTime gameTime)
        {
            var state = Mouse.GetState(_window);
            _rightButtonEvents.Update(state.RightButton, state.X, state.Y);
            _leftButtonEvents.Update(state.LeftButton, state.X, state.Y);
            _middleButtonEvents.Update(state.MiddleButton, state.X, state.Y);
        }

        public void OnLeftClick(Action<int,int> handler)
        {
            _leftButtonEvents.OnClick(handler);
        }

        public void OnRightClick(Action<int, int> handler)
        {
            _rightButtonEvents.OnClick(handler);
        }

        public void OnMiddleClick(Action<int, int> handler)
        {
            _middleButtonEvents.OnClick(handler);
        }

        private class MouseButtonEvents
        {
            private ButtonState _lastState = ButtonState.Released;
            private readonly List<Action<int,int>> _handlers = new List<Action<int,int>>();

            public void Update(ButtonState nextState, int x, int y)
            {
                if (_lastState == ButtonState.Released)
                {
                    if (nextState != ButtonState.Pressed)
                    {
                        return;
                    }
                    foreach (var h in _handlers)
                    {
                        h(x, y);
                    }
                    _lastState = ButtonState.Pressed;
                }
                else if (nextState == ButtonState.Released)
                {
                    _lastState = ButtonState.Released;
                }
            }

            public void OnClick(Action<int, int> handler)
            {
                _handlers.Add(handler);
            }
        }
    }
}