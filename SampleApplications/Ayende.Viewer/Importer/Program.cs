using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Model;

namespace Importer
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Usage: Importer [directory to process]");
				return;
			}
			string directory = args[0];
			if (!Directory.Exists(directory))
			{
				Console.WriteLine("{0} does not exists", directory);
				return;
			}
			Dictionary<Guid, Post> posts = new Dictionary<Guid, Post>();
			IDictionary<string, Category> categories = new Dictionary<string, Category>();
			List<Comment> comments = new List<Comment>();
			AddPosts(categories, directory, posts);
			foreach (string file in Directory.GetFiles(directory, "*feedback*.xml"))
			{
				IList<Comment> day_comments = ProcessComments(file, posts);
				comments.AddRange(day_comments);
			}
			Console.WriteLine("Found {0} posts in {1} categories with {2} comments", posts.Count, categories.Count, comments.Count);
			SaveToDatabase(categories, posts.Values, comments);
		}

		private static IList<Comment> ProcessComments(string file, IDictionary<Guid, Post> posts)
		{
			IList<Comment> comments = new List<Comment>();
			XmlDocument xdoc = GetXmlDocument(file);
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xdoc.NameTable);
			namespaceManager.AddNamespace("das", "urn:newtelligence-com:dasblog:runtime:data");
			foreach (XmlNode node in xdoc.SelectNodes("/das:DayExtra/das:Comments/das:Comment", namespaceManager))
			{
				Xml xml = new Xml(node, namespaceManager);
				Comment comment = new Comment();
				comment.Created = xml.GetDate("das:Created/text()");
				comment.Modified = xml.GetDate("das:Modified/text()");
				comment.Email = xml.GetString("das:AuthorEmail/text()");
				comment.Author = xml.GetString("das:Author/text()");
				comment.HomePage = xml.GetString("das:AuthorHomepage/text()");
				comment.CommentId = xml.GetGuid("das:EntryId/text()");
				Guid guid = xml.GetGuid("das:TargetEntryId/text()");
				comment.Post = posts[guid];
				comment.Content = xml.GetString("das:Content/text()");
				comments.Add(comment);
			}
			return comments;
		}

		private static void AddPosts(IDictionary<string, Category> categories, string directory, Dictionary<Guid, Post> posts)
		{
			foreach (string file in Directory.GetFiles(directory, "*dayentry*.xml"))
			{
				IList<Post> day_posts = ProcessDayEntry(file, categories);
				foreach (Post day_post in day_posts)
				{
					posts.Add(day_post.PostId, day_post);
				}
			}
		}

		private static void SaveToDatabase(IDictionary<string, Category> categories, ICollection<Post> posts, ICollection<Comment> comments)
		{
			try
			{
				ActiveRecordStarter.Initialize(typeof (Post).Assembly, ActiveRecordSectionHandler.Instance);
				Console.WriteLine("Creating database schema");
				ActiveRecordStarter.CreateSchema();
				Console.WriteLine("Schema created! Starting to save data");
				using (new SessionScope())
				{
					foreach (Category category in categories.Values)
					{
						category.Create();
					}

					foreach (Post post in posts)
					{
						post.Create();
					}
					foreach (Comment comment in comments)
					{
						comment.Create();
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to save to database: {0}", e);
			}
		}

		private static IList<Post> ProcessDayEntry(string file, IDictionary<string, Category> categories)
		{
			IList<Post> posts = new List<Post>();
			XmlDocument xdoc = GetXmlDocument(file);
			XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xdoc.NameTable);
			namespaceManager.AddNamespace("das", "urn:newtelligence-com:dasblog:runtime:data");
			foreach (XmlNode node in xdoc.SelectNodes("/das:DayEntry/das:Entries/das:Entry", namespaceManager))
			{
				Xml xml = new Xml(node, namespaceManager);
				Post post = new Post();
				post.Title = xml.GetString("das:Title/text()");
				post.Created = xml.GetDate("das:Created/text()");
				post.Modified = xml.GetDate("das:Modified/text()");
				post.Content = xml.GetString("das:Content/text()");
				post.PostId = xml.GetGuid("das:EntryId/text()");
				string[] categoriesNames = xml.GetString("das:Categories/text()")
					.Split(new string[]{";"},StringSplitOptions.RemoveEmptyEntries);
				foreach (string name in categoriesNames)
				{
					if (categories.ContainsKey(name) == false)
					{
						categories[name] = new Category(name);
					}
					post.AddToCategory(categories[name]);
				}
				posts.Add(post);
			}
			return posts;
		}

		private static XmlDocument GetXmlDocument(string file)
		{
			XmlDocument xdoc = new XmlDocument();
			try
			{
				xdoc.Load(file);
			}
			catch (Exception e)
			{
				Console.WriteLine("File {0} is not a valid XML file. {1}", file, e);
			}
			return xdoc;
		}
	}
}