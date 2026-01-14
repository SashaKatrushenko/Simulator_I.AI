using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace Simulator_I.AI
{
    internal class Game
    {
        private string ReadOption(params string[] validOptions)
        {
            var valid = validOptions.Select(v => v.Trim().ToLower()).ToArray();
            while (true)
            {
                string input = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrEmpty(input))
                {
                    //  Console.WriteLine();
                }
                if (valid.Contains(input))
                {
                    return input;
                }
                Console.WriteLine($"Zla volba. Moznosti: {string.Join(", ", validOptions)} Skus znova");
            }
        }

        private void WaitForEnter(string prompt = "Enter pre pokracovanie")
        {
            Console.WriteLine(prompt);
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {

                    break;
                }
            }
        }

        List<Lesson> lessons = new List<Lesson>();
        Random random = new Random();
        public void Start()
        {

            Player player = new Player();
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

                //Tu mi poradil chatgpt ze mam spravit kontrolu ci je suma 100% 
                if (sum != 100)
                {
                    Console.WriteLine($"\nSucet sansi nie je 100%. Riadok {parts[0]}, predmet {parts[2]}");
                    //  Console.WriteLine($"\nV riadku {+1} ma sumu {sum}%, ocakavane 100% ({lines[1]})");
                }
            }

            string[] dni = { "Pondelok", "Utorok", "Streda", "Stvrtok", "Piatok" };

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game start! Po kazdej hodine stracas 5 energie");
            Console.ResetColor();

            foreach (string den in dni)
            {
                player.Energy = 100;
                player.Mental = 100;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==== DEN: " + den + " ====");
                Console.ResetColor();
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

                    //EVENTY 
                    int roll = random.Next(1, 101);
                    if (roll <= aktualna.ChanceTest)
                    {
                        Test(player, aktualna, random);
                    }
                    else if (roll <= aktualna.ChanceTest + aktualna.ChanceDU)
                    {
                        DU(player, random, aktualna);
                    }
                    else if (roll <= aktualna.ChanceDU + aktualna.ChanceTest + aktualna.ChanceNic)
                    {
                        Nic(player, aktualna, random);
                    }
                    else
                    {
                        Zastup(player, aktualna);
                    }
                    player.ChangeEnergy(-5);
                    Console.WriteLine("Player energy: " + player.Energy);
                    Console.WriteLine("Player mental: " + player.Mental);
                    WaitForEnter();

                    //PREHRA
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
                }
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("--- KONIEC DNA ---");
                Console.ResetColor();
                Console.WriteLine("Energia: " + player.Energy);
                Console.WriteLine("Mental: " + player.Mental);
                WaitForEnter();

            }
            //  Console.WriteLine("lesson nacitane: " + lessons.Count)         
        }





        //METODY PRE EVENTY

        private void Test(Player player, Lesson l, Random random)
        {
            Console.WriteLine("Energia: " + player.Energy);
            Console.WriteLine("Mental: " + player.Mental);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Pisete patminutovku");
            Console.WriteLine("1. Odpisat na teste (-20 energie. 70% sansa na dobru znamku) 2. Skusit sam (30% sansa na dobru znamku)");
            string volba = ReadOption("1", "2");
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
            if (volba == "2")
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

        private void DU(Player player, Random random, Lesson aktualna)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Na tuto hodinu mal si pripravit domacu ulohu");
            Console.WriteLine("1-Spravit DU (-10 energie)");
            Console.WriteLine("2-Nespravit DU");
            Console.ResetColor();
            string volba = ReadOption("1", "2");
            bool spravilDU = false;
            if (volba == "1")
            {
                spravilDU = true;
                player.ChangeEnergy(-10);
            }
            else
            {
                spravilDU = false;
            }
            int kontrolaRoll = random.Next(1, 101);
            if (kontrolaRoll <= 50)
            {
                Console.WriteLine(aktualna.Ucitel + " kontroluje domacu ulohu...");
                if (spravilDU)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Všetko maš, dobra praca! +10m");
                    Console.ResetColor();
                    player.ChangeMental(10);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Nemáš DU, poznamka! -15m");
                    Console.ResetColor();
                    player.ChangeMental(-15);
                }
            }
            else
            {
                Console.WriteLine("Ucitel nekontroloval DU");
            }
        }

        private void Nic(Player player, Lesson aktualna, Random random)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("normalna hodina");

            if (aktualna.Predmet == "ETV")
            {
                Etika(player, aktualna);
                return;
            }
            if (aktualna.Predmet != "TSV") //tu som pytala chatgpt co mam spravit lebo nejslo mi najst "aktualna" v tejto metode
            {
                int chanceTabula = 30;
                int tabulaRoll = random.Next(1, 101);
                if (tabulaRoll <= chanceTabula)
                {
                    if (aktualna.Predmet == "MAT")
                    {
                        int a = random.Next(20, 100);
                        int b = random.Next(10, 80);
                        bool plus = random.Next(0, 2) == 0;

                        int correctAnswer;
                        if (plus)
                        {
                            correctAnswer = a + b;
                            Console.WriteLine(aktualna.Ucitel + " vovolala ta ku tabule! Vypocitaj: " + a + " + " + b);
                        }
                        else
                        {
                            if (b > a)
                            {
                                int temp = a;
                                a = b;
                                b = temp;
                            }
                            correctAnswer = a - b;
                            Console.WriteLine(aktualna.Ucitel + " vyvolala ta ku tabule! Vypocitaj: " + a + " - " + b);
                        }
                        string odpoved = Console.ReadLine();
                        int answer;
                        if (int.TryParse(odpoved, out answer))
                        {
                            if (answer == correctAnswer)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Spravna odpoved! +10m +10e");
                                player.ChangeEnergy(10);
                                player.ChangeMental(10);
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Nespravna odpoved! Spravna odpoved bola: " + correctAnswer + ". -10m -10e");
                                player.ChangeMental(-10);
                                player.ChangeEnergy(-10);
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("Neplatna odpoved! -10 mental");
                            player.ChangeMental(-10);
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.WriteLine(aktualna.Ucitel + " vola ta ku tabule! Vimysli si nieco...");
                        Console.WriteLine("1. Pytat spoluziakov o pomoc -10e -10m");
                        Console.WriteLine("2. Vimyslat -15e");
                        Console.WriteLine("3. Povedat ze nevies +5e -15m");
                        Console.ResetColor();
                        string choice = ReadOption("1", "2", "3");
                        if (choice == "1")
                        {
                            player.ChangeMental(-10);
                            player.ChangeEnergy(-10);
                            int pomocRoll = random.Next(1, 101);
                            if (pomocRoll <= 50)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Spoluziaci ti poradili spravne! +10e +10m");
                                player.ChangeMental(10);
                                player.ChangeEnergy(10);
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Spoluziaci ti moc nepomohli -10e -10m");
                                player.ChangeMental(-10);
                                player.ChangeEnergy(-10);
                                Console.ResetColor();
                            }
                            Console.ResetColor();
                        }
                        else if (choice == "2")
                        {
                        player.ChangeEnergy(-15);
                        int vymyslatRoll = random.Next(1, 101);
                            if (vymyslatRoll <=30)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Improvizácia ti moc nepomohla, dostal si štvorku -20m");
                                player.ChangeMental(-20);
                            }
                            else if (vymyslatRoll <=40)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Na trojku si to zvladol");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Učiteľ sa zasmial a dal ti dvojku +15m +15e");
                                player.ChangeMental(15);
                                player.ChangeEnergy(15);
                            }
                            Console.ResetColor();
                        }
                        else if (choice == "3")
                        {
                            player.ChangeEnergy(5);
                            player.ChangeMental(-15);
                            int neviemRoll = random.Next(1, 101);
                            if (neviemRoll <= 70)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Učiteľ ocenil uprimnost. Mas 5! -30m");
                                player.ChangeMental(-30);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Mas šťastie, učitel má dobru naladu. Dnes bez znamky. +25m");
                                player.ChangeMental(25);
                                Console.ResetColor();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("1. Davat pozor (-5e +10m)");
                    Console.WriteLine("2. Kreslit do zosita (+5e -5m)");
                    Console.WriteLine("3. Bavit sa so spoluziakmi (+10e -10m)");
                    string volba = ReadOption("1", "2", "3");
                    if (volba == "1")
                    {
                        player.ChangeEnergy(-5);
                        player.ChangeMental(10);
                    }
                    else if (volba == "2")
                    {
                        player.ChangeEnergy(5);
                        player.ChangeMental(-5);
                    }
                    else if (volba == "3")
                    {
                        player.ChangeEnergy(10);
                        player.ChangeMental(-10);
                    }
                }
            }
            Console.ResetColor();
        }

        private void Zastup(Player player, Lesson aktualna)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(aktualna.Ucitel + " chyba - zastup!");
            Console.WriteLine("1. Spat +20 energie 2. Hrat na mobile +20 mentalu");
            Console.ResetColor();
            string volba = ReadOption("1", "2");
            if (volba == "1")
                player.ChangeEnergy(20);
            else if (volba == "2")
                player.ChangeMental(20);
        }

        private void Etika(Player player, Lesson aktualna)
        {
            Console.ResetColor();
            Console.WriteLine("Hodina etiky. Dnešna tema: psychologia");
            Console.WriteLine("Ucitelka sa pyta otazky:");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Kto je sangvinik?");
            Console.WriteLine("1. Clovek ktory je spolocensky a optimisticky");
            Console.WriteLine("2. Clovek, ktory je tichy a uzavrety");
            Console.ResetColor();
            string volba = ReadOption("1", "2");
            if (volba == "1")
            {
                Console.WriteLine("Spravna odpoved");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Sangvinik je energický, spoločenský a veselý človek, ktorý sa ľahko prispôsobuje novým veciam a rýchlo sa spriatelí s ľuďmi.");
                Console.ResetColor();
            }
            else if (volba == "2")
            {
                Console.WriteLine("Spravna odpoved bola 1");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Sangvinik je energický, spoločenský a veselý človek, ktorý sa ľahko prispôsobuje novým veciam a rýchlo sa spriatelí s ľuďmi.");
                Console.ResetColor();
            }
            Console.WriteLine("Dalšia otazka:");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("čo znamena empatia?");
            Console.WriteLine("1. Schopnosť presadiť si svoj názor");
            Console.WriteLine("2. Schopnost pochopit a sdilet pocity ineho cloveka");
            Console.ResetColor();
            volba = ReadOption("1", "2");
            if (volba == "1")
            {
                Console.WriteLine("Spravna odpoved bola 1");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Empatia je schopnosť pochopiť a zdieľať pocity, emócie a skúsenosti inej osoby, schopnosť vžiť sa do jej prostredia a pozrieť sa na situáciu z jej pohľadu.");
                Console.ResetColor();
            }
            else if (volba == "2")
            {
                Console.WriteLine("Spravna odpoved");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Empatia je schopnosť pochopiť a zdieľať pocity, emócie a skúsenosti inej osoby, schopnosť vžiť sa do jej prostredia a pozrieť sa na situáciu z jej pohľadu.");
                Console.ResetColor();
            }

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
