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
                new Monster("미니언", 2, 10, 5),
                new Monster("공허충", 3, 10, 5),
                new Monster("대포미니언", 5, 10, 5)
            };

        }


        public void GameStart()
        {
            while (true)
            {
                // 몬스터 초기화
                spawnedMonsters.Clear();

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
            Console.Write($"Lv. ");
            Utill.WriteRedText($"{player.Level}");
            Console.WriteLine($" {player.Name} ({player.Class})");
            Console.Write($"HP ");
            Utill.WriteRedText($"{player.MaxHp}");
            Utill.WriteRedText($"/{player.Hp}\n\n");

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
                spawnedMonsters.Add(monsters[monsterIndex].DeepCopy()) ;
            }
        }
        void Battle()
        {
            while (true)
            {
                Console.Clear();
                Utill.WriteOrangeText("Battle!!\n");
                Console.WriteLine();

                int deadMonsterCount = 0;

                // 몬스터 정보 표시
                for (int i = 0; i < spawnedMonsters.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    if (spawnedMonsters[i].IsDead)
                    {
                        deadMonsterCount++;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write($"{i + 1} ");
                    spawnedMonsters[i].ShowStatus();
                }

                // 모든 몬스터가 죽었으면 게임종료
                if(deadMonsterCount == spawnedMonsters.Count)
                {
                    Victory(deadMonsterCount);
                    return;
                }
                // 내 체력이 0이되면 게임종료
                if (player.IsDead)
                {
                    Lose();
                    return;
                }
                // 플레이어 정보 표시
                Console.WriteLine("\n[내정보]");
                Console.Write($"Lv. ");
                Utill.WriteRedText($"{player.Level}");
                Console.WriteLine($" {player.Name} ({player.Class})");
                Console.Write($"HP ");
                Utill.WriteRedText($"{player.MaxHp}");
                Utill.WriteRedText($"/{player.Hp}\n\n");

                Utill.WriteRedText("0. ");
                Console.WriteLine("취소");

                SetNextAction(0, spawnedMonsters.Count);

                if (action == 0)
                {
                    StartBattle();
                    return;
                }

                // 몬스터 선택
                Monster selectedMonster = spawnedMonsters[action - 1];
                while(selectedMonster.IsDead)
                {
                    Console.WriteLine("이미 죽은 몬스터 입니다");
                    SetNextAction(0, spawnedMonsters.Count);
                    selectedMonster = spawnedMonsters[action - 1];
                }
                int damage = player.FinalAttack;
                selectedMonster.Attacked(damage);

                PlayerAttackResultMessage(selectedMonster, damage);
            }
        }
        void Victory(int deadMonsterCount)
        {
            Console.Clear();
            Utill.WriteOrangeText("Battle!! - Result\n");
            Console.WriteLine();

            Utill.WriteBlueText("Victory\n");

            Console.Write("던전에서 몬스터 ");
            Utill.WriteRedText($"{deadMonsterCount}");
            Console.WriteLine("마리를 잡았습니다.\n");

            // 플레이어 정보 표시
            Console.WriteLine("\n[내정보]");
            Console.Write($"Lv. ");
            Utill.WriteRedText($"{player.Level}");
            Console.WriteLine($" {player.Name} ({player.Class})");
            Console.Write($"HP ");
            Utill.WriteRedText($"{player.MaxHp}");
            Utill.WriteRedText($"/{player.Hp}\n\n");

            Utill.WriteRedText("0. ");
            Console.WriteLine("다음");

            SetNextAction(0, 0);
        }
        void Lose()
        {
            Console.Clear();
            Utill.WriteOrangeText("Battle!! - Result\n");
            Console.WriteLine();

            Utill.WriteRedText("You Lose\n");

            // 플레이어 정보 표시
            Console.WriteLine("\n[내정보]");
            Console.Write($"Lv. ");
            Utill.WriteRedText($"{player.Level}");
            Console.WriteLine($" {player.Name} ({player.Class})");
            Console.Write($"HP ");
            Utill.WriteRedText($"{player.MaxHp}");
            Utill.WriteRedText($"/{player.Hp}\n\n");

            Utill.WriteRedText("0. ");
            Console.WriteLine("다음");

            SetNextAction(0, 0);
        }

        void PlayerAttackResultMessage(Monster monster, int damage)
        {
            Console.Clear();
            Utill.WriteOrangeText("Battle!!\n");
            Console.WriteLine();

            // 플레이어 공격 표시
            Console.WriteLine($"{player.Name}의 공격!");
            Console.Write("Lv.");
            Utill.WriteRedText($"{monster.Level} ");
            Console.Write($"{monster.Name}을(를) 맞췄습니다. [데미지 : ");
            Utill.WriteRedText($"{damage}");
            Console.WriteLine("]\n");

            // 공격 결과 표시
            Console.Write("Lv.");
            Utill.WriteRedText($"{monster.Level} ");
            Console.WriteLine($"{monster.Name}");
            Console.Write("Hp ");
            Utill.WriteRedText($"{monster.Hp + damage} ");
            Console.WriteLine($"-> {(monster.IsDead ? "Dead" : monster.Hp)}\n");

            Utill.WriteRedText("0. ");
            Console.WriteLine("다음");

            SetNextAction(0, 0);

            switch (action)
            {
                case 0:
                    MonsterAttack();
                    break;
                default:
                    break;
            }
        }

        void MonsterAttack()
        {
            foreach(Monster monster in spawnedMonsters)
            {
                if(monster.IsDead == false)
                {
                    player.Attacked(monster.Attack);
                    MonsterAttackResultMessage(monster, monster.Attack);
                    if (player.IsDead)
                        return;
                }
            }
        }

        void MonsterAttackResultMessage(Monster monster, int damage)
        {
            Console.Clear();
            Utill.WriteOrangeText("Battle!!\n");
            Console.WriteLine();

            // 플레이어 공격 표시
            Console.WriteLine($"{monster.Name}의 공격!");
            Console.Write("Lv.");
            Utill.WriteRedText($"{player.Level} ");
            Console.Write($"{player.Name}을(를) 맞췄습니다. [데미지 : ");
            Utill.WriteRedText($"{damage}");
            Console.WriteLine("]\n");

            // 공격 결과 표시
            Console.Write("Lv.");
            Utill.WriteRedText($"{player.Level} ");
            Console.WriteLine($"{player.Name}");
            Console.Write("Hp ");
            Utill.WriteRedText($"{player.Hp + damage} ");
            Console.WriteLine($"-> {(player.IsDead ? "Dead" : player.Hp)}\n");

            Utill.WriteRedText("0. ");
            Console.WriteLine("다음");

            SetNextAction(0, 0);
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
