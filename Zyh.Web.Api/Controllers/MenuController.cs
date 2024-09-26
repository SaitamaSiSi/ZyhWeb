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
        }},
{{
          ""name"": ""Three"",
          ""path"": ""/three"",
          ""component"": ""/home/three/index"",
          ""meta"": {{
            ""title"": ""page.three.index""
          }}
        }}
      ]
    }},
    {{
      ""component"": ""BasicLayout"",
      ""meta"": {{
        ""icon"": ""mdi:video"",
        ""title"": ""page.video.title""
      }},
      ""name"": ""Video"",
      ""path"": ""/video"",
      ""redirect"": ""/zlmediakitPage"",
      ""children"": [
        {{
          ""name"": ""Zlmediakit"",
          ""path"": ""/zlmediakitPage"",
          ""component"": ""/video/zlmediakit/index"",
          ""meta"": {{
            ""title"": ""page.video.index""
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
