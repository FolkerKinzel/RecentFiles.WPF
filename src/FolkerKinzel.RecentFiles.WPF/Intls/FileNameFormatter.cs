using System.Globalization;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

internal static class FileNameFormatter
{
    private const int MAX_DISPLAYED_FILE_PATH_LENGTH = 88;
    private const string REPLACEMENT = " \u2026 ";

    internal static string GetMenuItemHeaderFromFilename(string fileName, int i)
    {
        if (fileName.Length > MAX_DISPLAYED_FILE_PATH_LENGTH)
        {
            const int QUARTER_DISPLAYED_FILE_PATH_LENGTH = MAX_DISPLAYED_FILE_PATH_LENGTH / 4;
            const int THREE_QUARTER_DISPLAYED_FILE_PATH_LENGTH = QUARTER_DISPLAYED_FILE_PATH_LENGTH * 3;

            fileName =
#if NET462
                        fileName.Substring(0, QUARTER_DISPLAYED_FILE_PATH_LENGTH - 3) +
                        REPLACEMENT +
                        fileName.Substring(fileName.Length - THREE_QUARTER_DISPLAYED_FILE_PATH_LENGTH);
#else
                    string.Concat(fileName.AsSpan(0, QUARTER_DISPLAYED_FILE_PATH_LENGTH - 3),
                              REPLACEMENT,
                              fileName.AsSpan(fileName.Length - THREE_QUARTER_DISPLAYED_FILE_PATH_LENGTH));
#endif
        }

        if (i < 9)
        {
            fileName = "_" + (i + 1).ToString(CultureInfo.InvariantCulture) + ": " + fileName;
        }
        else if (i == 9)
        {
            fileName = "1_0: " + fileName;
        }

        return fileName;
    }


    /// <summary>
    /// Helper method to support unit tests.
    /// </summary>
    /// <returns>The value of <see cref="REPLACEMENT"/>.</returns>
    internal static string GetReplacement() => REPLACEMENT;
}