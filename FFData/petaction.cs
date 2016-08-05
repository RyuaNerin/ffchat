using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> PetAction = new Dictionary<int, byte[]>
        {
            { 1, Encoding.UTF8.GetBytes("귀환") },
            { 2, Encoding.UTF8.GetBytes("추종") },
            { 3, Encoding.UTF8.GetBytes("이동") },
            { 4, Encoding.UTF8.GetBytes("대기") },
            { 5, Encoding.UTF8.GetBytes("공격") },
            { 6, Encoding.UTF8.GetBytes("평화") },
            { 7, Encoding.UTF8.GetBytes("자유 전투") },
            { 8, Encoding.UTF8.GetBytes("지시 전투") },
            { 9, Encoding.UTF8.GetBytes("돌풍") },
            { 10, Encoding.UTF8.GetBytes("날리기") },
            { 11, Encoding.UTF8.GetBytes("하강 기류") },
            { 12, Encoding.UTF8.GetBytes("에메랄드의 광휘") },
            { 13, Encoding.UTF8.GetBytes("할퀴기") },
            { 14, Encoding.UTF8.GetBytes("토파즈의 빛") },
            { 15, Encoding.UTF8.GetBytes("웅크리기") },
            { 16, Encoding.UTF8.GetBytes("몸통 박치기") },
            { 17, Encoding.UTF8.GetBytes("바람의 칼날") },
            { 18, Encoding.UTF8.GetBytes("충격파") },
            { 19, Encoding.UTF8.GetBytes("대기 난도질") },
            { 20, Encoding.UTF8.GetBytes("악화") },
            { 21, Encoding.UTF8.GetBytes("대기 폭발") },
            { 22, Encoding.UTF8.GetBytes("바위 쪼개기") },
            { 23, Encoding.UTF8.GetBytes("산 쪼개기") },
            { 24, Encoding.UTF8.GetBytes("대지의 수호") },
            { 25, Encoding.UTF8.GetBytes("산사태") },
            { 26, Encoding.UTF8.GetBytes("대지의 분노") },
            { 27, Encoding.UTF8.GetBytes("진홍 회오리") },
            { 28, Encoding.UTF8.GetBytes("불타는 일격") },
            { 29, Encoding.UTF8.GetBytes("광휘의 방패") },
            { 30, Encoding.UTF8.GetBytes("화염 작열") },
            { 31, Encoding.UTF8.GetBytes("지옥의 화염") },
            { 32, Encoding.UTF8.GetBytes("빛의 치유") },
            { 33, Encoding.UTF8.GetBytes("빛의 속삭임") },
            { 34, Encoding.UTF8.GetBytes("요정의 서약") },
            { 35, Encoding.UTF8.GetBytes("요정의 광휘") },
            { 36, Encoding.UTF8.GetBytes("빛의 치유") },
            { 37, Encoding.UTF8.GetBytes("빛의 침묵") },
            { 38, Encoding.UTF8.GetBytes("요정의 손길") },
            { 39, Encoding.UTF8.GetBytes("요정의 바람") },
            { 40, Encoding.UTF8.GetBytes("공격 지시") },
            { 41, Encoding.UTF8.GetBytes("보조 지시") },
        };
    }
}
