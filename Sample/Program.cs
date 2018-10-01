using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Classes;
using FilterCore;

namespace Sample
{
    class Program
    {
        public static List<string> names = new List<string>() { "Fulano", "Cliclano", "Astrogildo", "Robervaldo", "Jaílson", "Delícia" };
        static void Main(string[] args)
        {
            List<ObjectClass> objects = new List<ObjectClass>();
            int i = 0;

            while (i < 10)
            {
                objects.AddRange(ListOfObject());
                i++;
            }

            objects.Add(new ObjectClass() { Identification = 10, FullName = "Special", BirthDate = DateTime.Now.AddYears(-1).Date });

            foreach (var item in objects) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            var filter = new FilterClass()
            {
                Name = "Del"
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            filter = new FilterClass()
            {
                Id = 3
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            filter = new FilterClass()
            {
                BeginBirthDate = new DateTime(2008, 01, 01),
                EndBirthDate = new DateTime(2010, 12, 31)
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            filter = new FilterClass()
            {
                Id = 10,
                Name = "Sp",
                BeginBirthDate = DateTime.Now.AddYears(-1).Date,
                EndBirthDate = DateTime.Now.AddYears(-1).Date
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            filter = new FilterClass()
            {
                ChildId = 4
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            filter = new FilterClass()
            {
                Value = 20
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            filter = new FilterClass()
            {
                Decrease = 2
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();

            filter = new FilterClass()
            {
                HasValue = true
            };

            foreach (var item in objects.Where(filter)) 
            {
                Console.WriteLine(item);
            }
        }

        public static IEnumerable<ObjectClass> ListOfObject()
        {
            var random = new Random();

            yield return new ObjectClass() 
            {
                Identification = random.Next(5),
                FullName = names[random.Next(6)],
                BirthDate = DateTime.Now.AddDays(-random.Next(1825, 3650)),
                ChildObject = new ChildObjectClass()
                {
                    Identification = random.Next(5),
                    Value = Convert.ToDecimal(random.Next(55)),
                    InnerChild = new InnerChildObject()
                    {
                        Decrease = float.Parse(random.Next(5).ToString()),
                        HasValue = random.Next(6) % 2 == 0 ? true : false
                    }
                }
            };
        }
    }
}
