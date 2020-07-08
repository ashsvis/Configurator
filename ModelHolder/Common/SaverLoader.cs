using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace ModelHolder
{
    public static class SaverLoader
    {
        public static ModelRoot LoadFromFile(string fileName)
        {
            using (var fs = File.OpenRead(fileName))
            using (var zip = new GZipStream(fs, CompressionMode.Decompress))
            {
                var formatter = new BinaryFormatter();
                return (ModelRoot)formatter.Deserialize(zip);
            }
        }

        public static void SaveToFile(string fileName, ModelRoot root)
        {
            using (var fs = File.Create(fileName))
            using (var zip = new GZipStream(fs, CompressionMode.Compress))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(zip, root);
            }
        }
    }
}
