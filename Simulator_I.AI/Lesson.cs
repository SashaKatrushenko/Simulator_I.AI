using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_I.AI
{
    public class Lesson
    {

            public string Day { get; set; }       //Pondelok, Utorok, Streda, Štvrtok, Piatok
        public int Hour { get; set; }            //1-7
        public string Predmet { get; set; }     //Vyucovaie predmety
        public string Ucitel { get; set; }     //Meno ucitela

        public int ChanceTest { get; set; }    //Sanca na test v %
        public int ChanceDU { get; set; }      //Sanca na domacu uciu v %
        public int ChanceNic { get; set; }     //Sanca na nic v %
        public int ChanceZastup { get; set; }   //Sanca na zastup v %

        public Lesson(string day, int hour, string predmet, string ucitel,
                          int chanceTest, int chanceDU, int chanceNic, int chanceZastup)
            {
                Day = day;
                Hour = hour;
                Predmet = predmet;
                Ucitel = ucitel;
                ChanceTest = chanceTest;
                ChanceDU = chanceDU;
                ChanceNic = chanceNic;
                ChanceZastup = chanceZastup;
            }
    }
   
}
