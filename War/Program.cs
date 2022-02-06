using System;
using System.Collections.Generic;
using System.Threading;

namespace War
{
    class Program
    {
        static void Main(string[] args)
        {
            int serialNumber = 1;

            List<Warrior> allWarriors = new List<Warrior>()
            {
                new Knight(serialNumber++, "Белый рыцарь", 200, 35, "клинок света", 80),
                new Knight(serialNumber++, "Темный рыцарь", 180, 45, "клинок тьмы", 90),
                new Archer(serialNumber++, "Безумный лучник", 170, 45, "поток стрел", 100),
                new Archer(serialNumber++, "Эльфийский лучник", 160, 50, "стрела древних", 105),
                new Wizard(serialNumber++, "Ледяной маг", 150, 55, "ледяной шторм", 110),
                new Wizard(serialNumber++, "Маг дыма", 190, 40, "отравленное облако", 85),
                new Beast(serialNumber++, "Огнедышащий дракон", 205, 40, "огненное дыхание", 70),
                new Beast(serialNumber++, "Гигантский медведь", 200, 50, "когти разрушения", 65)
            };

            Battlefield battlefield = new Battlefield(allWarriors);

            Console.Clear();
            Console.WriteLine("Чтобы начать битву, нажмите любую клавишу");
            Console.WriteLine("Для выхода нажмите ESC");
            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    break;
                default:
                    battlefield.StartBattle();
                    break;
            }
        }
    }

    class Battlefield
    {
        private List<Warrior> _allWarriors;
        private Squad _firstSquad;
        private Squad _secondSquad;

        public Battlefield(List<Warrior> allWarriors)
        {
            _allWarriors = allWarriors;
            _firstSquad = new Squad();
            _secondSquad = new Squad();
        }

        public void StartBattle()
        {
            _firstSquad.Create(_allWarriors, 1);
            Console.Clear();
            _secondSquad.Create(_allWarriors, 2);

            bool isFight = true;
            int firstCounter = 0;
            int secondCounter = -1;

            while (isFight)
            {
                Console.Clear();
                _firstSquad.ShowInfo(1);
                _secondSquad.ShowInfo(2);

                if (_firstSquad.IsAlive())
                {
                    PrintBattleCourse(1);
                    Attack(_firstSquad, _secondSquad, firstCounter);
                    secondCounter = ChangeCounter(secondCounter, _secondSquad);
                }
                if (_secondSquad.IsAlive())
                {
                    PrintBattleCourse(2);
                    Attack(_secondSquad, _firstSquad, firstCounter);
                    firstCounter = ChangeCounter(firstCounter, _firstSquad);
                    Thread.Sleep(1500);
                }
                else
                {
                    isFight = false;
                }
            }

            Console.WriteLine();
            DefineWinner();
            Console.ReadKey();
        }

        private void PrintBattleCourse(int squadNumber)
        {
            Thread.Sleep(500);
            Console.WriteLine($"\n{squadNumber} отряд наносит удар: ");
        }

        private void Attack(Squad attackSquad, Squad defenseSquad, int queue)
        {
            Random random = new Random();

            int target = random.Next(0, defenseSquad.Count);
            Warrior defenseWarrior = defenseSquad.FindWarrior(target);
            Warrior attackWarrior = attackSquad.FindWarrior(queue);

            defenseWarrior.TakeDamage(attackWarrior.GetDamage());

            defenseSquad.RemoveDeadWarrior();
        }

        private void PrintColorMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void DefineWinner()
        {
            if (_firstSquad.Count > 0)
            {
                PrintColorMessage("\nПобедил отряд номер 1", ConsoleColor.Blue);
            }
            else
            {
                PrintColorMessage("\nПобедил отряд номер 2", ConsoleColor.Blue);
            }
        }

        private int ChangeCounter(int counter, Squad squad)
        {
            if (counter >= squad.Count - 1)
            {
                counter = 0;
            }
            else
            {
                counter++;
            }

            return counter;
        }
    }

    class Squad
    {
        private List<Warrior> _warriors;

        private int _size;

        public Squad()
        {
            _size = 4;
            _warriors = new List<Warrior>();
        }

        public int Count => _warriors.Count;

        public void Create(List<Warrior> allWarriors, int number)
        {
            PrintInfoForCreate(number);

            int squadCount = 0;
            int lastSerialNumber = allWarriors[allWarriors.Count - 1].SerialNumber;

            while (squadCount != _size)
            {
                foreach (Warrior warrior in allWarriors)
                {
                    warrior.ShowInfo();
                }

                string userInput = Console.ReadLine();
                bool success = int.TryParse(userInput, out int serialNumber);
                bool isFind = success && serialNumber > 0 && serialNumber <= lastSerialNumber;
                Warrior selectedWarrior = null;

                if (isFind)
                {
                    foreach (Warrior warrior in allWarriors)
                    {
                        if (warrior.SerialNumber == serialNumber)
                        {
                            selectedWarrior = warrior;
                            _warriors.Add(warrior);
                            allWarriors.Remove(selectedWarrior);
                            squadCount++;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Воин успешно добавлен. В отряде {squadCount} воинов");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Такого воина нет в списке");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        public Warrior FindWarrior(int number)
        {
            return _warriors[number];
        }

        public void RemoveDeadWarrior()
        {
            Warrior deadWarrior = null;

            foreach (Warrior warrior in _warriors)
            {
                if (warrior.IsDead)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{warrior.Name} погиб");
                    Console.ForegroundColor = ConsoleColor.White;
                    deadWarrior = warrior;
                }
            }

            _warriors.Remove(deadWarrior);
        }

        public bool IsAlive()
        {
            return _warriors.Count > 0;
        }

        public void ShowInfo(int squadNumber)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{squadNumber} отряд\n");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (Warrior warrior in _warriors)
            {
                Console.WriteLine($"{warrior.Name} ({warrior.Health})");
            }
            Console.WriteLine("\n***\n");
        }

        private void PrintInfoForCreate(int squadNumber)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nСоздайте отряд номер {squadNumber}.");
            Console.WriteLine("Выберите 4 воинов");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    abstract class Warrior
    {
        protected int MissChance;
        protected int DefenseChance;
        protected int CriticalAttackChance;
        private int _health;
        private int _damage;
        private string _name;
        private int _criticalDamage;
        private string _criticalAttackName;
        private int _serialNumber;

        public Warrior(int serialNumber, string name, int health, int damage, string criticalAttackName, int criticalDamage)
        {
            _serialNumber = serialNumber;
            _name = name;
            _damage = damage;
            _criticalAttackName = criticalAttackName;
            _criticalDamage = criticalDamage;
            _health = health;
        }

        public int Health
        {
            get
            {
                if (_health < 0)
                {
                    return 0;
                }
                else
                {
                    return _health;
                }
            }
        }

        public bool IsDead => _health <= 0;

        public string Name => _name;

        public int SerialNumber => _serialNumber;

        public void ShowInfo()
        {
            Console.WriteLine($"{_serialNumber}. {_name} (здоровье - {Health}, сила атаки - {_damage}, специальная атака - {_criticalDamage})");
        }

        public int GetDamage()
        {
            int miss = 0;
            int damage = 0;

            if (DefineEventChance(MissChance))
            {
                Console.WriteLine($"{_name} промахнулся. Урон: 0");
                damage = miss;
            }
            else if (DefineEventChance(CriticalAttackChance))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{_name} использовал специальную атаку {_criticalAttackName}. Урон: {_criticalDamage}");
                Console.ForegroundColor = ConsoleColor.White;
                damage = _criticalDamage;
            }
            else
            {
                Console.WriteLine($"{_name} атаковал. Урон: {_damage}");
                damage = _damage;
            }

            return damage;
        }

        public void TakeDamage(int damage)
        {
            if (DefineEventChance(DefenseChance) && damage > 0)
            {
                int damageReduction = 2;

                damage /= damageReduction;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{_name} отразил половину урона. Урон: {damage}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine($"{_name}: - {damage}");
            }

            _health -= damage;
        }

        private bool DefineEventChance(int chance)
        {
            int minRandomValue = 1;
            int maxRandomValue = 101;

            Random random = new Random();
            int randomNumber = random.Next(minRandomValue, maxRandomValue);
            return chance >= randomNumber;
        }
    }

    class Wizard : Warrior
    {
        public Wizard(int serialNumber, string name, int health, int damage, string criticaAttackName, int criticalDamage) :
        base(serialNumber, name, health, damage, criticaAttackName, criticalDamage)
        {
            MissChance = 10;
            DefenseChance = 2;
            CriticalAttackChance = 7;
        }
    }

    class Knight : Warrior
    {
        public Knight(int serialNumber, string name, int health, int damage, string criticaAttackName, int criticalDamage) :
        base(serialNumber, name, health, damage, criticaAttackName, criticalDamage)
        {
            MissChance = 2;
            DefenseChance = 10;
            CriticalAttackChance = 5;
        }
    }

    class Archer : Warrior
    {
        public Archer(int serialNumber, string name, int health, int damage, string criticaAttackName, int criticalDamage) :
        base(serialNumber, name, health, damage, criticaAttackName, criticalDamage)
        {
            MissChance = 11;
            DefenseChance = 3;
            CriticalAttackChance = 9;
        }
    }

    class Beast : Warrior
    {
        public Beast(int serialNumber, string name, int health, int damage, string criticaAttackName, int criticalDamage) :
        base(serialNumber, name, health, damage, criticaAttackName, criticalDamage)
        {
            MissChance = 2;
            DefenseChance = 7;
            CriticalAttackChance = 10;
        }
    }
}