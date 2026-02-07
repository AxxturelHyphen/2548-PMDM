using System.Collections.Generic;

namespace PMDM.Helpers
{
    /// <summary>
    /// Utilidades Fibonacci implementadas con funciones recursivas.
    /// Secuencia usada en el juego: 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584
    /// </summary>
    public static class FibonacciHelper
    {
        // Cache para evitar recalcular valores ya conocidos (memoización)
        private static readonly Dictionary<int, long> _cache = new Dictionary<int, long>();

        /// <summary>
        /// Calcula el n-ésimo número de Fibonacci de forma recursiva con memoización.
        /// Fib(0)=0, Fib(1)=1, Fib(2)=1, Fib(3)=2, Fib(4)=3, Fib(5)=5 ...
        /// </summary>
        public static long Fibonacci(int n)
        {
            if (n <= 0) return 0;
            if (n == 1) return 1;

            if (_cache.TryGetValue(n, out long cached))
                return cached;

            long result = Fibonacci(n - 1) + Fibonacci(n - 2);
            _cache[n] = result;
            return result;
        }

        /// <summary>
        /// Comprueba recursivamente si un valor pertenece a la secuencia de Fibonacci.
        /// Recorre la secuencia desde Fib(1) hacia arriba.
        /// </summary>
        public static bool IsFibonacci(long value)
        {
            return IsFibonacciRecursive(value, 1);
        }

        private static bool IsFibonacciRecursive(long value, int index)
        {
            long fib = Fibonacci(index);
            if (fib == value) return true;
            if (fib > value) return false;
            return IsFibonacciRecursive(value, index + 1);
        }

        /// <summary>
        /// Comprueba si dos valores son Fibonacci consecutivos (su suma es el siguiente Fibonacci).
        /// Caso especial: 1+1=2 es válido (los dos primeros 1 de la secuencia).
        /// </summary>
        public static bool AreConsecutiveFibonacci(long a, long b)
        {
            // Caso especial: dos unos
            if (a == 1 && b == 1) return true;

            long smaller = a < b ? a : b;
            long larger = a > b ? a : b;

            return AreConsecutiveRecursive(smaller, larger, 1);
        }

        private static bool AreConsecutiveRecursive(long smaller, long larger, int index)
        {
            long fibCurrent = Fibonacci(index);
            long fibNext = Fibonacci(index + 1);

            if (fibCurrent > larger) return false;

            if ((fibCurrent == smaller && fibNext == larger) ||
                (fibCurrent == larger && fibNext == smaller))
                return true;

            return AreConsecutiveRecursive(smaller, larger, index + 1);
        }

        /// <summary>
        /// Dado un valor Fibonacci, devuelve el siguiente en la secuencia.
        /// Usa recursión para encontrar la posición del valor actual.
        /// </summary>
        public static long GetNextFibonacci(long currentValue)
        {
            return GetNextFibonacciRecursive(currentValue, 1);
        }

        private static long GetNextFibonacciRecursive(long value, int index)
        {
            long fib = Fibonacci(index);
            if (fib == value)
                return Fibonacci(index + 1);
            if (fib > value)
                return -1; // No encontrado
            return GetNextFibonacciRecursive(value, index + 1);
        }

        /// <summary>
        /// Devuelve el resultado de fusionar dos tiles Fibonacci consecutivos.
        /// El resultado es la suma de ambos (= siguiente Fibonacci).
        /// </summary>
        public static long MergeResult(long a, long b)
        {
            if (!AreConsecutiveFibonacci(a, b)) return -1;
            return a + b;
        }

        /// <summary>
        /// Obtiene el índice (posición) de un valor en la secuencia de Fibonacci.
        /// Recursivo. Devuelve -1 si no es Fibonacci.
        /// </summary>
        public static int GetFibonacciIndex(long value)
        {
            return GetFibonacciIndexRecursive(value, 1);
        }

        private static int GetFibonacciIndexRecursive(long value, int index)
        {
            long fib = Fibonacci(index);
            if (fib == value) return index;
            if (fib > value) return -1;
            return GetFibonacciIndexRecursive(value, index + 1);
        }

        /// <summary>
        /// Genera recursivamente la secuencia Fibonacci hasta un valor máximo.
        /// Útil para debug y UI.
        /// </summary>
        public static List<long> GenerateSequenceUpTo(long maxValue)
        {
            var list = new List<long>();
            GenerateSequenceRecursive(list, maxValue, 1);
            return list;
        }

        private static void GenerateSequenceRecursive(List<long> list, long maxValue, int index)
        {
            long fib = Fibonacci(index);
            if (fib > maxValue) return;
            list.Add(fib);
            GenerateSequenceRecursive(list, maxValue, index + 1);
        }
    }
}
