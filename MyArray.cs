using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wielowatkowosc
{
    class MyArray
    {
        public delegate void ChangedEventHandler(int firstDimension, int secondDimenson);
        public event ChangedEventHandler ArraySizeChanged;

        protected virtual void OnArraySizeChanged(int firstDimension, int secondDimension)
        {
            if (ArraySizeChanged != null)
            {
                ArraySizeChanged(firstDimension, secondDimension);
            }
            else
            {
                Console.WriteLine("\nTablica rozszerzona.");
                Console.WriteLine("Aktualny rozmiar: {0}x{1}", firstDimension, secondDimension);
            }
        }

        public int[,] array;
        Object o = new Object();

       // public bool fail = false;

        public int[] Size
        {
            get
            {
                Monitor.Enter(o);
                try
                {
                    int[] s = new int[2];
                    s[0] = array.GetLength(0);
                    s[1] = array.GetLength(1);
                    return s;
                }
                finally
                {
                    Monitor.Exit(o);
                }
            }
        }

        public MyArray(int firstDimension, int secondDimension)
        {
            array = new int[firstDimension, secondDimension];
        }

        public int this[int firstIndex, int secondIndex]
        {
            get
            {
                lock (o)
                {
                    return ReadValue(firstIndex, secondIndex);
                }
            }
            set
            {
                lock (o)
                {
                    WriteValue(value, firstIndex, secondIndex);
                }

            }
        }
        
        public void Add(Object e)
        {

            if (Monitor.TryEnter(o))
            {
                try
                {
                    array.SetValue(Convert.ToInt32(e), Size[0] - 1, Size[1] - 1);
                    int[,] temp = array;
                    array = new int[temp.GetLength(0) + 1, temp.GetLength(1)];
                    Array.Copy(temp, array, temp.Length);

                    Console.WriteLine("SUCCEED\t wpisano natychmiast 4");
                }
                finally
                {
                    Monitor.Exit(o);
                }
            }
            else
            {
                Console.WriteLine("FAILED\t nie wpisano nic");
            }
        }

        public void AddBlock(Object e)
        {
            DateTime start = DateTime.Now;
            lock (o)
            {
                Thread.Sleep(1);

                DateTime stop = DateTime.Now;

                array.SetValue(Convert.ToInt32(e), Size[0] - 1, Size[1] - 1);
                int[,] temp = array;
                array = new int[temp.GetLength(0) + 1, temp.GetLength(1)];
                Array.Copy(temp, array, temp.Length);

                TimeSpan time = stop - start;
                Console.WriteLine("Delay time: {0} ms\t wpisano 48 po czasie {0}", time.TotalMilliseconds);
            }
        }

        public int ReadValue(int firstIndex, int secondIndex)
        {
            try
            {
                return (int)array.GetValue(firstIndex, secondIndex);

            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Przekroczenie rozmiaru tablicy\nRozmiar tablicy: " + Size[0] + "x" + Size[1]);
            }
        }

        public void WriteValue(int value, int firstIndex, int secondIndex)
        {
            int newFirstSize = array.GetLength(0);
            int newSecondSize = array.GetLength(1);

            try
            {
                array.SetValue(value, firstIndex, secondIndex);
            }
            catch (IndexOutOfRangeException)
            {
                if (firstIndex + 1 > array.GetLength(0))
                {
                    newFirstSize = firstIndex + 1;
                }
                if (secondIndex + 1 > array.GetLength(1))
                {
                    newSecondSize = secondIndex + 1;
                }

                int[,] newArray = new int[newFirstSize, newSecondSize];

                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        newArray[i, j] = array[i, j];
                    }
                }

                newArray.SetValue(value, firstIndex, secondIndex);
                array = newArray;
                OnArraySizeChanged(Size[0], Size[1]);
            }
        }
    }
}
