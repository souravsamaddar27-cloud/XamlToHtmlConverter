public static class XamlColorConverter
{
    public static string ToCss(string xamlColor)
    {
        if (xamlColor.StartsWith("#") && xamlColor.Length == 9)
        {
            var a = Convert.ToInt32(xamlColor.Substring(1, 2), 16);
            var r = Convert.ToInt32(xamlColor.Substring(3, 2), 16);
            var g = Convert.ToInt32(xamlColor.Substring(5, 2), 16);
            var b = Convert.ToInt32(xamlColor.Substring(7, 2), 16);

            var alpha = Math.Round(a / 255.0, 3);
            return $"rgba({r},{g},{b},{alpha})";
        }

        return xamlColor;
    }
}