using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace X2ProjectGenerator
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            string projectPath = args[0];
            Console.WriteLine("Project directory is " + projectPath);

            string projectFilePath = GetX2ProjectFilePath(projectPath);
            Console.WriteLine("Project file      is " + projectFilePath);

            Console.WriteLine();

            XmlDocument projectFile = LoadProjectFile(projectFilePath);

            string[] files = Directory.EnumerateFiles(projectPath, "*", SearchOption.AllDirectories)
                .Where(path => path != projectFilePath)
                .Select(path => path.Split(new[] {projectPath}, 2, StringSplitOptions.RemoveEmptyEntries))
                .Select(strings => strings[0])
                .Select(path => path.TrimStart('\\'))
                .ToArray();

            IEnumerable<string> folderPaths = files
                .SelectMany(path =>
                {
                    string[] folders = path.Split('\\');
                    folders = folders.Take(folders.Length - 1).ToArray();

                    List<string> result = new List<string>();

                    for (var i = 0; i < folders.Length; i++)
                    {
                        string folderPath = "";

                        for (int j = 0; j <= i; j++)
                        {
                            folderPath += folders[j] + "\\";
                        }

                        result.Add(folderPath);
                    }

                    return result;
                })
                .Distinct()
                .ToArray();

            UpdateProjectFile(projectFile, files, folderPaths);
            projectFile.Save(projectFilePath);
        }

        private static string GetX2ProjectFilePath(string projectPath)
        {
            string[] files = Directory.EnumerateFiles(projectPath, "*.x2proj", SearchOption.TopDirectoryOnly).ToArray();

            if (files.Length == 0)
            {
                throw new Exception("Failed to find .x2proj file");
            }

            if (files.Length > 1)
            {
                throw new Exception("Found more than one .x2proj file");
            }

            return files[0];
        }

        private static XmlDocument LoadProjectFile(string filePath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            return xmlDocument;
        }

        private static void UpdateProjectFile(
            XmlDocument projectFile,
            IEnumerable<string> filePaths,
            IEnumerable<string> folderPaths
        )
        {
            XmlNode projectNode = projectFile.ChildNodes.Cast<XmlNode>().First(node => node.Name == "Project");
            IEnumerable<XmlNode> itemNodes = projectNode.ChildNodes
                .Cast<XmlNode>()
                .Where(node => node.Name == "ItemGroup")
                .ToList();

            List<XmlNode> nodes = itemNodes
                .SelectMany(node => node.ChildNodes.Cast<XmlNode>())
                .ToList();

            UpdateItems(projectNode, nodes, "Folder", folderPaths);
            UpdateItems(projectNode, nodes, "Content", filePaths);

            // Remove current ItemGroups
            foreach (XmlNode node in itemNodes)
            {
                projectNode.RemoveChild(node);
            }

            // Create 1 new big container
            XmlNode itemGroupNode = projectFile.CreateElement("ItemGroup", projectNode.NamespaceURI);
            projectNode.AppendChild(itemGroupNode);

            foreach (XmlNode node in nodes.OrderByDescending(node => node.Name))
            {
                itemGroupNode.AppendChild(node);
            }
        }

        private static void UpdateItems(
            XmlNode projectNode, List<XmlNode> nodes,
            string nodeName, IEnumerable<string> paths
        )
        {
            IEnumerable<XmlNode> newNodes = paths
                .Except(GetExistingPaths(nodes, nodeName))
                .Select(path => CreateItemNode(projectNode, nodeName, path));

            nodes.AddRange(newNodes);
        }

        private static IEnumerable<string> GetExistingPaths(IEnumerable<XmlNode> nodes, string nodeName)
        {
            return nodes
                .Where(node => node.Name == nodeName)
                .Select(node =>
                {
                    Debug.Assert(node.Attributes != null, "node.Attributes != null");
                    return node.Attributes["Include"].Value;
                })
                .ToArray();
        }

        private static XmlNode CreateItemNode(XmlNode projectNode, string nodeName, string path)
        {
            XmlDocument projectFile = projectNode.OwnerDocument;
            Debug.Assert(projectFile != null, nameof(projectFile) + " != null");
            
            XmlNode node = projectFile.CreateElement(nodeName, projectNode.NamespaceURI);
            XmlAttribute includeAttribute = projectFile.CreateAttribute("Include");
            includeAttribute.Value = path;

            Debug.Assert(node.Attributes != null, "node.Attributes != null");
            node.Attributes.Append(includeAttribute);

            return node;
        }
    }
}