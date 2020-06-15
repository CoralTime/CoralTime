using System.IO;
using System.Linq;

namespace CoralTime.BL.Helpers
{
    public static class FileNameChecker
    {
        public static string CheckFileName(string fileName, string permittedExtensions, int permittedNameLength)
        {
            string errors = null;

            if (!IsFileNameCorrect(fileName, permittedExtensions, permittedNameLength))
            {
                errors = GetFileErrors(fileName, permittedExtensions, permittedNameLength);
            }

            return errors;
        }

        public static bool IsDirectoryNameValid(string directoryName)
        {
            return directoryName.Intersect(Path.GetInvalidFileNameChars()).Any();
        }

        public static bool IsFileNameCorrect(string fileName, string permittedExtensions, int permittedNameLength)
        {
            try
            {
                var containsInvalidCharacters = fileName.Intersect(Path.GetInvalidFileNameChars()).Any();
                var isFileExtensionCorrect = IsFileExtensionCorrect(fileName, permittedExtensions);
                var isFileNameContainsPath = IsFileNameContainsPath(fileName);
                var correctFileNameLength = IsFileNameLengthCorrect(fileName, permittedNameLength);

                return !containsInvalidCharacters && isFileExtensionCorrect && !isFileNameContainsPath && correctFileNameLength;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private static bool IsFileExtensionCorrect(string fileName, string permittedExtensions)
        {
            var z1 = Path.HasExtension(fileName);
            if (!z1)
                return false;

            var extension = Path.GetExtension(fileName).ToLower().Remove(0, 1);
            var whiteList = permittedExtensions.Split(',');
            foreach (var ext in whiteList)
            {
                if (extension == ext)
                    return true;
            }

            return false;
        }

        private static bool IsFileNameContainsPath(string fileName)
        {
            var path = Path.GetDirectoryName(fileName);
            return !string.IsNullOrEmpty(path);
        }

        private static bool IsFileNameLengthCorrect(string fileName, int permittedNameLength)
        {
            return fileName.Length < permittedNameLength;
        }

        public static string GetFileErrors(string fileName, string permittedExtensions, int permittedNameLength)
        {
            var errors = string.Empty;

            if (!IsFileExtensionCorrect(fileName, permittedExtensions))
            {
                errors += "Invalid file extension. ";
            }

            if (IsFileNameContainsPath(fileName))
            {
                errors += "File name contains path. ";
            }

            if (!IsFileNameLengthCorrect(fileName, permittedNameLength))
            {
                errors += "File name is too long. ";
            }

            return errors;
        }
    }
}
