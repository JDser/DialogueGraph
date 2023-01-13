using UnityEngine;

namespace DialogueEditor
{
    public interface IColorable
    {
        public Color UserDefinedColor { get; }

        public void SetUserDefinedColor(Color newColor);

        public void ChangeColor(ColorMode colorMode);

        public void ResetUserDefinedColor();
    }
}