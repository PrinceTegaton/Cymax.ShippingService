using Newtonsoft.Json;

namespace ParagoLogistics.API.Helpers
{
    public class JsonStoreUtil<T> where T : class
    {
        public string DataPath;
        private string FileName = typeof(T).Name;

        public JsonStoreUtil()
        {

        }

        public JsonStoreUtil(string fileName = null)
        {
            this.FileName = fileName;
        }

        private async Task<string> ReadRaw()
        {
            DataPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\DataStore\\{FileName}.json";
            if (!File.Exists(DataPath))
            {
                string dir = Path.GetDirectoryName(DataPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }

            var fileStr = await File.ReadAllTextAsync(DataPath);
            return fileStr;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            try
            {
                string raw = await ReadRaw();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(raw);
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }
    }
}