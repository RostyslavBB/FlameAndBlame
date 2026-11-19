using System.Globalization;

namespace FB.Game.UI
{
    /// <summary>Formats dollars as "$1 200" (space thousands separator, culture independent).</summary>
    public static class Money
    {
        private static readonly NumberFormatInfo Format =
            NumberFormatInfo.ReadOnly(new NumberFormatInfo { NumberGroupSeparator = " " });

        public static string Of(int dollars) => "$" + dollars.ToString("N0", Format);
    }
}
