using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
	/// <summary>
	/// Provides creation of strings by using template and arguments.
	/// </summary>
	public static class StringTemplate
	{
		#region Methods

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate{T}"/> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding"/> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variables">The collection of the variables within the template.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="template"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="template"/> is empty.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringTemplate<String> Initialize(Encoding encoding, String template, params String[] variables)
		{
			return Initialize(encoding, template, (IReadOnlyList<String>) variables);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate{T}"/> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding"/> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variables">The collection of the variables within the template.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="template"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="variables"/> is <c>null</c>.</exception>
		public static StringTemplate<String> Initialize(Encoding encoding, String template, IReadOnlyList<String> variables)
		{
			// Check argument
			if (variables == null)
			{
				throw new ArgumentNullException(nameof(variables));
			}

			// Get variables keys
			var variablesKeys = variables.ToDictionary(x => x);

			return new StringTemplate<String>(encoding, template, variablesKeys);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate{T}"/> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding"/> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variablesKeys">The collection of the variables within the template.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="template"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="variablesKeys"/> is <c>null</c>.</exception>
		public static StringTemplate<T> Initialize<T>(Encoding encoding, String template, IReadOnlyDictionary<String, T> variablesKeys)
		{
			// Check argument
			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			// Check argument
			if (template == null)
			{
				throw new ArgumentNullException(nameof(template));
			}

			return new StringTemplate<T>(encoding, template, variablesKeys);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate{T}"/> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding"/> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variablesPattern">The <see cref="Regex"/> pattern of the variables.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="template"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="template"/> is empty.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="variablesPattern"/> is <c>null</c>.</exception>
		public static StringTemplate<String> Initialize(Encoding encoding, String template, String variablesPattern)
		{
			// Check argument
			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			// Check argument
			if (template == null)
			{
				throw new ArgumentNullException(nameof(template));
			}

			// Check argument
			if (variablesPattern == null)
			{
				throw new ArgumentNullException(nameof(variablesPattern));
			}

			// Get variables keys
			var variablesKeys = Regex.Matches(template, variablesPattern).Cast<Match>().Select(match => match.Value).Distinct().ToDictionary(x => x);

			return new StringTemplate<String>(encoding, template, variablesKeys);
		}

		#endregion
	}

	/// <summary>
	/// Provides creation of strings by using template and arguments.
	/// </summary>
	/// <typeparam name="T">The type of the key of the variable.</typeparam>
	public sealed class StringTemplate<T>
	{
		#region Nested Types

		/// <summary>
		/// Represents the method that tries to get <paramref name="value"/> of the variable by <paramref name="key"/>.
		/// </summary>
		/// <param name="key">The key of the variable.</param>
		/// <param name="value">A value of the variable if operation was successful, <c>null</c> otherwise.</param>
		/// <returns><c>true</c> if operation was successful, <c>false</c> otherwise.</returns>
		public delegate Boolean TryGetValue(T key, out String value);

		/// <summary>
		/// Encapsulates information about the variable within the string template.
		/// </summary>
		private struct VariableInfo
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="VariableInfo"/> structure.
			/// </summary>
			/// <param name="key">The key of the variable.</param>
			/// <param name="index">The index of the variable.</param>
			/// <param name="beginIndex">The index of the beginning of the variable within the string template.</param>
			/// <param name="endIndex">The index of the ending of the variable within the string template.</param>
			internal VariableInfo(T key, Int32 index, Int32 beginIndex, Int32 endIndex)
				: this()
			{
				Key = key;

				Index = index;

				BeginIndex = beginIndex;

				EndIndex = endIndex;
			}

			#endregion

			#region Properties

			/// <summary>
			/// The index of the beginning of the variable within the string template.
			/// </summary>
			internal Int32 BeginIndex
			{
				get;
			}

			/// <summary>
			/// The index of the ending of the variable within the string template.
			/// </summary>
			internal Int32 EndIndex
			{
				get;
			}

			/// <summary>
			/// The index of the variable.
			/// </summary>
			internal Int32 Index
			{
				get;
			}

			/// <summary>
			/// The key of the variable.
			/// </summary>
			internal T Key
			{
				get;
			}

			#endregion
		}

		/// <summary>
		/// Represents a segment within the template.
		/// </summary>
		private struct Segment
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of <see cref="Segment"/> structure.
			/// </summary>
			/// <param name="index">The index of the argument if segment is variable.</param>
			/// <param name="data">The data of the argument if segment is constant.</param>
			/// <param name="encodedData">The encoded data of the argument if segment is constant.</param>
			/// <param name="variableKey">The key of the variable</param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Segment(Int32 index, String data = null, Byte[] encodedData = null, T variableKey = default(T))
				: this()
			{
				Index = index;

				Data = data;

				EncodedData = encodedData;

				VariableKey = variableKey;

				IsVariable = data == null;
			}

			#endregion

			#region Properties

			/// <summary>
			/// The data of the argument if segment is constant.
			/// </summary>
			public String Data
			{
				get;
			}

			/// <summary>
			/// The encoded data of the argument if segment is constant.
			/// </summary>
			public Byte[] EncodedData
			{
				get;
			}

			/// <summary>
			/// The index of the argument if segment is variable.
			/// </summary>
			public Int32 Index
			{
				get;
			}

			/// <summary>
			/// Specifies whether template segment is variable.
			/// </summary>
			public Boolean IsVariable
			{
				get;
			}

			/// <summary>
			/// The key of the variable.
			/// </summary>
			public T VariableKey
			{
				get;
			}

			#endregion
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate{T}"/> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding"/> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="expectedVariablesKeys">The collection of the expected variables within the template.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal StringTemplate(Encoding encoding, String template, IReadOnlyDictionary<String, T> expectedVariablesKeys)
		{
			// Check argument
			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			// Check argument
			if (template == null)
			{
				throw new ArgumentNullException(nameof(template));
			}

			// Check argument
			if (expectedVariablesKeys == null)
			{
				throw new ArgumentNullException(nameof(expectedVariablesKeys));
			}

			Encoding = encoding;

			Template = template;

			Segments = GetSegments(expectedVariablesKeys);

			Variables = Segments.Where(segment => segment.IsVariable).Select(segment => segment.VariableKey).ToArray();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the <see cref="Encoding"/> which is used for the template instantiation.
		/// </summary>
		public Encoding Encoding
		{
			get;
		}

		/// <summary>
		/// The collection of the segments of the template.
		/// </summary>
		private Segment[] Segments
		{
			get;
		}

		/// <summary>
		/// Gets the original template.
		/// </summary>
		public String Template
		{
			get;
		}

		/// <summary>
		/// The collection of the names of the variables within the template.
		/// </summary>
		public IReadOnlyList<T> Variables
		{
			get;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Parses the template and gets the collection of segments.
		/// </summary>
		/// <param name="expectedVariablesKeys">The collection of the expected variables within the template.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Segment[] GetSegments(IReadOnlyCollection<KeyValuePair<String, T>> expectedVariablesKeys)
		{
			// Check if there is no expected variables
			if (expectedVariablesKeys.Count == 0)
			{
				return new[]
				{
					new Segment(0, Template)
				};
			}

			// Try find variables in template
			var variables = new List<VariableInfo>();

			Int32 argumentIndex = 0, templatePointerPosition;

			foreach (var variable in expectedVariablesKeys)
			{
				templatePointerPosition = 0;

				while (true)
				{
					// Get argument start index
					var argumentStartIndex = Template.IndexOf(variable.Key, templatePointerPosition, StringComparison.Ordinal);

					if (argumentStartIndex == -1)
					{
						break;
					}

					// Calculate argument end index
					var argumentEndIndex = argumentStartIndex + variable.Key.Length;

					// Add to the list
					variables.Add(new VariableInfo(variable.Value, argumentIndex, argumentStartIndex, argumentEndIndex));

					templatePointerPosition = argumentEndIndex;
				}

				argumentIndex++;
			}

			// Check if there is no variables
			if (variables.Count == 0)
			{
				return new[]
				{
					new Segment(0, Template)
				};
			}

			templatePointerPosition = 0;

			// Sort by start index
			var sortedList = variables.OrderBy(x => x.BeginIndex).ToArray();

			var result = new List<Segment>();

			for (var index = 0; index < sortedList.Length; index++)
			{
				var argument = sortedList[index];

				String subString;

				// Check if argument is not first in template
				if (argument.BeginIndex != 0)
				{
					// Get sub string
					subString = Template.Substring(templatePointerPosition, argument.BeginIndex - templatePointerPosition);

					// Add constant segment
					result.Add(new Segment(-1, subString, Encoding.GetBytes(subString)));
				}

				// Add variable segment
				result.Add(new Segment(argument.Index, null, null, argument.Key));

				// Update current index
				templatePointerPosition = argument.EndIndex;

				// Check if argument is last and there is no more const data
				if ((index != sortedList.Length - 1) || (argument.EndIndex >= Template.Length - 1))
				{
					continue;
				}

				// Get sub string
				subString = Template.Substring(templatePointerPosition, Template.Length - argument.EndIndex);

				// Add constant segment
				result.Add(new Segment(-1, subString, Encoding.GetBytes(subString)));
			}

			return result.ToArray();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Instantiates the template using collection of <paramref name="arguments"/> specified.
		/// </summary>
		/// <param name="arguments">The collection of the arguments to use for template instantiation.</param>
		/// <returns>An instance of the template.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String Instantiate(params String[] arguments)
		{
			return Instantiate((IReadOnlyList<String>) arguments);
		}

		/// <summary>
		/// Instantiates the template using the collection of <paramref name="arguments"/>.
		/// </summary>
		/// <param name="arguments">The collection of the arguments to use for template instantiation.</param>
		/// <returns>An instance of the template.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String Instantiate(IReadOnlyList<String> arguments)
		{
			// Check if template contains only one not variable segment
			if (Segments.Length == 1 && !Segments[0].IsVariable)
			{
				return Segments[0].Data;
			}

			var builedr = new StringBuilder();

			foreach (var item in Segments)
			{
				builedr.Append(item.IsVariable ? arguments[item.Index] : item.Data);
			}

			return builedr.ToString();
		}

		/// <summary>
		/// Instantiates the template using <paramref name="tryGetValue"/> method.
		/// </summary>
		/// <param name="tryGetValue">A delegate to the method which tries to get value of the argument by name.</param>
		/// <returns>An instance of the template.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String Instantiate(TryGetValue tryGetValue)
		{
			// Check if template contains only one not variable segment
			if (Segments.Length == 1 && !Segments[0].IsVariable)
			{
				return Segments[0].Data;
			}

			var builedr = new StringBuilder();

			foreach (var item in Segments)
			{
				if (item.IsVariable)
				{
					String data;

					if (tryGetValue(item.VariableKey, out data))
					{
						builedr.Append(data);
					}
				}
				else
				{
					builedr.Append(item.Data);
				}
			}

			return builedr.ToString();
		}

		/// <summary>
		/// Instantiates the template using the collection of <paramref name="arguments"/>.
		/// </summary>
		/// <param name="stream">The output stream for the instance of the template.</param>
		/// <param name="arguments">An array of the encoded arguments to use for template instantiation.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Instantiate(Stream stream, params String[] arguments)
		{
			Instantiate(stream, (IReadOnlyList<String>) arguments);
		}

		/// <summary>
		/// Instantiates the template using the collection of <paramref name="arguments"/>.
		/// </summary>
		/// <param name="stream">The output stream for the instance of the template.</param>
		/// <param name="arguments">An array of the encoded arguments to use for template instantiation.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Instantiate(Stream stream, IReadOnlyList<String> arguments)
		{
			for (var index = 0; index < Segments.Length; index++)
			{
				var segment = Segments[index];

				if (segment.IsVariable)
				{
					// Check
					if (segment.Index >= arguments.Count)
					{
						continue;
					}

					// Get argument
					var argument = arguments[segment.Index];

					// Encode argumnent
					var encodedArgument = Encoding.GetBytes(argument);

					stream.Write(encodedArgument, 0, encodedArgument.Length);
				}
				else
				{
					stream.Write(segment.EncodedData, 0, segment.EncodedData.Length);
				}
			}
		}

		/// <summary>
		/// Instantiates the template using <paramref name="tryGetValue"/> method.
		/// </summary>
		/// <param name="stream">An output stream for the instance of the template.</param>
		/// <param name="tryGetValue">A delegate to the method which tries to get value of the argument by name.</param>
		/// <returns>A string instance of template.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Instantiate(Stream stream, TryGetValue tryGetValue)
		{
			for (var index = 0; index < Segments.Length; index++)
			{
				var segment = Segments[index];

				if (segment.IsVariable)
				{
					// Try get argument
					String argument;

					if (!tryGetValue(segment.VariableKey, out argument))
					{
						continue;
					}

					// Encode argumnent
					var encodedArgument = Encoding.GetBytes(argument);

					stream.Write(encodedArgument, 0, encodedArgument.Length);
				}
				else
				{
					stream.Write(segment.EncodedData, 0, segment.EncodedData.Length);
				}
			}
		}

		/// <summary>
		/// Initiates an asynchronous operation to instantiate the template using specified <paramref name="arguments"/>.
		/// </summary>
		/// <param name="stream">An output stream for the instance of the template.</param>
		/// <param name="arguments">An array of encoded arguments to use for template instantiation.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Task InstantiateAsync(Stream stream, params String[] arguments)
		{
			return InstantiateAsync(stream, (IReadOnlyList<String>) arguments);
		}

		/// <summary>
		/// Initiates an asynchronous operation to instantiate the template using specified <paramref name="arguments"/>.
		/// </summary>
		/// <param name="stream">An output stream for the instance of the template.</param>
		/// <param name="arguments">An array of encoded arguments to use for template instantiation.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async Task InstantiateAsync(Stream stream, IReadOnlyList<String> arguments)
		{
			for (var index = 0; index < Segments.Length; index++)
			{
				var segment = Segments[index];

				if (segment.IsVariable)
				{
					// Check
					if (segment.Index >= arguments.Count)
					{
						continue;
					}

					// Get argument
					var argument = arguments[segment.Index];

					// Encode argumnent
					var encodedArgument = Encoding.GetBytes(argument);

					await stream.WriteAsync(encodedArgument, 0, encodedArgument.Length);
				}
				else
				{
					await stream.WriteAsync(segment.EncodedData, 0, segment.EncodedData.Length);
				}
			}
		}

		/// <summary>
		/// Initiates an asynchronous operation to instantiate the template using <paramref name="tryGetValue"/> method.
		/// </summary>
		/// <param name="stream">An output stream for the instance of the template.</param>
		/// <param name="tryGetValue">A delegate to the method which tries to get value of the argument by name.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async Task InstantiateAsync(Stream stream, TryGetValue tryGetValue)
		{
			for (var index = 0; index < Segments.Length; index++)
			{
				var segment = Segments[index];

				if (segment.IsVariable)
				{
					// Try get argument
					String argument;

					if (!tryGetValue(segment.VariableKey, out argument))
					{
						continue;
					}

					// Encode argumnent
					var encodedArgument = Encoding.GetBytes(argument);

					stream.Write(encodedArgument, 0, encodedArgument.Length);
				}
				else
				{
					await stream.WriteAsync(segment.EncodedData, 0, segment.EncodedData.Length);
				}
			}
		}

		#endregion
	}
}