using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	/// <summary>
	/// Provides a load tests infrastructure.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class LoadTest
	{
		#region Nested Types

		/// <summary>
		/// Represents results of load test run.
		/// </summary>
		public sealed class ComparedResults
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of <see cref="ComparedResults" /> class.
			/// </summary>
			/// <param name="firstTestResult">First test run result.</param>
			/// <param name="secondTestResult">Second test run result.</param>
			internal ComparedResults(Result firstTestResult, Result secondTestResult)
			{
				FirstTestResult = firstTestResult;

				SecondTestResult = secondTestResult;

				DurationDifference = firstTestResult.Duration - secondTestResult.Duration;

				VelocityDifference = firstTestResult.Velocity - secondTestResult.Velocity;

				AverageIterationTimeDifference = firstTestResult.AverageIterationTime - firstTestResult.AverageIterationTime;
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets a difference in average time in milliseconds taken for single action execution by two tests.
			/// </summary>
			public Int64 AverageIterationTimeDifference
			{
				get;
			}

			/// <summary>
			/// Gets a difference in duration of two tests.
			/// </summary>
			public TimeSpan DurationDifference
			{
				get;
			}

			/// <summary>
			/// Gets a first test results.
			/// </summary>
			public Result FirstTestResult
			{
				get;
			}

			/// <summary>
			/// Gets a second test results.
			/// </summary>
			public Result SecondTestResult
			{
				get;
			}

			/// <summary>
			/// Gets a difference in count of action executed per second by two tests.
			/// </summary>
			public Int64 VelocityDifference
			{
				get;
			}

			#endregion

			#region Overrides of object

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			public override String ToString()
			{
				var stringBuilder = new StringBuilder();

				stringBuilder.AppendLine();

				stringBuilder.AppendFormat("Load Tests Results Comparsion\t: {0} vs {1}\r\n", FirstTestResult.Id, SecondTestResult.Id);

				stringBuilder.AppendFormat("Duration Difference\t\t: {0} ms.\r\n", (Int64) DurationDifference.TotalMilliseconds);

				stringBuilder.AppendFormat("Velocity Difference\t\t: {0} ops.\r\n", VelocityDifference);

				stringBuilder.AppendFormat("Aciton Time Difference\t\t: {0} nanaoseconds. (Average)\r\n", AverageIterationTimeDifference);

				stringBuilder.Append(FirstTestResult);

				stringBuilder.Append(SecondTestResult);

				return stringBuilder.ToString();
			}

			#endregion
		}

		/// <summary>
		/// Represents results of load test run.
		/// </summary>
		public sealed class Result
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of <see cref="Result" /> class.
			/// </summary>
			/// <param name="id">Test identifier.</param>
			/// <param name="iterationCount">A number of times a test was run.</param>
			/// <param name="duration">Total elapsed test run time, in milliseconds.</param>
			internal Result(String id, Int64 iterationCount, TimeSpan duration)
			{
				IterationCount = iterationCount;

				Id = id;

				Duration = duration;

				Velocity = (Int64) ((iterationCount * 1000) / duration.TotalMilliseconds);

				AverageIterationTime = (duration.Ticks * 100) / iterationCount;
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets average time in milliseconds taken for single action execution.
			/// </summary>
			public Int64 AverageIterationTime
			{
				get;
			}

			/// <summary>
			/// Gets a test duration.
			/// </summary>
			public TimeSpan Duration
			{
				get;
			}

			/// <summary>
			/// Gets a test identifier.
			/// </summary>
			public String Id
			{
				get;
			}

			/// <summary>
			/// Gets a count of times an action was executed.
			/// </summary>
			public Int64 IterationCount
			{
				get;
			}

			/// <summary>
			/// Gets a count of action executed per second.
			/// </summary>
			public Int64 Velocity
			{
				get;
			}

			#endregion

			#region Overrides of object

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			public override String ToString()
			{
				var stringBuilder = new StringBuilder();

				stringBuilder.AppendFormat("\r\nLoad Test\t\t: {0}\r\n", Id);

				stringBuilder.AppendFormat("Iteration Count\t\t: {0}\r\n", IterationCount);

				stringBuilder.AppendFormat("Duration\t\t\t: {0:N0} {1}.\r\n", Duration.TotalMilliseconds, TimeUnit.Millisecond);

				stringBuilder.AppendFormat("Velocity\t\t\t: {0} operations\\{1}.\r\n", Velocity, TimeUnit.Second);

				stringBuilder.AppendFormat("Average aciton time\t: {0} {1}.\r\n", AverageIterationTime, "Nanosecond");

				return stringBuilder.ToString();
			}

			#endregion
		}

		#endregion

		#region Methods

		/// <summary>
		/// Executes <paramref name="action" /> specified by <paramref name="iterationCount" /> number of times in a single task.
		/// </summary>
		/// <param name="testId">Test identifier.</param>
		/// <param name="action">An action to be executed.</param>
		/// <param name="iterationCount">A count of times an action will be executed.</param>
		/// <returns>A result of test execution.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="action" /> is <c>null</c>.</exception>
		public static Result Execute(String testId, Action<Int64> action, Int64 iterationCount)
		{
			// Check context
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			// Start stopwatch
			var stopwatch = Stopwatch.StartNew();

			// Execute action
			for (var index = 0; index < iterationCount; index++)
			{
				action(index);
			}

			// Stop stopwatch
			stopwatch.Stop();

			return new Result(testId, iterationCount, stopwatch.Elapsed);
		}

		/// <summary>
		/// Executes two tests to compare results.
		/// </summary>
		/// <param name="firstTestId">An identifier of the first test.</param>
		/// <param name="firstAction">An action of the first test.</param>
		/// <param name="secondTestId">An identifier of the second test.</param>
		/// <param name="secondAction">An action of the second test.</param>
		/// <param name="iterationCount">A count of times an action will be executed.</param>
		/// <returns>A result of test execution.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="firstAction" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="secondAction" /> is <c>null</c>.</exception>
		public static ComparedResults ExecuteCompare(String firstTestId, Action<Int64> firstAction, String secondTestId, Action<Int64> secondAction, Int64 iterationCount)
		{
			// Execute first action
			var firstTestReusult = Execute(firstTestId, firstAction, iterationCount);

			// Execute second action
			var secondTestReusult = Execute(secondTestId, secondAction, iterationCount);

			return new ComparedResults(firstTestReusult, secondTestReusult);
		}

		/// <summary>
		/// Asynchronously executes an <paramref name="action" /> specified by <paramref name="iterationCount" /> number of times in a specified by <paramref name="tasksCount" /> number of parallel tasks.
		/// </summary>
		/// <param name="testId">Test identifier.</param>
		/// <param name="action">An action to be executed.</param>
		/// <param name="iterationCount">A count of times an action will be executed.</param>
		/// <param name="tasksCount">A number of parallel tasks to execute <paramref name="action" />.</param>
		/// <returns>A result of test execution.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="action" /> is <c>null</c>.</exception>
		public static async Task<Result> ExecuteParallelAsync(String testId, Action<Int64> action, Int64 iterationCount, Int64 tasksCount)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			Action<Int64, Int64> atom = (from, to) =>
			{
				for (var index = from; index < to; index++)
				{
					action(index);
				}
			};

			var tasks = new Task[tasksCount];

			Int64 rest;

			var increment = Math.DivRem(iterationCount, tasksCount, out rest);

			// Start stopwatch
			var stopwatch = Stopwatch.StartNew();

			for (var taskIndex = 0; taskIndex < tasksCount; taskIndex++)
			{
				var from = taskIndex * increment;

				var to = taskIndex * increment + increment + (taskIndex == (tasksCount - 1) ? rest : 0);

				tasks[taskIndex] = Task.Run(() => atom(from, to));
			}

			// Wait all tasks to complete
			await Task.WhenAll(tasks);

			// Stop stopwatch
			stopwatch.Stop();

			return new Result(testId, iterationCount, stopwatch.Elapsed);
		}

		/// <summary>
		/// Asynchronously executes a <paramref name="func" /> specified by <paramref name="iterationCount" /> number of times in a specified by <paramref name="tasksCount" /> number of parallel tasks.
		/// </summary>
		/// <param name="testId">Test identifier.</param>
		/// <param name="func">A function to to be executed.</param>
		/// <param name="iterationCount">A count of times an action will be executed.</param>
		/// <param name="tasksCount">A number of parallel tasks to execute <paramref name="func" />.</param>
		/// <returns>A result of test execution.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="func" /> is <c>null</c>.</exception>
		public static async Task<Result> ExecuteParallelAsync(String testId, Func<Int64, Task> func, Int64 iterationCount, Int64 tasksCount)
		{
			if (func == null)
			{
				throw new ArgumentNullException(nameof(func));
			}

			Func<Int64, Int64, Task> atom = async (from, to) =>
			{
				for (var index = from; index < to; index++)
				{
					await func(index);
				}
			};

			Int64 rest;

			var increment = Math.DivRem(iterationCount, tasksCount, out rest);

			var tasks = new Task[tasksCount];

			// Start stopwatch
			var stopwatch = Stopwatch.StartNew();

			for (var taskIndex = 0; taskIndex < tasksCount; taskIndex++)
			{
				var from = taskIndex * increment;

				var to = taskIndex * increment + increment + (taskIndex == (tasksCount - 1) ? rest : 0);

				tasks[taskIndex] = atom(from, to);
			}

			// Wait all tasks to complete
			await Task.WhenAll(tasks);

			// Stop stopwatch
			stopwatch.Stop();

			return new Result(testId, iterationCount, stopwatch.Elapsed);
		}

		/// <summary>
		/// Executes transformation test.
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <param name="iterationCount"></param>
		/// <param name="initialize"></param>
		/// <param name="forward"></param>
		/// <param name="backward"></param>
		/// <param name="onError"></param>
		/// <returns>A result of test execution.</returns>
		public static Tuple<Result, Result> ExecuteTransformation<TSource, TTarget>(Int64 iterationCount, Func<Int64, TSource> initialize, Func<TSource, TTarget> forward, Func<TTarget, TSource> backward, Action<Int64, TSource, TTarget, TSource> onError) where TSource : IEquatable<TSource>
		{
			if (initialize == null)
			{
				throw new ArgumentNullException(nameof(initialize));
			}

			if (forward == null)
			{
				throw new ArgumentNullException(nameof(forward));
			}

			if (backward == null)
			{
				throw new ArgumentNullException(nameof(backward));
			}

			if (onError == null)
			{
				throw new ArgumentNullException(nameof(onError));
			}

			// Initialize array of expected results
			var expectedResults = new TSource[iterationCount];

			for (var index = 0; index < iterationCount; index++)
			{
				expectedResults[index] = initialize(index);
			}

			// Initialize array of intermediate results
			var intermediateResults = new TTarget[iterationCount];

			// Execute to target transformation
			var forwardResult = Execute("To Target Type Transformation", index => intermediateResults[index] = forward(expectedResults[index]), iterationCount);

			// Initialize array of actual results
			var actualResults = new TSource[iterationCount];

			// Execute to source transformation
			var backwardResult = Execute("To Source Type Transformation", index => actualResults[index] = backward(intermediateResults[index]), iterationCount);

			// Check results
			for (var index = 0; index < iterationCount; index++)
			{
				var expectedResult = expectedResults[index];

				var intermediateResult = intermediateResults[index];

				var actualResult = actualResults[index];

				if (!expectedResult.Equals(actualResult))
				{
					onError(index, expectedResult, intermediateResult, actualResult);
				}
			}

			return new Tuple<Result, Result>(forwardResult, backwardResult);
		}

		#endregion
	}
}