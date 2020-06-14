using System;
using System.Collections.Generic;

namespace RCSHTools {
    /// <summary>
    /// Extension methods for arrays and such
    /// </summary>
    public static class ArrayExt {
        /// <summary>
        /// preforms binary search in an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="target"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static int BinarySearch<T>(this T[] array, T target, Comparison<T> comparison){
            return BinarySearch(array, 0, array.Length, target, comparison);
        }
        /// <summary>
        /// Binary search in a sorted array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="target"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static int BinarySearch<T>(this T[] array, int start, int end, T target, Comparison<T> comparison){
            
            if(end - start == 0){
                System.Console.WriteLine(start + " " + end);
                return -1;
            }

            int index = (end - start) / 2 + start;
            
            int compare = comparison(target, array[index]);
            if(compare == 0)
                return index;
            if(compare < 0){
                return BinarySearch(array, index - (end - start) / 2, index, target, comparison);
            }
            else {
                return BinarySearch(array, index , index + (int)Math.Ceiling((end - start) / 2.0), target, comparison);
            }
        }
        /// <summary>
        /// Sorts an array using a specific algorithem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="algorithem"></param>
        /// <param name="comparison"></param>
        public static void Sort<T>(this T[] array, ArraySortAlgorithem algorithem,Comparison<T> comparison){
            switch(algorithem){
                case ArraySortAlgorithem.Merge:
                    MergeSort(array, comparison);
                    break;
            }
        }
        /// <summary>
        /// Swaps 2 element in an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void Swap<T>(this T[] array,int i, int j){
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        /// <summary>
        /// Shuffles the elements in the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void Shuffle<T>(this T[] array){
            int ptr = array.Length - 1;
            Random rnd = new Random();
            while(ptr > 0){
                int random = rnd.Next(0, ptr);
                array.Swap(random, ptr);
                ptr--;
            }
        }
    
        private static void QuickSort<T>(T[] array, int start, int end,Comparison<T> comparison){
            if(start >= end)
                return;
            
            int j = array.Partition(start, end, comparison);

            QuickSort(array,start, j - 1,comparison);
            QuickSort(array,j + 1, end, comparison);
        }
        
        private static int Partition<T>(this T[] array, int low, int high, Comparison<T> comparison){
            int median = array.MedianOf3(low, high, comparison);
            T pivot = array[low + (high - low)/2];

            int i = low - 1;
            int j = high + 1;

            while(true){
                while(comparison(array[i], pivot) < 0) {
                    i++;
                }
                while(comparison(array[j], pivot) > 0){
                    j--;
                }

                if(i >= j)
                    return j;
                array.Swap(i, j);
            }
        }

        private static int MedianOf3<T>(this T[] array, int low, int high, Comparison<T> comparison){
            int mid = (low + high) / 2;
            if (comparison(array[mid] ,array[low]) < 0)
                array.Swap(low, mid);
            if(comparison(array[high] ,array[low]) < 0)
                array.Swap(low, high);
            if(comparison(array[mid], array[high]) < 0){
                array.Swap(mid, high);
            }
            return high;
        }

        /// <summary>
        /// Returns a string represnetation of the elements in the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string Printable<T>(this T[] array){
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in array)
            {
                sb.AppendFormat(item + ",");   
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private static void MergeSort<T>(T[] array, Comparison<T> comp){
            var stack = MergeSort(array, 0, array.Length, comp);
            int pointer = 0;
            while(stack.Count > 0){
                array[pointer] = stack.Dequeue();
                pointer++;
            }
        }

        private static Queue<T> MergeSort<T>(T[] array, int start, int end, Comparison<T> comp){
            var stack = new Queue<T>();
            
            if(end - start == 1){
                stack.Enqueue(array[start]);
                return  stack;
            }
            
            int med = start + (end - start) / 2;
            var a1 = MergeSort(array, start, med, comp);
            var a2 = MergeSort(array, med, end, comp);
            
            while(a1.Count > 0 || a2.Count > 0){
                if(a1.Count == 0){
                    stack.Enqueue(a2.Dequeue());
                }else if(a2.Count == 0){
                    stack.Enqueue(a1.Dequeue());
                }else if(comp(a1.Peek(), a2.Peek()) > 0){
                    stack.Enqueue(a2.Dequeue());
                }else {
                    stack.Enqueue(a1.Dequeue());
                }
            }
            return stack;
        }

        /// <summary>
        /// Creates a sub mask for a <see cref="StringMask"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static StringMask Mask(this string str, int from, int to)
        {
            return new StringMask(str, from, to);
        }
        /// <summary>
        /// Creates a sub mask for an <see cref="ArrayMask{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ArrayMask<T> Mask<T>(this T[] array, int from, int to)
        {
            return new ArrayMask<T>(array, from, to);
        }
        /// <summary>
        /// Reverses the array elements (without coping)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Reverse<T>(this T[] array)
        {
            Array.Reverse(array);
            return array;
        }

        public static int Flip(this int v)
        {
            throw new Exception();
        }
    }
}