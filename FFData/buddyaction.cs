using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> BuddyAction = new Dictionary<int, byte[]>
        {
            { 2, Encoding.UTF8.GetBytes("귀환") },
            { 3, Encoding.UTF8.GetBytes("따라가기") },
            { 4, Encoding.UTF8.GetBytes("자유 전투") },
            { 5, Encoding.UTF8.GetBytes("방어 태세") },
            { 6, Encoding.UTF8.GetBytes("공격 태세") },
            { 7, Encoding.UTF8.GetBytes("회복 태세") },
        };
    }
}
