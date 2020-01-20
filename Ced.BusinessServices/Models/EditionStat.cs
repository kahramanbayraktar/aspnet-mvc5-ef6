namespace Ced.BusinessServices.Models
{
    public class EditionStat
    {
        public int EventId { get; set; }
        
        public int EditionNo { get; set; }
        
        public short Year { get; set; }
        
        public decimal LocalNumber { get; set; }
        
        public decimal InternationalNumber { get; set; }
        
        // TODO: Adını TotalValue gibi genel bir isimle değiştirsem?
        public decimal TotalNumber { get; set; }

        // TODO:
        public string Countries { get; set; }

        // TODO: Change to this
        //public int EventId { get; set; }

        //public int EditionNo { get; set; }

        //public short Year { get; set; }

        //public decimal LocalValue { get; set; }

        //public decimal InternationalValue { get; set; }

        //public decimal TotalValue { get; set; }
        //// TODO:
        //public string Countries { get; set; }
    }
}
