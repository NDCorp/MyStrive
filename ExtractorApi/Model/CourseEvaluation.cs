namespace ExtractorApi.Model
{
    public class CourseEvaluation
    {
        public string School { get; set; }
        public string Term { get; set; }
        public string Name { get; set; }
        public DateTime Started { get; set; }
        public List<Evaluation> Evaluations { get; set; }
        public bool Demo { get; set; }
        public double Version { get; set; }
    }
}