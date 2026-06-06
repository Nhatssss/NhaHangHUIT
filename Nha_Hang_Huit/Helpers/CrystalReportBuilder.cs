using System;
using System.Data;
using System.IO;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportAppServer.ClientDoc;
using CrystalDecisions.ReportAppServer.Controllers;
using CrystalDecisions.ReportAppServer.DataSetConversion;
using CRRptDef = CrystalDecisions.ReportAppServer.ReportDefModel;

namespace Nha_Hang_Huit.Helpers
{
    /// <summary>
    /// Xay dung .rpt file tu DataTable ma KHONG can CR Designer.
    /// Dung RAS SDK qua Engine.ReportClientDocument (bridge).
    /// 
    /// Luong hoat dong:
    ///   1. Load template rong (co sections, khong co table/field)
    ///   2. Add table structure tu DataSet qua DataSetConverter + AddDataSource
    ///   3. Add field objects vao section qua AddByName
    ///   4. Add text objects (tieu de, header, footer)
    ///   5. Set data source
    ///   6. Export/Print
    /// </summary>
    public class CrystalReportBuilder : IDisposable
    {
        private readonly ReportDocument _doc;
        private bool _tableAdded;
        private bool _disposed;

        /// <summary>Template da duoc load</summary>
        public ReportDocument Document => _doc;

        /// <summary>RAS ReportClientDocument (bridge qua Engine)</summary>
        public ISCDReportClientDocument ReportClientDocument => _doc.ReportClientDocument;

        /// <summary>RAS ReportDefController (de add objects)</summary>
        public ISCRReportDefController2 ReportDefController
            => (ISCRReportDefController2)_doc.ReportClientDocument.ReportDefController;

        /// <summary>
        /// Khoi tao builder. Template phai co sections.
        /// </summary>
        public CrystalReportBuilder(string templatePath)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Khong tim thay template .rpt", templatePath);

            _doc = new ReportDocument();
            _doc.Load(templatePath);
            _tableAdded = false;

            Debug.WriteLine($"[CrystalReportBuilder] Loaded: {templatePath}");
            Debug.WriteLine($"  Sections: {_doc.ReportDefinition.Sections.Count}");
        }

        #region Table management

        /// <summary>
        /// Them table vao report tu DataTable. Goi truoc AddField.
        /// </summary>
        public DataTable AddTable(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            var ds = new DataSet(dataTable.TableName + "DS");
            ds.Tables.Add(dataTable.Copy());
            return AddTable(ds, dataTable.TableName);
        }

        /// <summary>
        /// Them table vao report tu DataSet.
        /// Goi duoc nhieu lan de add nhieu tables (master-detail).
        /// </summary>
        public DataTable AddTable(DataSet dataSet, string tableName)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));

            var iscrDS = DataSetConverter.Convert(dataSet);
            ReportClientDocument.DatabaseController.AddDataSource(iscrDS);
            _tableAdded = true;

            Debug.WriteLine($"[CrystalReportBuilder] Table '{tableName}' added. Engine tables: {_doc.Database.Tables.Count}");
            return dataSet.Tables[tableName];
        }

        /// <summary>
        /// Kiem tra template da co tables chua
        /// </summary>
        public bool HasTables() => _doc.Database.Tables.Count > 0;

        #endregion

        #region Field management

        /// <summary>
        /// Them field object vao section (tu dong vao PageHeader + Detail).
        /// Can goi AddTable truoc.
        /// </summary>
        public bool AddField(string fieldFormula, string displayName)
        {
            if (!_tableAdded)
                throw new InvalidOperationException("Phai goi AddTable truoc AddField.");

            try
            {
                ReportDefController.ReportObjectController.AddByName(fieldFormula, displayName);
                Debug.WriteLine($"[CrystalReportBuilder] Added field: {fieldFormula}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CrystalReportBuilder] Field {fieldFormula} failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Them nhieu fields tu DataTable columns
        /// </summary>
        public int AddFields(DataTable table)
        {
            int count = 0;
            foreach (DataColumn col in table.Columns)
            {
                if (AddField($"{{{table.TableName}.{col.ColumnName}}}", col.ColumnName))
                    count++;
            }
            return count;
        }

        #endregion

        #region Text management

        /// <summary>
        /// Them TextObject vao section.
        /// </summary>
        public void AddText(string text, int sectionIndex, int left, int top, int width, int height)
        {
            var rasSections = GetRASSections();
            if (sectionIndex < 0 || sectionIndex >= rasSections.Length)
                throw new IndexOutOfRangeException($"Section index {sectionIndex} out of range.");

            var txtObj = new CRRptDef.TextObjectClass
            {
                Name = "txt_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                Kind = CRRptDef.CrReportObjectKindEnum.crReportObjectKindText,
                Left = left,
                Top = top,
                Width = width,
                Height = height
            };

            var para = new CRRptDef.ParagraphClass();
            var textElem = new CRRptDef.ParagraphTextElementClass
            {
                Text = text,
                Kind = CRRptDef.CrParagraphElementKindEnum.crParagraphElementKindText
            };
            para.ParagraphElements.Add(textElem);
            txtObj.Paragraphs.Add(para);

            ReportDefController.ReportObjectController.Add(txtObj, rasSections[sectionIndex], -1);
            Debug.WriteLine($"[CrystalReportBuilder] Text added to section[{sectionIndex}]: \"{text}\"");
        }

        #endregion

        #region Data binding

        public void SetDataSource(DataTable dataTable)
        {
            if (dataTable == null)
                throw new ArgumentNullException(nameof(dataTable));

            _doc.SetDataSource(dataTable);
            Debug.WriteLine($"[CrystalReportBuilder] SetDataSource OK. Rows: {dataTable.Rows.Count}");
        }

        public void SetDataSource(DataSet dataSet)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));
            _doc.SetDataSource(dataSet);
        }

        public void ReadRecords() => _doc.ReadRecords();

        #endregion

        #region Output

        public void ExportToPdf(string outputPath)
        {
            string dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _doc.ExportToDisk(ExportFormatType.PortableDocFormat, outputPath);
            Debug.WriteLine($"[CrystalReportBuilder] PDF: {outputPath}");
        }

        public void SaveRpt(string outputPath)
        {
            string dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _doc.SaveAs(outputPath);
            Debug.WriteLine($"[CrystalReportBuilder] .rpt saved: {outputPath}");
        }

        public void Print(int copies = 1, bool collate = false)
        {
            _doc.PrintToPrinter(copies, collate, 0, 0);
        }

        #endregion

        #region Helpers

        private CRRptDef.Section[] GetRASSections()
        {
            // RAS ReportDefinition has named area properties (ReportHeaderArea, etc.)
            // Each area implements ISCRArea, its Sections collection is 0-indexed
            // Template has sections named Section1 (RH), Section2 (PH), Section3 (Detail), 
            //   Section4 (RF), Section5 (PF) - one section per area
            var rptCtrlRaw = _doc.ReportClientDocument.ReportDefController;
            var rptDefObj = rptCtrlRaw.GetType().GetProperty("ReportDefinition").GetValue(rptCtrlRaw, null);
            if (rptDefObj == null) return new CRRptDef.Section[0];

            string[] areaProps = { "ReportHeaderArea", "PageHeaderArea", "DetailArea", 
                                    "ReportFooterArea", "PageFooterArea" };
            var sectionsList = new System.Collections.Generic.List<CRRptDef.Section>();

            foreach (var ap in areaProps)
            {
                try
                {
                    var areaObj = rptDefObj.GetType().GetProperty(ap).GetValue(rptDefObj, null);
                    if (areaObj == null) continue;

                    var area = (CRRptDef.ISCRArea)areaObj;
                    // Sections index is 0-based for int
                    var sec = (CRRptDef.Section)area.Sections[0];
                    if (sec != null) sectionsList.Add(sec);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[CrystalReportBuilder] Skip section {ap}: {ex.Message}");
                }
            }

            Debug.WriteLine($"[CrystalReportBuilder] Got {sectionsList.Count} RAS sections");
            return sectionsList.ToArray();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _doc?.Close();
            }
        }

        #endregion
    }
}
