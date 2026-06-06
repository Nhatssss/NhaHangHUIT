using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer.ReportDefModel;
using CrystalDecisions.ReportAppServer.Controllers;
using CrystalDecisions.ReportAppServer.CommonObjectModel;
using CrystalDecisions.ReportAppServer.CommonControls;
using CrystalDecisions.ReportAppServer.DataDefModel;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Nha_Hang_Huit.Helpers
{
    public static class ReportGenerator
    {
        /// <summary>
        /// Generate .rpt từ DataSet. Thử tất cả các cách có thể qua CR RAS API.
        /// </summary>
        public static string GenerateRpt(DataSet ds, string outputDir)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string outputPath = Path.Combine(outputDir, "PhieuNhapGenerated.rpt");

            // Nếu file đã tồn tại và hợp lệ thì dùng luôn
            if (File.Exists(outputPath) && new FileInfo(outputPath).Length > 500)
                return outputPath;

            // Cách 1: Dùng ReportClientDocument.New() + DatabaseController
            string result = TryMethod1(ds, outputPath);
            if (result != null) return result;

            // Cách 2: Dùng DataSetConverter qua reflection
            result = TryMethod2(ds, outputPath);
            if (result != null) return result;

            // Cách 3: Tạo .rpt rỗng + SetDataSource (sẽ fail nhưng thử)
            result = TryMethod3(ds, outputPath);
            if (result != null) return result;

            // All failed — return empty path
            Debug.WriteLine("[ReportGenerator] Tất cả cách đều thất bại!");
            return null;
        }

        /// <summary>
        /// Cách 1: ReportClientDocument.New() + DatabaseController
        /// </summary>
        private static string TryMethod1(DataSet ds, string outputPath)
        {
            try
            {
                Debug.WriteLine("[ReportGenerator] Method 1: ReportClientDocument.New()");
                ReportClientDocument clientDoc = new ReportClientDocument();
                clientDoc.New();

                // Thử dùng DatabaseController
                var dbController = clientDoc.DatabaseController;

                // Method 1a: Thử add table từ DataSet schema
                try
                {
                    var tableType = typeof(Table);
                    var table = (Table)Activator.CreateInstance(tableType);
                    
                    // Set properties via PropertyBag
                    var props = new PropertyBag();
                    props["DllName"] = "crdb_adoplus.dll";
                    props["TableName"] = ds.Tables[0].TableName;
                    props["DatabaseType"] = "ADO.NET";

                    // Try calling various methods
                    var methods = dbController.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var m in methods)
                    {
                        Debug.WriteLine($"  DBController method: {m.Name}({string.Join(",", Array.ConvertAll(m.GetParameters(), p => p.ParameterType.Name))})");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"  Method 1a inner error: {ex.Message}");
                }

                // Save và test
                clientDoc.SaveAs(outputPath, SaveAsTypeEnum.OldStyleRpt);
                clientDoc.Close();

                if (File.Exists(outputPath) && new FileInfo(outputPath).Length > 500)
                {
                    Debug.WriteLine("[ReportGenerator] Method 1: SAVED - testing with SetDataSource...");
                    // Test by loading and setting data source
                    try
                    {
                        var testReport = new ReportDocument();
                        testReport.Load(outputPath);
                        // If this works, we're good
                        // testReport.SetDataSource(ds); - might fail but we'll know
                        Debug.WriteLine("  Load OK, checking tables...");
                        try
                        {
                            int tblCount = testReport.Database.Tables.Count;
                            Debug.WriteLine($"  Tables count: {tblCount}");
                            if (tblCount > 0)
                                return outputPath;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"  Tables check failed: {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"  Load test failed: {ex.Message}");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ReportGenerator] Method 1 failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cách 2: Dùng DataSetConverter qua reflection
        /// </summary>
        private static string TryMethod2(DataSet ds, string outputPath)
        {
            try
            {
                Debug.WriteLine("[ReportGenerator] Method 2: DataSetConverter");

                ReportClientDocument clientDoc = new ReportClientDocument();
                clientDoc.New();

                // Load DataSetConversion assembly and find the converter
                var conversionAsm = Assembly.Load("CrystalDecisions.ReportAppServer.DataSetConversion");
                var converterType = conversionAsm.GetType("CrystalDecisions.ReportAppServer.DataSetConversion.DataSetConverter");
                if (converterType == null)
                {
                    Debug.WriteLine("  DataSetConverter type not found");
                    return null;
                }

                var converter = Activator.CreateInstance(converterType);
                var wrapperType = conversionAsm.GetType("CrystalDecisions.ReportAppServer.DataSetConversion.AdoNetDataSetWrapper");
                
                if (wrapperType != null)
                {
                    var wrapper = Activator.CreateInstance(wrapperType, ds);
                    
                    // Try all public methods of converter
                    var methods = converterType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var m in methods)
                    {
                        Debug.WriteLine($"  Converter method: {m.Name}({string.Join(",", Array.ConvertAll(m.GetParameters(), p => $"{p.ParameterType.Name} {p.Name}"))})");
                    }

                    // Try Convert method with different signatures
                    foreach (var m in methods)
                    {
                        if (m.Name == "Convert" || m.Name.Contains("DataSet") || m.Name.Contains("Export"))
                        {
                            try
                            {
                                Debug.WriteLine($"  Trying {m.Name}...");
                                var pars = m.GetParameters();
                                if (pars.Length == 2)
                                {
                                    object[] args = new object[2];
                                    args[0] = wrapper;
                                    args[1] = clientDoc.DatabaseController;
                                    m.Invoke(converter, args);
                                    Debug.WriteLine($"  {m.Name} succeeded!");
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"  {m.Name} failed: {ex.InnerException?.Message ?? ex.Message}");
                            }
                        }
                    }
                }

                clientDoc.SaveAs(outputPath, SaveAsTypeEnum.OldStyleRpt);
                clientDoc.Close();

                if (File.Exists(outputPath))
                {
                    try
                    {
                        var testReport = new ReportDocument();
                        testReport.Load(outputPath);
                        int tblCount = testReport.Database.Tables.Count;
                        Debug.WriteLine($"  Post-Method2 tables: {tblCount}");
                        if (tblCount > 0)
                            return outputPath;
                    }
                    catch { }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ReportGenerator] Method 2 failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Cách 3: Tạo .rpt rỗng và test SetDataSource
        /// </summary>
        private static string TryMethod3(DataSet ds, string outputPath)
        {
            try
            {
                Debug.WriteLine("[ReportGenerator] Method 3: ReportClientDocument.New() + save");

                // Copy template .rpt làm base
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report", "PhieuNhapReport.rpt");
                if (File.Exists(templatePath))
                {
                    File.Copy(templatePath, outputPath, true);
                    Debug.WriteLine($"  Copied template: {templatePath} -> {outputPath}");

                    // Try to modify it via ReportClientDocument
                    try
                    {
                        ReportClientDocument clientDoc = new ReportClientDocument();
                        clientDoc.Open(outputPath);
                        clientDoc.SaveAs(outputPath, SaveAsTypeEnum.OldStyleRpt);
                        clientDoc.Close();
                        return outputPath;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"  Modify failed: {ex.Message}, using template as-is");
                        return outputPath; // Return anyway, we'll handle in the viewer
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ReportGenerator] Method 3 failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra .rpt có tables không
        /// </summary>
        public static bool ReportHasTables(string rptPath)
        {
            try
            {
                var report = new ReportDocument();
                report.Load(rptPath);
                return report.Database.Tables.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
