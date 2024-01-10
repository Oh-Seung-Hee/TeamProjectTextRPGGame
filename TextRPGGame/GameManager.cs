﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TextRPGGame
{
    // 게임의 전반적인 관리
    // 1. 게임 시작
    // 2. 행동 선택
    // 3.
    class GameManager
    {
        #region GameManager
        // 싱글톤 방식으로 하나의 GameManager만 구현한다.
        // GameManager은 곳곳에서 불러올 수가 있다.
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }
        #endregion

        public Player player;
        public List<Monster> monsters;
        public List<Monster> spawnedMonsters = new List<Monster>();
        public int action;

        private Random random;

        public GameManager()
        {
            random = new Random();

            player = new Player("Chad", "전사", 10, 5, 100);

            monsters = new List<Monster>
            {
                new Monster("미니언", 2, 15, 5),
                new Monster("공허충", 3, 10, 9),
                new Monster("대포미니언", 5, 25, 8)
            };

        }


        public void GameStart()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("00던전에 오신 것을 환영합니다");
                Console.WriteLine("이제 전투를 시작할 수 있습니다\n");
                Utill.WriteRedText("1. ");
                Console.WriteLine("상태 보기");
                Utill.WriteRedText("2. ");
                Console.WriteLine("전투 시작");
                // 원하는 행동 선택
                SetNextAction(1, 2);

                switch (action)
                {
                    case 1:
                        ShowStatus();
                        break;
                    case 2:
                        SpawnMonster();
                        StartBattle();
                        break;
                }
            }
        }

        void ShowStatus()
        {
            Console.Clear();
            Console.WriteLine("캐릭터의 정보가 표시 됩니다.");

            player.ShowStatus();

            Utill.WriteRedText("0. ");
            Console.WriteLine("나가기");
            SetNextAction(0, 0);
        }

        void StartBattle()
        {
            Console.Clear();
            Utill.WriteOrangeText("Battle!!\n");
            Console.WriteLine();

            // 몬스터 정보 표시
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].ShowStatus();
            }

            // 플레이어 정보 표시
            Console.WriteLine("\n[내정보]");
            Console.WriteLine($"Lv.{player.Level} {player.Name} ({player.Class})");
            Console.WriteLine($"HP {player.MaxHp}/{player.Hp}\n");

            Utill.WriteRedText("1. ");
            Console.WriteLine("공격");

            SetNextAction(1, 1);

            switch (action)
            {
                case 1:
                    Battle();
                    break;
            }
        }

        void SpawnMonster()
        {
            int monsterCount = random.Next(1, 5);
            for (int i = 0; i < monsterCount; i++)
            {
                int monsterIndex = random.Next(monsters.Count);
                spawnedMonsters.Add(monsters[monsterIndex]);
            }
        }
        void Battle()
        {
            while (true)
            {
                Console.Clear();
                Utill.WriteOrangeText("Battle!!\n");
                Console.WriteLine();

                // 몬스터 정보 표시
                for (int i = 0; i < monsters.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    if (monsters[i].IsDead)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write($"{i + 1} ");
                    monsters[i].ShowStatus();
                }

                // 플레이어 정보 표시
                Console.WriteLine("\n[내정보]");
                Console.WriteLine($"Lv.{player.Level} {player.Name} ({player.Class})");
                Console.WriteLine($"HP {player.MaxHp}/{player.Hp}\n");

                Utill.WriteRedText("0. ");
                Console.WriteLine("취소");

                SetNextAction(0, monsters.Count);

                if (action == 0)
                {
                    StartBattle();
                    return;
                }

                Monster selectedMonster = monsters[action - 1];
                selectedMonster.Attacked(player.Attack);

                BattleResultMessage(selectedMonster);
            }
        }

        void BattleResultMessage(Monster monster)
        {
            
        }

        #region 행동 선택
        void SetNextAction(int minValue, int maxValue)
        {
            Console.WriteLine("\n원하는 행동을 입력해 주세요");
            while (IsInValidAction(minValue, maxValue) == false) { Console.WriteLine("잘못된 입력입니다 다시 선택해 주세요"); }
            Console.WriteLine();
        }
        bool IsInValidAction(int minValue, int maxValue)
        {
            if (int.TryParse(Console.ReadLine(), out action) == false) return false;

            if (action > maxValue || action < minValue) return false;

            return true;
        }
        #endregion
    }
}
