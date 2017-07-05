using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace UserUI.Data
{
    public class Section
    {
        public static List<Section> EmptyGroup = new List<Section>();

        SectionConfiguration configuration;
        int depth;
        DirectoryInfo directory;
        List<Section> group;
        Section parent;
        bool isLeap;

        public SectionConfiguration Configuration { get { return configuration; } }
        public int Depth { get { return depth; } }
        public DirectoryInfo Directory { get { return directory; } }
        public List<Section> Group
        {
            get
            {
                if (group == null)  
                    group = new List<Section>();
                return group;
            }
        }
        public string Name { get { return directory.Name; } }
        public Section Parent { get { return parent; } }
        public bool IsLeap { get { return isLeap; } }

        public Section()
        {
        }

        public Section(DirectoryInfo directory, int depth, Section parent)
        {
            this.directory = directory;
            this.depth = depth;
            this.parent = parent;
            isLeap = false;
        }

        public Section(DirectoryInfo directory, int depth, Section parent, SectionConfiguration configuration) : this(directory, depth, parent)
        {
            this.configuration = configuration;
        }

        public Section(DirectoryInfo directory, int depth, Section parent, SectionConfiguration configuration, IEnumerable<Section> group) : this(directory, depth, parent, configuration)
        {
            this.group = new List<Section>(group);
        }

        public void SetConfiguration(SectionConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Image))
            {
                MessageBox.Show("发现错误:\n\n" + directory.FullName, "文件夹结构有误……");
            }
            this.configuration = configuration;
        }

        public static bool CanStopSearch(DirectoryInfo directory)
        {
            bool canStopSearch = false;
            DirectoryInfo[] childDirectory = directory.GetDirectories();
            if (childDirectory.Length == 1 && childDirectory[0].Name == "Image")
            {
                canStopSearch = true;
            }
            FileInfo[] childFile = directory.GetFiles();
            if (childFile.Length == 1 && Regex.IsMatch(childFile[0].Name, directory.Name + @"\.(?:jpg|jpeg|png|bmp)"))
            {
                canStopSearch = true;
            }
            return canStopSearch;
        }

        public void SetIsLeap()
        {
            isLeap = true;
        }

        public string[] GetRelation()
        {
            string[] relation = new string[depth];
            RelationSetting(this, relation);
            return relation;
        }

        private void RelationSetting(Section section, string[] relation)
        {
            if (section.parent.depth > 0)
            {
                RelationSetting(section.parent, relation);
            }
            relation[section.depth - 1] = section.Name;
        }
    }
}
