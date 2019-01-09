﻿using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Vulnerator.Model.Object;

namespace Vulnerator.Model.DataAccess
{
    public class DatabaseInterface
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        public void CreateVulnerabilityRelatedIndices()
        {
            try
            {
                if (!DatabaseBuilder.sqliteConnection.State.ToString().Equals("Open"))
                { DatabaseBuilder.sqliteConnection.Open(); }
                using (SQLiteCommand sqliteCommand = DatabaseBuilder.sqliteConnection.CreateCommand())
                {
                    sqliteCommand.CommandText = "PRAGMA user_version";
                    int latestVersion = int.Parse(sqliteCommand.ExecuteScalar().ToString());
                    for (int i = 0; i <= latestVersion; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    sqliteCommand.CommandText = ReadDdl("Vulnerator.Resources.DdlFiles.v6-2-0_CreateVulnerabilityRelatedIndices.ddl");
                                    break;
                                }
                            default:
                                { break; }
                        }
                        if (sqliteCommand.CommandText.Equals(string.Empty))
                        { return; }
                        sqliteCommand.ExecuteNonQuery();
                        sqliteCommand.CommandText = string.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to create vulnerability related indices."));
                throw exception;
            }
            finally
            { DatabaseBuilder.sqliteConnection.Close(); }
        }

        public void DeleteVulnerabilityToCciMapping(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.DeleteVulnerabilityCciMapping;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to delete Vulnerability / CCI mapping \"{0} - {1}\".",
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString(),
                    sqliteCommand.Parameters["CCI"].Value.ToString()));
                log.Debug("Exception details:", exception);
                throw exception;
            }
        }

        public void DeleteRemovedVulnerabilities(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.DeleteVulnerability;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to delete Vulnerability \"{0}\".",
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString()));
                log.Debug("Exception details:", exception);
                throw exception;
            }
        }

        public void DropVulnerabilityRelatedIndices()
        {
            try
            {
                if (!DatabaseBuilder.sqliteConnection.State.ToString().Equals("Open"))
                { DatabaseBuilder.sqliteConnection.Open(); }
                using (SQLiteCommand sqliteCommand = DatabaseBuilder.sqliteConnection.CreateCommand())
                {
                    sqliteCommand.CommandText = "PRAGMA user_version";
                    int latestVersion = int.Parse(sqliteCommand.ExecuteScalar().ToString());
                    for (int i = 0; i <= latestVersion; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    sqliteCommand.CommandText = ReadDdl("Vulnerator.Resources.DdlFiles.v6-2-0_DropVulnerabilityRelatedIndices.ddl");
                                    break;

                                }
                            default:
                                { break; }
                        }
                        if (sqliteCommand.CommandText.Equals(string.Empty))
                        { return; }
                        sqliteCommand.ExecuteNonQuery();
                        sqliteCommand.CommandText = string.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to drop vulnerability related indices."));
                throw exception;
            }
            finally
            { DatabaseBuilder.sqliteConnection.Close(); }
        }

        public void InsertAndMapIpAddress(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertIpAddress;
                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.CommandText = Properties.Resources.MapIpToHardware;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert IP Address \"{0}\"."));
                log.Debug("Exception details:", exception);
                throw exception;
            }
        }

        public void InsertAndMapMacAddress(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertMacAddress;
                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.CommandText = Properties.Resources.MapMacToHardware;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert / map MAC Address \"{0}\" belonging to host \"{1}\"."));
                throw exception;
            }
        }

        public void InsertAndMapPort(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertPPS;
                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.CommandText = Properties.Resources.MapPortToHardware;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert or map port \"{0} {1}\".",
                    sqliteCommand.Parameters["Protocol"].Value.ToString(),
                    sqliteCommand.Parameters["Port"].Value.ToString()));
                throw exception;
            }
        }

        public void InsertAndMapVulnerabilityReferences(SQLiteCommand sqliteCommand, List<string> references, string referenceType)
        {
            try
            {
                foreach (string reference in references)
                {
                    sqliteCommand.CommandText = Properties.Resources.InsertVulnerabilityReference;
                    if (!referenceType.Equals("CVE") && !referenceType.Equals("CPE"))
                    {
                        referenceType = reference.Split(':')[0];
                        sqliteCommand.Parameters.Add(new SQLiteParameter("Reference", reference.Split(':')[1]));
                    }
                    else
                    { sqliteCommand.Parameters.Add(new SQLiteParameter("Reference", reference)); }
                    sqliteCommand.Parameters.Add(new SQLiteParameter("Reference_Type", referenceType));
                    sqliteCommand.ExecuteNonQuery();
                    sqliteCommand.CommandText = Properties.Resources.MapReferenceToVulnerability;
                    sqliteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert / map reference."));
                throw exception;
            }
        }

        public void InsertAndMapVulnerabilityReferences(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertVulnerabilityReference;
                sqliteCommand.ExecuteNonQuery();
                sqliteCommand.CommandText = Properties.Resources.MapReferenceToVulnerability;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert / map reference."));
                throw exception;
            }
        }

        public void InsertDataEntryDate(SQLiteCommand sqliteCommand)
        { 
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertDataEntryDate;
                sqliteCommand.Parameters.Add(new SQLiteParameter("Entry_Date", DateTime.Now.ToShortDateString()));
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert a new Data Entry Date"));
                throw exception;
            }
        }

        public void InsertGroup(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertGroup;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert group into database."));
                throw exception;
            }
        }

        public void InsertHardware(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.Parameters.Add(new SQLiteParameter("Is_Virtual_Server", "False"));
                sqliteCommand.CommandText = Properties.Resources.InsertHardware;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert host \"{0}\".", sqliteCommand.Parameters["IP_Address"].Value.ToString()));
                throw exception;
            }
        }

        public void InsertMitigationOrCondition(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertMitigationOrCondition;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert a new mitigation."));
                throw exception;
            }
        }

        public void InsertParameterPlaceholders(SQLiteCommand sqliteCommand)
        {
            try
            {
                string[] parameters = new string[]
                {
                    // CCI Table
                    "CCI",
                    // Groups Table
                    "Group_ID", "Group_Name", "Is_Accreditation", "Accreditation_ID", "Organization_ID",
                    // FindingTypes Table
                    "Finding_Type",
                    // Hardware Table
                    "Hardware_ID", "Host_Name", "FQDN", "NetBIOS", "Is_Virtual_Server", "NIAP_Level", "Manufacturer", "ModelNumber", "OS",
                    "Is_IA_Enabled", "SerialNumber", "Role", "Lifecycle_Status_ID", "Scan_IP", "Found_21745", "Found_26917", "Displayed_Host_Name",
                    // IP_Addresses Table
                    "IP_Address_ID", "IP_Address",
                    // MAC_Addresses Table
                    "MAC_Address_ID", "MAC_Address",
                    // PPS Table
                    "PPS_ID", "Port", "Protocol", "Discovered_Service", "Display_Service",
                    // Software Table
                    "Software_ID", "Discovered_Software_Name", "Displayed_Software_Name", "Software_Acronym", "Software_Version",
                    "Function", "Install_Date", "DADMS_ID", "DADMS_Disposition", "DADMS_LDA", "Has_Custom_Code", "IaOrIa_Enabled",
                    "Is_OS_Or_Firmware", "FAM_Accepted", "Externally_Authorized", "ReportInAccreditation_Global",
                    "ApprovedForBaseline_Global", "BaselineApprover_Global", "Instance",
                    // UniqueFindings Table
                    "Unique_Finding_ID", "Instance_Identifier", "Tool_Generated_Output", "Comments", "Finding_Details", "Technical_Mitigation",
                    "Proposed_Mitigation", "Predisposing_Conditions", "Impact", "Likelihood", "Severity", "Risk", "Residual_Risk",
                    "First_Discovered", "Last_Observed", "Approval_Status", "Approval_Date", "Approval_Expiration_Date", "Approved_By",
                    "Delta_Analysis_Required", "Finding_Type_ID", "Finding_Source_ID", "Status", "Vulnerability_ID", "Hardware_ID",
                    "Severity_Override", "Severity_Override_Justification", "Technology_Area", "Web_DB_Site", "Web_DB_Instance",
                    "Classification", "CVSS_Environmental_Score", "CVSS_Environmental_Vector",
                    // UniqueFindingSourceFiles Table
                    "Finding_Source_File_ID", "Finding_Source_File_Name", 
                    // Vulnerabilities Table
                    "Vulnerability_ID", "Unique_Vulnerability_Identifier", "Vulnerability_Group_ID", "Vulnerability_Group_Title",
                    "Secondary_Vulnerability_Identifier", "VulnerabilityFamilyOrClass", "Vulnerability_Version", "Vulnerability_Release",
                    "Vulnerability_Title", "Vulnerability_Description", "Risk_Statement", "Fix_Text", "Published_Date", "Modified_Date",
                    "Fix_Published_Date", "Raw_Risk", "CVSS_Base_Score", "CVSS_Base_Vector", "CVSS_Temporal_Score", "CVSS_Temporal_Vector",
                    "Check_Content", "False_Positives", "False_Negatives", "Documentable", "Mitigations", "Mitigation_Control", "Is_Active",
                    "Potential_Impacts", "Third_Party_Tools", "Security_Override_Guidance", "Overflow", "Severity_Override_Guidance", "Overflow",
                    // VulnerabilityReferences Table
                    "Reference_ID", "Reference", "Reference_Type",
                    // VulnerabilitySources Table
                    "Vulnerability_Source_ID", "Source_Name", "Source_Secondary_Identifier", "Vulnerability_Source_File_Name",
                    "Source_Description", "Source_Version", "Source_Release",
                    // ScapScores Table
                    "Score", "Scan_Date"
                };
                foreach (string parameter in parameters)
                { sqliteCommand.Parameters.Add(new SQLiteParameter(parameter, string.Empty)); }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert SQLiteParameter placeholders into SQLiteCommand"));
                throw exception;
            }
        }

        public void InsertParsedFileSource(SQLiteCommand sqliteCommand, Object.File file)
        {
            try
            {
                sqliteCommand.Parameters.Add(new SQLiteParameter("Finding_Source_File_Name", file.FileName));
                sqliteCommand.CommandText = Properties.Resources.InsertUniqueFindingSource;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert unique finding source file \"{0}\".", file.FileName));
                throw exception;
            }
        }

        public void InsertParsedFileSource(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertUniqueFindingSource;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert unique finding source file \"{0}\".", sqliteCommand.Parameters["Finding_Source_File_Name"].Value.ToString()));
                throw exception;
            }
        }

        public void InsertScapScore(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertScapScore;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert SCAP score for \"{0}\" - \"{1}\".",
                    sqliteCommand.Parameters["Host_Name"].Value.ToString(),
                    sqliteCommand.Parameters["Source_Name"].Value.ToString()));
                throw exception;
            }
        }

        public void InsertSoftware(SQLiteCommand sqliteCommand)
        {
            try
            {
                if (sqliteCommand.Parameters["Finding_Type"].Value.ToString().Equals("Fortify"))
                { sqliteCommand.Parameters["Has_Custom_Code"].Value = "True"; }
                else
                { sqliteCommand.Parameters["Has_Custom_Code"].Value = "False"; }
                sqliteCommand.CommandText = Properties.Resources.InsertSoftware;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert software \"{0}\" into table.", sqliteCommand.Parameters["Discovered_Software_Name"].Value.ToString()));
                throw exception;
            }
        }

        public void InsertUniqueFinding(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertUniqueFinding;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to generate a new unique finding for \"{0}\", \"{1}\", \"{2}\".",
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString(),
                    sqliteCommand.Parameters["Host_Name"].Value.ToString(),
                    sqliteCommand.Parameters["Scan_IP"].Value.ToString()));
                throw exception;
            }
        }

        public void InsertVulnerability(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertVulnerability;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert vulnerability \"{0}\".", sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString()));
                throw exception;
            }
        }

        public void InsertVulnerabilitySource(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.InsertVulnerabilitySource;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert vulnerability source \"{0}\".", sqliteCommand.Parameters["Source_Name"].Value.ToString()));
                throw exception;
            }
        }

        public void MapMitigationToGroup(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.MapMitigationToGroup;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to map mitigation to group."));
                throw exception;
            }
        }

        public void MapHardwareToGroup(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.MapHardwareToGroup;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to map \"{0}\" to \"{1}\".",
                    sqliteCommand.Parameters["Host_Name"].Value.ToString(),
                    sqliteCommand.Parameters["Group_Name"].Value.ToString()));
                throw exception;
            }
        }

        public void MapHardwareToSoftware(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.MapHardwareToSoftware;
                sqliteCommand.Parameters.Add(new SQLiteParameter("ReportInAccreditation", "False"));
                sqliteCommand.Parameters.Add(new SQLiteParameter("ApprovedForBaseline", "False"));
                sqliteCommand.Parameters.Add(new SQLiteParameter("BaselineApprover", DBNull.Value));
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to map software \"{0}\" to \"{1}\".",
                    sqliteCommand.Parameters["Discovered_Software_Name"].Value.ToString(),
                    sqliteCommand.Parameters["Host_Name"].Value.ToString()));
                throw exception;
            }
        }

        public void MapHardwareToVulnerabilitySource(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.MapHardwareToVulnerabilitySource;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to map hardware to source."));
                log.Debug("Exception details: " + exception);
            }
        }

        public void MapVulnerabilityToCci(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.MapVulnerabilityToCci;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to map CCI \"{0}\" to vulnerability \"{1}\".",
                    sqliteCommand.Parameters["CCI"].Value.ToString(),
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString()));
                throw exception;
            }
        }

        public void MapVulnerabilityToSource(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.MapVulnerabilityToSource;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to map vulnerability \"{0}\" to source \"{1}\"."));
                throw exception;
            }
        }

        public int SelectLastInsertRowId(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = "SELECT last_insert_rowid();";
                return int.Parse(sqliteCommand.ExecuteScalar().ToString());
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to select the last inserted Row ID."));
                throw exception;
            }
        }

        public List<string> SelectUniqueVulnerabilityIdentifiersBySource(SQLiteCommand sqliteCommand)
        {
            try
            {
                List<string> vulnerabilityIds = new List<string>();
                sqliteCommand.CommandText = Properties.Resources.SelectUniqueVulnerabilityIdentiferBySource;
                using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
                {
                    if (sqliteDataReader.HasRows)
                    {
                        while (sqliteDataReader.Read())
                        { vulnerabilityIds.Add(sqliteDataReader["Unique_Vulnerability_Identifier"].ToString()); }
                    }
                }
                return vulnerabilityIds;
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to generate "));
                throw exception;
            }
        }

        public void SelectVulnerabilitySourceName(SQLiteCommand sqliteCommand)
        { 
            try
            {
                sqliteCommand.CommandText = Properties.Resources.SelectVulnerabilitySourceName;
                using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
                {
                    if (sqliteDataReader.HasRows)
                    {
                        while (sqliteDataReader.Read())
                        { sqliteCommand.Parameters["Source_Name"].Value = sqliteDataReader["Source_Name"].ToString(); }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to retrieve the vulnerability source name for vulnerability source {0}",
                    sqliteCommand.Parameters["Vulnerability_Source_ID"].Value.ToString()));
                throw exception;
            }
        }

        public void SelectHardware(SQLiteCommand sqliteCommand)
        { 
            try
            {
                sqliteCommand.CommandText = Properties.Resources.SelectHardware;
                using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
                {
                    if (sqliteDataReader.HasRows)
                    {
                        while (sqliteDataReader.Read())
                        {
                            sqliteCommand.Parameters["Host_Name"].Value = sqliteDataReader["Host_Name"].ToString();
                            sqliteCommand.Parameters["FQDN"].Value = sqliteDataReader["FQDN"].ToString();
                            sqliteCommand.Parameters["NetBIOS"].Value = sqliteDataReader["NetBIOS"].ToString();
                            sqliteCommand.Parameters["Scan_IP"].Value = sqliteDataReader["Scan_IP"].ToString();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format(""));
                throw exception;
            }
        }

        public void SetCredentialedScanStatus(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.SetCredentialedScanStatus;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to set the credentialed scan status for \"{0}\".",
                    sqliteCommand.Parameters["Scan-IP"].Value.ToString()));
                throw exception;
            }
        }

        public void UpdateMitigationOrCondition(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.UpdateMitigationOrCondition;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to update MitigationOrCondition."));
                throw exception;
            }
        }

        public void UpdateUniqueFinding(SQLiteCommand sqliteCommand)
        {
            try
            {
                switch (sqliteCommand.Parameters["Finding_Type"].Value.ToString())
                {
                    case "ACAS":
                        {
                            sqliteCommand.CommandText = Properties.Resources.UpdateUniqueFindingFromAcas;
                            sqliteCommand.ExecuteNonQuery();
                            break;
                        }
                    case "CKL":
                        {
                            sqliteCommand.CommandText = Properties.Resources.UpdateUniqueFindingFromCkl;
                            sqliteCommand.ExecuteNonQuery();
                            break;
                        }
                    default:
                        { break; }
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to update the unique finding for \"{0}\", \"{1}\", \"{2}\".",
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString(),
                    sqliteCommand.Parameters["Host_Name"].Value.ToString(),
                    sqliteCommand.Parameters["Scan_IP"].Value.ToString()));
                throw exception;
            }
        }

        public void UpdateVulnerability(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.UpdateVulnerability;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to insert vulnerability \"{0}\".", sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString()));
                throw exception;
            }
        }

        public void UpdateDeltaAnalysisFlags(SQLiteCommand sqliteCommand)
        { 
            try
            {
                sqliteCommand.CommandText = Properties.Resources.UpdateDeltaAnalysisFlag;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to update the delta analysis flag(s) for unique findings related to \"{0}\".",
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString()));
                throw exception;
            }
        }

        public void UpdateVulnerabilitySource(SQLiteCommand sqliteCommand)
        {
            try
            {
                switch (sqliteCommand.Parameters["Finding_Type"].Value.ToString())
                {
                    case "ACAS":
                        {
                            sqliteCommand.CommandText = Properties.Resources.UpdateVulnerabilitySourceFromAcas;
                            sqliteCommand.ExecuteNonQuery();
                            sqliteCommand.CommandText = Properties.Resources.DeleteAcasVulnerabilitiesMappedToUnknownVersion;
                            sqliteCommand.ExecuteNonQuery();
                            return;
                        }
                    case "CKL":
                        {
                            if (VulnerabilitySourceUpdateRequired(sqliteCommand))
                            {
                                sqliteCommand.CommandText = Properties.Resources.UpdateVulnerabilitySource;
                                sqliteCommand.ExecuteNonQuery();
                            }
                            return;
                        }
                    default:
                        { break; }
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to update vulnerability source \"{0}\".", sqliteCommand.Parameters["Source_Name"].Value.ToString()));
                throw exception;
            }
        }

        public void UpdateRequiredReportSelected(SQLiteCommand sqliteCommand)
        {
            try
            {
                sqliteCommand.CommandText = Properties.Resources.UpdateRequiredReportIsSelected;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to update the \"Is_Report_Selected\" field for Report ID {0}", 
                    sqliteCommand.Parameters["Is_Report_Selected"].Value.ToString()));
                log.Debug("Exception details:", exception);
            }
        }

        private void MapVulnerabilityToIAControl(SQLiteCommand sqliteCommand)
        {
            try
            {

            }
            catch (Exception exception)
            {
                log.Error("Unable to vulnerability to IA Control.");
                log.Debug("Exception details: " + exception);
            }
        }

        private string ReadDdl(string ddlResourceFile)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string ddlText = string.Empty;
                using (Stream stream = assembly.GetManifestResourceStream(ddlResourceFile))
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    { ddlText = streamReader.ReadToEnd(); }
                }
                return ddlText;
            }
            catch (Exception exception)
            {
                log.Error("Unable to read DDL Resource File.");
                throw exception;
            }
        }

        private bool VulnerabilitySourceUpdateRequired(SQLiteCommand sqliteCommand)
        { 
            try
            {
                bool versionUpdated = false;
                bool versionSame = false;
                bool releaseUpdated = false;
                sqliteCommand.CommandText = Properties.Resources.SelectVulnerabilitySourceVersionAndRelease;
                using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
                {
                    if (sqliteDataReader.HasRows)
                    {
                        while (sqliteDataReader.Read())
                        {
                            if (string.IsNullOrWhiteSpace(sqliteDataReader["Source_Version"].ToString()))
                            { return true; }
                            Regex regex = new Regex(@"\D");
                            int newVersion;
                            int newRelease;
                            bool newVersionParsed = int.TryParse(regex.Replace(sqliteCommand.Parameters["Source_Version"].Value.ToString(), string.Empty), out newVersion);
                            bool newReleaseParsed = int.TryParse(regex.Replace(sqliteCommand.Parameters["Source_Release"].Value.ToString(), string.Empty), out newRelease);
                            int oldVersion;
                            int oldRelease;
                            bool oldVersionParsed = int.TryParse(regex.Replace(sqliteDataReader["Source_Version"].ToString(), string.Empty), out oldVersion);
                            bool oldReleaseParsed = int.TryParse(regex.Replace(sqliteDataReader["Source_Release"].ToString(), string.Empty), out oldRelease);
                            if (newVersionParsed && oldVersionParsed)
                            {
                                versionUpdated = (newVersion > oldVersion) ? true : false;
                                versionSame = (newVersion == oldVersion) ? true : false;
                            }
                            if (newReleaseParsed && oldReleaseParsed && (newRelease > oldRelease))
                            { releaseUpdated = true; }
                            if (versionUpdated || (versionSame && releaseUpdated))
                            { return true; }
                            sqliteCommand.Parameters["Source_Version"].Value = sqliteDataReader["Source_Version"].ToString();
                            sqliteCommand.Parameters["Source_Release"].Value = sqliteDataReader["Source_Release"].ToString();
                        }
                    }
                    return false;
                }
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to determine if an update is required for vulnerability \"{0}\".",
                    sqliteCommand.Parameters["Source_Name"].Value.ToString()));
                throw exception;
            }
        }

        public string CompareVulnerabilityVersions(SQLiteCommand sqliteCommand)
        { 
            try
            {
                bool versionsMatch = false;
                bool releasesMatch = false;
                bool ingestedVersionIsNewer = false;
                bool ingestedReleaseIsNewer = false;
                bool existingVersionIsNewer = false;
                bool existingReleaseIsNewer = false;
                sqliteCommand.CommandText = Properties.Resources.SelectVulnerabilityVersionAndRelease;
                using (SQLiteDataReader sqliteDataReader = sqliteCommand.ExecuteReader())
                {
                    if (sqliteDataReader.HasRows)
                    {
                        while (sqliteDataReader.Read())
                        {
                            if (string.IsNullOrWhiteSpace(sqliteDataReader["Vulnerability_Version"].ToString()))
                            { return "Record Not Found"; }
                            Regex regex = new Regex(@"\D");
                            int newVersion;
                            int newRelease;
                            bool newVersionParsed = int.TryParse(regex.Replace(sqliteCommand.Parameters["Vulnerability_Version"].Value.ToString(), string.Empty), out newVersion);
                            bool newReleaseParsed = int.TryParse(regex.Replace(sqliteCommand.Parameters["Vulnerability_Release"].Value.ToString(), string.Empty), out newRelease);
                            int oldVersion;
                            int oldRelease;
                            bool oldVersionParsed = int.TryParse(regex.Replace(sqliteDataReader["Vulnerability_Version"].ToString(), string.Empty), out oldVersion);
                            bool oldReleaseParsed = int.TryParse(regex.Replace(sqliteDataReader["Vulnerability_Release"].ToString(), string.Empty), out oldRelease);
                            if (newVersionParsed && oldVersionParsed)
                            {
                                ingestedVersionIsNewer = (newVersion > oldVersion) ? true : false;
                                existingVersionIsNewer = (newVersion < oldVersion) ? true : false;
                                versionsMatch = (newVersion == oldVersion) ? true : false;
                            }
                            if (newReleaseParsed && oldReleaseParsed)
                            {
                                ingestedReleaseIsNewer = (newRelease > oldRelease) ? true : false;
                                existingReleaseIsNewer = (newRelease < oldRelease) ? true : false;
                                releasesMatch = (newRelease == oldRelease) ? true : false;
                            }
                            if (ingestedVersionIsNewer)
                            { return "Ingested Version Is Newer"; }
                            if (existingVersionIsNewer)
                            {
                                sqliteCommand.Parameters["Vulnerability_Version"].Value = sqliteDataReader["Vulnerability_Version"].ToString();
                                sqliteCommand.Parameters["Vulnerability_Release"].Value = sqliteDataReader["Vulnerability_Release"].ToString();
                                return "Existing Version Is Newer";
                            }
                            if (versionsMatch)
                            {
                                if (releasesMatch)
                                { return "Identical Versions"; }
                                if (ingestedReleaseIsNewer)
                                { return "Ingested Version Is Newer"; }
                                if (existingReleaseIsNewer)
                                {
                                    sqliteCommand.Parameters["Vulnerability_Version"].Value = sqliteDataReader["Vulnerability_Version"].ToString();
                                    sqliteCommand.Parameters["Vulnerability_Release"].Value = sqliteDataReader["Vulnerability_Release"].ToString();
                                    return "Existing Version Is Newer";
                                }
                            }
                            
                        }
                    }
                }
                return "Record Not Found";
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to compare vulnerability version information for \"{0}\" against current database information.", 
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString()));
                throw exception;
            }
        }

        public void UpdateVulnerabilityModifiedDate(SQLiteCommand sqliteCommand)
        { 
            try
            {
                sqliteCommand.CommandText = Properties.Resources.UpdateVulnerabilityModifiedDate;
                sqliteCommand.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("Unable to update the Modified Date for \"{0}\".",
                    sqliteCommand.Parameters["Unique_Vulnerability_Identifier"].Value.ToString()));
                throw exception;
            }
        }
    }
}
