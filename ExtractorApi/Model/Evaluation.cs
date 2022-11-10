namespace ExtractorApi.Model
{
    public class Evaluation
    {
        public string Title { get; set; }
        public double ActualWeightPerc { get; set; }
        public double RelativeWeightPerc { get; set; }
        public string Category { get; set; }
        public string Due { get; set; }
        public DateTime? DueDate { get; set; }
    }
}