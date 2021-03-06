﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NHibernate.Dialect;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	using System.Threading.Tasks;

	[TestFixture]
	public class SqlClientDriverFixtureAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "DriverTest.MultiTypeEntity.hbm.xml" }; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

		[Test]
		public async Task CrudAsync()
		{
			// Should use default dimension for CRUD op because the mapping does not 
			// have dimensions specified.
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				savedId = await (s.SaveAsync(new MultiTypeEntity
									{
										StringProp = "a",
										StringClob = "a",
										BinaryBlob = new byte[]{1,2,3},
										Binary = new byte[] { 4, 5, 6 },
										Currency = 123.4m,
										Double = 123.5d,
										Decimal = 789.5m
									}));
				await (t.CommitAsync());
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var m = await (s.GetAsync<MultiTypeEntity>(savedId));
				m.StringProp = "b";
				m.StringClob = "b";
				m.BinaryBlob = new byte[] {4,5,6};
				m.Binary = new byte[] {7,8,9};
				m.Currency = 456.78m;
				m.Double = 987.6d;
				m.Decimal = 1323456.45m;
				await (t.CommitAsync());
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.CreateQuery("delete from MultiTypeEntity").ExecuteUpdateAsync());
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task QueryPlansAreReusedAsync()
		{
			if (!(Sfi.ConnectionProvider.Driver is SqlClientDriver))
				Assert.Ignore("Test designed for SqlClientDriver only");

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				// clear the existing plan cache
				await (s.CreateSQLQuery("DBCC FREEPROCCACHE").ExecuteUpdateAsync());
				await (t.CommitAsync());
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var countPlansCommand = s.CreateSQLQuery("SELECT COUNT(*) FROM sys.dm_exec_cached_plans");

				var beforeCount = await (countPlansCommand.UniqueResultAsync<int>());

				var insertCount = 10;
				for (var i=0; i<insertCount; i++)
				{
					await (s.SaveAsync(new MultiTypeEntity() { StringProp = new string('x', i + 1) }));
					await (s.FlushAsync());
				}

				var afterCount = await (countPlansCommand.UniqueResultAsync<int>());

				Assert.That(afterCount - beforeCount, Is.LessThan(insertCount - 1),
					string.Format("Excessive query plans created: before={0} after={1}", beforeCount, afterCount));

				await (t.RollbackAsync());
			}
		}
	}
}