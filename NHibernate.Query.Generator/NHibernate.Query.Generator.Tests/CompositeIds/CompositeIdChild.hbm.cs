//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 20/12/2007 1:53:02 PM
// This source code was auto-generated by NHQG (), Version 1.9.1.0.
// 
namespace Query {
    
    
    public partial class Where {
        
        /// <summary>
        /// Query helper for member Where.CompositeIdChild
        /// </summary>
        public static Root_Query_CompositeIdChild CompositeIdChild {
            get {
                return new Root_Query_CompositeIdChild();
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Query_CompositeIdChild
        /// </summary>
        public partial class Query_CompositeIdChild<T1> : QueryBuilder<T1>
         {
            
            /// <summary>
            /// Query helper for member Query_CompositeIdChild..ctor
            /// </summary>
            public Query_CompositeIdChild() : 
                    this(null, "this", null) {
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeIdChild..ctor
            /// </summary>
            public Query_CompositeIdChild(QueryBuilder<T1> parent, string name, string associationPath) : 
                    base(parent, name, associationPath) {
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeIdChild..ctor
            /// </summary>
            public Query_CompositeIdChild(QueryBuilder<T1> parent, string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(parent, name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeIdChild.
            /// </summary>
            public virtual Query_CompositeIdParent<T1> Parent {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Parent");
                    Query_CompositeIdParent<T1> child = new Query_CompositeIdParent<T1>(null, "Parent", temp, true);
                    child.ShouldSkipJoinOnIdEquality = this.ShouldSkipJoinOnIdEquality;
                    return child;
                }
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeIdChild.
            /// </summary>
            public virtual PropertyQueryBuilder<T1> CompositeIdPart1 {
                get {
                    string temp = associationPath;
                    PropertyQueryBuilder<T1> child = new PropertyQueryBuilder<T1>(null, "CompositeIdPart1", temp);
                    child.ShouldSkipJoinOnIdEquality = this.ShouldSkipJoinOnIdEquality;
                    return child;
                }
            }
            
            /// <summary>
            /// Query helper for member Query_CompositeIdChild.
            /// </summary>
            public virtual PropertyQueryBuilder<T1> CompositeIdPart2 {
                get {
                    string temp = associationPath;
                    PropertyQueryBuilder<T1> child = new PropertyQueryBuilder<T1>(null, "CompositeIdPart2", temp);
                    child.ShouldSkipJoinOnIdEquality = this.ShouldSkipJoinOnIdEquality;
                    return child;
                }
            }
        }
        
        /// <summary>
        /// Query helper for member Where.Root_Query_CompositeIdChild
        /// </summary>
        public partial class Root_Query_CompositeIdChild : Query_CompositeIdChild<NHibernate.Query.Generator.Tests.CompositeIds.CompositeIdChild> {
        }
    }
    
    public partial class OrderBy {
        
        /// <summary>
        /// Query helper for member OrderBy.CompositeIdChild
        /// </summary>
        public partial class CompositeIdChild {
            
            /// <summary>
            /// Query helper for member dummy.CompositeIdPart1
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeIdChild.CompositeIdPart1
            /// </summary>
            public static OrderByClause CompositeIdPart1 {
                get {
                    return new OrderByClause("dummy.CompositeIdPart1");
                }
            }
            
            /// <summary>
            /// Query helper for member dummy.CompositeIdPart2
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeIdChild.CompositeIdPart2
            /// </summary>
            public static OrderByClause CompositeIdPart2 {
                get {
                    return new OrderByClause("dummy.CompositeIdPart2");
                }
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// <summary>
        /// Query helper for member ProjectBy.CompositeIdChild
        /// </summary>
        public partial class CompositeIdChild {
            
            /// <summary>
            /// Query helper for member dummy.CompositeIdPart1
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeIdChild.CompositeIdPart1
            /// </summary>
            public static PropertyProjectionBuilder CompositeIdPart1 {
                get {
                    return new PropertyProjectionBuilder("dummy.CompositeIdPart1");
                }
            }
            
            /// <summary>
            /// Query helper for member dummy.CompositeIdPart2
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeIdChild.CompositeIdPart2
            /// </summary>
            public static PropertyProjectionBuilder CompositeIdPart2 {
                get {
                    return new PropertyProjectionBuilder("dummy.CompositeIdPart2");
                }
            }
        }
    }
    
    public partial class GroupBy {
        
        /// <summary>
        /// Query helper for member GroupBy.CompositeIdChild
        /// </summary>
        public partial class CompositeIdChild {
            
            /// <summary>
            /// Query helper for member dummy.CompositeIdPart1
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeIdChild.CompositeIdPart1
            /// </summary>
            public static NHibernate.Criterion.IProjection CompositeIdPart1 {
                get {
                    return NHibernate.Criterion.Projections.GroupProperty("dummy.CompositeIdPart1");
                }
            }
            
            /// <summary>
            /// Query helper for member dummy.CompositeIdPart2
            /// </summary>
            /// <summary>
            /// Query helper for member CompositeIdChild.CompositeIdPart2
            /// </summary>
            public static NHibernate.Criterion.IProjection CompositeIdPart2 {
                get {
                    return NHibernate.Criterion.Projections.GroupProperty("dummy.CompositeIdPart2");
                }
            }
        }
    }
}