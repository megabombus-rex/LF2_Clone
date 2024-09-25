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

        public static unsafe string PtrToStringUtf8(sbyte* ptr)
        {
            if (ptr == null) return string.Empty;

            int length = 0;
            while (ptr[length] != 0) length++;

            return Encoding.UTF8.GetString((byte*)ptr, length);
        }

    }
}
