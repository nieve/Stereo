// Type: System.Tuple`2
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System
{
	/// <summary>
	/// Represents a 2-tuple, or pair.
	/// </summary>
	/// <typeparam name="T1">The type of the tuple's first component.</typeparam><typeparam name="T2">The type of the tuple's second component.</typeparam><filterpriority>2</filterpriority>
	[Serializable]
	public class Tuple<T1, T2> : IComparable
	{
		private readonly T1 m_Item1;
		private readonly T2 m_Item2;
		
		/// <summary>
		/// Gets the value of the current <see cref="T:System.Tuple`2"/> object's first component.
		/// </summary>
		/// 
		/// <returns>
		/// The value of the current <see cref="T:System.Tuple`2"/> object's first component.
		/// </returns>
		public T1 Item1
		{
		  get
		  {
		    return this.m_Item1;
		  }
		}
		
		/// <summary>
		/// Gets the value of the current <see cref="T:System.Tuple`2"/> object's second component.
		/// </summary>
		/// 
		/// <returns>
		/// The value of the current <see cref="T:System.Tuple`2"/> object's second component.
		/// </returns>
		public T2 Item2
		{
		  get
		  {
		    return this.m_Item2;
		  }
		}
		
		int Size
		{
		  get
		  {
		    return 2;
		  }
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Tuple`2"/> class.
		/// </summary>
		/// <param name="item1">The value of the tuple's first component.</param><param name="item2">The value of the tuple's second component.</param>
		public Tuple(T1 item1, T2 item2)
		{
		  this.m_Item1 = item1;
		  this.m_Item2 = item2;
		}
		
		/// <summary>
		/// Returns a value that indicates whether the current <see cref="T:System.Tuple`2"/> object is equal to a specified object.
		/// </summary>
		/// 
		/// <returns>
		/// true if the current instance is equal to the specified object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with this instance.</param>
		public override bool Equals(object obj)
		{
		  return this.Equals(obj, (IEqualityComparer) EqualityComparer<object>.Default);
		}
		
		bool Equals(object other, IEqualityComparer comparer)
		{
		  if (other == null)
		    return false;
		  Tuple<T1, T2> tuple = other as Tuple<T1, T2>;
		  if (tuple == null || !comparer.Equals((object) this.m_Item1, (object) tuple.m_Item1))
		    return false;
		  else
		    return comparer.Equals((object) this.m_Item2, (object) tuple.m_Item2);
		}
		
		int IComparable.CompareTo(object obj)
		{
		  return this.CompareTo(obj, (IComparer) Comparer<object>.Default);
		}
		
		int CompareTo(object other, IComparer comparer)
		{
		  if (other == null)
		    return 1;
		  Tuple<T1, T2> tuple = other as Tuple<T1, T2>;
		  if (tuple == null)
		  {
		    throw new ArgumentException("ArgumentException_TupleIncorrectType");
		  }
		  else
		  {
		    int num = comparer.Compare((object) this.m_Item1, (object) tuple.m_Item1);
		    if (num != 0)
		      return num;
		    else
		      return comparer.Compare((object) this.m_Item2, (object) tuple.m_Item2);
		  }
		}
		
		/// <summary>
		/// Returns the hash code for the current <see cref="T:System.Tuple`2"/> object.
		/// </summary>
		/// 
		/// <returns>
		/// A 32-bit signed integer hash code.
		/// </returns>
		public override int GetHashCode()
		{
		  return this.GetHashCode((IEqualityComparer) EqualityComparer<object>.Default);
		}
		
		int GetHashCode(IEqualityComparer comparer)
		{
		  return Tuple.CombineHashCodes(comparer.GetHashCode((object) this.m_Item1), comparer.GetHashCode((object) this.m_Item2));
		}
		
		/// <summary>
		/// Returns a string that represents the value of this <see cref="T:System.Tuple`2"/> instance.
		/// </summary>
		/// 
		/// <returns>
		/// The string representation of this <see cref="T:System.Tuple`2"/> object.
		/// </returns>
		public override string ToString()
		{
		  StringBuilder sb = new StringBuilder();
		  sb.Append("(");
		  return this.ToString(sb);
		}
		
		string ToString(StringBuilder sb)
		{
		  sb.Append((object) this.m_Item1);
		  sb.Append(", ");
		  sb.Append((object) this.m_Item2);
		  sb.Append(")");
		  return ((object) sb).ToString();
		}
	}
	
	public static class Tuple
	{
		/// <summary>
		/// Creates a new 2-tuple, or pair.
		/// </summary>
		/// 
		/// <returns>
		/// A 2-tuple whose value is (<paramref name="item1"/>, <paramref name="item2"/>).
		/// </returns>
		/// <param name="item1">The value of the first component of the tuple.</param><param name="item2">The value of the second component of the tuple.</param><typeparam name="T1">The type of the first component of the tuple.</typeparam><typeparam name="T2">The type of the second component of the tuple.</typeparam>
		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
		  return new Tuple<T1, T2>(item1, item2);
		}
		
		internal static int CombineHashCodes(int h1, int h2)
		{
		  return (h1 << 5) + h1 ^ h2;
		}
	}
}