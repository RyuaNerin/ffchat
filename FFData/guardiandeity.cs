using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> GuardianDeity = new Dictionary<int, byte[]>
        {
            { 1, Encoding.UTF8.GetBytes("할로네") },
            { 2, Encoding.UTF8.GetBytes("메느피나") },
            { 3, Encoding.UTF8.GetBytes("살리아크") },
            { 4, Encoding.UTF8.GetBytes("니메이아") },
            { 5, Encoding.UTF8.GetBytes("리믈렌") },
            { 6, Encoding.UTF8.GetBytes("오쉬온") },
            { 7, Encoding.UTF8.GetBytes("비레고") },
            { 8, Encoding.UTF8.GetBytes("랄거") },
            { 9, Encoding.UTF8.GetBytes("아제마") },
            { 10, Encoding.UTF8.GetBytes("날달") },
            { 11, Encoding.UTF8.GetBytes("노피카") },
            { 12, Encoding.UTF8.GetBytes("알디크") },
        };
    }
}
