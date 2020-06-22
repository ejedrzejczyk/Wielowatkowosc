using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wielowatkowosc
{
    class Program
    {
        public static void AddBlock(Object o)
        {
            for (int i = 0; i < 20; i++)
            {
                if (o is MyArray)
                {
                    ((MyArray)o).AddBlock(48);
                }
            }
        }

        public static void Add(Object o)
        {
            for (int i = 0; i < 20; i++)
            {
                if (o is MyArray)
                {
                    ((MyArray)o).Add(4);
                }
            }
        }

        static void Main(string[] args)
        {
            MyArray myArray = new MyArray(1, 1);

            ParameterizedThreadStart a = new ParameterizedThreadStart(Add);
            ParameterizedThreadStart ab = new ParameterizedThreadStart(AddBlock);


            Thread th1 = new Thread(a);
            Thread th2 = new Thread(a);
            Thread th3 = new Thread(ab);
            //Thread th4 = new Thread(a);

            th1.Start(myArray);
            th2.Start(myArray);
            th3.Start(myArray);

            th1.Join();
            th2.Join();
            th3.Join();

            string[] tab = new string[myArray.Size[0] +1 * myArray.Size[1] +1];
            int d = 0;

            for (int i = 0; i < myArray.Size[0]; i++)
            {
                for (int j = 0; j < myArray.Size[1]; j++)
                {
                    tab[d] = myArray[i, j].ToString();
                    d++;
                }
            }

            for(int x = 0; x < tab.Length; x++)
            {
                Console.WriteLine("\t{0}", tab[x]);
            }

            try
            {
            File.WriteAllLines("D:\\studia\\sem6\\nowoczesne języki zorientowane obiektowo\\workspace\\Wielowatkowosc\\123.txt", tab);
            }
            catch
            {
                Console.WriteLine("Błąd zapisu do pliku");
            }
            Console.ReadKey();
        }
    }
}
