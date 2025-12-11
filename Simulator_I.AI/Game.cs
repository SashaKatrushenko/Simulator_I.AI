using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Simulator_I.AI
{
    internal class Game
    {
        List<Lesson> lessons = new List<Lesson>();
        Random random = new Random();
        public void Start()
        {   
            string fileName = "Book(Sheet1).csv";
            string[] lines = File.ReadAllLines(fileName);
            bool prvyRiadok = true;

            foreach (string line in lines)
            {
                if (prvyRiadok)
                {
                    prvyRiadok = false;
                    continue;
                }
                string[] parts = line.Split(',');

                string den = parts[0];
                int hodina = int.Parse(parts[1]);
                string predmet = parts[2];
                string ucitel = parts[3];
                int sanceTest = int.Parse(parts[4]);
                int sanceDu = int.Parse(parts[5]);
                int sanceNic = int.Parse(parts[6]);
                int sanceZastup = int.Parse(parts[7]);

                Lesson lesson = new Lesson(den, hodina, predmet, ucitel, sanceTest, sanceDu, sanceNic, sanceZastup);
                lessons.Add(lesson);
                int sum = sanceTest + sanceDu + sanceNic + sanceZastup;
                if (sum != 100)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\nSucet sansi nie je 100%. Riadok {parts[0]}, predmet {parts[2]}");
                  //  Console.WriteLine($"\nUpozornenie. Vriadku {+1} ma sumu {sum}%, ocakavane 100% ({lines[1]})");
                  Console.ResetColor();
                }
            }

            string[] dni = { "Pondelok", "Utorok", "Streda", "Stvrtok", "Piatok" };


            foreach (string den in dni)
            {
                Console.WriteLine("=== " + den + " ===");
                for (int hod = 1; hod <= 7; hod++)
                {
                    Lesson aktualna = null;
                    foreach (Lesson l in lessons)
                    {
                        if (l.Day == den && l.Hour == hod)
                        {
                            aktualna = l;
                            break;
                        }
                    }
                    if (aktualna == null)
                    {
                        Console.WriteLine("Hodina " + hod + "prazdna");
                        continue;
                    }
                    Console.WriteLine("Hodina " + hod + ": " + aktualna.Predmet);

                }
            }

            


            //  Console.WriteLine("lesson nacitane: " + lessons.Count);

            Player player = new Player();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game start! Po kazdej hodine stracas 5 energie");
            Console.ResetColor();
    
            for (int day = 1; day <= 5; day++)              // Цикл по дням
            {
              //  int testsThisDay = 0;
              // int goodMaeksThisDay = 0;
              // int duDone = 0;
                
                player.Energy = 100;
                player.Mental = 100;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==== DEN: " + day + " ====");
                Console.ResetColor();
                Console.WriteLine("Player energy: " + player.Energy);
                Console.WriteLine("Player mental: " + player.Mental);
                

                foreach (Lesson l in lessons)
                {
                   // Console.WriteLine($"Dnes je: {l.Day}, {l.Hour} hodina, {l.Predmet}");
                    int roll = random.Next(1, 101);
                    if (roll <= l.ChanceTest)
                    {
                        Console.WriteLine("Energia: " + player.Energy);
                        Console.WriteLine("Mental: " + player.Mental);
                        Console.ForegroundColor = ConsoleColor.Blue; 
                        Console.WriteLine("Mas test!");
                        Console.WriteLine("1. Odpisat na teste (-20 energie. 70% sansa na dobru znamku) 2. Skusit sam (30% sansa na dobru znamku)");
                        string volba = Console.ReadLine();
                        Console.ResetColor();
                        if (volba == "1")
                        {
                            player.ChangeEnergy(-20);
                            int chanceZnamka1 = random.Next(1, 100);
                            if (chanceZnamka1 <= 70)
                            {
                                Console.WriteLine("Dostavas dobru znamku! +20 mental");
                                player.ChangeMental(20);
                            }
                            else
                            {
                                Console.WriteLine("Dostavas zlu znamku! (-20 mental)");
                                player.ChangeMental(-20);
                            }
                        }
                    }
                    else if (roll <= l.ChanceTest + l.ChanceDU)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Mas domacu ulohu!");
                        Console.WriteLine("1-Spravit du (-10 energie)/ 2-nespravit du (mental -15)");
                        Console.ResetColor();
                        string volba = Console.ReadLine();
                        if (volba == "1")
                            player.ChangeEnergy(-10);
                        else if (volba == "2")
                            player.ChangeMental(-15);
                    }
                    else if (roll <= l.ChanceDU + l.ChanceTest + l.ChanceNic)
                    {
                        int chanceTabula = 20;
                        int tabulaRoll = random.Next(1, 100);
                        if (chanceTabula <= chanceTabula)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("Ucitel ta vyvolal ku tabule! Vimysli si nieco...");
                            Console.WriteLine("1. Pytat spoluziakov o pomoc -5e -5m");
                            Console.WriteLine("2. Vimyslat -5e");
                            Console.WriteLine("3. Povedat pravdu -15m");
                            Console.ResetColor();
                            string choice = Console.ReadLine();
                            if (choice == "1")
                            {
                                player.ChangeMental(-5);
                                player.ChangeEnergy(-5);
                            }
                            else if (choice == "2")
                            {
                                player.ChangeEnergy(-5);
                            }
                            else if (choice == "3")
                            {
                                player.ChangeMental(-15);
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Ucitel tu nie je - zastup!");
                        Console.WriteLine("1. Spat +20 energie 2. Hrat na mobile +20 mentalu");
                        Console.ResetColor();
                        string volba = Console.ReadLine();
                        if (volba == "1")
                            player.ChangeEnergy(20);
                        else if (volba == "2")
                            player.ChangeMental(20);

                    }
                 
                   
               

                    if (player.Energy <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Citis sa vycerpano. Nedokazes pokracovat...GAME OVER");
                        Console.ResetColor();
                        return;
                    }
                    if (player.Mental <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Nervove zrutenie, nezvladol si stres. Nedokazes pokracovat...GAME OVER");

                        Console.ResetColor();
                        return;
                    }
                    Console.WriteLine("Player energy: " + player.Energy);
                    Console.WriteLine("Player mental: " + player.Mental);

                    player.ChangeEnergy(-5);
                    Console.WriteLine("Enter pre pokracovanie");
                    Console.ReadLine();
                }


                   
                    
            }
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("--- KONIEC DNA " + "*day*" + " ---");
                Console.ResetColor();
                Console.WriteLine("Energia: " + player.Energy);
                Console.WriteLine("Mental: " + player.Mental);
                Console.WriteLine("Enter pre pokracovanie");
                Console.ReadLine();
        }
           
        private void Test(Player player, Lesson l, Random random)
        {

        }
          
        private void DU(Player player)
        {

        }

        private void Nic(Player player, Random random)
        {

        }

        private void Zastup(Player player)
        {

        }


    }

       

}






/*
for (int hod = 1; hod <= 7; hod++)         // Цикл по часам
{


    //Скелет выбора действия, позже добавлю таблицу
    Console.WriteLine(" HODINA: " + hod);
    player.ChangeEnergy(-5);
    //   Console.WriteLine(" Energia po hodine: " + player.Energy);      

    int chance = random.Next(1, 101);               
    Console.WriteLine("Sansa: " + chance);
    if (chance <= 15)                          
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Mas test!");
        Console.WriteLine("1. Odpisat na teste (-20 energie. 70% sansa na dobru znamku) 2. Skusit sam (30% sansa na dobru znamku)");
        string volba = Console.ReadLine();
        Console.ResetColor();
        if (volba == "1")
        {
            player.ChangeEnergy(-20);
            int chanceZnamka1 = random.Next(1, 100);
            if (chanceZnamka1 <= 70)
            {
                Console.WriteLine("Dostavas dobru znamku! +20 mental");
                player.ChangeMental(20);
            }
            else
            {
                Console.WriteLine("Dostavas zlu znamku! (-20 mental)");
                player.ChangeMental(-20);
            }
        }

        else if (volba == "2")
        {
            int chanceZnamka = random.Next(1, 100);
            if (chanceZnamka <= 30)
            {
                Console.WriteLine("Dostavas dobru znamku! +20 mental");
                player.ChangeMental(20);
            }
            else
            {
                Console.WriteLine("Dostavas zlu znamku! (-20 mental)");
                player.ChangeMental(-20);
            }
        }
    }
    else if (chance <= 60)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Mas domacu ulohu!");
        Console.WriteLine("1-Spravit du (-10 energie)/ 2-nespravit du (mental -15)");
        Console.ResetColor();
        string volba = Console.ReadLine();
        if (volba == "1")
        player.ChangeEnergy(-10);
        else if (volba == "2")
            player.ChangeMental(-15);
    }
    else if (chance <= 90)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Je normalna hodina");
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Ucitel tu nie je - zastup!");
        Console.WriteLine("1. Spat +20 energie 2. Hrat na mobile +20 mentalu");
        Console.ResetColor();
        string volba = Console.ReadLine();
        if (volba == "1")
            player.ChangeEnergy(20);
        else if (volba == "2")
            player.ChangeMental(20);

    }
*/
