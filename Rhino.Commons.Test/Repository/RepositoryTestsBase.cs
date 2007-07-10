#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Expression;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
    public class RepositoryTestsBase : NHibernateInMemoryTestFixtureBase
    {
        protected IList<Parent> parentsInDb;


        [TestFixtureSetUp]
        public void OneTimeTestInitialize()
        {
            sessionFactory = null;

            string path =
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repository\Windsor.config"));
            OneTimeInitalize(path, typeof (Parent).Assembly);
        }


        protected void SaveInCurrentSession(IList<Parent> toSave)
        {
            foreach (Parent parent in toSave)
                UnitOfWork.CurrentSession.Save(parent);
        }


        protected static void FlushAndClearCurrentSession()
        {
            UnitOfWork.Current.Flush();
            UnitOfWork.CurrentSession.Clear();
        }


        public static Parent CreateExampleParentObject(string parentName, int age)
        {
            Parent parent = new Parent();
            parent.Name = parentName;
            parent.Age = age;

            Child child1 = new Child();
            child1.Parent = parent;
            parent.Children.Add(child1);

            Child child2 = new Child();
            child2.Parent = parent;
            parent.Children.Add(child2);
            return parent;
        }


        protected static IList<T> LoadAll<T>()
        {
            DetachedCriteria criteria = DetachedCriteria.For<T>();
            criteria.SetResultTransformer(CriteriaUtil.DistinctRootEntity);
            return criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<T>();
        }
    }
}