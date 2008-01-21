namespace Rhino.DSL
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.IO;
    using Boo.Lang.Compiler.Pipelines;

    /// <summary>
    /// Base class for DSL engines, handles most of the routine tasks that a DSL
    /// engine needs to do. Compilation, caching, creation, etc.
    /// </summary>
    public abstract class DslEngine
    {
        private readonly IDictionary<Uri, Type> urlToTypeCache = new Dictionary<Uri, Type>();

        /// <summary>
        /// Try to get a cached type for this URL.
        /// </summary>
        /// <param name="url">The url to use as a key for the cache</param>
        /// <returns>The compiled DSL or null if not found</returns>
        public virtual Type GetFromCache(Uri url)
        {
            Type result;
            urlToTypeCache.TryGetValue(url, out result);
            return result;
        }

        /// <summary>
        /// Create a new instance of this DSL type.
        /// </summary>
        /// <param name="type">The type to create</param>
        /// <param name="parametersForConstructor">optional ctor paraemters</param>
        /// <returns>The newly created instance</returns>
        public virtual object CreateInstance(Type type, params object[] parametersForConstructor)
        {
            return Activator.CreateInstance(type, parametersForConstructor);
        }

        /// <summary>
        /// Compile the DSL and return the resulting context
        /// </summary>
        /// <param name="urls">The files to compile</param>
        /// <returns>The resulting compiler context</returns>
        public virtual CompilerContext Compile(params Uri[] urls)
        {
            BooCompiler compiler = new BooCompiler();
            compiler.Parameters.OutputType = CompilerOutputType;
            compiler.Parameters.GenerateInMemory = true;
            compiler.Parameters.Pipeline = new CompileToMemory();
            CustomizeCompiler(compiler, compiler.Parameters.Pipeline, urls);
            AddInputs(compiler, urls);
            CompilerContext compilerContext = compiler.Run();
            if (compilerContext.Errors.Count != 0)
                throw CreateCompilerException(compilerContext);
            return compilerContext;
        }

        /// <summary>
        /// Create an exception that would be raised on compilation errors.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Exception CreateCompilerException(CompilerContext context)
        {
            return context.Errors[0];
        }

        
        private void AddInputs(BooCompiler compiler, IEnumerable<Uri> urls)
        {
            foreach (Uri url in urls)
            {
                compiler.Parameters.Input.Add(CreateInput(url));
            }
        }

        /// <summary>
        /// Create a compiler input from the URL.
        /// </summary>
        /// <param name="url">The url</param>
        /// <returns>The compiler input</returns>
        protected virtual ICompilerInput CreateInput(Uri url)
        {
            return new FileInput(url.AbsolutePath);
        }

        /// <summary>
        /// Customise the compiler to fit this DSL engine.
        /// This is the most commonly overriden method.
        /// </summary>
        protected virtual void CustomizeCompiler(BooCompiler compiler, CompilerPipeline pipeline, Uri[] urls)
        {
        }

        ///<summary>
        /// The type of assembly compilation should produce
        ///</summary>
        public virtual CompilerOutputType CompilerOutputType
        {
            get { return CompilerOutputType.Library; }
        }


        /// <summary>
        /// Get a type from the assembly according to the URL.
        /// This is used to match a class with its originating file
        /// </summary>
        public virtual Type GetTypeForUrl(Assembly assembly, Uri url)
        {
            string className = Path.GetFileNameWithoutExtension(url.AbsolutePath);
            return assembly.GetType(className, false, true);
        }

        /// <summary>
        /// Put the type in the cache, with the url as the key
        /// </summary>
        public virtual void SetInCache(Uri url, Type type)
        {
            urlToTypeCache[url] = type;
        }

        /// <summary>
        /// The file name format of this DSL
        /// </summary>
        public virtual string FileNameFormat
        {
            get
            {
                return "*.boo";
            }
        }

        /// <summary>
        /// Will retrieve all the _canonised_ urls from the given directory that
        /// this Dsl Engine can process.
        /// </summary>
        public virtual Uri[] GetMatchingUrlsIn(string directory)
        {
            List<Uri> urls = new List<Uri>();
            foreach (string url in Directory.GetFiles(directory, FileNameFormat))
            {
                urls.Add(new Uri(url));
            }
            return urls.ToArray();
        }
    }
}