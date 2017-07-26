using System;
using System.IO;
using System.Linq;

namespace lib
{
    public static class FileSystemExtensions
    {
        public static bool HasSubdir(this DirectoryInfo root, string name)
        {
            return root.GetSubdir(name).Exists;
        }

        public static FileInfo GetFile(this DirectoryInfo dir, string filename)
        {
            return new FileInfo(Path.Combine(dir.FullName, filename));
        }

        public static DirectoryInfo GetSubdir(this DirectoryInfo dir, string name)
        {
            return new DirectoryInfo(Path.Combine(dir.FullName, name));
        }

        public static DirectoryInfo GetOrCreateSubdir(this DirectoryInfo dir, string name)
        {
            var subdir = dir.GetSubdir(name);
            if (!subdir.Exists)
                subdir.Create();
            return subdir;
        }

        public static string GetRelativePathTo(this FileSystemInfo file, string folder)
        {
            var pathUri = new Uri(file.FullName);
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                folder += Path.DirectorySeparatorChar;
            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(
                folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static string[] GetFilenames(this DirectoryInfo di, string dirPath)
        {
            var dir = di.GetSubdir(dirPath);
            if (!dir.Exists)
                throw new Exception("No " + dirPath + " in " + di.Name);
            return dir.GetFiles().Select(f => Path.Combine(dirPath, f.Name)).ToArray();
        }
    }
}