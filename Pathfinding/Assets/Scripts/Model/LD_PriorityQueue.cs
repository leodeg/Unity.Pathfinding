using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD.PathFinding
{
	public class LD_PriorityQueue<T> where T : IComparable<T>
	{
		private List<T> m_Data;

		public LD_PriorityQueue ()
		{
			m_Data = new List<T> ();
		}

		public int Count { get { return m_Data.Count; } }

		public void Enqueue (T item)
		{
			m_Data.Add (item);

			int childIndex = m_Data.Count - 1;
			while (childIndex > 0)
			{
				int parentIndex = ( childIndex - 1 ) / 2;
				if (m_Data[childIndex].CompareTo (m_Data[parentIndex]) >= 0)
				{
					break;
				}

				T temp = m_Data[childIndex];
				m_Data[childIndex] = m_Data[parentIndex];
				m_Data[parentIndex] = temp;

				childIndex = parentIndex;
			}
		}
		public T Dequeue ()
		{
			int lastIndex = m_Data.Count - 1;
			T frontItem = m_Data[0];

			m_Data[0] = m_Data[lastIndex];
			m_Data.RemoveAt (lastIndex);
			lastIndex--;

			int parentIndex = 0;

			while (true)
			{
				int childIndex = parentIndex * 2 + 1;
				if (childIndex > lastIndex)
				{
					break;
				}

				int rightChild = childIndex + 1;
				if (rightChild <= lastIndex && m_Data[rightChild].CompareTo (m_Data[childIndex]) < 0)
				{
					childIndex = rightChild;
				}

				if (m_Data[parentIndex].CompareTo (m_Data[childIndex]) <= 0)
				{
					break;
				}

				T temp = m_Data[parentIndex];
				m_Data[parentIndex] = m_Data[childIndex];
				m_Data[childIndex] = temp;

				parentIndex = childIndex;
			}

			return frontItem;
		}
		public T Peek ()
		{
			T frontItem = m_Data[0];
			return frontItem;
		}
		public bool Contains (T item)
		{
			return m_Data.Contains (item);
		}
		public void Clear ()
		{
			m_Data.Clear ();

		}

		public List<T> ToList ()
		{

			return new List<T> (m_Data);
		}
		public Array ToArray ()
		{
			return m_Data.ToArray ();
		}
	}
}
