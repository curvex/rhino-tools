//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Query {
    
    
    public partial class Where {
        
        /// <summary>
        /// Query helper for member Where.Cat
        /// </summary>
        public static Root_Query_Cat Cat {
            get {
                return new Root_Query_Cat();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.DomesticCat
        /// </summary>
        public static Root_Query_DomesticCat DomesticCat {
            get {
                return new Root_Query_DomesticCat();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_Cat
        /// </summary>
        public partial class Query_Cat<T1> : QueryBuilder<T1>
         {
            
            /// <summary>
            /// Query helper for member Query_Cat..ctor
            /// </summary>
            public Query_Cat(QueryBuilder<T1> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_Cat..ctor
            /// </summary>
            public Query_Cat(QueryBuilder<T1> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_Cat.
            /// </summary>
            public virtual PropertyQueryBuilder<T1> subclass {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T1>(this, "subclass", temp);
                }
            }
            
            /// <summary>
            /// Query helper for member Query_Cat.
            /// </summary>
            public virtual QueryBuilder<T1> Id {
                get {
                    string temp = associationPath;
                    return new QueryBuilder<T1>(this, "Id", temp);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_Cat
        /// </summary>
        public partial class Root_Query_Cat : Query_Cat<NHibernate.Query.Generator.Tests.ActiveRecord.Cat> {
            
            /// <summary>
            /// Query helper for member Root_Query_Cat..ctor
            /// </summary>
            public Root_Query_Cat() : 
                    base(null, "this", null) {
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_DomesticCat
        /// </summary>
        public partial class Query_DomesticCat<T2> : Query_Cat<T2>
         {
            
            /// <summary>
            /// Query helper for member Query_DomesticCat..ctor
            /// </summary>
            public Query_DomesticCat(QueryBuilder<T2> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_DomesticCat..ctor
            /// </summary>
            public Query_DomesticCat(QueryBuilder<T2> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_DomesticCat.
            /// </summary>
            public virtual PropertyQueryBuilder<T2> Name {
                get {
                    string temp = associationPath;
                    return new PropertyQueryBuilder<T2>(this, "Name", temp);
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_DomesticCat
        /// </summary>
        public partial class Root_Query_DomesticCat : Query_DomesticCat<NHibernate.Query.Generator.Tests.ActiveRecord.DomesticCat> {
            
            /// <summary>
            /// Query helper for member Root_Query_DomesticCat..ctor
            /// </summary>
            public Root_Query_DomesticCat() : 
                    base(null, "this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// <summary>
        /// Query helper for member OrderBy.Cat
        /// </summary>
        public partial class Cat {
            
            /// <summary>
            /// Query helper for member Cat.subclass
            /// </summary>
            public static OrderByClause subclass {
                get {
                    return new OrderByClause("subclass");
                }
            }
            
            /// <summary>
            /// Query helper for member Cat.Id
            /// </summary>
            public static OrderByClause Id {
                get {
                    return new OrderByClause("Id");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member OrderBy.DomesticCat
        /// </summary>
        public partial class DomesticCat : Cat {
            
            /// <summary>
            /// Query helper for member DomesticCat.Name
            /// </summary>
            public static OrderByClause Name {
                get {
                    return new OrderByClause("Name");
                }
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// <summary>
        /// Query helper for member ProjectBy.Cat
        /// </summary>
        public partial class Cat {
            
            /// <summary>
            /// Query helper for member Cat.subclass
            /// </summary>
            public static PropertyProjectionBuilder subclass {
                get {
                    return new PropertyProjectionBuilder("subclass");
                }
            }
            
            /// <summary>
            /// Query helper for member Cat.Id
            /// </summary>
            public static NumericPropertyProjectionBuilder Id {
                get {
                    return new NumericPropertyProjectionBuilder("Id");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member ProjectBy.DomesticCat
        /// </summary>
        public partial class DomesticCat : Cat {
            
            /// <summary>
            /// Query helper for member DomesticCat.Name
            /// </summary>
            public static PropertyProjectionBuilder Name {
                get {
                    return new PropertyProjectionBuilder("Name");
                }
            }
        }
    }
    
    public partial class GroupBy {
        
        /// <summary>
        /// Query helper for member GroupBy.Cat
        /// </summary>
        public partial class Cat {
            
            /// <summary>
            /// Query helper for member Cat.subclass
            /// </summary>
            public static NHibernate.Expression.IProjection subclass {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("subclass");
                }
            }
        }
        
        /// <summary>
        /// Query helper for member GroupBy.DomesticCat
        /// </summary>
        public partial class DomesticCat : Cat {
            
            /// <summary>
            /// Query helper for member DomesticCat.Name
            /// </summary>
            public static NHibernate.Expression.IProjection Name {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Name");
                }
            }
        }
    }
}