using Microsoft.AspNetCore.Mvc;
using Zyh.Common.Net;
using Zyh.Web.Api.Models;

namespace ZyhWebApi.Controllers
{
    /// <summary>
    /// 路由菜单相关接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        [HttpGet, Route("all")]
        public string All()
        {
            List<MenuRouter> routers = new List<MenuRouter>() { GetDashboard(), GetHome(), GetVideo(), GetAnamation() };
            return ReqResult<List<MenuRouter>>.Success(routers).ToJsonString();
        }

        private MenuRouter GetDashboard()
        {
            MenuRouter dashboard = new MenuRouter();
            dashboard.component = "BasicLayout";
            dashboard.meta.order = -1;
            dashboard.meta.title = "page.dashboard.title";
            dashboard.name = "Dashboard";
            dashboard.path = "/";
            dashboard.redirect = "/analytics";
            MenuRouter analytics = new MenuRouter();
            analytics.name = "Analytics";
            analytics.path = "/analytics";
            analytics.component = "/dashboard/analytics/index";
            analytics.meta.affixTab = true;
            analytics.meta.title = "page.dashboard.analytics";
            MenuRouter workspace = new MenuRouter();
            workspace.name = "Workspace";
            workspace.path = "/workspace";
            workspace.component = "/dashboard/workspace/index";
            workspace.meta.title = "page.dashboard.workspace";
            dashboard.children = new List<MenuRouter>() { analytics, workspace };

            return dashboard;
        }
        private MenuRouter GetHome()
        {
            MenuRouter home = new MenuRouter();
            home.component = "BasicLayout";
            home.meta.icon = "mdi:home";
            home.meta.title = "page.home.title";
            home.name = "Home";
            home.path = "/home";
            home.redirect = "/homePage";
            MenuRouter homePage = new MenuRouter();
            homePage.name = "HomePage";
            homePage.path = "/homePage";
            homePage.component = "/home/homePage/index";
            homePage.meta.title = "page.home.index";
            MenuRouter Gantt = new MenuRouter();
            Gantt.name = "Gantt";
            Gantt.path = "/gantt";
            Gantt.component = "/home/gantt/index";
            Gantt.meta.title = "page.gantt.index";
            MenuRouter Gantt2 = new MenuRouter();
            Gantt2.name = "Gantt2";
            Gantt2.path = "/gantt2";
            Gantt2.component = "/home/gantt2/index";
            Gantt2.meta.title = "page.gantt.index";
            home.children = new List<MenuRouter>() { homePage, Gantt, Gantt2 };

            return home;
        }
        private MenuRouter GetVideo()
        {
            MenuRouter video = new MenuRouter();
            video.component = "BasicLayout";
            video.meta.icon = "mdi:video";
            video.meta.title = "page.video.title";
            video.name = "Video";
            video.path = "/video";
            video.redirect = "/zlmediakitPage";
            MenuRouter zlmediakitPage = new MenuRouter();
            zlmediakitPage.name = "Zlmediakit";
            zlmediakitPage.path = "/zlmediakitPage";
            zlmediakitPage.component = "/video/zlmediakit/index";
            zlmediakitPage.meta.title = "page.video.index";
            MenuRouter zlmediakit2Page = new MenuRouter();
            zlmediakit2Page.name = "Zlmediakit2";
            zlmediakit2Page.path = "/zlmediakit2Page";
            zlmediakit2Page.component = "/video/zlmediakit2/index";
            zlmediakit2Page.meta.title = "page.video.index";
            MenuRouter zlmediakit3Page = new MenuRouter();
            zlmediakit3Page.name = "Zlmediakit3";
            zlmediakit3Page.path = "/zlmediakit3Page";
            zlmediakit3Page.component = "/video/zlmediakit3/index";
            zlmediakit3Page.meta.title = "page.video.index";
            MenuRouter zlmediakit4Page = new MenuRouter();
            zlmediakit4Page.name = "Zlmediakit4";
            zlmediakit4Page.path = "/zlmediakit4Page";
            zlmediakit4Page.component = "/video/zlmediakit4/index";
            zlmediakit4Page.meta.title = "page.video.index";
            video.children = new List<MenuRouter>() { zlmediakitPage, zlmediakit2Page, zlmediakit3Page, zlmediakit4Page };

            return video;
        }
        private MenuRouter GetAnamation()
        {
            MenuRouter anamation = new MenuRouter();
            anamation.component = "BasicLayout";
            anamation.meta.icon = "mdi:video";
            anamation.meta.title = "page.anamation.title";
            anamation.name = "Anamation";
            anamation.path = "/anamation";
            anamation.redirect = "/threePage";
            MenuRouter threePage = new MenuRouter();
            threePage.name = "ThreePage";
            threePage.path = "/threePage";
            threePage.component = "/anamation/three/index";
            threePage.meta.title = "page.anamation.index";
            MenuRouter canvasPage = new MenuRouter();
            canvasPage.name = "CanvasPage";
            canvasPage.path = "/canvasPage";
            canvasPage.component = "/anamation/canvas/index";
            canvasPage.meta.title = "page.anamation.index";
            anamation.children = new List<MenuRouter>() { threePage, canvasPage };

            return anamation;
        }
    }
}
