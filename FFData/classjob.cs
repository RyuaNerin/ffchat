using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> ClassJob = new Dictionary<int, byte[]>
        {
            { 0, Encoding.UTF8.GetBytes("모험가") },
            { 1, Encoding.UTF8.GetBytes("검술사") },
            { 2, Encoding.UTF8.GetBytes("격투사") },
            { 3, Encoding.UTF8.GetBytes("도끼술사") },
            { 4, Encoding.UTF8.GetBytes("창술사") },
            { 5, Encoding.UTF8.GetBytes("궁술사") },
            { 6, Encoding.UTF8.GetBytes("환술사") },
            { 7, Encoding.UTF8.GetBytes("주술사") },
            { 8, Encoding.UTF8.GetBytes("목수") },
            { 9, Encoding.UTF8.GetBytes("대장장이") },
            { 10, Encoding.UTF8.GetBytes("갑주제작사") },
            { 11, Encoding.UTF8.GetBytes("보석공예가") },
            { 12, Encoding.UTF8.GetBytes("가죽공예가") },
            { 13, Encoding.UTF8.GetBytes("재봉사") },
            { 14, Encoding.UTF8.GetBytes("연금술사") },
            { 15, Encoding.UTF8.GetBytes("요리사") },
            { 16, Encoding.UTF8.GetBytes("광부") },
            { 17, Encoding.UTF8.GetBytes("원예가") },
            { 18, Encoding.UTF8.GetBytes("어부") },
            { 19, Encoding.UTF8.GetBytes("나이트") },
            { 20, Encoding.UTF8.GetBytes("몽크") },
            { 21, Encoding.UTF8.GetBytes("전사") },
            { 22, Encoding.UTF8.GetBytes("용기사") },
            { 23, Encoding.UTF8.GetBytes("음유시인") },
            { 24, Encoding.UTF8.GetBytes("백마도사") },
            { 25, Encoding.UTF8.GetBytes("흑마도사") },
            { 26, Encoding.UTF8.GetBytes("비술사") },
            { 27, Encoding.UTF8.GetBytes("소환사") },
            { 28, Encoding.UTF8.GetBytes("학자") },
            { 29, Encoding.UTF8.GetBytes("쌍검사") },
            { 30, Encoding.UTF8.GetBytes("닌자") },
            { 31, Encoding.UTF8.GetBytes("기공사") },
            { 32, Encoding.UTF8.GetBytes("암흑기사") },
            { 33, Encoding.UTF8.GetBytes("점성술사") },
        };
    }
}
