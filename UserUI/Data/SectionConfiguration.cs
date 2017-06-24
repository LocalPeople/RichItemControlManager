using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UserUI.Data
{
    public class SectionConfiguration
    {
        string image;
        string file;

        public string Image { get { return image; } }
        public string File { get { return file; } }

        public SectionConfiguration(DirectoryInfo directory)
        {
            var package = from d in directory.GetDirectories() where d.Name == "Image" select d;
            DirectoryInfo imagePackage = package.FirstOrDefault();
            if (null != imagePackage)
            {
                image = GetFile(imagePackage, directory.Name + @"\.(?:jpg|jpeg|png|bmp)");
            }
            else
            {
                image = GetFile(directory, directory.Name + @"\.(?:jpg|jpeg|png|bmp)");
            }
            file = GetFile(directory, directory.Name + @"\.xml");
        }

        private string GetFile(DirectoryInfo directory, string pattern)
        {
            string result = string.Empty;
            var file = from f in directory.GetFiles() where Regex.IsMatch(f.Name, pattern) select f;
            FileInfo fileInfo = file.FirstOrDefault();
            if (null != fileInfo)
            {
                result = fileInfo.FullName;
            }
            return result;
        }
    }
}
