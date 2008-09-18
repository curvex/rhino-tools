using System;
using System.Collections.Generic;
using System.Reflection;
using Iesi.Collections.Generic;
using Rhino.Commons;

namespace Rhino.Commons.Facilities
{
    public class NHibernateUnitOfWorkFacilityConfig
    {
        private ISet<Assembly> assemblies = new HashedSet<Assembly>();
        private ISet<Type> entities = new HashedSet<Type>();
        private string nhibernateConfigurationFile = "hibernate.cfg.xml";
        private Predicate<Type> isCandidateForRepository = IsCandidateForRepositoryAttribute.IsCandidate;

        public NHibernateUnitOfWorkFacilityConfig()
        { 
        }

        public NHibernateUnitOfWorkFacilityConfig(string assembly)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(assembly), "Assembly name cannot be null or empty.");
            assemblies.Add(Assembly.Load(assembly));
        }

        public NHibernateUnitOfWorkFacilityConfig(Assembly assembly)
        {
            Guard.Against<ArgumentNullException>(assembly == null, "Assembly cannot be null.");
            assemblies.Add(assembly);
        }

        public NHibernateUnitOfWorkFacilityConfig NHibernateConfiguration(string file)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(file), "File name cannot be null or empty.");
            nhibernateConfigurationFile = file;
            return this;
        }

        public NHibernateUnitOfWorkFacilityConfig RegisterEntitiesWhere(Predicate<Type> typeIsSpecifiedBy)
        {
            Guard.Against<ArgumentNullException>(typeIsSpecifiedBy == null, "Predicate cannot be null.");
            isCandidateForRepository = typeIsSpecifiedBy;
            return this;
        }

        public NHibernateUnitOfWorkFacilityConfig AddAssembly(string assembly)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(assembly), "Assembly name cannot be null or empty.");
            return AddAssembly(Assembly.Load(assembly));
        }

        public NHibernateUnitOfWorkFacilityConfig AddAssembly(Assembly assembly)
        {
            Guard.Against<ArgumentNullException>(assembly == null, "Assembly cannot be null.");
            assemblies.Add(assembly);
            return this;
        }

        public NHibernateUnitOfWorkFacilityConfig AddEntity(Type entity)
        {
            Guard.Against<ArgumentNullException>(entity == null, "Type of entity cannot be null.");
            entities.Add(entity);
            return this;
        }

        public Predicate<Type> IsCandidateForRepository
        {
            get { return isCandidateForRepository; }
        }

        public Assembly[] Assemblies
        {
            get { return new List<Assembly>(assemblies).ToArray(); }
        }

        public Type[] Entities
        {
            get { return new List<Type>(entities).ToArray(); }
        }

        public string NHibernateConfigurationFile
        {
            get { return nhibernateConfigurationFile; }
        }
    }
}