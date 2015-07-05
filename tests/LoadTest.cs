using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdRout.Themis
{
	/// <summary>
	/// Provides a base class for load tests.
	/// </summary>
	public static class LoadTest
	{
		#region Nested Types

		#region Nested type: ComparedResults

		/// <summary>
		/// Represents results of load test run.
		/// </summary>
		public sealed class ComparedResults
		{
			#region Contstructors

			/// <summary>
			/// Initializes a new instance of <see cref="ComparedResults"/> class.
			/// </summary>
			/// <param name="firstTestResult">First test run result.</param>
			/// <param name="secondTestResult">Second test run result.</param>
			internal ComparedResults(Result firstTestResult, Result secondTestResult)
			{
				FirstTestResult = firstTestResult;

				SecondTestResult = secondTestResult;

				DurationDifference = firstTestResult.Duration - secondTestResult.Duration;

				VelocityDifference = firstTestResult.Velocity - secondTestResult.Velocity;

				AverageIterationTimeDiffernece = firstTestResult.AverageIterationTime - firstTestResult.AverageIterationTime;
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets a first test results.
			/// </summary>
			public Result FirstTestResult
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets a second test results.
			/// </summary>
			public Result SecondTestResult
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets a difference in duration of two tests.
			/// </summary>
			public TimeSpan DurationDifference
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets a difference in count of action executed per second by two tests.
			/// </summary>
			public Int64 VelocityDifference
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets a difference in average time in milliseconds taken for single action execution by two tests.
			/// </summary>
			public Int64 AverageIterationTimeDiffernece
			{
				get;

				private set;
			}

			#endregion

			#region Methods Overrides of Object

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			public override string ToString()
			{
				var stringBuilder = new StringBuilder();

				stringBuilder.AppendLine();

				stringBuilder.AppendFormat("Load Tests Results Comparsion\t: {0} vs {1}\r\n", FirstTestResult.Id, SecondTestResult.Id);

				stringBuilder.AppendFormat("Duration Difference\t\t: {0} ms.\r\n", (Int64) DurationDifference.TotalMilliseconds);

				stringBuilder.AppendFormat("Velocity Difference\t\t: {0} ops.\r\n", VelocityDifference);

				stringBuilder.AppendFormat("Aciton Time Difference\t\t: {0} ns. (Average)\r\n", AverageIterationTimeDiffernece);

				stringBuilder.Append(FirstTestResult);

				stringBuilder.Append(SecondTestResult);

				return stringBuilder.ToString();
			}

			#endregion
		}

		#endregion

		#region Nested type: Result

		/// <summary>
		/// Represents results of load test run.
		/// </summary>
		public sealed class Result
		{
			#region Contstructors

			/// <summary>
			/// Initializes a new instance of <see cref="Result"/> class.
			/// </summary>
			/// <param name="iterationCount">A number of times a test was run.</param>
			/// <param name="id">Test identifier.</param>
			/// <param name="duration">Total elapsed test run time, in milliseconds.</param>
			internal Result(Int64 iterationCount, String id, Int64 duration)
			{
				IterationCount = iterationCount;

				Id = id;

				Duration = TimeSpan.FromMilliseconds(duration);

				Velocity = (iterationCount * 1000) / duration;

				AverageIterationTime = (duration * 1000000) / iterationCount;
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets a test identifier.
			/// </summary>
			public String Id
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets a count of times an action was executed.
			/// </summary>
			public Int64 IterationCount
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets a test duration.
			/// </summary>
			public TimeSpan Duration
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets a count of action executed per second.
			/// </summary>
			public Int64 Velocity
			{
				get;

				private set;
			}

			/// <summary>
			/// Gets average time in milliseconds taken for single action execution.
			/// </summary>
			public Int64 AverageIterationTime
			{
				get;

				private set;
			}

			#endregion

			#region Methods Overrides of Object

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			public override string ToString()
			{
				var stringBuilder = new StringBuilder();

				stringBuilder.AppendFormat("\r\nLoad Test\t: {0}\r\n", Id);

				stringBuilder.AppendFormat("Iteration Count\t: {0}\r\n", IterationCount);

				stringBuilder.AppendFormat("Duration\t\t: {0} ms.\r\n", Duration.TotalMilliseconds);

				stringBuilder.AppendFormat("Velocity\t\t: {0} ops.\r\n", Velocity);

				stringBuilder.AppendFormat("Aciton time\t: {0} nanoseconds. (Average)\r\n", AverageIterationTime);

				return stringBuilder.ToString();
			}

			#endregion
		}

		#endregion

		#endregion

		#region Methods

		/// <summary>
		/// Executes load test in single task.
		/// </summary>
		/// <param name="iterationCount">A count of times an action will be executed.</param>
		/// <param name="id">Test identifier.</param>
		/// <param name="action">An action to be executed.</param>
		/// <returns>A result of test execution.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
		public static Result Execute(Int64 iterationCount, string id, Action<Int64> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			var stopwatch = Stopwatch.StartNew();

			for (var index = 0; index < iterationCount; index++)
			{
				action(index);
			}

			stopwatch.Stop();

			return new Result(iterationCount, id, stopwatch.ElapsedMilliseconds);
		}

		/// <summary>
		/// Executes two tests to compare results.
		/// </summary>
		/// <param name="iterationCount">A count of times an action will be executed.</param>
		/// <param name="firstTestId">An identifier of the first test.</param>
		/// <param name="firstAction">An action of the first test.</param>
		/// <param name="secondTestId">An identifier of the second test.</param>
		/// <param name="secondAction">An action of the second test.</param>
		/// <returns>A result of test execution.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="firstAction"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="secondAction"/> is <c>null</c>.</exception>
		public static ComparedResults ExecuteCompare(Int64 iterationCount, string firstTestId, Action<Int64> firstAction, string secondTestId, Action<Int64> secondAction)
		{
			var firstTestReusult = Execute(iterationCount, firstTestId, firstAction);

			var secondTestReusult = Execute(iterationCount, secondTestId, secondAction);

			return new ComparedResults(firstTestReusult, secondTestReusult);
		}

		/// <summary>
		/// Executes load test in parallel tasks.
		/// </summary>
		/// <param name="iterationCount">A count of times an action will be executed.</param>
		/// <param name="id">Test identifier.</param>
		/// <param name="action">An action to be executed.</param>
		/// <returns>A result of test execution.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
		public static Result ExecuteParallel(Int64 iterationCount, string id, Action<Int64> action, Int64 tasksCount)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			var tasks = new Task[tasksCount];

			Int64 rest;

			var increment = Math.DivRem(iterationCount, tasksCount, out rest);

			var stopwatch = Stopwatch.StartNew();

			for (var taskIndex = 0; taskIndex < tasksCount; taskIndex++)
			{
				var from = taskIndex * increment;

				var to = taskIndex * increment + increment + (taskIndex == (tasksCount - 1) ? rest : 0);

				tasks[taskIndex] = Task.Run
					(
						() => Atom(from, to, action)
					);
			}

			Task.WaitAll(tasks);

			stopwatch.Stop();

			return new Result(iterationCount, id, stopwatch.ElapsedMilliseconds);
		}

		private static void Atom(long from, long to, Action<Int64> action)
		{
			for (var index = from; index < to; index++)
			{
				action(index);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TTarget"></typeparam>
		/// <param name="iterationCount"></param>
		/// <param name="initialize"></param>
		/// <param name="toTargetType"></param>
		/// <param name="toSourceType"></param>
		/// <returns></returns>
		public static Tuple<Result, Result> ExecuteTransformation<TSource, TTarget>(Int64 iterationCount, Func<Int64, TSource> initialize, Func<TSource, TTarget> toTargetType, Func<TTarget, TSource> toSourceType)
			where TSource : IEquatable<TSource>
		{
			// Initialize array of expected results
			var expectedResults = new TSource[iterationCount];

			for (var index = 0; index < iterationCount; index++)
			{
				expectedResults[index] = initialize(index);
			}

			// Initialize array of intermediate results
			var intermediateResults = new TTarget[iterationCount];

			// Execut to target transformation
			var toTargetTestResult = Execute
				(
					iterationCount,
					"To Target Type Transformation",
					index => intermediateResults[index] = toTargetType(expectedResults[index])
				);

			// Initialize array of actual results
			var actualResults = new TSource[iterationCount];

			// Execut to source transformation
			var toSourceTestResult = Execute
				(
					iterationCount,
					"To Source Type Transformation",
					index => actualResults[index] = toSourceType(intermediateResults[index])
				);

			// Check results
			for (var index = 0; index < iterationCount; index++)
			{
				var expectedResult = expectedResults[index];

				var actualResult = actualResults[index];

				Assert.IsTrue(expectedResult.Equals(actualResult), "Equals failed at item {0}", index);
			}

			return new Tuple<Result, Result>(toTargetTestResult, toSourceTestResult);
		}

		#endregion
	}
}