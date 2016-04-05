﻿using System;
using System.Collections.Specialized;
using Quartz;
using Quartz.Core;
using Quartz.Impl;

namespace VitalChoice.Jobs.Jobs
{
	public class AutofacSchedulerFactory : StdSchedulerFactory
	{
		private readonly AutofacJobFactory _jobFactory;

		/// <summary>
		///     Initializes a new instance of the <see cref="T:Quartz.Impl.StdSchedulerFactory" /> class.
		/// </summary>
		/// <param name="jobFactory">Job factory.</param>
		public AutofacSchedulerFactory(AutofacJobFactory jobFactory)
		{
			if (jobFactory == null) throw new ArgumentNullException(nameof(jobFactory));
			_jobFactory = jobFactory;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:Quartz.Impl.StdSchedulerFactory" /> class.
		/// </summary>
		/// <param name="props">The properties.</param>
		/// <param name="jobFactory">Job factory</param>
		public AutofacSchedulerFactory(NameValueCollection props, AutofacJobFactory jobFactory)
			: base(props)
		{
			_jobFactory = jobFactory;
		}

		/// <summary>
		///     Instantiates the scheduler.
		/// </summary>
		/// <param name="rsrcs">The resources.</param>
		/// <param name="qs">The scheduler.</param>
		/// <returns>Scheduler.</returns>
		protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
		{
			var scheduler = base.Instantiate(rsrcs, qs);
			scheduler.JobFactory = _jobFactory;
			return scheduler;
		}
	}
}
