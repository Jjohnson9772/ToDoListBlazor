namespace TodoService
{
    public class TodoTask
    {
        public int ItemID { get; set; }
        public string? Name { get; set; }
        public bool Completed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Class {  get; set; }
    }
}
