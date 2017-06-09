using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RichItemsControlManager
{
    static class VisualUtil
    {
        public static List<TResult> FindChildren<TResult>(DependencyObject reference) where TResult : DependencyObject
        {
            List<TResult> result = new List<TResult>();
            List<DependencyObject> queue = new List<DependencyObject>();
            queue.Add(reference);
            for (int head = 0; head < queue.Count; head++)
            {
                if (queue[head] is TResult)
                {
                    result.Add(queue[head] as TResult);
                }
                int count = VisualTreeHelper.GetChildrenCount(queue[head]);
                for (int idx = 0; idx < count; idx++)
                {
                    queue.Add(VisualTreeHelper.GetChild(queue[head], idx));
                }
            }
            return result;
        }
    }
}
