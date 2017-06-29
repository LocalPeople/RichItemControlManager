using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserUI.Data;

namespace UserUI.Event
{
    public class SectionChangedEventArgs : EventArgs
    {
        public Section NewSection { get; set; }
        public Section OldSection { get; set; }

        public SectionChangedEventArgs(Section newSection, Section oldSection)
        {
            NewSection = newSection;
            OldSection = oldSection;
        }
    }
}
