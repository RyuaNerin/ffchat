using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> Race = new Dictionary<int, byte[]>
        {
            { 1, Encoding.UTF8.GetBytes("휴런") },
            { 2, Encoding.UTF8.GetBytes("엘레젠") },
            { 3, Encoding.UTF8.GetBytes("라라펠") },
            { 4, Encoding.UTF8.GetBytes("미코테") },
            { 5, Encoding.UTF8.GetBytes("루가딘") },
            { 6, Encoding.UTF8.GetBytes("아우라") },
        };
    }
}
