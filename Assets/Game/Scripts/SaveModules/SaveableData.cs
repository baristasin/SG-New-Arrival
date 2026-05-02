using System;

namespace Game.Scripts.SaveModules
{
    public abstract class SaveableData
    {
        public event Action<SaveableData> OnSaveRequested;

        protected void MarkDirty() => OnSaveRequested?.Invoke(this);

        /// <summary> Set field if changed; auto-fires save event. </summary>
        protected bool SetField<T>(ref T field, T value)
        {
            if (Equals(field, value)) return false;
            field = value;
            MarkDirty();
            return true;
        }
    }
}