namespace WebAPI.Models.Entities
{
    public class CaseLawData
    {
        public int ID { get; set; }
        public int CaseLawID { get; set; }
        public string FileName { get; set; }
        public string Filetext { get; set; }
        public string FileHTML { get; set; }
        public int Srn { get; set; }   // maps to column 'srn'
    }
}