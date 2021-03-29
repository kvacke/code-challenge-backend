using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int jens_skillLevel = 1;
            double edvins_skillLevel = Double.MaxValue;

            while(jens_skillLevel < edvins_skillLevel)
            {
                int jens_skillIncrease = WriTE_awZm_cOde();
                jens_skillLevel += jens_skillIncrease;
                Console.WriteLine("hehe snart e ja bättre en edvin");
            }
            Console.WriteLine("CONGRATULATIONS!");
            Console.WriteLine("YOU");
            Console.WriteLine("ARE");
            Console.WriteLine("FAGS!");
        }
        public static int WriTE_awZm_cOde()
        {
            Random rng = new Random();
            return rng.Next(0, 1);
        }
    }
}
