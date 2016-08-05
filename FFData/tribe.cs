using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> Tribe = new Dictionary<int, byte[]>
        {
            { 1, Encoding.UTF8.GetBytes("중원 부족") },
            { 2, Encoding.UTF8.GetBytes("고원 부족") },
            { 3, Encoding.UTF8.GetBytes("숲 부족") },
            { 4, Encoding.UTF8.GetBytes("황혼 부족") },
            { 5, Encoding.UTF8.GetBytes("평원 부족") },
            { 6, Encoding.UTF8.GetBytes("사막 부족") },
            { 7, Encoding.UTF8.GetBytes("태양의 추종자") },
            { 8, Encoding.UTF8.GetBytes("달의 수호자") },
            { 9, Encoding.UTF8.GetBytes("바다늑대") },
            { 10, Encoding.UTF8.GetBytes("불꽃지킴이") },
            { 11, Encoding.UTF8.GetBytes("아우라 렌") },
            { 12, Encoding.UTF8.GetBytes("아우라 젤라") },
        };
    }
}
