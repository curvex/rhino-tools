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
        
        static Root_Query_Category _root_query_Category = new Root_Query_Category();
        
        public static Root_Query_Category Category {
            get {
                return _root_query_Category;
            }
        }
        
        public partial class Query_Category<T1> : Query.QueryBuilder<T1>
         {
            
            public Query_Category(string name, string assoicationPath) : 
                    base(name, assoicationPath) {
            }
            
            public Query_Category(string name, string assoicationPath, bool backTrackAssoicationOnEquality) : 
                    base(name, assoicationPath, backTrackAssoicationOnEquality) {
            }
            
            public virtual Query.PropertyQueryBuilder<T1> Name {
                get {
                    string temp = assoicationPath;
                    return new Query.PropertyQueryBuilder<T1>("Name", temp);
                }
            }
            
            public virtual Query.QueryBuilder<T1> CategoryId {
                get {
                    string temp = assoicationPath;
                    return new Query.QueryBuilder<T1>("CategoryId", temp);
                }
            }
        }
        
        public partial class Root_Query_Category : Query_Category<Model.Category> {
            
            public Root_Query_Category() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        public partial class Category {
            
            public static Query.OrderByClause Name {
                get {
                    return new Query.OrderByClause("Name");
                }
            }
        }
    }
    
    public partial class ProjectBy {
        
        public partial class Category {
            
            public static Query.PropertyProjectionBuilder Name {
                get {
                    return new Query.PropertyProjectionBuilder("Name");
                }
            }
        }
    }
    
    public partial class GroupBy {
        
        public partial class Category {
            
            public static NHibernate.Expression.IProjection Name {
                get {
                    return NHibernate.Expression.Projections.GroupProperty("Name");
                }
            }
        }
    }
}
