using System;
namespace Core.Build
{
    // Bitmask so a tower can allow multiple slot types (shows as flags in the Inspector)
    [Flags]
    public enum SlotMask
    {
        None      = 0,
        Capillary = 1 << 0,
        Lymph     = 1 << 1,
        Barrier   = 1 << 2,
        Any       = Capillary | Lymph | Barrier
    }

    public static class SlotMaskUtil
    {
        public static SlotMask FromType(SlotType t)
        {
            switch (t)
            {
                case SlotType.Capillary: return SlotMask.Capillary;
                case SlotType.Lymph:     return SlotMask.Lymph;
                case SlotType.Barrier:   return SlotMask.Barrier;
                default:                 return SlotMask.None;
            }
        }
    }
}
