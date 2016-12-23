using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace XYZZ.Tools
{
    /// <summary>
    /// 缓存工具
    /// </summary>
    public class Cache
    {
        private const string CacheName = "System";
        private static string FileName = "Cache.txt";
        private static string FilePath = Environment.CurrentDirectory + @"\Plugins\";
        private static MemoryCache MemoryCache = new MemoryCache(CacheName);
        private static DateTimeOffset DestroyTime = DateTimeOffset.Now.AddMonths(1);
        private static Timer Timer = new Timer();
        /// <summary>
        /// <see cref="Cache"/>的实例
        /// </summary>
        public static readonly Cache Instance = new Cache();

        private Cache()
        {
            Directory.CreateDirectory(FilePath);
            FileStream cacheStream = new FileInfo(FilePath + FileName).Open(FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(cacheStream, Encoding.UTF8);
            string json = Decrypt(reader.ReadToEnd());
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    JObject jObject = JObject.Parse(json);
                    foreach (var item in jObject)
                    {
                        MemoryCache.Set(item.Key, item.Value.ToString(), DestroyTime);
                    }
                }
                catch { }
            }
            cacheStream.Close();
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="name">数据名称</param>
        /// <param name="value">数据值</param>
        public static void Set(string name, string value)
        {
            if (MemoryCache.Contains(name))
            {
                MemoryCache[name] = value;
            }
            else
            {
                MemoryCache.Set(name, value, DestroyTime);
            }
            WriteFile();
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="name">数据名称</param>
        /// <returns></returns>
        public static string Get(string name)
        {
            if (MemoryCache.Contains(name))
            {
                return MemoryCache.Get(name).ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        private static void WriteFile()
        {
            //通过定时器延时，避免多次写入
            Timer.Stop();
            Timer = new Timer(1 * 1000);
            Timer.AutoReset = false;
            Timer.Elapsed += new ElapsedEventHandler(StartWrite);
            Timer.Start();
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        private static async void StartWrite(object sender, ElapsedEventArgs e)
        {
            Timer.Stop();
            await Task.Run(() =>
          {
              try
              {
                  using (FileStream cacheStream = new FileInfo(FilePath + FileName).Open(FileMode.Create))
                  {
                      JObject jObject = new JObject();
                      foreach (var item in MemoryCache)
                      {
                          jObject.Add(item.Key, item.Value.ToString());
                      }
                      byte[] requestBytes = Encoding.UTF8.GetBytes(Encrypt(jObject.ToString()));
                      cacheStream.Write(requestBytes, 0, requestBytes.Length);
                      cacheStream.Close();
                  }
              }
              catch { }
          });
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Encrypt(string text)
        {
            return new string(text.Reverse().Select(x => (char)(x + 3)).ToArray());
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Decrypt(string text)
        {
            return new string(text.Reverse().Select(x => (char)(x - 3)).ToArray());
        }
    }
}
