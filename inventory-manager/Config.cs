using System;
namespace InventoryManager
{
    public static class Config
    {
        /// <summary>
        /// Window title
        /// </summary>
        public static readonly string WINDOW_TITLE = "Inventory Manager";

        /// <summary>
        /// Tells if FPS should be displayed on screen
        /// </summary>
        public static readonly bool DISPLAY_FPS = true;

        /// <summary>
        /// FPS cap
        /// </summary>
        public static readonly int FPS_CAP = 60;

        /// <summary>
        /// Keyboard is polled every
        /// </summary>
        public static readonly TimeSpan POLL_KEYBOARD_INTERVAL = TimeSpan.FromMilliseconds(100); 
    }
}
