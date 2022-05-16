using System;

namespace ICouldGames.Deadline
{
    public class DataWithDeadline<T> : IComparable<DataWithDeadline<T>>
    {
        public T Data { get; }
        public int Deadline { get; }

        public DataWithDeadline(T data, int deadline)
        {
            Data = data;
            Deadline = deadline;
        }

        public int CompareTo(DataWithDeadline<T> other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            return Deadline.CompareTo(other.Deadline);
        }
    }
}