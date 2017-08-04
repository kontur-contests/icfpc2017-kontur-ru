using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib
{
    public static class MapLoader
    {
        public static readonly string DefaultPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\maps");

        [NotNull]
        public static NamedMap LoadMap([NotNull] string path)
        {
            if (!File.Exists(path))
                throw new Exception($"File {path} not exists");
            Map map = JsonConvert.DeserializeObject<Map>(Encoding.UTF8.GetString(File.ReadAllBytes(path)));
            
            return new NamedMap(
                Path.GetFileNameWithoutExtension(path),
                map);
        }
                                                                                                                          p
        [NotNull]
        public static IEnumerable<NamedMap> LoadMaps([NotNull] string path)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                throw new Exception($"Directory {path} not exists");
            return directory.EnumerateFiles().Select(x => LoadMap(x.FullName));
        }

        [NotNull]
        public static IEnumerable<NamedMap> LoadDefaultMaps()
        {
            return LoadMaps(DefaultPath);
        }
    }

    [TestFixture]
    public class MapLoaderTests
    {
        [Test]
        public void Test()
        {
            Assert.That(
                MapLoader.LoadDefaultMaps().Select(x => x.Name),
                Is.EquivalentTo(
                    new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\maps"))
                        .EnumerateFiles().Select(x => Path.GetFileNameWithoutExtension(x.FullName))));
        }
    }
}