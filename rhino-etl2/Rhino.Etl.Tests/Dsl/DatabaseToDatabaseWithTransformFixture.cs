namespace Rhino.Etl.Tests.Dsl
{
    using System.Collections.Generic;
    using System.Data;
    using Commons;
    using Core;
    using MbUnit.Framework;
    using Rhino.Etl.Dsl;

    [TestFixture]
    public class DatabaseToDatabaseWithTransformFixture : BaseUserToPeopleTest
    {
        [Test]
        public void CanCompile()
        {
            using(EtlProcess process = EtlDslEngine.Facotry.Create<EtlProcess>("Dsl/UsersToPeople.boo"))
                Assert.IsNotNull(process);
        }

        [Test]
        public void CanCopyTableWithTransform()
        {
            using(EtlProcess process = EtlDslEngine.Facotry.Create<EtlProcess>("Dsl/UsersToPeople.boo"))
                process.Execute();

            List<string[]> names = Use.Transaction<List<string[]>>("test", delegate(IDbCommand cmd)
            {
                List<string[]> tuples = new List<string[]>();
                cmd.CommandText = "SELECT firstname, lastname from people order by userid";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tuples.Add(new string[] { reader.GetString(0), reader.GetString(1) });
                    }
                }
                return tuples;
            });
            AssertNames(names);
        }
    }
}