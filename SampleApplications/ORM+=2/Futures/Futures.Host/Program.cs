﻿using System;
using System.IO;
using HumanResources.Model;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Tool.hbm2ddl;
using Util;

namespace Futures.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("nhprof.log4net.config"));

            Configuration cfg = new Configuration()
                  .Configure("nhibernate.cfg.xml");
            using (new ConsoleColorer("nhibernate"))
                new SchemaExport(cfg).Execute(false, true, false, true);
            var factory = cfg.BuildSessionFactory();

            CreateData(factory);
            Console.Clear();
            using (new ConsoleColorer("future"))
            using (ISession session = factory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                var employees = session.CreateCriteria(typeof (Employee))
                    .Future<Employee>();
                var salaries = session.CreateCriteria(typeof (Salary))
                    .Future<Salary>();

                foreach (var employee in employees)
                {
                    Console.WriteLine(employee.Name);
                }
                foreach (var salary in salaries)
                {
                    Console.WriteLine(salary.Name + ": " + salary.HourlyRate);
                }

                tx.Commit();
            }
        }

        private static void CreateData(ISessionFactory factory)
        {
            using (new ConsoleColorer("nhibernate"))
            using (ISession session = factory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                var salary = new Salary { HourlyRate = 22m, Name = "MinPay" };
                var emp = new Employee { Name = "ayende", Salary = salary };

                session.Save(salary);
                session.Save(emp);

                session.Save(new TimesheetEntry
                                 {
                                     Employee = emp,
                                     Start = DateTime.Today.AddHours(8),
                                     End = DateTime.Today.AddHours(17)
                                 });

                session.Save(new TimesheetEntry
                                 {
                                     Employee = emp,
                                     Start = DateTime.Today.AddDays(1).AddHours(8),
                                     End = DateTime.Today.AddDays(1).AddHours(19)
                                 });

                tx.Commit();
            }
        }
    }
}
