using System.Collections.Generic;
using System.Linq;

namespace System.Diagnostics
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="PerformanceCounterSettings"/> class.
	/// </summary>
	public static class PerformanceCounterSettingsEx
	{
		#region Methods

		/// <summary>
		/// Installs the specified performance <paramref name="counters"/>.
		/// </summary>
		/// <param name="counters">A list of counters to install.</param>
		/// <exception cref="ArgumentNullException"><paramref name="counters"/> is <c>null</c>.</exception>
		public static void Install(this IEnumerable<PerformanceCounterSettings> counters)
		{
			if (counters == null)
			{
				throw new ArgumentNullException(nameof(counters));
			}

			// Group counters by category
			var countersByCategories =
				(
					from
						counter in counters
					group
						counter by counter.Category
					into
						countersByCategory
					select
						countersByCategory
					).ToDictionary(c => c.Key);

			// For each counters by category
			foreach (var countersByCategory in countersByCategories)
			{
				// Get group category
				var category = countersByCategory.Key;

				// Check if category exists
				if (PerformanceCounterCategory.Exists(category))
				{
					// Check if all counters exists within the category
					if (countersByCategory.Value.All(counter => PerformanceCounterCategory.CounterExists(counter.Name, counter.Category)))
					{
						continue;
					}

					// Some or all counters does not exists within the category
					// Delete category
					PerformanceCounterCategory.Delete(category);
				}

				// Create new counters collection
				var counterData = new CounterCreationDataCollection();

				foreach (var counterCreationData in countersByCategory.Value.Select(counterConfig => new CounterCreationData(counterConfig.Name, String.Empty, counterConfig.CounterType)))
				{
					counterData.Add(counterCreationData);
				}

				// Create category with counters
				PerformanceCounterCategory.Create(category, String.Empty, PerformanceCounterCategoryType.SingleInstance, counterData);
			}
		}

		#endregion
	}
}