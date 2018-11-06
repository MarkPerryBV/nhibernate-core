﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Proxy;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH1439
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<CompositeEntity>(rc =>
			{
				rc.ComposedId(
					c =>
					{
						c.Property(t => t.Id);
						c.Property(t => t.Name);
					});

				rc.Property(x => x.LazyProperty, x => x.Lazy(true));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new CompositeEntity { Id = 1, Name = "Bob", LazyProperty = "LazyProperty"};
				session.Save(e1);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public async Task LazyPropertyShouldBeUninitializedAndLoadableAsync()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var e1 = await (session.Query<CompositeEntity>().SingleAsync());
				Assert.Multiple(
					() =>
					{
						Assert.That(
							NHibernateUtil.IsPropertyInitialized(e1, nameof(CompositeEntity.LazyProperty)),
							Is.False,
							"Lazy property initialization status");
						Assert.That(
							e1.IsProxy(),
							Is.True,
							"Entity IsProxy");
						Assert.That(
							e1,
							Has.Property(nameof(CompositeEntity.LazyProperty)).EqualTo("LazyProperty"));
					});
				await (tran.CommitAsync());
			}
		}
	}
}
