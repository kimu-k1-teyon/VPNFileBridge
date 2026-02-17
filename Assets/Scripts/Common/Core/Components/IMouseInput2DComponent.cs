using System;
using UnityEngine;

namespace Scripts.Common.Core.Components
{
    public interface IMouseInput2DComponent
    {
        event Action<int, Vector2> DragStarted;
        event Action<int, Vector2> DragDelta;
        event Action<int> DragEnded;
        event Action<float> ScrollDelta;
        void Tick();
    }
}
