﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNext.Collections.Generic
{
    internal readonly struct SingletonList<T> : IReadOnlyList<T>
    {
        internal struct Enumerator : IEnumerator<T>
        {
            private bool requested;

            internal Enumerator(T element)
            {
                Current = element;
                requested = false;
            }

            public T Current { get; }

            object IEnumerator.Current => Current;

            void IDisposable.Dispose() => this = default;

            public bool MoveNext()
                => requested ? false : requested = true;

            public void Reset() => requested = false;
        }

        private readonly T item;

        internal SingletonList(T item) => this.item = item;

        T IReadOnlyList<T>.this[int index]
            => index == 0 ? item : throw new IndexOutOfRangeException(ExceptionMessages.IndexShouldBeZero);

        int IReadOnlyCollection<T>.Count => 1;

        /// <summary>
        /// Gets enumerator for the single element in the list.
        /// </summary>
        /// <returns>The enumerator for th</returns>
        public Enumerator GetEnumerator() => new Enumerator(item);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
