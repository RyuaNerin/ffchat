using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> GeneralAction = new Dictionary<int, byte[]>
        {
            { 1, Encoding.UTF8.GetBytes("자동 공격") },
            { 2, Encoding.UTF8.GetBytes("점프") },
            { 3, Encoding.UTF8.GetBytes("리미트 브레이크") },
            { 4, Encoding.UTF8.GetBytes("전력 질주") },
            { 5, Encoding.UTF8.GetBytes("분해") },
            { 6, Encoding.UTF8.GetBytes("수리") },
            { 7, Encoding.UTF8.GetBytes("텔레포") },
            { 8, Encoding.UTF8.GetBytes("데존") },
            { 9, Encoding.UTF8.GetBytes("탈것 무작위 호출") },
            { 10, Encoding.UTF8.GetBytes("꼬마 친구 무작위 호출") },
            { 12, Encoding.UTF8.GetBytes("마테리아 장착") },
            { 13, Encoding.UTF8.GetBytes("마테리아 장착 2") },
            { 14, Encoding.UTF8.GetBytes("마테리아화") },
            { 15, Encoding.UTF8.GetBytes("염색") },
            { 16, Encoding.UTF8.GetBytes("조준: 순서대로") },
            { 17, Encoding.UTF8.GetBytes("조준: 역순으로") },
            { 18, Encoding.UTF8.GetBytes("설치") },
            { 19, Encoding.UTF8.GetBytes("지도 해독") },
            { 20, Encoding.UTF8.GetBytes("탐색") },
            { 21, Encoding.UTF8.GetBytes("정제") },
            { 22, Encoding.UTF8.GetBytes("장비 투영") },
            { 23, Encoding.UTF8.GetBytes("내리기") },
        };
    }
}
