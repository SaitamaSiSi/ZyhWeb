namespace Zyh.Web.Api.Models
{
    public class MenuRouter
    {
        public string component { get; set; } = string.Empty;
        public RouttMeta meta { get; set; } = new RouttMeta();
        public string name { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public string redirect { get; set; } = string.Empty;
        public List<MenuRouter>? children { get; set; }
    }
}
