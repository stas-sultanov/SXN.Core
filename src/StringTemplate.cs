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
	public sealed class StringTemplate
	{
		#region Nested Types

		/// <summary>
		/// Encapsulates information about the variable within the string template.
		/// </summary>
		private struct VariableInfo
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="VariableInfo" /> structure.
			/// </summary>
			/// <param name="name">The name of the variable.</param>
			/// <param name="index"></param>
			/// <param name="beginIndex">The index of the beginning of the variable within the string template.</param>
			/// <param name="endIndex">The index of the ending of the variable within the string template.</param>
			internal VariableInfo(String name, Int32 index, Int32 beginIndex, Int32 endIndex)
				: this()
			{
				Name = name;

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
			/// The index of
			/// </summary>
			internal Int32 Index
			{
				get;
			}

			/// <summary>
			/// The name of the variable.
			/// </summary>
			internal String Name
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
			#region Fields

			/// <summary>
			/// The data of the argument if segment is constant.
			/// </summary>
			internal readonly String data;

			/// <summary>
			/// The encoded data of the argument if segment is constant.
			/// </summary>
			internal readonly Byte[] encodedData;

			/// <summary>
			/// The index of the argument if segment is variable.
			/// </summary>
			internal readonly Int32 index;

			/// <summary>
			/// Specifies whether template segment is variable.
			/// </summary>
			internal readonly Boolean isVariable;

			/// <summary>
			/// The name of the segment.
			/// </summary>
			internal readonly String name;

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of <see cref="Segment" /> structure.
			/// </summary>
			/// <param name="index">The index of the argument if segment is variable.</param>
			/// <param name="data">The data of the argument if segment is constant.</param>
			/// <param name="encodedData">The encoded data of the argument if segment is constant.</param>
			/// <param name="name">The name of the argument.</param>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal Segment(Int32 index, String data = null, Byte[] encodedData = null, String name = null)
			{
				this.index = index;

				this.data = data;

				this.encodedData = encodedData;

				this.name = name;

				isVariable = data == null;
			}

			#endregion
		}

		#endregion

		#region Fields

		private readonly Segment[] segments;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate" /> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding" /> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variablesPattern">The <see cref="Regex" /> pattern of the variables.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="template" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="template" /> is empty.</exception>
		public StringTemplate(Encoding encoding, String template, String variablesPattern)
		{
			// Check argument
			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			Encoding = encoding;

			// Check argument
			if (template == null)
			{
				throw new ArgumentNullException(nameof(template));
			}

			Template = template;

			// Check argument
			if (variablesPattern == null)
			{
				throw new ArgumentNullException(nameof(variablesPattern));
			}

			// Find variables that matches the pattern
			Variables = Regex.Matches(template, variablesPattern).Cast<Match>().Select(match => match.Value).Distinct().ToArray();

			// Get segments
			segments = GetSegments(encoding, template, Variables);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate" /> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding" /> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variables">The collection of the variables within the template.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="template" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="template" /> is empty.</exception>
		public StringTemplate(Encoding encoding, String template, params String[] variables)
			: this(encoding, template, (IReadOnlyList<String>) variables)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplate" /> class.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding" /> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variables">The collection of the variables within the template.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="template" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="variables" /> is <c>null</c>.</exception>
		public StringTemplate(Encoding encoding, String template, IReadOnlyList<String> variables)
		{
			// Check argument
			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			Encoding = encoding;

			// Check argument
			if (template == null)
			{
				throw new ArgumentNullException(nameof(template));
			}

			Template = template;

			// Check argument
			if (variables == null)
			{
				throw new ArgumentNullException(nameof(variables));
			}

			// Set variables
			Variables = variables;

			// Get segments
			segments = GetSegments(encoding, template, variables);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the <see cref="Encoding" /> which is used for the template instantiation.
		/// </summary>
		public Encoding Encoding
		{
			get;
		}

		/// <summary>
		/// Gets the original template.
		/// </summary>
		public String Template
		{
			get;

			private set;
		}

		/// <summary>
		/// The collection of the names of the variables within the template.
		/// </summary>
		public IReadOnlyList<String> Variables
		{
			get;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Parses the <paramref name="template" /> and gets the collection of segments.
		/// </summary>
		/// <param name="encoding">The <see cref="Encoding" /> which is used for the template instantiation.</param>
		/// <param name="template">The string template.</param>
		/// <param name="variables">The collection of the variables within the template.</param>
		private static Segment[] GetSegments(Encoding encoding, String template, IReadOnlyList<String> variables)
		{
			// Check if there is no variables in template
			if (variables.Count == 0)
			{
				return new[]
				{
					new Segment(0, template)
				};
			}

			var variableSegments = new List<VariableInfo>();

			Int32 currentIndex;

			for (var argumentIndex = 0; argumentIndex < variables.Count; argumentIndex++)
			{
				// Get argument
				var variableName = variables[argumentIndex];

				currentIndex = 0;

				while (true)
				{
					// Get argument start index
					var argumentStartIndex = template.IndexOf(variableName, currentIndex, StringComparison.Ordinal);

					if (argumentStartIndex == -1)
					{
						break;
					}

					// Calculate argument end index
					var argumentEndIndex = argumentStartIndex + variableName.Length;

					// Add to the list
					variableSegments.Add(new VariableInfo(variableName, argumentIndex, argumentStartIndex, argumentEndIndex));

					currentIndex = argumentEndIndex;
				}
			}

			currentIndex = 0;

			// Sort by start index
			var sortedList = variableSegments.OrderBy(x => x.BeginIndex).ToArray();

			var result = new List<Segment>();

			for (var index = 0; index < sortedList.Length; index++)
			{
				var argument = sortedList[index];

				String subString;

				// Check if argument is not first in template
				if (argument.BeginIndex != 0)
				{
					// Get sub string
					subString = template.Substring(currentIndex, argument.BeginIndex - currentIndex);

					// Add constant segment
					result.Add(new Segment(-1, subString, encoding.GetBytes(subString)));
				}

				// Add variable segment
				result.Add(new Segment(argument.Index, null, null, argument.Name));

				// Update current index
				currentIndex = argument.EndIndex;

				// Check if argument is last and there is no more const data
				if ((index != sortedList.Length - 1) || (argument.EndIndex >= template.Length - 1))
				{
					continue;
				}

				// Get sub string
				subString = template.Substring(currentIndex, template.Length - argument.EndIndex);

				// Add constant segment
				result.Add(new Segment(-1, subString, encoding.GetBytes(subString)));
			}

			return result.ToArray();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Encodes arguments with <see cref="Encoding" /> into a sequence of bytes.
		/// </summary>
		/// <param name="arguments">A dictionary of arguments to encode.</param>
		/// <returns>A dictionary of encoded arguments.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlyDictionary<String, Byte[]> EncodeArguments(IReadOnlyDictionary<String, String> arguments)
		{
			var result = new Dictionary<String, Byte[]>(arguments.Count);

			foreach (var pair in arguments)
			{
				result.Add(pair.Key, Encoding.GetBytes(pair.Value));
			}

			return result;
		}

		/// <summary>
		/// Encodes arguments with <see cref="Encoding" /> into a sequence of bytes.
		/// </summary>
		/// <param name="args">An array of arguments to encode.</param>
		/// <returns>An array of encoded arguments.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlyList<Byte[]> EncodeArguments(params String[] args)
		{
			var result = new Byte[args.Length][];

			for (var index = 0; index < args.Length; index++)
			{
				var argument = args[index];

				result[index] = Encoding.GetBytes(argument);
			}

			return result;
		}

		/// <summary>
		/// Instantiates the template using <paramref name="args" /> specified.
		/// </summary>
		/// <param name="args">An array of the arguments to use for template instantiation.</param>
		/// <returns>A string instance of template.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String Instantiate(params String[] args)
		{
			// Check if template contains only one not variable segment
			if (segments.Length == 1 && !segments[0].isVariable)
			{
				return segments[0].data;
			}

			var builedr = new StringBuilder();

			foreach (var item in segments)
			{
				builedr.Append(item.isVariable ? args[item.index] : item.data);
			}

			return builedr.ToString();
		}

		/// <summary>
		/// Instantiates the template using <paramref name="args" /> specified.
		/// </summary>
		/// <param name="args">A dictionary of the arguments to use for template instantiation.</param>
		/// <returns>A string instance of template.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String Instantiate(IReadOnlyDictionary<String, String> args)
		{
			// Check if template contains only one not variable segment
			if (segments.Length == 1 && !segments[0].isVariable)
			{
				return segments[0].data;
			}

			var builedr = new StringBuilder();

			foreach (var item in segments)
			{
				if (item.isVariable)
				{
					String data;

					if (args.TryGetValue(item.name, out data))
					{
						builedr.Append(data);
					}
				}
				else
				{
					builedr.Append(item.data);
				}
			}

			return builedr.ToString();
		}

		/// <summary>
		/// Instantiates the template using specified <paramref name="encodedArguments" />.
		/// </summary>
		/// <param name="stream">The output stream for the instance of the template.</param>
		/// <param name="encodedArguments">An array of the encoded arguments to use for template instantiation.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Instantiate(Stream stream, IReadOnlyList<Byte[]> encodedArguments)
		{
			for (var index = 0; index < segments.Length; index++)
			{
				var segment = segments[index];

				if (segment.isVariable)
				{
					var ancodedArgument = encodedArguments[segment.index];

					stream.Write(ancodedArgument, 0, ancodedArgument.Length);
				}
				else
				{
					stream.Write(segment.encodedData, 0, segment.encodedData.Length);
				}
			}
		}

		/// <summary>
		/// Instantiates the template using specified <paramref name="encodedArguments" />.
		/// </summary>
		/// <param name="stream">An output stream for the instance of the template.</param>
		/// <param name="encodedArguments">A dictionary of the encoded arguments to use for template instantiation.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Instantiate(Stream stream, IReadOnlyDictionary<String, Byte[]> encodedArguments)
		{
			for (var index = 0; index < segments.Length; index++)
			{
				var segment = segments[index];

				if (segment.isVariable)
				{
					Byte[] encodedData;

					if (encodedArguments.TryGetValue(segment.name, out encodedData))
					{
						stream.Write(encodedData, 0, encodedData.Length);
					}
				}
				else
				{
					stream.Write(segment.encodedData, 0, segment.encodedData.Length);
				}
			}
		}

		/// <summary>
		/// Initiates an asynchronous operation to instantiate the template using specified <paramref name="encodedArguments" />.
		/// </summary>
		/// <param name="stream">An output stream for the instance of the template.</param>
		/// <param name="encodedArguments">An array of encoded arguments to use for template instantiation.</param>
		/// <returns>A <see cref="Task" /> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async Task InstantiateAsync(Stream stream, IReadOnlyList<Byte[]> encodedArguments)
		{
			for (var index = 0; index < segments.Length; index++)
			{
				var segment = segments[index];

				if (segment.isVariable)
				{
					var encodedArgument = encodedArguments[segment.index];

					await stream.WriteAsync(encodedArgument, 0, encodedArgument.Length);
				}
				else
				{
					await stream.WriteAsync(segment.encodedData, 0, segment.encodedData.Length);
				}
			}
		}

		/// <summary>
		/// Initiates an asynchronous operation to instantiate the template using specified <paramref name="encodedArguments" />.
		/// </summary>
		/// <param name="stream">An output stream for the instance of the template.</param>
		/// <param name="encodedArguments">A dictionary of the encoded arguments to use for template instantiation.</param>
		/// <returns>A <see cref="Task" /> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async Task InstantiateAsync(Stream stream, IReadOnlyDictionary<String, Byte[]> encodedArguments)
		{
			for (var index = 0; index < segments.Length; index++)
			{
				var segment = segments[index];

				if (segment.isVariable)
				{
					var encodedArgument = encodedArguments[segment.name];

					await stream.WriteAsync(encodedArgument, 0, encodedArgument.Length);
				}
				else
				{
					await stream.WriteAsync(segment.encodedData, 0, segment.encodedData.Length);
				}
			}
		}

		#endregion
	}
}