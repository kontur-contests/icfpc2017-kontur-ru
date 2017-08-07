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
        private static string LocateMapsFolder(string startDir = null)
        {
            var dir = new DirectoryInfo(startDir ?? AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !dir.HasSubdir("maps"))
            {
                dir = dir.Parent;
            }
            return dir.GetSubdir("maps").FullName;
        }

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

        [NotNull]
        public static NamedMap LoadMapByName([NotNull] string filenameWithoutPath)
        {
            var fn = Path.Combine(LocateMapsFolder(), filenameWithoutPath);
            return LoadMap(fn);
        }
        [NotNull]
        public static NamedMap LoadMapByNameInTests([NotNull] string filenameWithoutPath)
        {
            var fn = Path.Combine(LocateMapsFolder(TestContext.CurrentContext.TestDirectory), filenameWithoutPath);
            return LoadMap(fn);
        }

        [NotNull]
        public static IEnumerable<NamedMap> LoadMaps([NotNull] string path)
        {
            var directory = new DirectoryInfo(path);
            if (!directory.Exists)
                throw new Exception($"Directory {path} not exists");
            return directory.EnumerateFiles("*.json").Select(x => LoadMap(x.FullName));
        }

        [NotNull]
        public static IEnumerable<NamedMap> LoadDefaultMaps()
        {
            return LoadMaps(LocateMapsFolder());
        }
        [NotNull]
        public static IEnumerable<NamedMap> LoadOnlineMaps()
        {
            var sizes = LoadOnlineMapSizes();
            return sizes.Select(s =>
            {
                var map = LoadMapByName(s.Key);
                map.IsOnline = true;
                map.PlayersCount = s.Value;
                return map;
            });
        }
        [NotNull]
        public static Dictionary<string, int> LoadOnlineMapSizes()
        {
            string text = File.ReadAllText(Path.Combine(LocateMapsFolder(), "online-map-sizes"));
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(text);
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
                        .EnumerateFiles("*.json").Select(x => Path.GetFileNameWithoutExtension(x.FullName))));
        }
    }
}