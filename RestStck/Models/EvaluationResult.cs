namespace RestStck.Models
{
    public class EvaluationResult
    {
        public string Expression { get; set; }
        public string Error { get; set; }
        public EvaluationContext Context { get; set; }
    }
}