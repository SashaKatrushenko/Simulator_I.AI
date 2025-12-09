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
                /*
                player.Energy = 100;
                player.Mental = 100;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==== DEN: " + day + " ====");
                Console.ResetColor();
                Console.WriteLine("Player energy: " + player.Energy);
                Console.WriteLine("Player mental: " + player.Mental);
                */


                for (int hod = 1; hod <= 7; hod++)         // Цикл по часам
                {

                    /*
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
                    */
                }
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("--- KONIEC DNA " + day + " ---");
                Console.ResetColor();
                Console.WriteLine("Energia: " + player.Energy);
                Console.WriteLine("Mental: " + player.Mental);
                Console.WriteLine("Enter pre pokracovanie");
                Console.ReadLine();
            }
           

          
        }

       

    }
}
