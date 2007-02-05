using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Rhino.Commons
{
	/// <summary>
	/// Adapted from the MsBuild Community Tasks code, for use from code
	/// and not from MSBuild.
	/// </summary>
	public class XmlUpdate
	{
		private string _xmlFileName;

		/// <summary>
		/// Gets or sets the name of the XML file.
		/// </summary>
		/// <value>The name of the XML file.</value>
		public string XmlFileName
		{
			get { return _xmlFileName; }
			set { _xmlFileName = value; }
		}

		private string _xpath;

		/// <summary>
		/// Gets or sets the XPath.
		/// </summary>
		/// <value>The XPath.</value>
		public string XPath
		{
			get { return _xpath; }
			set { _xpath = value; }
		}

		private string _value;

		/// <summary>
		/// Gets or sets the value to write.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		private string _namespace;

		/// <summary>
		/// Gets or sets the default namespace.
		/// </summary>
		/// <value>The namespace.</value>
		public string Namespace
		{
			get { return _namespace; }
			set { _namespace = value; }
		}

		private string _prefix;

		/// <summary>
		/// Gets or sets the prefix to associate with the namespace being added.
		/// </summary>
		/// <value>The namespace prefix.</value>
		public string Prefix
		{
			get { return _prefix; }
			set { _prefix = value; }
		}

		public void Execute()
		{
			XmlDocument document = new XmlDocument();
			document.Load(_xmlFileName);

			XPathNavigator navigator = document.CreateNavigator();
			XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);

			if (!string.IsNullOrEmpty(_prefix) && !string.IsNullOrEmpty(_namespace))
			{
				manager.AddNamespace(_prefix, _namespace);
			}

			XPathExpression expression = XPathExpression.Compile(_xpath, manager);
			XPathNodeIterator nodes = navigator.Select(expression);


			while (nodes.MoveNext())
				nodes.Current.SetValue(_value);

			using (XmlTextWriter writer = new XmlTextWriter(_xmlFileName, Encoding.UTF8))
			{
				writer.Formatting = Formatting.Indented;
				document.Save(writer);
				writer.Close();
			}
		}
	}
}