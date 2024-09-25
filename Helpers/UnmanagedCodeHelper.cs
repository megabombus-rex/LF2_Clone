using System.Text;

namespace LF2Clone.Helpers
{
    public static class UnmanagedCodeHelper
    {
        public static unsafe sbyte* StringToSbyte(string text, Encoding e)
        {
            byte[] bytes = e.GetBytes(text);

            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    return (sbyte*)ptr;
                }
            }
        }
    }
}
