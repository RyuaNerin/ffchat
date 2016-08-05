﻿using System.Collections.Generic;
using System.Text;

namespace FFChat
{
    static partial class FFData
    {
        public static readonly IDictionary<int, byte[]> MainCommand = new Dictionary<int, byte[]>
        {
            { 1, Encoding.UTF8.GetBytes("무기 넣기/꺼내기") },
            { 2, Encoding.UTF8.GetBytes("내 캐릭터") },
            { 3, Encoding.UTF8.GetBytes("기술 목록") },
            { 4, Encoding.UTF8.GetBytes("퀘스트") },
            { 5, Encoding.UTF8.GetBytes("임무 정보 목록") },
            { 6, Encoding.UTF8.GetBytes("업적 목록") },
            { 7, Encoding.UTF8.GetBytes("채집수첩") },
            { 8, Encoding.UTF8.GetBytes("토벌수첩") },
            { 9, Encoding.UTF8.GetBytes("제작수첩") },
            { 10, Encoding.UTF8.GetBytes("소지품") },
            { 11, Encoding.UTF8.GetBytes("임무용 아이템 목록") },
            { 12, Encoding.UTF8.GetBytes("파티원") },
            { 13, Encoding.UTF8.GetBytes("친구 목록 관리") },
            { 14, Encoding.UTF8.GetBytes("차단 목록 관리") },
            { 15, Encoding.UTF8.GetBytes("플레이어 검색") },
            { 16, Encoding.UTF8.GetBytes("지도") },
            { 17, Encoding.UTF8.GetBytes("감정 표현") },
            { 18, Encoding.UTF8.GetBytes("대상 표식") },
            { 19, Encoding.UTF8.GetBytes("시스템 설정") },
            { 20, Encoding.UTF8.GetBytes("단축키 설정") },
            { 21, Encoding.UTF8.GetBytes("매크로 관리") },
            { 22, Encoding.UTF8.GetBytes("HUD 배열 변경") },
            { 23, Encoding.UTF8.GetBytes("접속 종료") },
            { 24, Encoding.UTF8.GetBytes("게임 종료") },
            { 25, Encoding.UTF8.GetBytes("장비함") },
            { 27, Encoding.UTF8.GetBytes("자유부대") },
            { 28, Encoding.UTF8.GetBytes("링크셸") },
            { 29, Encoding.UTF8.GetBytes("낚시수첩") },
            { 30, Encoding.UTF8.GetBytes("도움말") },
            { 31, Encoding.UTF8.GetBytes("추천 임무") },
            { 33, Encoding.UTF8.GetBytes("임무 찾기") },
            { 34, Encoding.UTF8.GetBytes("캐릭터 설정") },
            { 35, Encoding.UTF8.GetBytes("텔레포") },
            { 36, Encoding.UTF8.GetBytes("데존") },
            { 37, Encoding.UTF8.GetBytes("등급 및 라이선스 표시") },
            { 38, Encoding.UTF8.GetBytes("수첩") },
            { 39, Encoding.UTF8.GetBytes("플레이어 간 교류") },
            { 40, Encoding.UTF8.GetBytes("시스템") },
            { 41, Encoding.UTF8.GetBytes("어류도감") },
            { 42, Encoding.UTF8.GetBytes("버디") },
            { 43, Encoding.UTF8.GetBytes("임무") },
            { 44, Encoding.UTF8.GetBytes("하우징") },
            { 45, Encoding.UTF8.GetBytes("화면 설정") },
            { 46, Encoding.UTF8.GetBytes("소리 설정") },
            { 47, Encoding.UTF8.GetBytes("그래픽 설정") },
            { 48, Encoding.UTF8.GetBytes("마우스 설정") },
            { 49, Encoding.UTF8.GetBytes("<if(PLYR: 76 ? 1 ) {컨트롤러 설정} else {게임패드 설정}>") },
            { 50, Encoding.UTF8.GetBytes("기타 설정") },
            { 51, Encoding.UTF8.GetBytes("조작 설정") },
            { 52, Encoding.UTF8.GetBytes("사용자 인터페이스 설정") },
            { 53, Encoding.UTF8.GetBytes("이름 표시 설정") },
            { 54, Encoding.UTF8.GetBytes("단축바 설정") },
            { 55, Encoding.UTF8.GetBytes("대화창 설정") },
            { 56, Encoding.UTF8.GetBytes("PvP 프로필") },
            { 57, Encoding.UTF8.GetBytes("파티 찾기") },
            { 58, Encoding.UTF8.GetBytes("지면 표식") },
            { 59, Encoding.UTF8.GetBytes("준비 확인") },
            { 60, Encoding.UTF8.GetBytes("공략수첩") },
            { 61, Encoding.UTF8.GetBytes("탈것 목록") },
            { 62, Encoding.UTF8.GetBytes("꼬마 친구 목록") },
            { 63, Encoding.UTF8.GetBytes("접근성 설정") },
            { 64, Encoding.UTF8.GetBytes("탐험수첩") },
            { 65, Encoding.UTF8.GetBytes("골드 소서") },
            { 66, Encoding.UTF8.GetBytes("화폐 목록") },
            { 67, Encoding.UTF8.GetBytes("풍맥의 샘 교감 정보") },
            { 68, Encoding.UTF8.GetBytes("공식 가이드") },
        };
    }
}
