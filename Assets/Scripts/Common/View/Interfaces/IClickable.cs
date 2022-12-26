using System;

namespace Common.View
{
    public interface IClickable
    {
        event Action OnLevelTypeButtonClicked;
    }
}