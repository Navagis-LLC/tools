namespace NavagisProjRegistration.Models
{
    public class Project
    {
        public string ProjectId { get; set; }
        public long ProjectNumber { get; set; }
        public string Name { get; set; }
        public string LifecycleState { get; set; }
        public bool IsLinked { get; set; }
        public string TableTrClass { get; set; }
        public string TableRadioButton { get; set; }
        public string IsLinkedText { get; set; }
    }
}