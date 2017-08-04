using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using lib;

namespace Pack
{
    class Program
    {
        static void Main(string[] args)
        {
            var lib = LocateLibFolder();
            var output = lib.Parent.GetSubdir(".pack");
            if (output.Exists)
                output.Delete(true);
            Copy(lib, output.GetSubdir("lib"), true);
            var process = Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = output.GetSubdir("lib").FullName,
                FileName = lib.Parent.GetSubdir("tools").GetFile("nuget.exe").FullName,
                Arguments = "restore -PackagesDirectory ..\\packages",
                UseShellExecute = false
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine("Failed to restore packages");
                Environment.Exit(process.ExitCode);
            }
            Copy(lib.Parent.GetSubdir("pack-template"), output, false);

            CreatePackagesReferences(output.GetSubdir("packages"), output.GetSubdir("lib").GetFile("lib.csproj"));
        }

        private static void CreatePackagesReferences(DirectoryInfo packagesDir, FileInfo csprojFile)
        {
            const string ns = "http://schemas.microsoft.com/developer/msbuild/2003";
            var csproj = XDocument.Load(csprojFile.FullName);
            var itemGroup = new XElement(XName.Get("ItemGroup", ns));
            csproj.Root.Add(itemGroup);
            foreach (var dir in packagesDir.GetDirectories())
            {
                var packageLibDir = FindPackageLibFolder(dir);
                if (packageLibDir != null)
                {
                    foreach (var dll in packageLibDir.GetFiles("*.dll"))
                    {
                        var reference = new XElement(XName.Get("Reference", ns));
                        itemGroup.Add(reference);
                        reference.Add(new XAttribute("Include", GetPackageName(dir)));
                        var hintPath = new XElement(XName.Get("HintPath", ns));
                        reference.Add(hintPath);
                        hintPath.Value = $"..\\packages\\{dir.Name}\\lib\\{packageLibDir.Name}\\{dll.Name}";
                    }
                }
            }
            csproj.Save(csprojFile.FullName, SaveOptions.None);
        }

        private static DirectoryInfo FindPackageLibFolder(DirectoryInfo dir)
        {
            var lib = dir.GetSubdir("lib");
            if (!lib.Exists)
                return null;
            return lib
                .GetDirectories("net*")
                .Select(
                    libDir =>
                    {
                        var regex = new Regex(@"^net\d+$");
                        return regex.IsMatch(libDir.Name) ? libDir : null;
                    })
                .Where(x => x != null)
                .OrderByDescending(x => x.Name)
                .FirstOrDefault();
        }

        private static string GetPackageName(DirectoryInfo packageDir)
        {
            var regex = new Regex(@"^(?<name>.*)(?:\.\d+){3}$");
            var match = regex.Match(packageDir.Name);
            return match.Groups["name"].Value;
        }

        public static void Copy(DirectoryInfo source, DirectoryInfo target, bool processFiles)
        {
            foreach (var dir in source.GetDirectories().Where(FilterDir))
                Copy(dir, target.GetSubdir(dir.Name), processFiles);

            foreach (var file in source.GetFiles())
                Copy(file, target, processFiles);
        }

        private static bool FilterDir(DirectoryInfo dir)
        {
            return dir.Name != "bin"
                   && dir.Name != "obj";
        }

        private static void Copy(FileInfo file, DirectoryInfo target, bool processFiles)
        {
            if (!processFiles)
            {
                target.Create();
                file.CopyTo(target.GetFile(file.Name).FullName);
                return;
            }
            switch (file.Extension)
            {
                case ".cs":
                    if (File.ReadAllText(file.FullName).Contains("pack: exclude"))
                        return;
                    target.Create();
                    file.CopyTo(target.GetFile(file.Name).FullName);
                    break;
                case ".csproj":
                    target.Create();
                    var document = XDocument.Load(File.OpenRead(file.FullName));
                    using (var writer = new StreamWriter(target.GetFile("packages.config").FullName))
                    {
                        writer.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
                        writer.WriteLine("<packages>");
                        foreach (var element in document.XPathSelectElements("//PackageReference"))
                        {
                            writer.WriteLine($@"  <package id=""{element.Attribute("Include")?.Value}"" version=""{element.Attribute("Version")?.Value}"" />");
                        }
                        writer.WriteLine("</packages>");
                    }
                    break;

            }
        }

        private static DirectoryInfo LocateLibFolder()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !dir.HasSubdir("lib"))
            {
                dir = dir.Parent;
            }
            return dir.GetSubdir("lib");
        }
    }
}