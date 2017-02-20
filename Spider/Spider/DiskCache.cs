using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spider
{
    /// <summary>
    /// 文件
    /// </summary>
    public class File
    {
        /// <summary>
        /// 文件保存路径全名
        /// </summary>
        public string FileName{get;set;}
        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] FileContent{get;set;}
        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool Override{get;set;}

        public File()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">文件路径名</param>
        /// <param name="fileContent">文件内容</param>
        public File(string fileName, byte[] fileContent)
        {
            FileName = fileName;
            FileContent = fileContent;
        }
        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            return System.IO.File.Exists(FileName);
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        public void Save()
        {
            if (Override || !Exists())
                System.IO.File.WriteAllBytes(FileName, FileContent);
        }
    }
    /// <summary>
    /// 磁盘缓存
    /// </summary>
    public class DiskCache
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private Cache<File> _cache = new Cache<File>();
        /// <summary>
        /// 启动后台写文件
        /// </summary>
        public void Begin()
        {
            _cache.Begin(WriteFile);
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="file"></param>
        public void Write(File file)
        {
            _cache.Write(file);
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="files"></param>
        public void Write(IEnumerable<File> files)
        {
            _cache.Write(files);
        }
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        public void Write(string fileName, byte[] fileContent)
        {
            _cache.Write(new File(fileName, fileContent));
        }

        private void WriteFile(File file)
        {
            file.Save();
        }
        /// <summary>
        /// 停止写文件
        /// </summary>
        public void End()
        {
            _cache.End();
        }
    }
}
