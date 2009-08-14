﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Interfaces;
using System.Xml.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.IO;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.ObjectModelConverters
{
    public class EDMConverter
    {
        public static EDM CreateEDMFromIDatabase(IDatabase database, string modelNamespace)
        {
            EDM edm = new EDM();

            edm.SSDLContainer = SSDLConverter.CreateSSDLContainer(database, modelNamespace);
            
            return edm;
        }

        public static XDocument GetSSDLXML(IDatabase database, string modelNamespace)
        { 
            SSDLContainer ssdlContainer = SSDLConverter.CreateSSDLContainer(database, modelNamespace);
            return SSDLIO.WriteXDocument(ssdlContainer);
        }

        public static XDocument CreateEDMXFromIDatabase(IDatabase database, string modelNamespace, string objectContextNamespace, string objectContextName)
        {
            XDocument ssdlXDocument = GetSSDLXML(database, modelNamespace);

            string ssdlTempFilename = GetTempFilenameWithExtension("ssdl");
            ssdlXDocument.Save(ssdlTempFilename);

            FileInfo fileInfo = new FileInfo(ssdlTempFilename);
            string filenameRump = Path.GetTempPath() + fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

            string edmGenPath = RuntimeEnvironment.GetRuntimeDirectory() + "\\EdmGen.exe";
            edmGenPath = @"C:\Windows\Microsoft.NET\Framework\v3.5\EdmGen.exe";

            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = Path.GetTempPath();
            processStartInfo.FileName = edmGenPath;
            processStartInfo.Arguments = string.Format(@"/mode:FromSSDLGeneration /inssdl:string.empty{0}.ssdlstring.empty /outcsdl:string.empty{0}.csdlstring.empty /outmsl:string.empty{0}.mslstring.empty /outobjectlayer:string.empty{1}.Designer.csstring.empty /outviews:string.empty{1}.Views.csstring.empty /Namespace:{2} /EntityContainer:{1}",
                filenameRump, objectContextName, objectContextNamespace);
            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();

            XDocument csdlXDocument = XDocument.Load(filenameRump + ".csdl");
            XDocument mslXDocument = XDocument.Load(filenameRump + ".msl");

            XNamespace edmxNamespace = "http://schemas.microsoft.com/ado/2007/06/edmx";

            XDocument edmXDocument = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement(edmxNamespace + "Edmx", new XAttribute("Version", "1.0"), new XAttribute(XNamespace.Xmlns + "edmx", edmxNamespace.NamespaceName),
                    new XElement(edmxNamespace + "Runtime",
                        new XElement(edmxNamespace + "StorageModels",
                            ssdlXDocument.Root),
                        new XElement(edmxNamespace + "ConceptualModels",
                            csdlXDocument.Root),
                        new XElement(edmxNamespace + "Mappings",
                            mslXDocument.Root))));

            return edmXDocument;
        }

        private static string GetTempFilename()
        {
            return Path.GetTempFileName();
        }

        private static string GetTempFilenameWithExtension(string filenamePrefix, string extension)
        {
            string tempFileAuto = GetTempFilename();

            string tempFile =
                Path.GetTempPath() +
                filenamePrefix +
                Path.GetFileNameWithoutExtension(tempFileAuto)
                + "." + extension;

            return tempFile;
        }

        private static string GetTempFilenameWithExtension(string extension)
        {
            return GetTempFilenameWithExtension(string.Empty, extension);
        }
    }
}
