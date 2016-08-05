using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> Pet = new Dictionary<int, byte[]>
        {
            { 1, Encoding.UTF8.GetBytes("카벙클 에메랄드") },
            { 2, Encoding.UTF8.GetBytes("카벙클 토파즈") },
            { 3, Encoding.UTF8.GetBytes("이프리트 에기") },
            { 4, Encoding.UTF8.GetBytes("타이탄 에기") },
            { 5, Encoding.UTF8.GetBytes("가루다 에기") },
            { 6, Encoding.UTF8.GetBytes("요정 에오스") },
            { 7, Encoding.UTF8.GetBytes("요정 셀레네") },
            { 8, Encoding.UTF8.GetBytes("자동포탑 룩") },
            { 9, Encoding.UTF8.GetBytes("자동포탑 비숍") },
        };
    }
}
