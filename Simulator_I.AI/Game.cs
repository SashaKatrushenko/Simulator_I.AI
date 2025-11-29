using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_I.AI
{
    internal class Game
    {
        Random random = new Random();
        public void Start()
        {
            Player player = new Player();
            Console.WriteLine("Game start!");


            for (int day = 1; day <= 5; day++)              // Цикл по дням
            {
                player.Energy = 100;
                Console.WriteLine("Den: " + day);
                Console.WriteLine("Player energy: " + player.Energy);
                Console.WriteLine("Player mental: " + player.Mental);


                for (int hod = 1; hod <= 7; hod++)         // Цикл по часам
                {
                    //Скелет выбора действия, позже добавлю таблицу
                    Console.WriteLine(" hodina: " + hod);
                    player.ChanceEnergy(-5);
                    Console.WriteLine(" Energia po hodine: " + player.Energy);      
                     
                    int chance = random.Next(1, 101);               
                    Console.WriteLine("Sansa: " + chance);
                    if (chance <= 15)                          
                    {
                        Console.WriteLine("Mas test!");
                    }
                    else if (chance <= 65)
                    {
                        Console.WriteLine("Mas domacu ulohu!");
                        Console.WriteLine("1-Spravit du (-10 energie)/ 2-nespravit du (mental -15)");
                        string volba = Console.ReadLine();
                        if (volba == "1")
                        player.ChanceEnergy(-10);
                        else if (volba == "2")
                            player.ChanceMental(-15);
                    }
                    else if (chance <= 90)
                    {
                        Console.WriteLine("Je normalna hodina");
                    }
                    else
                    {
                        Console.WriteLine("Ucitel tu nie je - zastup");
                    }

                    Console.WriteLine("Player energy: " + player.Energy);
                    Console.WriteLine("Player mental: " + player.Mental);
                }
            }
           


        }

       

    }
}
