using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides management of the instances of the <see cref="MemoryStream"/> class.
	/// </summary>
	public sealed class MemoryStreamManager : IDisposable
	{
		#region Fields

		/// <summary>
		/// A dictionary of ready to be used streams grouped by size.
		/// </summary>
		private readonly ConcurrentQueue<MemoryStream> freeStreams;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="MemoryStreamManager"/> class.
		/// </summary>
		/// <param name="capacity">The maximal capacity of the <see cref="MemoryStream"/> in bytes.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than<c>1</c>.</exception>
		public MemoryStreamManager(Int32 capacity)
		{
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), @"is less than 1");
			}

			Capacity = (Int32) ((UInt32) capacity).Align(128);

			freeStreams = new ConcurrentQueue<MemoryStream>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the capacity of <see cref="MemoryStream"/>.
		/// </summary>
		public Int32 Capacity
		{
			get;
		}

		#endregion

		#region Methods of IDisposable

		/// <summary>
		/// Releases resources.
		/// </summary>
		public void Dispose()
		{
			foreach (var memoryStream in freeStreams)
			{
				memoryStream.Dispose();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets a stream.
		/// </summary>
		/// <returns>An instance of <see cref="MemoryStream"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public MemoryStream Get()
		{
			MemoryStream result;

			// Try get the stream from the queue
			if (!freeStreams.TryDequeue(out result))
			{
				result = new MemoryStream(Capacity);
			}

			// Create a new stream
			return result;
		}

		/// <summary>
		/// Puts <paramref name="stream"/> into the set of free streams.
		/// </summary>
		/// <param name="stream">An instance of <see cref="MemoryStream"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Put(MemoryStream stream)
		{
			if (stream == null)
			{
				return;
			}

			// Reset stream
			stream.SetLength(0);

			// Enqueue a stream
			freeStreams.Enqueue(stream);
		}

		#endregion
	}
}