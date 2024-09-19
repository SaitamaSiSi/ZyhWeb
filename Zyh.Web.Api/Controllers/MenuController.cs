using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZyhWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {

        [HttpGet, Route("all")]
        public string All()
        {
            return @$"{{
  ""code"": 0,
  ""data"": [
    {{
      ""component"": ""BasicLayout"",
      ""meta"": {{
        ""order"": -1,
        ""title"": ""page.dashboard.title""
      }},
      ""name"": ""Dashboard"",
      ""path"": ""/"",
      ""redirect"": ""/analytics"",
      ""children"": [
        {{
          ""name"": ""Analytics"",
          ""path"": ""/analytics"",
          ""component"": ""/dashboard/analytics/index"",
          ""meta"": {{
            ""affixTab"": true,
            ""title"": ""page.dashboard.analytics""
          }}
        }},
        {{
          ""name"": ""Workspace"",
          ""path"": ""/workspace"",
          ""component"": ""/dashboard/workspace/index"",
          ""meta"": {{
            ""title"": ""page.dashboard.workspace""
          }}
        }}
      ]
    }},
    {{
      ""component"": ""BasicLayout"",
      ""meta"": {{
        ""icon"": ""mdi:home"",
        ""title"": ""page.home.title""
      }},
      ""name"": ""Home"",
      ""path"": ""/home"",
      ""redirect"": ""/homePage"",
      ""children"": [
        {{
          ""name"": ""HomePage"",
          ""path"": ""/homePage"",
          ""component"": ""/home/homePage/index"",
          ""meta"": {{
            ""affixTab"": true,
            ""title"": ""page.home.index""
          }}
        }},
        {{
          ""name"": ""Gantt"",
          ""path"": ""/gantt"",
          ""component"": ""/home/gantt/index"",
          ""meta"": {{
            ""title"": ""page.gantt.index""
          }}
        }},
        {{
          ""name"": ""Gantt2"",
          ""path"": ""/gantt2"",
          ""component"": ""/home/gantt2/index"",
          ""meta"": {{
            ""title"": ""page.gantt.index""
          }}
        }}
      ]
    }}
  ],
  ""error"": null,
  ""message"": ""ok""
}}";
        }
    }
}
