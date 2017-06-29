using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserUI.Data;

namespace UserUI.Util
{
    static class DataReader
    {
        public static Section Read(string root, int depth)
        {
            if (!Directory.Exists(root))
                throw new DirectoryNotFoundException(string.Format("指定目录不存在:\n{0}", root));
            DirectoryInfo rootInfo = new DirectoryInfo(root);
            return Read(rootInfo, depth);
        }

        public static Section Read(DirectoryInfo root, int depth)
        {
            Section rootSection = new Section();
            Queue<Section> BFSQueue = new Queue<Section>();
            foreach (DirectoryInfo child in root.GetDirectories())
            {
                BFSQueue.Enqueue(new Section(child, 1, rootSection));
            }
            while (BFSQueue.Count > 0)
            {
                Section section = BFSQueue.Dequeue();
                section.Parent.Group.Add(section);
                if ((depth > 0 && section.Depth >= depth) || Section.CanStopSearch(section.Directory))
                {
                    section.SetConfiguration(new SectionConfiguration(section.Directory));
                    section.SetIsLeap();
                }
                else
                {
                    foreach (DirectoryInfo child in section.Directory.GetDirectories())
                    {
                        BFSQueue.Enqueue(new Section(child, section.Depth + 1, section));
                    }
                }
            }
            return rootSection;
        }
    }
}
