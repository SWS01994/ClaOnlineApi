using System;

namespace WebAPI.Models.Entities
{
    public class CaseLaw
    {
        public int id { get; set; }                 // PK is lowercase 'id' in this table
        public string FileName { get; set; }
        public string Sections { get; set; }
        public string Versus { get; set; }
        public string Citation { get; set; }
        public string Equi_Citation { get; set; }
        public string CaseNo { get; set; }
        public string Judge { get; set; }
        public string Date_of_Judgement { get; set; }   // legacy nvarchar date
        public string HeadNote { get; set; }
        public string AdvocateName { get; set; }
        public string CasesReffered { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string CitYear { get; set; }
        public string CourtName { get; set; }
        public string Vol { get; set; }
        public string PageNO { get; set; }
        public string Books { get; set; }
        public string BusinessSupp { get; set; }
        public string IssueMonth { get; set; }
        public string IssueYear { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? Judgement_date_new { get; set; }   // newer datetime date
        public string Equivalent_SCL { get; set; }
        public string Equivalent_Comp_Cas { get; set; }
        public string Equivalent_CLJ { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool ShowOnHome { get; set; }
    }
}