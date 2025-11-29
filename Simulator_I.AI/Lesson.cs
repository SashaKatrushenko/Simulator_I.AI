using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_I.AI
{
    public class Lesson
    {
        public string Predmet { get; set; } //nazov predmetu
        public int ChanceTest { get; set; } //sansa na test
        public int ChanceDU {  get; set; } //sansa na domacu
        public int ChanceZastup { get; set; } //sansa zastupu
        public int ChanceNic { get; set; } //sansa normalna hodina

        public Lesson(string name, int chanceTest, int chanceDU, int chanceNic, int chanceZastup)
        {
            Predmet = name;
            ChanceTest = chanceTest;
            ChanceDU = chanceDU;
            ChanceNic = chanceNic;
            ChanceZastup = chanceZastup;
        }

    }
}
